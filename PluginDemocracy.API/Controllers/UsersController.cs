﻿using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.Data;
using PluginDemocracy.Models;
using PluginDemocracy.DTOs;
using Azure.Storage.Blobs;
using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;

namespace PluginDemocracy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(PluginDemocracyContext context, APIUtilityClass utilityClass) : ControllerBase
    {
        private readonly PluginDemocracyContext _context = context;
        private readonly APIUtilityClass _utilityClass = utilityClass;
        #region PUBLIC ENDPOINTS
        [HttpPost("signup")]
        public async Task<ActionResult<PDAPIResponse>> SignUp(UserDto registeringUser)
        {
            //Create response object
            PDAPIResponse apiResponse = new();

            //Check validity of input
            if (!ModelState.IsValid || !(registeringUser.Password.Length >= 7)) return BadRequest(ModelState);

            //Create new User object
            User newUser = UserDto.ReturnUserFromUserDto(registeringUser);

            //hash password && assign
            PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
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
            apiResponse.User = UserDto.ReturnUserDtoFromUser(newUser);

            return Ok(apiResponse);
        }
        [HttpPost("login")]
        public async Task<ActionResult<PDAPIResponse>> LogIn(LoginInfoDto loginInfo)
        {
            //Create response object
            PDAPIResponse apiResponse = new();

            //Look up user by email
            User? existingUser = await _context.Users.Include(u => u.Notifications).FirstOrDefaultAsync(u => u.Email == loginInfo.Email);
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
                apiResponse.User = UserDto.ReturnUserDtoFromUser(existingUser);
                //Send a SessionJWT to the client so that they can maintain a session
                apiResponse.SessionJWT = _utilityClass.CreateJWT(existingUser.Id, 7);
                //Redirect to home feed after login in or join community page if no community
                apiResponse.RedirectTo = FrontEndPages.CommunityFeed;
                return Ok(apiResponse);
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
                    apiResponse.User = UserDto.ReturnUserDtoFromUser(existingUser);

                    //Send an email saying that the email address has been confirmed
                    try
                    {
                        await _utilityClass.SendEmailAsync(toEmail: existingUser.Email, subject: _utilityClass.Translate(ResourceKeys.EmailOutEmailConfirmedTitle, existingUser.Culture), body: _utilityClass.Translate(ResourceKeys.EmailOutEmailConfirmedBody, existingUser.Culture));
                        return Ok(apiResponse);
                    }
                    catch (Exception ex)
                    {
                        apiResponse.AddAlert("error", $"Unable to send thank you for confirming email\nError:\n{ex.Message}");
                        return Ok(apiResponse);
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
        public async Task<ActionResult<PDAPIResponse>> ForgotPassword(LoginInfoDto loginInfo)
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
                    await _utilityClass.SendEmailAsync(toEmail: existingUser.Email, subject: _utilityClass.Translate(ResourceKeys.ResetPasswordEmailSubject, existingUser.Culture), body: _utilityClass.Translate(ResourceKeys.ResetPasswordEmailBody, existingUser.Culture) + $"/n<a href=\"{link}\">{link}</a>");
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
        public async Task<ActionResult<PDAPIResponse>> ResetPassword(LoginInfoDto loginInfoDto, [FromQuery] string token)
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
        #region AUTHRORIZED ENDPOINTS
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
                response.User = UserDto.ReturnUserDtoFromUser(existingUser);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", $"Unable to save changes to database\nError:\n{ex.Message}");
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        [Authorize]
        [HttpPost("updateaccount")]
        public async Task<ActionResult<PDAPIResponse>> UpdateAccount(UserDto userDto)
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
                response.User = UserDto.ReturnUserDtoFromUser(existingUser);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", $"Unable to save changes to database\nError:\n{ex.Message}");
                return StatusCode(500, response);
            }
            return Ok(response);
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
            if (!fileExtension.StartsWith(".")) fileExtension = "." + fileExtension;
            if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
            {
                response.AddAlert("error", "File extension is not jpg, jpeg, or png");
                return BadRequest(response);
            }

            //Connect to Blob service
            string blobSasURL = Environment.GetEnvironmentVariable("BlobSASURL") ?? string.Empty;
            if (string.IsNullOrEmpty(blobSasURL)) throw new Exception("BlobSASURL environment variable is null or empty");
            BlobContainerClient blobContainerClient = new(new Uri(blobSasURL));

            string blobName = $"user/profilepicture/{existingUser.Id}{fileExtension}";
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
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
                await using var fileStream = file.OpenReadStream();
                await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType }); //aqui esta el error
                existingUser.ProfilePicture = blobClient.Uri.ToString();
                await _context.SaveChangesAsync();
                response.AddAlert("success", _utilityClass.Translate(ResourceKeys.ProfilePictureUpdated, existingUser.Culture));
                response.User = UserDto.ReturnUserDtoFromUser(existingUser);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", $"Unable to save changes to database\nError:\n{ex.Message}");
                return BadRequest(response);
            }
        }
        [Authorize]
        [HttpGet("getnotification")]
        public async Task<ActionResult<PDAPIResponse>> GetNotification([FromQuery] int notificationId)
        {
            //Create response object
            PDAPIResponse response = new();
            //Extract User from claims
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User, response);
            if (existingUser == null) return BadRequest(response);
            existingUser = _context.Users.Include(u => u.Notifications).FirstOrDefault(u => u.Id == existingUser.Id);
            if (existingUser == null) return BadRequest(response);
            //Ensure that the notification belongs to the user
            Notification? notification = existingUser.Notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification == null)
            {
                response.AddAlert("error", "Notification does not belong to user");
                return BadRequest(response);
            }
            //Mark notification as read
            notification.Read = true;
            await _context.SaveChangesAsync();
            response.User = UserDto.ReturnUserDtoFromUser(existingUser);
            response.RedirectTo = FrontEndPages.GenericMessage;
            response.RedirectParameters["Title"] = notification.Title;
            response.RedirectParameters["Body"] = notification.Message;
            return Ok(response);
        }
        [Authorize]
        [HttpGet("UpdateNotifications")]
        public async Task<ActionResult<PDAPIResponse>> UpdateNotifications()
        {
            PDAPIResponse response = new();
            //Extract User from claims
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User, response);
            if (existingUser == null) return BadRequest(response);
            existingUser = _context.Users.Include(u => u.Notifications).FirstOrDefault(u => u.Id == existingUser.Id);
            if (existingUser == null) return BadRequest(response);
            response.User = UserDto.ReturnUserDtoFromUser(existingUser);
            return Ok(response);
        }
        #endregion AUTHORIZED ENDPOINTS
    }
}
