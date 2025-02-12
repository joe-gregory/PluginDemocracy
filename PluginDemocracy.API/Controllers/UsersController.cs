﻿using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.Data;
using PluginDemocracy.Models;
using PluginDemocracy.DTOs;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace PluginDemocracy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(PluginDemocracyContext context, APIUtilityClass utilityClass) : ControllerBase
    {
        private readonly PluginDemocracyContext _context = context;
        private readonly APIUtilityClass _utilityClass = utilityClass;
        #region PUBLIC ENDPOINTS
        [HttpPost(ApiEndPoints.PostSignUp)]
        public async Task<ActionResult<PDAPIResponse>> SignUp(UserDTO registeringUser)
        {
            //Create response object
            PDAPIResponse apiResponse = new();

            //Check validity of input
            if (!ModelState.IsValid || !(registeringUser.Password.Length >= 7)) return BadRequest(ModelState);
            User newUser = new(firstName: registeringUser.FirstName, lastName: registeringUser.LastName, email: registeringUser.Email, phoneNumber: registeringUser.PhoneNumber, address: registeringUser.Address, dateOfBirth: registeringUser.DateOfBirth, culture: registeringUser.Culture, middleName: registeringUser.MiddleName, secondLastName: registeringUser.SecondLastName);
            //hash password && assign
            PasswordHasher<User> _passwordHasher = new();
            newUser.HashedPassword = _passwordHasher.HashPassword(newUser, registeringUser.Password);
            // Save the new user to the context
            _context.Users.Add(newUser);
            try
            {
                await _context.SaveChangesAsync();
                apiResponse.AddAlert("success", _utilityClass.Translate(ResourceKeys.NewUserCreated, newUser.Culture));
            }
            catch (Exception ex)
            {
                apiResponse.AddAlert("error", _utilityClass.Translate(ResourceKeys.UnableToCreateNewUser, newUser.Culture) + $"Error: {ex.Message}");
                //Return here if unable to save user. 
                return StatusCode(503, apiResponse);
            }

            //Send confirmation email
            await _utilityClass.SendConfirmationEmail(newUser, apiResponse);

            //Attach user data to response object
            apiResponse.User = new(newUser);

            return Ok(apiResponse);
        }
        [HttpPost(ApiEndPoints.PostLogin)]
        public async Task<ActionResult<PDAPIResponse>> LogIn(LoginInfoDTO loginInfo)
        {
            //Create response object
            PDAPIResponse apiResponse = new();

            //Look up user by email
            User? existingUser;
            try
            {
                existingUser = await _context.Users
                    .Include(u => u.Notifications)
                    .Include(u => u.Roles)
                        .ThenInclude(r => r.Community)
                    .Include(u => u.ResidentOfHomes)
                        .ThenInclude(h => h.ResidentialCommunity)
                    .Include(u => u.HomeOwnerships)
                        .ThenInclude(ho => ho.Home)
                            .ThenInclude(h => h.ResidentialCommunity)
                    .FirstOrDefaultAsync(u => u.Email == loginInfo.Email);
                if (existingUser == null)
                {
                    apiResponse.AddAlert("error", _utilityClass.Translate(ResourceKeys.EmailNotFound, loginInfo.Culture));
                    return BadRequest(apiResponse);
                }

                loginInfo.Password ??= string.Empty;

                //Compare passwords
                PasswordHasher<User> _passwordHasher = new();
                PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.HashedPassword, loginInfo.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    apiResponse.AddAlert("error", _utilityClass.Translate(ResourceKeys.PasswordMismatch, loginInfo.Culture));
                    return BadRequest(apiResponse);
                }
                else
                {
                    apiResponse.AddAlert("success", _utilityClass.Translate(ResourceKeys.YouHaveLoggedIn, existingUser.Culture));
                    //Convert User to UserDto
                    apiResponse.User = new(existingUser);

                    //Send a SessionJWT to the client so that they can maintain a session
                    apiResponse.SessionJWT = _utilityClass.CreateJWT(existingUser.Id, 7);
                    //Redirect to home feed after login in or join community page if no community
                    apiResponse.RedirectTo = FrontEndPages.Feed;

                    return APIUtilityClass.ReturnPDAPIResponseContentResult(apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse.AddAlert("error", $"Unable to look up user by email\nError:\n{ex.Message}");
                return StatusCode(500, apiResponse);
            }


        }
        [HttpGet("confirmemail")]
        public async Task<ActionResult<PDAPIResponse>> ConfirmEmail([FromQuery] int userId, [FromQuery] string emailConfirmationToken)
        {
            PDAPIResponse apiResponse = new()
            {
                RedirectTo = FrontEndPages.GenericMessage
            };

            User? existingUser = await _context.FindAsync<User>(userId);

            if (existingUser == null)
            {
                apiResponse.AddAlert("error", "User not found");
                apiResponse.RedirectParameters["Title"] = _utilityClass.GetAllTranslationsInNewLines("UserNotFound");
                apiResponse.RedirectParameters["Body"] = _utilityClass.GetAllTranslationsInNewLines("UserNotFoundBody");
                return BadRequest(apiResponse);
            }

            //If user does exist and confirmationToken does match
            if (existingUser.EmailConfirmationToken == emailConfirmationToken)
            {
                existingUser.EmailConfirmed = true;
                try
                {
                    await _context.SaveChangesAsync();

                    //Add Messages saying the email was confirmed
                    apiResponse.AddAlert("success", _utilityClass.Translate(ResourceKeys.YourEmailHasBeenConfirmed, existingUser.Culture));
                    apiResponse.RedirectParameters["Title"] = _utilityClass.Translate(ResourceKeys.EmailOutEmailConfirmedTitle, existingUser.Culture);
                    apiResponse.RedirectParameters["Body"] = _utilityClass.Translate(ResourceKeys.EmailOutEmailConfirmedBody, existingUser.Culture);
                    //Send a SessionJWT to the client so that they can maintain a session
                    apiResponse.SessionJWT = _utilityClass.CreateJWT(existingUser.Id, 7);

                    //Send an email saying that the email address has been confirmed
                    try
                    {
                        await _utilityClass.SendEmailAsync(toEmail: existingUser.Email, subject: _utilityClass.Translate(ResourceKeys.EmailOutEmailConfirmedTitle, existingUser.Culture), body: _utilityClass.Translate(ResourceKeys.EmailOutEmailConfirmedBody, existingUser.Culture));
                        return Ok(apiResponse);
                    }
                    catch (Exception ex)
                    {
                        apiResponse.AddAlert("error", $"Unable to send thank you for confirming email\nError:\n{ex.Message}");
                        return StatusCode(500, apiResponse);
                    }
                }
                catch (Exception ex)
                {
                    apiResponse.AddAlert("error", $"Unable to save changes to database\nError:\n{ex.Message}");
                    return StatusCode(500, apiResponse);
                }

            }
            //If user exists but the confirmationToken does not match
            else
            {
                apiResponse.RedirectParameters["Title"] = _utilityClass.Translate(ResourceKeys.EmailTokenNoMatchTitle, existingUser.Culture);
                apiResponse.RedirectParameters["Body"] = _utilityClass.Translate(ResourceKeys.EmailTokenNoMatchBody, existingUser.Culture);
                apiResponse.AddAlert("error", _utilityClass.Translate(ResourceKeys.EmailTokenNoMatchTitle, existingUser.Culture));
                return Ok(apiResponse);
            }
        }
        [HttpPost("sendforgotpasswordemail")]
        public async Task<ActionResult<PDAPIResponse>> ForgotPassword(LoginInfoDTO loginInfo)
        {
            PDAPIResponse response = new();
            //Generate a secure, unique token. It should expire after a certain time and should contain information about the user.
            //Ensure that a user exists with the given email.
            User? existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginInfo.Email);
            if (existingUser != null)
            {
                //JWT Expires in 2 days. 
                string token = _utilityClass.CreateJWT(existingUser.Id, 2);
                //Create link to send to user. It should point to the app
                string link = $"{_utilityClass.WebAppBaseUrl}{FrontEndPages.ResetPassword}?token={token}";
                //Send an email with a link to reset the password.
                try
                {
                    await _utilityClass.SendEmailAsync(toEmail: existingUser.Email, subject: _utilityClass.Translate(ResourceKeys.ResetPasswordEmailSubject, existingUser.Culture), body: _utilityClass.Translate(ResourceKeys.ResetPasswordEmailBody, existingUser.Culture) + $"\n<a href=\"{link}\">{link}</a>");
                }
                catch (Exception ex)
                {
                    response.AddAlert("error", $"Unable to send email\nError:\n{ex.Message}");
                    return StatusCode(500, response);
                }
            }
            //enviar PDAPIResponse diciendo que si existe el correo se envio el link para resetear la contraseña
            response.RedirectTo = FrontEndPages.GenericMessage;
            response.RedirectParameters["Title"] = _utilityClass.Translate(ResourceKeys.ResetPasswordEmailSentTitle, existingUser?.Culture ?? new CultureInfo("en-US"));
            response.RedirectParameters["Body"] = _utilityClass.Translate(ResourceKeys.ResetPasswordEmailSentBody, existingUser?.Culture ?? new CultureInfo("en-US"));
            return Ok(response);

        }
        [HttpPost("resetpassword")]
        public async Task<ActionResult<PDAPIResponse>> ResetPassword(LoginInfoDTO loginInfoDto, [FromQuery] string token)
        {
            PDAPIResponse response = new();

            //Unpack the token y checa que usuario es. Si todo se ve bien, save the new password in the corresponding user.
            int? userId = _utilityClass.ReturnUserIdFromJWT(token);

            if (userId == null)
            {
                response.AddAlert("error", "Returned null as userId in token");
                return BadRequest(response);
            }
            User? existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (existingUser == null || loginInfoDto.Password == null)
            {
                response.AddAlert("error", "User not found or loginInfoDto.Password is null");
                return BadRequest(response);
            }

            //hash password && assign
            PasswordHasher<User> _passwordHasher = new();
            existingUser.HashedPassword = _passwordHasher.HashPassword(existingUser, loginInfoDto.Password);
            try
            {
                _context.SaveChanges();
                //Alert success message
                response.AddAlert("success", _utilityClass.Translate(ResourceKeys.NewPasswordSuccess, existingUser.Culture));
                //Redirect page and message
                response.RedirectTo = FrontEndPages.GenericMessage;
                response.RedirectParameters["Title"] = _utilityClass.Translate(ResourceKeys.SuccessfullyUpdatedPasswordTitle, existingUser.Culture);
                response.RedirectParameters["Body"] = _utilityClass.Translate(ResourceKeys.SuccessfullyUpdatedPasswordBody, existingUser.Culture);
                //Send email
                await _utilityClass.SendEmailAsync(toEmail: existingUser.Email, subject: _utilityClass.Translate(ResourceKeys.SuccessfullyUpdatedPasswordTitle, existingUser.Culture), body: _utilityClass.Translate(ResourceKeys.SuccessfullyUpdatedPasswordBody, existingUser.Culture));
            }
            catch (Exception ex)
            {
                response.AddAlert("error", $"Unable to save changes to database\nError:\n{ex.Message}");
                response.RedirectTo = FrontEndPages.Home;
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        #endregion
        #region AUTHORIZED ENDPOINTS
        /// <summary>
        /// Refreshes PDAPIResponse data: Notifications, Community information, JWT
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet(ApiEndPoints.RefreshUserData)]
        public async Task<ActionResult<PDAPIResponse>> RefreshUserData()
        {
            //Create response object
            PDAPIResponse response = new();
            //Extract userId from claims: 
            System.Security.Claims.Claim? userIdClaim = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            if (userIdClaim == null)
            {
                response.AddAlert("error", "Unable to find userId claim");
                return BadRequest(response);
            }
            //Now use this extracted userIdClaim to find the user in the database
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                response.AddAlert("error", "Unable to parse userId from claims");
                return BadRequest(response);
            }
            try
            {
                User? fullDataUser = await _context.Users
                    .Include(u => u.Notifications)
                    .Include(u => u.Roles)
                        .ThenInclude(r => r.Community)
                    .Include(u => u.ResidentOfHomes)
                        .ThenInclude(h => h.ResidentialCommunity)
                    .Include(u => u.HomeOwnerships)
                        .ThenInclude(ho => ho.Home)
                            .ThenInclude(h => h.ResidentialCommunity)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (fullDataUser == null)
                {
                    response.AddAlert("error", "Unable to find user in database");
                    return BadRequest(response);
                }
                response.User = new(fullDataUser);
                //Send a SessionJWT to the client so that they can maintain a session
                response.SessionJWT = _utilityClass.CreateJWT(fullDataUser.Id, 7);
                response.RedirectTo = null;
                return APIUtilityClass.ReturnPDAPIResponseContentResult(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", $"Error:\n{ex.Message}");
                return StatusCode(500, response);
            }
        }
        [Authorize]
        [HttpPost("toggleuserculture")]
        public async Task<ActionResult<PDAPIResponse>> ToggleUserCulture()
        {
            //Create response object
            PDAPIResponse response = new();
            //Extract User from claims

            User? existingUser = await _utilityClass.ReturnUserFromClaims(User, response);
            if (existingUser == null) return BadRequest(response);

            //Toggle culture
            if (existingUser.Culture.Name == "en-US") existingUser.Culture = new CultureInfo("es-MX");
            else if (existingUser.Culture.Name == "es-MX") existingUser.Culture = new CultureInfo("en-US");
            else
            {
                response.AddAlert("error", "Culture not USA or MEX");
                return BadRequest(response);
            }
            //Save changes
            try
            {
                await _context.SaveChangesAsync();
                response.AddAlert("success", _utilityClass.Translate(ResourceKeys.CultureUpdatedSuccessfully, existingUser.Culture));
                response.User = new(existingUser);
                return APIUtilityClass.ReturnPDAPIResponseContentResult(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", $"Unable to save changes to database\nError:\n{ex.Message}");
                return StatusCode(500, response);
            }
        }
        [Authorize]
        [HttpPost(ApiEndPoints.PostUpdateAccount)]
        public async Task<ActionResult<PDAPIResponse>> UpdateAccount([FromBody] UserDTO userDto)
        {
            //Create response object
            PDAPIResponse response = new();
            //Extract User from claims
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User, response);
            if (existingUser == null) return BadRequest(response);

            //Does the UserDto match the one from the SessionJWT?
            if (existingUser.Id != userDto.Id)
            {
                response.AddAlert("error", "User Id does not match the one in the JWT");
                return BadRequest(response);
            }
            //Update user
            existingUser.FirstName = userDto.FirstName;
            existingUser.MiddleName = userDto.MiddleName;
            existingUser.LastName = userDto.LastName;
            existingUser.SecondLastName = userDto.SecondLastName;
            existingUser.PhoneNumber = userDto.PhoneNumber;
            existingUser.Address = userDto.Address;
            existingUser.DateOfBirth = userDto.DateOfBirth;
            existingUser.Culture = userDto.Culture;
            if (existingUser.Email != userDto.Email)
            {
                //Make sure no other users have this email address:
                if (await _context.Users.AnyAsync(u => u.Email == userDto.Email)) response.AddAlert("error", "There is already a user with this email address");
                else
                {
                    existingUser.Email = userDto.Email;
                    existingUser.EmailConfirmed = false;
                    //Send confirmation email
                    try
                    {
                        await _utilityClass.SendConfirmationEmail(existingUser, response);
                        response.AddAlert("info", _utilityClass.Translate(ResourceKeys.ConfirmEmailCheckInbox, existingUser.Culture));
                    }
                    catch (Exception ex)
                    {
                        response.AddAlert("error", $"Unable to send confirmation email\nError:\n{ex.Message}");
                    }
                }
            }
            //Save changes
            try
            {
                await _context.SaveChangesAsync();
                response.AddAlert("success", _utilityClass.Translate(ResourceKeys.AccountUpdatedSuccessfully, existingUser.Culture));
                //Attach user data to response object
                response.User = new(existingUser);
                return APIUtilityClass.ReturnPDAPIResponseContentResult(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", $"Unable to save changes to database\nError:\n{ex.Message}");
                return StatusCode(500, response);
            }
        }
        [Authorize]
        [HttpPost("updateprofilepicture")]
        public async Task<ActionResult<PDAPIResponse>> UpdateProfilePicture([FromForm] IFormFile file)
        {
            //Create response object
            PDAPIResponse response = new();
            //Extract User from claims
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User, response);
            if (existingUser == null) return BadRequest(response);

            //Make sure it is the correct type of file
            string fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!fileExtension.StartsWith('.')) fileExtension = "." + fileExtension;
            if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
            {
                response.AddAlert("error", "File extension is not jpg, jpeg, or png");
                return BadRequest(response);
            }

            //Connect to Blob service
            string blobSasURL = Environment.GetEnvironmentVariable("BlobSASURL") ?? string.Empty;
            if (string.IsNullOrEmpty(blobSasURL)) throw new Exception("BlobSASURL environment variable is null or empty");
            BlobContainerClient blobContainerClient = new(new Uri(blobSasURL));

            //if the user has a profile picture, delete it
            if (!String.IsNullOrEmpty(existingUser.ProfilePicture))
            {
                Uri profilePicUri = new(existingUser.ProfilePicture);
                string oldBlobName = profilePicUri.AbsolutePath.Replace($"/{profilePicUri.Segments[1]}", "").TrimStart('/');
                BlobClient oldBlobClient = blobContainerClient.GetBlobClient(oldBlobName);
                try
                {
                    await oldBlobClient.DeleteAsync();
                }
                catch (Exception ex)
                {
                    response.AddAlert("error", $"Unable to delete old profile picture\nError:\n{ex.Message}");
                    return BadRequest(response);
                }
            }

            string newBlobName = $"user/{existingUser.Id}/profileimage/{Guid.NewGuid()}{fileExtension}";
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive) because I am checking previous to this.
            string contentType = fileExtension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
            };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            try
            {
                BlobClient blobClient = blobContainerClient.GetBlobClient(newBlobName);
                await using var fileStream = file.OpenReadStream();
                await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });
                existingUser.ProfilePicture = blobClient.Uri.ToString();
                await _context.SaveChangesAsync();
                response.AddAlert("success", _utilityClass.Translate(ResourceKeys.ProfilePictureUpdated, existingUser.Culture));
                response.User = new(existingUser);
                return APIUtilityClass.ReturnPDAPIResponseContentResult(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", $"Unable to save changes to database\nError:\n{ex.Message}");
                return BadRequest(response);
            }
        }
        /// <summary>
        /// Returns true if Notification.Read was successfully updated to true
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut(ApiEndPoints.MarkNotificationAsRead)]
        public async Task<ActionResult<bool>> MarkNotificationAsRead([FromBody] int notificationId)
        {
            bool wasSuccessful = false;

            System.Security.Claims.Claim? userIdClaim = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            if (userIdClaim == null) return BadRequest();
            //Now use this extracted userIdClaim to find the user in the database
            if (!int.TryParse(userIdClaim.Value, out int userId)) return BadRequest();
            try
            {
                User? userWithNotifications = await _context.Users
                    .Include(u => u.Notifications)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if (userWithNotifications == null) return BadRequest();
                Notification? notification = userWithNotifications.Notifications.FirstOrDefault(n => n.Id == notificationId);
                if (notification == null) return BadRequest(wasSuccessful);
                notification.Read = true;
                await _context.SaveChangesAsync();
                wasSuccessful = true;
                return Ok(wasSuccessful);
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [Authorize]
        [HttpGet(ApiEndPoints.GetUserDTOFromEmail)]
        public async Task<ActionResult<UserDTO>> GetUserDTOByEmail([FromQuery] string email)
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null)
            {
                return BadRequest("You are not logged in");
            }
            User? userToReturn = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (userToReturn == null)
            {
                return NotFound("User not found");
            }
            UserDTO userToReturnDTO = new(userToReturn);
            string usertoReturnDTOJson = APIUtilityClass.SerializeUserDTOWithNewtonSoft(userToReturnDTO);
            return new ContentResult
            {
                Content = usertoReturnDTOJson,
                ContentType = "application/json",
                StatusCode = 200
            };
        }
        [Authorize]
        [HttpGet(ApiEndPoints.GetUserPetitionDrafts)]
        public async Task<ActionResult<List<PetitionDTO>>> GetUserPetitionDrafts()
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest("You are not logged in");
            List<Petition> petitions = await _context.Petitions.Include(p => p.Authors).Where(p => p.Authors.Contains(existingUser) && p.Published == false).ToListAsync();
            List<PetitionDTO> petitionDTOs = [];
            foreach (Petition petition in petitions) petitionDTOs.Add(new PetitionDTO(petition));
            return Ok(petitionDTOs);
        }
        [Authorize]
        [HttpPost(ApiEndPoints.SavePetitionDraft)]
        public async Task<ActionResult<PDAPIResponse>> SavePetitionDraft([FromForm] PetitionDTO petitionDTO)
        {
            PDAPIResponse response = new();
            //If the petition does not have a community specified, reject

            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();

            Petition? petition = await _context.Petitions
                .Include(p => p.Authors)
                .Include(p => p.AuthorsReadyToPublish)
                .Include(p => p.Signatures)
                .FirstOrDefaultAsync(p => p.Id == petitionDTO.Id);

            //NEW PETITION: if the petition does not exist, or petitionDTO.Id = 0, it is a new petition. 
            if (petition == null)
            {
                try
                {
                    if (petitionDTO.CommunityDTOId == 0)
                    {
                        response.AddAlert("error", "Petition must have a community specified");
                        return BadRequest(response);
                    }
                    petition = new(existingUser)
                    {
                        Title = petitionDTO.Title,
                        Description = petitionDTO.Description,
                        ActionRequested = petitionDTO.ActionRequested,
                        SupportingArguments = petitionDTO.SupportingArguments,
                        DeadlineForResponse = petitionDTO.DeadlineForResponse,
                        LastUpdated = DateTime.UtcNow
                    };
                    ResidentialCommunity? petitionsCommunity = null;
                    petitionsCommunity = await _context.ResidentialCommunities.FirstOrDefaultAsync(c => c.Id == petitionDTO.CommunityDTOId);
                    if (petitionsCommunity != null) petition.Community = petitionsCommunity;
                    //Before adding the files, save the petition so an Id is assigned by SQL and it can be used as part of the document's urls
                    _context.Petitions.Add(petition);
                    await _context.SaveChangesAsync();
                    //Add the other authors
                    User? originalAuthor = await _context.Users.FirstOrDefaultAsync(u => u.Id == petitionDTO.AuthorsIds[0]);
                    if (originalAuthor == null)
                    {
                        response.AddAlert("error", "Original author could not be determined.");
                        return response;
                    }
                    string title = "You have been added as an author for a new petition draft";
                    string body = $"You have been added as an author for the petition draft titled: {petition.Title}. Original author: {originalAuthor.FullName}. Please follow this link to the petition: <a href=\"{_utilityClass.WebAppBaseUrl}{FrontEndPages.CreatePetition}?petitionId={petition.Id}\">Click here.</a> You can remove yourself as an author at any time. \nPetition drafts with multiple authors can only be published if all authors mark the petition as ready to publish.";
                    foreach (int userDTOId in petitionDTO.AuthorsIds)
                    {
                        if (!petition.Authors.Any(u => u.Id == userDTOId))
                        {
                            User? author = await _context.Users.FirstOrDefaultAsync(u => u.Id == userDTOId);
                            if (author != null)
                            {

                                petition.AddAuthor(author);
                                author.AddNotification(title, body);
                                await _utilityClass.SendEmailAsync(toEmail: author.Email, subject: title, body: body);
                            }
                            else response.AddAlert("error", $"Author with id {userDTOId} not found");
                        }
                    }

                    //BLOB STORAGE SUPPORTING DOCUMENTS
                    string blobContainerURL = Environment.GetEnvironmentVariable("BlobContainerURL") ?? string.Empty;
                    string blobSASToken = Environment.GetEnvironmentVariable("BlobSASToken") ?? string.Empty;
                    string readOnlyBlobSASToken = Environment.GetEnvironmentVariable("ReadOnlyBlobSASToken") ?? string.Empty;
                    if (string.IsNullOrEmpty(blobContainerURL) || string.IsNullOrEmpty(blobSASToken) || string.IsNullOrEmpty(readOnlyBlobSASToken)) throw new Exception("One of the environment variables for blob storage is null or empty");
                    BlobContainerClient containerClient = new(new Uri($"{blobContainerURL}?{blobSASToken}"));
                    //Files to add to blob
                    foreach (IFormFile file in petitionDTO.SupportingDocumentsToAdd)
                    {
                        string blobName = $"petition/{petition.Id}/documents/{file.FileName}";
                        BlobClient blobClient = containerClient.GetBlobClient(blobName);
                        await using Stream filestream = file.OpenReadStream();
                        //Upload the document
                        await blobClient.UploadAsync(filestream, new BlobHttpHeaders { ContentType = file.ContentType });
                        //Remove Sas Token: 
                        UriBuilder uriBuilder = new(blobClient.Uri);
                        System.Collections.Specialized.NameValueCollection query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                        query.Clear();
                        uriBuilder.Query = query.ToString();
                        string blobUrlWithoutSas = uriBuilder.ToString();
                        petition.AddLinkToSupportingDocument($"{blobUrlWithoutSas}?{readOnlyBlobSASToken}");
                    }
                    petition.LastUpdated = DateTime.UtcNow;
                    //All done, save the petition
                    await _context.SaveChangesAsync();
                    response.PetitionDTO = new PetitionDTO(petition);
                    response.AddAlert("success", "New petition draft saved successfully");
                    response.SuccessfulOperation = true;
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    response.AddAlert("error", ex.Message);
                    return BadRequest(response);
                }
            }
            //EXISTING PETITION: The case where the petition already exists. petition is not null. 
            else
            {
                //Compare the changes between petitionDTO and petition and update the petition.
                //Do everything inside a try catch block because Domain models throw errors... as they should...
                try
                {
                    // Ensure the petition is not published
                    if (petition.Published)
                    {
                        response.AddAlert("error", "Cannot modify a published petition.");
                        return BadRequest(response);
                    }
                    if (petition.AuthorsReadyToPublish.Count > 0)
                    {
                        response.AddAlert("error", "You cannot modify this petition because other authors marked it as ready to publish. They will need to unmark it to make further edits.");
                        return BadRequest(response);
                    }
                    //Checking to see if there are any new authors
                    //Check which Ids are missing or are extra

                    //first, if petitionDTO.authors is empty, delete the petition
                    if (petitionDTO.AuthorsIds.Count == 0)
                    {
                        existingUser.RemovePetitionDraft(petition);

                        //Delete associated documents form blob storage
                        string readWriteSasToken = Environment.GetEnvironmentVariable("BlobSASToken") ?? string.Empty;
                        if (string.IsNullOrEmpty(readWriteSasToken)) throw new Exception("BlobSASToken environment variable is null or empty");
                        foreach (string fileLink in petition.LinksToSupportingDocuments)
                        {
                            string decodedFileLink = System.Net.WebUtility.UrlDecode(fileLink);
                            // Remove existing SAS token from URL if present
                            UriBuilder uriBuilder = new(decodedFileLink)
                            {
                                Query = null // This removes the existing query (SAS token)
                            };
                            // Append the new SAS token
                            uriBuilder.Query = readWriteSasToken;
                            BlobClient blobClient = new(uriBuilder.Uri);
                            await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
                        }
                        _context.Petitions.Remove(petition);
                        _context.SaveChanges();
                        response.AddAlert("success", "The petition has been deleted.");
                        response.RedirectTo = FrontEndPages.CreatePetition;
                        return Ok(response);
                    }
                    List<int> extraAuthors = [];
                    User? originalAuthor = await _context.Users.FirstOrDefaultAsync(u => u.Id == petitionDTO.AuthorsIds[0]);
                    if (originalAuthor == null)
                    {
                        response.AddAlert("error", "Original author could not be determined. Petition already existed.");
                        return response;
                    }
                    string title = "You have been added as an author for a new petition draft";
                    string body = $"You have been added as an author for the petition draft titled: {petition.Title}. Original author: {originalAuthor.FullName}. Please follow this link to the petition: <a href=\"{_utilityClass.WebAppBaseUrl}{FrontEndPages.CreatePetition}?petitionId={petition.Id}\">Click here.</a> You can remove yourself as an author at any time. \nPetition drafts with multiple authors can only be published if all authors mark the petition as ready to publish.";
                    foreach (int authorDTOId in petitionDTO.AuthorsIds) if (!petition.Authors.Any(a => a.Id == authorDTOId)) extraAuthors.Add(authorDTOId);
                    foreach (int id in extraAuthors)
                    {
                        User? extraAuthor = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                        if (extraAuthor != null)
                        {
                            petition.AddAuthor(extraAuthor);
                            extraAuthor.AddNotification(title, body);
                            await _utilityClass.SendEmailAsync(toEmail: extraAuthor.Email, subject: title, body: body);
                        }
                        else
                        {
                            response.AddAlert("error", $"Extra author with id {id} not found");
                            return BadRequest(response);
                        }
                    }
                    //You can only remove yourself as author. Did you removed yourself as an author? 
                    if (!petitionDTO.AuthorsIds.Any(id => id == existingUser.Id))
                    {
                        petition.RemoveAuthor(existingUser);
                        response.AddAlert("success", "You have been removed as an author from this petition draft");
                        response.RedirectTo = FrontEndPages.PetitionDrafts;
                    }

                    //if the petition doesn't have any authors now, delete it
                    if (petition.Authors.Count == 0)
                    {
                        _context.Petitions.Remove(petition);
                        await _context.SaveChangesAsync();
                        response.SuccessfulOperation = true;
                        response.AddAlert("success", "Petition draft deleted successfully");
                        return Ok(response);
                    }
                    //If the petition is not being deleted, Update the rest of the petition
                    petition.Title = petitionDTO.Title;
                    petition.Description = petitionDTO.Description;
                    petition.ActionRequested = petitionDTO.ActionRequested;
                    petition.SupportingArguments = petitionDTO.SupportingArguments;
                    petition.DeadlineForResponse = petitionDTO.DeadlineForResponse;
                    ResidentialCommunity? petitionsCommunity = null;
                    petitionsCommunity = await _context.ResidentialCommunities.FirstOrDefaultAsync(c => c.Id == petitionDTO.CommunityDTOId);
                    if (petitionsCommunity != null) petition.Community = petitionsCommunity;

                    //BLOB STORAGE SUPPORTING DOCUMENTS
                    string blobContainerURL = Environment.GetEnvironmentVariable("BlobContainerURL") ?? string.Empty;
                    string blobSASToken = Environment.GetEnvironmentVariable("BlobSASToken") ?? string.Empty;
                    string readOnlyBlobSASToken = Environment.GetEnvironmentVariable("ReadOnlyBlobSASToken") ?? string.Empty;
                    //string blobSasUrl = Environment.GetEnvironmentVariable("BlobSASURL") ?? string.Empty;
                    //string readonlyBlobSasUrl = Environment.GetEnvironmentVariable("ReadOnlyBlobSASURL") ?? string.Empty;
                    if (string.IsNullOrEmpty(blobContainerURL) || string.IsNullOrEmpty(blobSASToken) || string.IsNullOrEmpty(readOnlyBlobSASToken)) throw new Exception("One of the environment variables for blob storage is null or empty");
                    //if (string.IsNullOrEmpty(blobSasUrl)) throw new Exception("BlobSASURL environment variable is null or empty");
                    BlobContainerClient containerClient = new(new Uri($"{blobContainerURL}?{blobSASToken}"));
                    //Files to add to blob
                    foreach (IFormFile file in petitionDTO.SupportingDocumentsToAdd)
                    {
                        string blobName = $"petition/{petition.Id}/documents/{file.FileName}";
                        BlobClient blobClient = containerClient.GetBlobClient(blobName);
                        await using Stream filestream = file.OpenReadStream();
                        //Upload the document
                        await blobClient.UploadAsync(filestream, new BlobHttpHeaders { ContentType = file.ContentType });
                        //Remove Sas Token: 
                        UriBuilder uriBuilder = new(blobClient.Uri);
                        System.Collections.Specialized.NameValueCollection query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                        query.Clear();
                        uriBuilder.Query = query.ToString();
                        string blobUrlWithoutSas = uriBuilder.ToString();
                        petition.AddLinkToSupportingDocument($"{blobUrlWithoutSas}?{readOnlyBlobSASToken}");
                    }
                    //Save
                    petition.LastUpdated = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    response.AddAlert("success", "Petition draft updated successfully");
                    response.PetitionDTO = new PetitionDTO(petition);
                    response.SuccessfulOperation = true;
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    response.AddAlert("error", ex.Message);
                    response.SuccessfulOperation = false;
                    return BadRequest(response);
                }
            }
        }
        /// <summary>
        /// Removes author. If there are no authors left, the petition is deleted.
        /// This uses <see cref="Petition.RemoveAuthor(Models.User)"/>. If after the removal there 
        /// are no authors left, the petition is deleted. 
        /// </summary>
        /// <param name="petitionId">Petition.Id</param>
        /// <returns>PDAPIResponse</returns>
        [Authorize]
        [HttpDelete(ApiEndPoints.DeletePetitionDraft)]
        public async Task<ActionResult<PDAPIResponse>> DeletePetitionDraft([FromQuery] int petitionId)
        {
            PDAPIResponse response = new();
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();

            Petition? petition = await _context.Petitions
                .Include(p => p.Authors)
                .Include(p => p.AuthorsReadyToPublish)
                .Include(p => p.Signatures)
                .FirstOrDefaultAsync(p => p.Id == petitionId);

            if (petition == null)
            {
                response.AddAlert("error", "Petition not found");
                return BadRequest(response);
            }
            if (petition.Published)
            {
                response.AddAlert("error", "Cannot delete a published petition.");
                return BadRequest(response);
            }
            //Make sure user is an author of the petition
            if (!petition.Authors.Contains(existingUser))
            {
                response.AddAlert("error", "You are not an author of this petition");
                return BadRequest(response);
            }
            try
            {
                petition.RemoveAuthor(existingUser);
                if (petition.Authors.Count == 0)
                {
                    //Delete associated documents form blob storage
                    string readWriteSasToken = Environment.GetEnvironmentVariable("BlobSASToken") ?? string.Empty;
                    if (string.IsNullOrEmpty(readWriteSasToken)) throw new Exception("BlobSASToken environment variable is null or empty");
                    foreach (string fileLink in petition.LinksToSupportingDocuments)
                    {
                        string decodedFileLink = System.Net.WebUtility.UrlDecode(fileLink);
                        // Remove existing SAS token from URL if present
                        UriBuilder uriBuilder = new(decodedFileLink)
                        {
                            Query = null // This removes the existing query (SAS token)
                        };
                        // Append the new SAS token
                        uriBuilder.Query = readWriteSasToken;
                        BlobClient blobClient = new(uriBuilder.Uri);
                        await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
                    }
                    _context.Petitions.Remove(petition);
                    await _context.SaveChangesAsync();
                    response.SuccessfulOperation = true;
                    response.AddAlert("success", "Petition draft deleted successfully");
                    return Ok(response);
                }
                else
                {
                    await _context.SaveChangesAsync();
                    response.AddAlert("success", "You have been removed as an author from this petition draft");
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.AddAlert("error", ex.Message);
                return BadRequest(response);
            }
        }
        [Authorize]
        [HttpGet(ApiEndPoints.GetPetitionDraft)]
        public async Task<ActionResult<PetitionDTO>> GetPetitionDraft([FromQuery] int petitionId)
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();

            Petition? petition = await _context.Petitions
                .Include(p => p.Authors)
                    .ThenInclude(a => a.HomeOwnerships)
                .Include(p => p.Authors)
                .Include(p => p.Authors)
                    .ThenInclude(a => a.ResidentOfHomes)
                .Include(p => p.AuthorsReadyToPublish)
                .Include(p => p.Signatures)
                .FirstOrDefaultAsync(p => p.Id == petitionId);

            if (petition == null)
            {
                return NotFound("The petition was not found.");
            }
            if (petition.Published) return BadRequest("This petition has already been published.");
            //Make sure user is an author of the petition
            if (!petition.Authors.Contains(existingUser)) return BadRequest("You are not an author of this petition");
            PetitionDTO petitionDTO = new(petition);
            return Ok(petitionDTO);
        }
        [Authorize]
        [HttpDelete(ApiEndPoints.DeleteDocumentFromPetition)]
        public async Task<ActionResult<PDAPIResponse>> DeleteDocumentFromPetition([FromForm] int petitionId, [FromForm] string fileLink)
        {
            PDAPIResponse response = new();
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();

            Petition? petition = await _context.Petitions
                .Include(p => p.Authors)
                .Include(p => p.AuthorsReadyToPublish)
                .Include(p => p.Signatures)
                .FirstOrDefaultAsync(p => p.Id == petitionId);

            if (petition == null)
            {
                response.AddAlert("error", "Petition not found");
                return BadRequest(response);
            }
            if (petition.Published)
            {
                response.AddAlert("error", "Cannot modify a published petition.");
                return BadRequest(response);
            }
            //Make sure user is an author of the petition
            if (!petition.Authors.Contains(existingUser))
            {
                response.AddAlert("error", "You are not an author of this petition");
                return BadRequest(response);
            }
            //Make sure the document exists
            if (!petition.LinksToSupportingDocuments.Contains(fileLink))
            {
                response.AddAlert("error", "Document not found in petition");
                return BadRequest(response);
            }
            //Delete the document from blob storage
            try
            {
                string readWriteSasToken = Environment.GetEnvironmentVariable("BlobSASToken") ?? string.Empty;
                if (string.IsNullOrEmpty(readWriteSasToken)) throw new Exception("BlobSASToken environment variable is null or empty");

                string decodedFileLink = System.Net.WebUtility.UrlDecode(fileLink);

                // Remove existing SAS token from URL if present
                UriBuilder uriBuilder = new(decodedFileLink)
                {
                    Query = null // This removes the existing query (SAS token)
                };
                // Append the new SAS token
                uriBuilder.Query = readWriteSasToken;
                BlobClient blobClient = new(uriBuilder.Uri);
                await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
                petition.RemoveLinkToSupportingDocument(fileLink);
                await _context.SaveChangesAsync();
                response.SuccessfulOperation = true;
                response.AddAlert("success", "Document deleted successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", ex.Message);
                return BadRequest(response);
            }
        }
        [Authorize]
        [HttpPost(ApiEndPoints.AuthorReadyToPublishPetition)]
        public async Task<ActionResult<PDAPIResponse>> MarkPetitionAsReadyToPublish([FromQuery] int petitionId)
        {
            PDAPIResponse response = new();
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null)
            {
                response.AddAlert("error", "User from claims not found");
                return BadRequest(response);
            }
            Petition? petition = await _context.Petitions.Include(p => p.Authors).Include(p => p.Community).Include(p => p.AuthorsReadyToPublish).FirstOrDefaultAsync(p => p.Id == petitionId);

            //if petition is null, bad request
            if (petition == null)
            {
                response.AddAlert("error", "Petition not found");
                return BadRequest(response);
            }
            //If the current user is not an author, cannot publish
            if (!petition.Authors.Any(a => a.Id == existingUser.Id))
            {
                response.AddAlert("error", "User is not an author of this petition");
                return BadRequest(response);
            }
            //If the petition is already published, return bad request
            if (petition.Published)
            {
                response.AddAlert("error", "Petition is already published");
                return BadRequest(response);
            }
            try
            {
                petition.MarkAsReadyToPublish(existingUser);
                _context.SaveChanges();
                if (petition.Published)
                {
                    response.AddAlert("success", "Petition was published");
                    response.RedirectTo = $"{FrontEndPages.Petition}?petitionId={petition.Id}";
                    string title = $"Petition {petition.Id} has been published";
                    string body = $"Petition {petition.Id} has been published and residents can e-sign it. Click on the following link to navigate to the petition page: <a style=\"text-decoration: underline\" href=\"{_utilityClass.WebAppBaseUrl}{FrontEndPages.Petition}?petitionId={petition.Id}\">Petition Page</a>";
                    foreach (User author in petition.Authors)
                    {
                        author.AddNotification(title, body);
                        await _utilityClass.SendEmailAsync(author.Email, title, body);
                    }
                    //Create Post and add it to community
                    string titleForAllUsers = $"A new petition has been published in your community.";
                    string bodyForAllUsers = $"A new petition has been published in your community. Click on the following link to navigate to the petition page: <a style=\"text-decoration: underline\" href=\"{_utilityClass.WebAppBaseUrl}{FrontEndPages.Petition}?petitionId={petition.Id}\">Petition Page</a>. If you agree with the petition, you can e-sign it.";
#pragma warning disable CS8604 // Possible null reference argument. petition.Community is not null here because it is published.
                    Post newPetitionPost = new(petition.Community, body);
#pragma warning restore CS8604 // Possible null reference argument.
                    petition.Community.AddPost(newPetitionPost);

                    //Send Email to all citizens of that community and add Notificatin for all citizens
                    List<Task> sendingEmails = [];
                    foreach (User citizen in petition.Community.Citizens)
                    {
                        citizen.AddNotification(titleForAllUsers, bodyForAllUsers);
                        sendingEmails.Add(_utilityClass.SendEmailAsync(citizen.Email, titleForAllUsers, bodyForAllUsers));
                    }
                    _context.SaveChanges();
                    //TODO: Maybe not necessary to await the emails. Fire and forget for quicker publishing.
                    //foreach (Task task in sendingEmails)
                    //{
                    //    await task;
                    //}
                }
                else
                {
                    response.AddAlert("success", "You have marked the petition as ready to publish");
                    string title = $"{existingUser.FirstName} marked petition {petition.Id} as ready to publish.";
                    string body = $"{existingUser.FirstName} marked petition {petition.Id} as ready to publish. Navigate to: <a style=\"text-decoration: underline\" href=\"{_utilityClass.WebAppBaseUrl}{FrontEndPages.CreatePetition}?petitionId={petition.Id}\">Petition Draft Page</a>";
                    foreach (User author in petition.Authors)
                    {
                        author.AddNotification(title, body);
                        await _utilityClass.SendEmailAsync(author.Email, title, body);
                    }
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", ex.Message);
                return BadRequest(response);
            }
        }
        [Authorize]
        [HttpPost(ApiEndPoints.UnmarkPetitionReadyToPublish)]
        public async Task<ActionResult<PDAPIResponse>> UnmarkPetitionAsReadyToPublish([FromQuery] int petitionId)
        {
            PDAPIResponse response = new();
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null)
            {
                response.AddAlert("error", "User from claims not found");
                return BadRequest(response);
            }
            Petition? petition = await _context.Petitions.Include(p => p.Authors).Include(p => p.Community).Include(p => p.AuthorsReadyToPublish).FirstOrDefaultAsync(p => p.Id == petitionId);

            //if petition is null, bad request
            if (petition == null)
            {
                response.AddAlert("error", "Petition not found");
                return BadRequest(response);
            }
            //If the current user is not an author, cannot publish
            if (!petition.Authors.Any(a => a.Id == existingUser.Id))
            {
                response.AddAlert("error", "User is not an author of this petition");
                return BadRequest(response);
            }
            //If the petition is already published, return bad request
            if (petition.Published)
            {
                response.AddAlert("error", "Petition is already published");
                return BadRequest(response);
            }
            try
            {
                petition.NotReadyToPublish(existingUser);
                _context.SaveChanges();
                response.AddAlert("success", "Your have removed your mark to publish this petition");
                string title = $"{existingUser.FirstName} removed mark to publish petition {petition.Id}.";
                string body = $"{existingUser.FirstName} removed mark to publish petition {petition.Id}. Navigate to: <a style=\"text-decoration: underline\" href=\"{_utilityClass.WebAppBaseUrl}{FrontEndPages.CreatePetition}?petitionId={petition.Id}\"></a>";
                foreach (User author in petition.Authors)
                {
                    author.AddNotification(title, body);
                    await _utilityClass.SendEmailAsync(author.Email, title, body);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", ex.Message);
                return BadRequest(response);
            }
        }
        [Authorize]
        [HttpGet(ApiEndPoints.GetUserAbout)]
        public async Task<ActionResult<UserDTO>> GetUserAbout([FromQuery] int userId)
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null)
            {
                return BadRequest();
            }

            User? userToGetAbout = _context.Users.Include(u => u.HomeOwnerships).ThenInclude(ho => ho.Home).Include(u => u.ResidentOfHomes).Include(u => u.Roles).ThenInclude(r => r.Community).FirstOrDefault(u => u.Id == userId);

            if (userToGetAbout == null)
            {
                return NotFound();
            }
            //You can only get information about this user if you are a citizen in a community where this user is a citizen,
            //or you have a role in one of those communities or you are an admin.

            // Check if existingUser has common citizenships with userFromRequest
            bool hasCommonCitizenship = existingUser.Citizenships
                .Any(c => userToGetAbout.Citizenships.Any(uc => uc.Id == c.Id));

            // Check if existingUser has a role in any of the communities userFromRequest is a citizen of
            bool hasRoleInCommonCommunity = existingUser.Roles
                .Any(r => userToGetAbout.Citizenships.Any(uc => uc.Id == r.Community.Id));

            // Check if existingUser is an admin
            bool isAdmin = existingUser.Admin;

            // Only proceed if the existingUser meets one of the conditions
            if (hasCommonCitizenship || hasRoleInCommonCommunity || isAdmin)
            {
                UserDTO userToReturn = new(userToGetAbout);

                // Serialize the userToReturn using Newtonsoft.Json
                var settings = new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Auto,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                string json = JsonConvert.SerializeObject(userToReturn, settings);
                return Content(json, "application/json");
            }
            else
            {
                return Forbid();
            }
        }
        [Authorize]
        [HttpPost(ApiEndPoints.ESign)]
        public async Task<ActionResult<PDAPIResponse>> ESign([FromQuery] int petitionId, [FromBody] ESignatureDTO? eSignatureDTO)
        {
            PDAPIResponse response = new();
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null)
            {
                response.AddAlert("error", "User from claims not found");
                return BadRequest(response);
            }
            if (eSignatureDTO == null)
            {
                response.AddAlert("error", "eSignatureDTO is null");
                return BadRequest(response);
            }
            if (eSignatureDTO.SignatureImage == null)
            {
                response.AddAlert("error", "Signature image is null");
                return BadRequest(response);
            }
            if (eSignatureDTO.Intent == null)
            {
                response.AddAlert("error", "Intent is null.");
                return BadRequest(response);
            }
            if (string.IsNullOrEmpty(eSignatureDTO.SignatureImageBase64))
            {
                response.AddAlert("error", "Signature image base64 is null or empty");
                return BadRequest(response);
            }
            Petition? petition = await _context.Petitions.Include(p => p.Community).FirstOrDefaultAsync(p => p.Id == petitionId);
            //if petition is null, bad request
            if (petition == null)
            {
                response.AddAlert("error", "Petition not found");
                return BadRequest(response);
            }
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddress))
            {
                response.AddAlert("error", "IP Address not found");
                return BadRequest(response);
            };
            ESignature eSignature = new(existingUser, ipAddress, petition, eSignatureDTO.SignatureImage, eSignatureDTO.SignatureImageBase64, eSignatureDTO.Intent);
            try
            {
                petition.Sign(eSignature);
                await _context.SaveChangesAsync();
                response.AddAlert("success", "You have signed the petition");
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", ex.Message);
                return BadRequest(response);
            }
        }
        [Authorize]
        [HttpPost(ApiEndPoints.SaveProposalDraft)]
        public async Task<ActionResult<PDAPIResponse>> SaveProposalDraft(ProposalDTO? proposalDTO)
        {
            PDAPIResponse response = new();

            if (proposalDTO == null)
            {
                response.AddAlert("error", "proposalDTO was null");
                return BadRequest(response);
            }

            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();

            //Both saving new or existing require these checks: 
            if (string.IsNullOrEmpty(proposalDTO.Title))
            {
                response.AddAlert("error", "Proposal must have a title");
                return BadRequest(response);
            }
            if (proposalDTO.Community == null)
            {
                response.AddAlert("error", "Proposal must have a community specified");
                return BadRequest(response);
            }

            //if proposalDTO.Id == null, it is a new proposal
            if (proposalDTO.Id == null)
            {
                //Title, Community are required but Content is also added if it is present 
                
                ResidentialCommunity? communityForNewProposal = await _context.ResidentialCommunities.FirstOrDefaultAsync(c => c.Id == proposalDTO.Community.Id);
                if (communityForNewProposal == null)
                {
                    response.AddAlert("error", "Community not found");
                    return BadRequest(response);
                }
                Proposal newProposal = new(existingUser, communityForNewProposal, proposalDTO.Title, proposalDTO.Content);
                _context.Proposals.Add(newProposal);
                await _context.SaveChangesAsync();
                response.AddAlert("success", "New proposal draft saved successfully");
                response.ProposalDTO = new ProposalDTO(newProposal);
                response.SuccessfulOperation = true;
                return Ok(response);
            }
            else
            {
                Proposal? proposal = await _context.Proposals
                    .Include(p => p.Author)
                    .Include(p => p.Community)
                    .FirstOrDefaultAsync(p => p.Id == proposalDTO.Id);

                if (proposal == null)
                {
                    response.AddAlert("error", "No proposals were found with given Id.");
                    return BadRequest(response);
                }

                proposal.Title = proposalDTO.Title;
                proposal.Content = proposalDTO.Content;

                await _context.SaveChangesAsync();
                response.AddAlert("success", "Proposal draft updated successfully");
                response.ProposalDTO = new ProposalDTO(proposal);
                response.SuccessfulOperation = true;
                return Ok(response);
            }
        }
        [Authorize]
        [HttpGet(ApiEndPoints.GetProposalDraft)]
        public async Task<ActionResult<ProposalDTO>> GetProposalDraft([FromQuery] string proposalId)
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();

            Guid parsedGuid = Guid.Parse(proposalId);

            try
            {
                Proposal? proposal = await _context.Proposals
                .Include(p => p.Author)
                .Include(p => p.Community)
                .FirstOrDefaultAsync(p => p.Id == parsedGuid);

                if (proposal == null)
                {
                    return NotFound("The proposal was not found.");
                }
                //Make sure user is an author of the proposal
                if (proposal.Author.Id != existingUser.Id) return BadRequest("You are not an author of this proposal");
                ProposalDTO proposalDTO = new(proposal);
                return Ok(proposalDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet(ApiEndPoints.GetUserProposalDrafts)]
        public async Task<ActionResult<List<ProposalDTO>>> GetUserProposalDraftsList()
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest("You are not logged in");
            List<Proposal> proposals = await _context.Proposals.Include(p => p.Author).Where(p => p.Author.Id == existingUser.Id && p.Status == ProposalStatus.Draft).ToListAsync();
            List<ProposalDTO> proposalDTOs = [];
            foreach (Proposal proposal in proposals) proposalDTOs.Add(new ProposalDTO(proposal));
            return Ok(proposalDTOs);
        }
        [Authorize]
        [HttpDelete(ApiEndPoints.DeleteProposalDraft)]
        public async Task<ActionResult<PDAPIResponse>> DeleteProposalDraft([FromQuery] string proposalId)
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();
            PDAPIResponse response = new();

            Guid parsedGuid = Guid.Parse(proposalId);

            try
            {
                Proposal? proposal = await _context.Proposals
                .Include(p => p.Author)
                .Include(p => p.Community)
                .FirstOrDefaultAsync(p => p.Id == parsedGuid);

                if (proposal == null)
                {
                    return NotFound("The proposal was not found.");
                }
                if(proposal.Author.Id != existingUser.Id)
                {
                    response.AddAlert("error", "You are not an author of this proposal");
                    return BadRequest(response);
                }
                if(proposal.Status != ProposalStatus.Draft)
                {
                    response.AddAlert("error", "You cannot delete a proposal that is not in draft state.");
                    return BadRequest(response);
                }
                _context.Proposals.Remove(proposal);
                await _context.SaveChangesAsync();
                response.AddAlert("success", "Proposal draft deleted successfully");
                response.SuccessfulOperation = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", $"Error deleting proposal draft: {ex.Message}");
                return BadRequest(response);
            }
        }
        [Authorize]
        [HttpPost(ApiEndPoints.PublishProposal)]
        public async Task<ActionResult<PDAPIResponse>> PublishProposal([FromQuery] string proposalId)
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();
            PDAPIResponse response = new();

            Guid parsedGuid = Guid.Parse(proposalId);

            try
            {
                Proposal? proposal = await _context.Proposals
                .Include(p => p.Author)
                .Include(p => p.Community)
                .FirstOrDefaultAsync(p => p.Id == parsedGuid);

                if (proposal == null)
                {
                    return NotFound("The proposal was not found.");
                }
                if (proposal.Author.Id != existingUser.Id)
                {
                    response.AddAlert("error", "You are not an author of this proposal");
                    return BadRequest(response);
                }
                if (proposal.Status != ProposalStatus.Draft)
                {
                    response.AddAlert("error", "You cannot publish a proposal that is not in draft state.");
                    return BadRequest(response);
                }
                if (string.IsNullOrEmpty(proposal.Content))
                {
                    response.AddAlert("error", "You cannot publish a proposal without content.");
                    return BadRequest(response);
                }
                proposal.Publish();

                //Add post to community: 
                string body = $"Proposal {proposal.Id} {proposal.Title} has been published and home owners can now vote on it. Click on the following link to navigate to the proposal's page: <a style=\"text-decoration: underline\" href=\"{_utilityClass.WebAppBaseUrl}{FrontEndPages.Proposal}?proposalId={proposal.Id}\">Proposal Page</a>";
                Post newPublishedProposalPost = new(proposal.Community, body);
                proposal.Community.AddPost(newPublishedProposalPost);
                //Send email to all citizens of the community: 
                string titleForAllUsers = $"A new proposal has been published in {proposal.Community.Name}: {proposal.Title}";
                string bodyForAllUsers = $"A new proposal has been published in your community {proposal.Community.Name}. Click on the following link to navigate to the proposal page: <a style=\"text-decoration: underline\" href=\"{_utilityClass.WebAppBaseUrl}{FrontEndPages.Proposal}?proposalId={proposal.Id}\">Proposal Page</a>. Homeowners can sign the proposal and it will pass with majority home owners votes.";
                //Send Email to all citizens of that community and add Notificatin for all citizens
                List<Task> sendingEmails = [];
                foreach (User citizen in proposal.Community.Citizens)
                {
                    citizen.AddNotification(titleForAllUsers, bodyForAllUsers);
                    sendingEmails.Add(_utilityClass.SendEmailAsync(citizen.Email, titleForAllUsers, bodyForAllUsers));
                }
                foreach (Task task in sendingEmails) await task;

                await _context.SaveChangesAsync();
                response.RedirectTo = $"{FrontEndPages.Proposal}?proposalId={proposal.Id}";
                response.AddAlert("success", "Proposal published successfully.");
                response.SuccessfulOperation = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", $"Error deleting proposal draft: {ex.Message}");
                return BadRequest(response);
            }
        }
    }
    #endregion
}
