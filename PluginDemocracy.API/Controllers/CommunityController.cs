using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MigraDoc.DocumentObjectModel;
using Newtonsoft.Json;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.Data;
using PluginDemocracy.DTOs;
using PluginDemocracy.DTOs.CommunitiesDto;
using PluginDemocracy.Models;
using System.Globalization;
using MigraDoc;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Drawing;
using System;
using System.IO;
using MigraDoc.DocumentObjectModel.Shapes;
using System.Text;

namespace PluginDemocracy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityController(PluginDemocracyContext context, APIUtilityClass utilityClass, IConfiguration configuration) : ControllerBase
    {
        private readonly PluginDemocracyContext _context = context;
        private readonly APIUtilityClass _utilityClass = utilityClass;
        private readonly IConfiguration _configuration = configuration;

        [Authorize]
        [HttpPost(ApiEndPoints.RegisterCommunity)]
        public async Task<ActionResult<PDAPIResponse>> Register(ResidentialCommunityDTO communityDto)
        {
            
            PDAPIResponse response = new();
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) 
            { 
                response.AddAlert("error", "User from claims not found");
                return BadRequest(response); 
            }

            if (string.IsNullOrEmpty(communityDto.Name))
            {
                response.AddAlert("error", "Community name is required");
                return BadRequest(response);
            }
            if (string.IsNullOrEmpty(communityDto.Address))
            {
                response.AddAlert("error", "Community address is required");
                return BadRequest(response);
            }
            if (string.IsNullOrEmpty(communityDto.OfficialCurrency))
            {
                response.AddAlert("error", "Community official currency is required");
                return BadRequest(response);
            }
            if (communityDto.OfficialLanguages.Count == 0)
            {
                response.AddAlert("error", "Community official languages are required");
                return BadRequest(response);
            }
            if (communityDto.Homes.Count == 0)
            {
                response.AddAlert("error", "Community has no registered homes");
                return BadRequest(response);
            }
            //Create Community instance
            ResidentialCommunity newCommunity = new(communityDto.Name, communityDto.Address)
            {
                Name = communityDto.Name,
                Address = communityDto.Address,
                OfficialCurrency = communityDto.OfficialCurrency,
                Description = communityDto.Description,
            };
            foreach (CultureInfo language in communityDto.OfficialLanguages) newCommunity.AddOfficialLanguage(language);
            foreach (HomeDTO homeDTO in communityDto.Homes) newCommunity.AddHome(new Home(newCommunity, homeDTO.Number, homeDTO.InternalAddress));
            try
            {
                _context.ResidentialCommunities.Add(newCommunity);
                await _context.SaveChangesAsync();
                response.AddAlert("success", "Community created successfully");
                response.RedirectTo = FrontEndPages.JoinCommunity;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", ex.Message);
                return BadRequest(response);
            }
        }
        [HttpGet(ApiEndPoints.GetListOfAllCommunities)]
        public async Task<ActionResult<PDAPIResponse>> GetListOfAllCommunities()
        {
            PDAPIResponse response = new();
            try
            {
                List<ResidentialCommunity> communities = await _context.ResidentialCommunities.ToListAsync();
                foreach (ResidentialCommunity community in communities) response.AllCommunitiesDTO.Add(new ResidentialCommunityDTO()
                {
                    Id = community.Id,
                    Name = community.Name,
                    FullName = community.FullName,
                    Description = community.Description,
                    Address = community.Address,
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", ex.Message);
                return BadRequest(response);
            }
        }
        /// <summary>
        /// Used in JoinCommunity page to get list of homes for a community after a community is selected from the radio group
        /// </summary>
        /// <param name="commuityId">The Id of the community to search for</param>
        /// <returns>PDAPIResponse and the list of HomeDtos is located in PDAPIResponse.Community.Homes</returns>
        [HttpGet(ApiEndPoints.GetListOfHomesForCommunity)]
        public async Task<ActionResult<PDAPIResponse>> GetListOfHomesForCommunity([FromQuery] int communityId)
        {
            PDAPIResponse response = new();
            try
            {
                ResidentialCommunity? community = await _context.ResidentialCommunities.Include(c => c.Homes).ThenInclude(h => h.Residents).Include(c => c.Homes).ThenInclude(h => h.Ownerships).FirstOrDefaultAsync(c => c.Id == communityId);
                if (community == null)
                {
                    response.AddAlert("error", "Community not found");
                    return BadRequest(response);
                }
                response.CommunityDTO = ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(community);
                foreach (Home home in community.Homes) response.CommunityDTO.Homes.Add(new(home));
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", ex.Message);
                return BadRequest(response);
            }
        }
        /// <summary>
        /// Controller method by which a request to join a community is posted. 
        /// Community.AddJoinCommunityRequest makes validations. The controller needs to check that the user from claims 
        /// matches the user in the request and that the community matches but the rest of the validation is done in community
        /// If something doesn't work, the community will throw an exception and the controller will catch it and return a BadRequest. 
        /// Otherwise, save changes. The constructor for JoinCommunityReques also makes some verifications like you can only be joining 
        /// as owner or resident, not both.
        /// </summary>
        /// <param name="joinCommunityRequestUploadDTO"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(ApiEndPoints.JoinCommunityRequest)]
        public async Task<ActionResult<PDAPIResponse>> JoinCommunityRequest([FromForm] JoinCommunityRequestUploadDTO joinCommunityRequestUploadDTO)
        {
            PDAPIResponse pdApiresponse = new();
            if (joinCommunityRequestUploadDTO == null)
            {
                pdApiresponse.AddAlert("error", "Request is null");
                return BadRequest(pdApiresponse);
            }
            //Extract User from claims
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User, pdApiresponse);
            if (existingUser == null)
            {
                pdApiresponse.AddAlert("error", "User from claims not found");
                return BadRequest(pdApiresponse);
            }
            //Does the user from claims match the user in the request?
            if (joinCommunityRequestUploadDTO.CommunityId == 0)
            {
                pdApiresponse.AddAlert("error", "CommunityId for request is zero");
                return BadRequest(pdApiresponse);
            }
            if (joinCommunityRequestUploadDTO.HomeId == 0)
            {
                pdApiresponse.AddAlert("error", "HomeId is zero.");
                return BadRequest(pdApiresponse);
            }
            //Does the community exist?
            if (joinCommunityRequestUploadDTO.JoiningAsOwner == joinCommunityRequestUploadDTO.JoiningAsResident)
            {
                pdApiresponse.AddAlert("error", "Both flags for joining as owner and joining as resident set.");
                return BadRequest(pdApiresponse);
            }
            ResidentialCommunity? community = await _context.ResidentialCommunities.Include(c => c.Homes).FirstOrDefaultAsync(c => c.Id == joinCommunityRequestUploadDTO.CommunityId);
            if (community == null)
            {
                pdApiresponse.AddAlert("error", "Community not found in database with given Id.");
                return BadRequest(pdApiresponse);
            }
            if (joinCommunityRequestUploadDTO.JoiningAsOwner == false && joinCommunityRequestUploadDTO.JoiningAsResident == false)
            {
                pdApiresponse.AddAlert("error", "Both Request.JoiningAsOwner and Request.JoiningAsResident can't be false.");
                return BadRequest(pdApiresponse);
            }
            try
            {
                Home home = community.Homes.First(h => h.Id == joinCommunityRequestUploadDTO.HomeId);
                JoinCommunityRequest joinCommunityRequest = new(community, home, existingUser, joinCommunityRequestUploadDTO.JoiningAsOwner, joinCommunityRequestUploadDTO.OwnershipPercentage);
                community.AddJoinCommunityRequest(joinCommunityRequest);

                //Send notifications to the corresponding parties
                //Send notification to role that has the capability to accept new home owners. Lookup all the Roles that have CanEditHomeOwnership and CanEditResidency
                //Search for all the roles that have CanEditHomeOwnership and CanEditResidency in the Community
                //If user is joining as home owner, send notification to Roles with corresponding powers. If role is not there, default send notification to app admin.
                List<User?> roleHoldersWithJoinPower = community.Roles.Where(r => r.Powers.CanEditHomeOwnership && r.Powers.CanEditResidency).Select(r => r.Holder).ToList();
                string link = $"{_utilityClass.WebAppBaseUrl}{FrontEndPages.JoinCommunityRequests}?requestId={joinCommunityRequest.Id}";
                string body = $"{existingUser.FullName} has requested to join the community as a {(joinCommunityRequest.JoiningAsOwner ? $"home owner" : "resident")} for home {home.InternalAddress} in community {community.FullName}.Please follow the following link to accept, reject or review:\n <a href=\"{link}\">{link}</a>.";
                string? appManagerEmail = _configuration["PluginDemocracy:AppManagerEmail"];
                if (joinCommunityRequest.JoiningAsOwner)
                {
                    foreach (User? user in roleHoldersWithJoinPower)
                    {
                        if (user != null)
                        {
                            string title = _utilityClass.Translate(ResourceKeys.NewJoinRequest, user.Culture);
                            user.AddNotification(title, body);
                            await _utilityClass.SendEmailAsync(user.Email, title, body);
                        }
                    }
                    if (appManagerEmail != null) await _utilityClass.SendEmailAsync(appManagerEmail, _utilityClass.Translate(ResourceKeys.NewJoinRequest), body);
                }
                //If user is joining as resident, send notification to home owner AND roles with powers, default to app admin otherwise. 
                else
                {
                    List<User> usersToEmail = [];
                    foreach (User user in home.Owners) usersToEmail.Add(user);
                    foreach (User? user in roleHoldersWithJoinPower) if (user != null) usersToEmail.Add(user);
                    //send the emails and add the notifications
                    foreach (User user in usersToEmail)
                    {
                        user.AddNotification(_utilityClass.Translate(ResourceKeys.NewJoinRequest, user.Culture), body);
                        await _utilityClass.SendEmailAsync(user.Email, _utilityClass.Translate(ResourceKeys.NewJoinRequest, user.Culture), body);
                    }
                    if (appManagerEmail != null) await _utilityClass.SendEmailAsync(appManagerEmail, _utilityClass.Translate(ResourceKeys.NewJoinRequest), body);
                }
                //The emails and notifications have been sent.

                //At this point the request should be validated and the notifications sent. It can be saved to database. 
                //But before, I will save the files to the blob. If that fails, don't save any changes. 
                string blobContainerURL = Environment.GetEnvironmentVariable("BlobContainerURL") ?? string.Empty;
                string blobSASToken = Environment.GetEnvironmentVariable("BlobSASToken") ?? string.Empty;
                string readOnlyBlobSASToken = Environment.GetEnvironmentVariable("ReadOnlyBlobSASToken") ?? string.Empty;
                if (string.IsNullOrEmpty(blobContainerURL) || string.IsNullOrEmpty(blobSASToken) || string.IsNullOrEmpty(readOnlyBlobSASToken)) throw new Exception("One of the environment variables for blob storage is null or empty");
                BlobContainerClient containerClient = new(new Uri($"{blobContainerURL}?{blobSASToken}"));
                //Files to add to blob
                //but first save the JCR so that it has an Id
                await _context.SaveChangesAsync();
                foreach (IFormFile file in joinCommunityRequestUploadDTO.SupportingDocumentsToAdd)
                {
                    
                    string blobName = $"joinCommunityRequests/{joinCommunityRequest.Id}/{file.FileName}";
                    BlobClient blobClient = containerClient.GetBlobClient(blobName);
                    await using Stream fileStream = file.OpenReadStream();
                    //upload the document
                    await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = file.ContentType });
                    //Remove Sas write token: 
                    UriBuilder uriBuilder = new(blobClient.Uri);
                    System.Collections.Specialized.NameValueCollection query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                    query.Clear();
                    uriBuilder.Query = query.ToString();
                    string blobUrlWithoutSas = uriBuilder.ToString();
                    joinCommunityRequest.AddLinkToFile($"{blobUrlWithoutSas}?{readOnlyBlobSASToken}");
                }
                _context.SaveChanges();
                pdApiresponse.AddAlert("success", "Request sent successfully. La solicitud ha sido enviada.");
                pdApiresponse.RedirectTo = $"{FrontEndPages.JoinCommunityRequests}?requestId={joinCommunityRequest.Id}";
                pdApiresponse.SuccessfulOperation = true;
                return Ok(pdApiresponse);
            }
            catch (Exception exception)
            {
                pdApiresponse.AddAlert("error", exception.Message);
                return BadRequest(pdApiresponse);
            }
        }
        [Authorize]
        [HttpGet(ApiEndPoints.GetAllJoinCommunityRequestsForUser)]
        public async Task<ActionResult<List<JoinCommunityRequestDTO>>> GetAlLJoinCommunityRequestsForUser()
        {

            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();
            List<JoinCommunityRequest>? joinCommunityRequests = await _context.JoinCommunityRequests.Where(j => j.User.Id == existingUser.Id).ToListAsync();
            joinCommunityRequests ??= [];

            List<JoinCommunityRequestDTO> joinCommunityRequestDTOs = [];

            foreach (JoinCommunityRequest joinRequest in joinCommunityRequests)
            {
                JoinCommunityRequestDTO jcr = new()
                {
                    Id = joinRequest.Id,
                    CommunityDTO = new()
                    {
                        Name = joinRequest.Community.Name
                    },
                    JoiningAsOwner = joinRequest.JoiningAsOwner,
                    JoiningAsResident = joinRequest.JoiningAsResident,
                    Approved = joinRequest.Approved,
                    DateRequested = joinRequest.DateRequested
                };

                joinCommunityRequestDTOs.Add(jcr);
            }

            return Ok(joinCommunityRequestDTOs);
        }
        [Authorize]
        [HttpGet(ApiEndPoints.GetJoinCommunityRequest)]
        public async Task<ActionResult<JoinCommunityRequestDTO>> GetJoinCommunityRequest([FromQuery] int requestId)
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();
            JoinCommunityRequest? joinRequest = await _context.JoinCommunityRequests.Include(j => j.Community).Include(j => j.Home).Include(j => j.User).Include(j => j.Messages).ThenInclude(m => m.Sender).FirstOrDefaultAsync(j => j.Id == requestId);
            if (joinRequest == null) return BadRequest();
            //Only the user with the Id as the request and individuals with roles in the community can see the request
            //if this is the user from the request, return the request
            bool canThisPersonViewIt = joinRequest.User.Id == existingUser.Id;

            // Does this person have roles in this community?
            if (!canThisPersonViewIt)
            {
                canThisPersonViewIt = existingUser.Roles.Any(role =>
                    role.Community.Id == joinRequest.Community.Id &&
                    role.Powers.CanEditHomeOwnership &&
                    role.Powers.CanEditResidency);
            }
            if (existingUser.Admin == true) canThisPersonViewIt = true;
            if (!canThisPersonViewIt) return Forbid();
            return Ok(new JoinCommunityRequestDTO(joinRequest));
        }
        [Authorize]
        [HttpGet(ApiEndPoints.GetHomeForJoinCommunityRequestInfo)]
        public async Task<ActionResult<HomeDTO>> GetHomeForJoinCommunityRequestInfo([FromQuery] int requestId)
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();
            JoinCommunityRequest? joinRequest = await _context.JoinCommunityRequests.Include(j => j.Community).Include(j => j.Home).ThenInclude(h => h.Residents).Include(j => j.Home).ThenInclude(h => h.Ownerships).Include(j => j.User).FirstOrDefaultAsync(j => j.Id == requestId);
            if (joinRequest == null) return BadRequest();
            //Only the user with the Id as the request and individuals with roles in the community can see the request
            //if this is the user from the request, return the request
            bool canThisPersonViewIt = joinRequest.User.Id == existingUser.Id;

            // Does this person have roles in this community?
            if (!canThisPersonViewIt)
            {
                canThisPersonViewIt = existingUser.Roles.Any(role =>
                    role.Community.Id == joinRequest.Community.Id &&
                    role.Powers.CanEditHomeOwnership &&
                    role.Powers.CanEditResidency);
            }
            if (existingUser.Admin == true) canThisPersonViewIt = true;
            if (!canThisPersonViewIt) return Forbid();
            return Ok(new HomeDTO(joinRequest.Home));
        }
        [Authorize]
        [HttpPost(ApiEndPoints.AddAdditionalSupportingDocumentsToJoinCommunityRequest)]
        public async Task<ActionResult<PDAPIResponse>> AddAdditionalSupportingDocumentsToJoinCommunityRequest([FromForm] List<IFormFile> files, [FromQuery] int requestId)
        {
            PDAPIResponse pdApiResponse = new();
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User, pdApiResponse);
            if (existingUser == null) 
            { 
                pdApiResponse.AddAlert("error", "User from claims not found");
                return BadRequest(pdApiResponse); 
            }
            if (requestId == 0)
            {
                pdApiResponse.AddAlert("error", "petitionId is zero.");
                return BadRequest(pdApiResponse);
            }
            JoinCommunityRequest? joinCommunityRequest = await _context.JoinCommunityRequests.Include(j => j.Community).ThenInclude(c => c.Roles).ThenInclude(r => r.Holder).Include(j => j.Home).Include(j => j.User).FirstOrDefaultAsync(j => j.Id == requestId);
            if (joinCommunityRequest == null)
            {
                pdApiResponse.AddAlert("error", "Request not found");
                return BadRequest(pdApiResponse);
            }
            //Only the user from the request can upload files
            if (joinCommunityRequest.User.Id != existingUser.Id)
            {
                pdApiResponse.AddAlert("error", "User from claims does not match user from request.");
                return BadRequest(pdApiResponse);
            }
            try
            {
                string blobContainerURL = Environment.GetEnvironmentVariable("BlobContainerURL") ?? string.Empty;
                string blobSASToken = Environment.GetEnvironmentVariable("BlobSASToken") ?? string.Empty;
                string readOnlyBlobSASToken = Environment.GetEnvironmentVariable("ReadOnlyBlobSASToken") ?? string.Empty;
                if (string.IsNullOrEmpty(blobContainerURL) || string.IsNullOrEmpty(blobSASToken) || string.IsNullOrEmpty(readOnlyBlobSASToken)) throw new Exception("One of the environment variables for blob storage is null or empty");
                BlobContainerClient containerClient = new(new Uri($"{blobContainerURL}?{blobSASToken}"));
                //Files to add to blob
                foreach (IFormFile file in files)
                {
                    string blobName = $"joinCommunityRequests/{joinCommunityRequest.Id}/{file.FileName}";
                    BlobClient blobClient = containerClient.GetBlobClient(blobName);
                    await using Stream fileStream = file.OpenReadStream();
                    //upload the document
                    await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = file.ContentType });
                    //Remove Sas write token:
                    UriBuilder uriBuilder = new(blobClient.Uri);
                    System.Collections.Specialized.NameValueCollection query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                    query.Clear();
                    uriBuilder.Query = query.ToString();
                    string blobUrlWithoutSas = uriBuilder.ToString();
                    joinCommunityRequest.AddLinkToFile($"{blobUrlWithoutSas}?{readOnlyBlobSASToken}");
                }
                string link = $"{_utilityClass.WebAppBaseUrl}{FrontEndPages.JoinCommunityRequests}?requestId={joinCommunityRequest.Id}";

                string title = $"New files uploaded to join request {joinCommunityRequest.Id}.";
                string body = $"New files have been added to join community request {joinCommunityRequest.Id}. Follow this link: <a href=\"{link}\">{link}</a>";

                foreach (Role role in joinCommunityRequest.Community.Roles)
                {
                    if (role.Holder != null)
                    {
                        role.Holder.AddNotification(title, body);
                        await _utilityClass.SendEmailAsync(role.Holder.Email, title, body);
                    }
                }
                _context.SaveChanges();
                pdApiResponse.AddAlert("success", "Files uploaded successfully.");
                pdApiResponse.SuccessfulOperation = true;
                return Ok(pdApiResponse);
            }
            catch (Exception ex)
            {
                pdApiResponse.AddAlert("error", ex.Message);
                return BadRequest(pdApiResponse);
            }
        }
        [Authorize]
        [HttpPost(ApiEndPoints.AddMessageToJoinCommunityRequest)]
        public async Task<ActionResult<PDAPIResponse>> AddMessageToJoinCommunityRequest([FromBody] string message, [FromQuery] int requestId)
        {
            PDAPIResponse pdApiResponse = new();
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User, pdApiResponse);
            if (existingUser == null)
            {
                pdApiResponse.AddAlert("error", "User from claims not found");
                return BadRequest(pdApiResponse);
            }
            if (requestId == 0)
            {
                pdApiResponse.AddAlert("error", "requestId is zero.");
                return BadRequest(pdApiResponse);
            }
            if (string.IsNullOrEmpty(message))
            {
                pdApiResponse.AddAlert("error", "Message text is null or empty.");
                return BadRequest(pdApiResponse);
            }
            JoinCommunityRequest? joinCommunityRequest = await _context.JoinCommunityRequests.Include(j => j.Community).ThenInclude(c => c.Roles).ThenInclude(r => r.Holder).Include(j => j.Home).Include(j => j.User).FirstOrDefaultAsync(j => j.Id == requestId);

            if (joinCommunityRequest == null)
            {
                pdApiResponse.AddAlert("error", "Request not found");
                return BadRequest(pdApiResponse);
            }
            //User needs to be requester from joinCommunityRequest, Role with permissions or admin
            bool canThisPersonViewIt = joinCommunityRequest.User.Id == existingUser.Id;

            // Does this person have roles in this community?
            if (!canThisPersonViewIt)
            {
                canThisPersonViewIt = existingUser.Roles.Any(role =>
                    role.Community.Id == joinCommunityRequest.Community.Id &&
                    role.Powers.CanEditHomeOwnership &&
                    role.Powers.CanEditResidency);
            }
            if (existingUser.Admin == true) canThisPersonViewIt = true;
            if (!canThisPersonViewIt) 
            {
                pdApiResponse.AddAlert("error", "You don't have permission to add a message.");
                return Forbid(); 
            }
            try
            {
                joinCommunityRequest.AddMessage(existingUser, message);
                //if the person leaving the message is not the requester, send a notification to the requester
                if (joinCommunityRequest.User.Id != existingUser.Id)
                {
                    string title = "New message on your request to join community.";
                    string body = $"{existingUser.FullName} has left a message on your join request for community {joinCommunityRequest.Community.Name} for home {joinCommunityRequest.Home.FullAddress}. Please follow the following link to review the message: <a href=\"{_utilityClass.WebAppBaseUrl}{FrontEndPages.JoinCommunityRequests}?requestId={joinCommunityRequest.Id}\">{_utilityClass.WebAppBaseUrl}{FrontEndPages.JoinCommunityRequests}?requestId={joinCommunityRequest.Id}</a>.";
                    joinCommunityRequest.User.AddNotification(title, body);
                    await _utilityClass.SendEmailAsync(joinCommunityRequest.User.Email, title, body);
                }
                //if the person leaving the message is the requester, send message to all individuals in this community that have roles powers needed for the type of request
                else
                {
                    string title = "New message on a join request.";
                    string body = $"{existingUser.FullName} has left a message on a join request for community {joinCommunityRequest.Community.Name} for home {joinCommunityRequest.Home.FullAddress}. Please follow the following link to review the message: <a href=\"{_utilityClass.WebAppBaseUrl}{FrontEndPages.JoinCommunityRequests}?requestId={joinCommunityRequest.Id}\">{_utilityClass.WebAppBaseUrl}{FrontEndPages.JoinCommunityRequests}?requestId={joinCommunityRequest.Id}</a>.";
                    List<User?> roleHoldersWithJoinPower = joinCommunityRequest.Community.Roles.Where(r => r.Powers.CanEditHomeOwnership && r.Powers.CanEditResidency).Select(r => r.Holder).ToList();
                    foreach (User? user in roleHoldersWithJoinPower)
                    {
                        if (user != null)
                        {
                            user.AddNotification(title, body);
                            await _utilityClass.SendEmailAsync(user.Email, title, body);
                        }
                    }
                }
                await _context.SaveChangesAsync();
                pdApiResponse.SuccessfulOperation = true;
                pdApiResponse.AddAlert("success", "Message added successfully.");
                return Ok(pdApiResponse);
            }
            catch (Exception ex)
            {
                pdApiResponse.AddAlert("error", ex.Message);
                return BadRequest(pdApiResponse);
            }
        }
        [Authorize]
        [HttpPost(ApiEndPoints.AcceptOrRejectJoinCommunityRequest)]
        public async Task<ActionResult<PDAPIResponse>> AcceptOrRejectJoinCommunityRequest([FromBody] bool accepted, [FromQuery] int requestId)
        {
            PDAPIResponse apiResponse = new();
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User, apiResponse);
            if (existingUser == null)
            {
                apiResponse.AddAlert("error", "User from claims not found");
                return BadRequest(apiResponse);
            }
            if (requestId == 0)
            {
                apiResponse.AddAlert("error", "Invalid request Id.");
                return BadRequest(apiResponse);
            }
            JoinCommunityRequest? joinCommunityRequest = await _context.JoinCommunityRequests.Include(j => j.Community).ThenInclude(c => c.Roles).Include(j => j.Home).Include(j => j.User).FirstOrDefaultAsync(j => j.Id == requestId);
            if (joinCommunityRequest == null)
            {
                apiResponse.AddAlert("error", "No request found with given id.");
                return BadRequest(apiResponse);
            }
            //Community method already checks if the user is a role holder or an admin.
            try
            {
                if (accepted) 
                { 
                    joinCommunityRequest.Community.ApproveJoinCommunityRequest(joinCommunityRequest, existingUser);
                    string title = "Join Request Approved";
                    string body = $"Your request to join {joinCommunityRequest.Community.Name} community has been approved for home {joinCommunityRequest.Home.FullAddress} as a {(joinCommunityRequest.JoiningAsOwner ? "owner" : "resident")}. Welcome to the community.";
                    joinCommunityRequest.User.AddNotification(title, body);
                    await _utilityClass.SendEmailAsync(joinCommunityRequest.User.Email, title, body);
                }
                else 
                { 
                    joinCommunityRequest.Community.RejectJoinCommunityRequest(joinCommunityRequest, existingUser);
                    string title = "Join Request Rejected";
                    string body = $"Your request to join {joinCommunityRequest.Community.Name} community has been rejected for home {joinCommunityRequest.Home.FullAddress} as a {(joinCommunityRequest.JoiningAsOwner ? "owner" : "resident")}. If you think this was an error, contact an admin for the community or send another join request.";
                    joinCommunityRequest.User.AddNotification(title, body);
                    await _utilityClass.SendEmailAsync(joinCommunityRequest.User.Email, title, body);
                }
                await _context.SaveChangesAsync();
                apiResponse.SuccessfulOperation = true;
                apiResponse.AddAlert("success", "Request accepted or rejected successfully.");
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.AddAlert("error", ex.Message);
                return BadRequest(apiResponse);
            }
        }
        [Authorize]
        [HttpPost(ApiEndPoints.CreateNewPost)]
        public async Task<ActionResult<PDAPIResponse>> CreateNewPost([FromForm] CreatePostRequestDto request)
        {
            PDAPIResponse response = new();
            //Extract User from claims
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User, response);
            if (existingUser == null)
            {
                response.AddAlert("error", "User from claims not found");
                return BadRequest(response);
            }
            //Check the contents of the post
            if (string.IsNullOrEmpty(request.Body))
            {
                response.AddAlert("error", "Post body was empty and as such no action was taken. There must either be a text body in the post, images or both.");
                return response;
            }
            Post newPost = new(existingUser, request.Body);
            ResidentialCommunity? community = await _context.ResidentialCommunities.FirstOrDefaultAsync(c => c.Id == request.CommunityId);
            if (community == null)
            {
                response.AddAlert("error", "Community not found");
                return BadRequest(response);
            }
            community.AddPost(newPost);
            try
            {

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.AddAlert("error", $"There was an internal error creating a new post. {ex.Message}");
                return StatusCode(500, response);
            }

            //Handle image uploads
            try
            {
                string blobSasUrl = Environment.GetEnvironmentVariable("BlobSasUrl") ?? string.Empty;
                if (string.IsNullOrEmpty(blobSasUrl)) throw new Exception("BlobSASURL environment variable is null or empty");
                BlobContainerClient containerClient = new(new Uri(blobSasUrl));
                string readOnlyBlobSASToken = Environment.GetEnvironmentVariable("ReadOnlyBlobSASToken") ?? string.Empty;

                foreach (IFormFile file in request.Files)
                {
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                    {
                        response.AddAlert("error", $"File type not supported. Only .jpg, .jpeg, and .png are supported. Filename: {file.Name}");
                        return BadRequest(response);
                    }

                    Guid guid = Guid.NewGuid();
                    string blobName = $"community/{community.Id}/posts/{newPost.Id}/images/{guid}";

                    //Create a blob client for the image
                    BlobClient blobClient = containerClient.GetBlobClient(blobName);
                    await using Stream filestream = file.OpenReadStream();
                    //Upload the image
                    await blobClient.UploadAsync(filestream, new BlobHttpHeaders { ContentType = file.ContentType });
                    //Remove Sas Token: 
                    UriBuilder uriBuilder = new(blobClient.Uri);
                    System.Collections.Specialized.NameValueCollection query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                    query.Clear();
                    uriBuilder.Query = query.ToString();
                    string blobUrlWithoutSas = uriBuilder.ToString();
                    newPost.AddImage($"{blobUrlWithoutSas}?{readOnlyBlobSASToken}");
                }
                await _context.SaveChangesAsync();
                response.AddAlert("success", _utilityClass.Translate(ResourceKeys.PostCreatedSuccessfully, existingUser.Culture));
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", $"There was an internal error creating a new post. {ex.Message}");
                return StatusCode(500, response);
            }
        }
        [Authorize]
        [HttpGet(ApiEndPoints.GetFeed)]
        public async Task<ActionResult<PDAPIResponse>> GetFeed([FromQuery] int communityId)
        {
            PDAPIResponse response = new();
            try
            {

                //Extract User from claims
                User? existingUser = await _utilityClass.ReturnUserFromClaims(User, response);
                if (existingUser == null)
                {
                    response.AddAlert("error", "User from claims not found");
                    return BadRequest(response);
                }
                ResidentialCommunity? community = await _context.ResidentialCommunities
                    .Include(c => c.Posts)
                        .ThenInclude(p => p.Reactions)
                    .Include(c => c.Posts)
                        .ThenInclude(p => p.Comments)
                            .ThenInclude(c => c.Author)
                    .FirstOrDefaultAsync(c => c.Id == communityId);
                if (community == null)
                {
                    response.AddAlert("error", "Community not found");
                    return BadRequest(response);
                }

                foreach (Post post in community.PostsByLatestActivity) response.PostsDTO.Add(new PostDTO(post));
                var settings = new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Auto,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                string json = JsonConvert.SerializeObject(response, settings);
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                response.AddAlert("error", ex.Message);
                return BadRequest(response);
            }

        }
        [Authorize]
        [HttpDelete(ApiEndPoints.DeletePost)]
        public async Task<ActionResult<PDAPIResponse>> DeletePost([FromQuery] int postId)
        {
            PDAPIResponse response = new();
            //Extract User from claims
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User, response);
            if (existingUser == null)
            {
                response.AddAlert("error", "User from claims not found");
                return BadRequest(response);
            }
            Post? post = await _context.Posts.Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == postId);

            try
            {
                if (post == null)
                {
                    response.AddAlert("error", "Post not found");
                    return BadRequest(response);
                }
                if (post?.Author is User user && user.Id != existingUser.Id)
                {
                    response.AddAlert("error", "User does not have permission to delete this post");
                    return BadRequest(response);
                }
                //If post has images in the blob storage, delete them
#pragma warning disable CS8602 // Suppressing warning dereference of a possibly null reference because checking for nullability inside try statement.
                if (post.ImagesLinks.Count != 0)
                {
                    string blobSasUrl = Environment.GetEnvironmentVariable("BlobSasUrl") ?? string.Empty;
                    if (string.IsNullOrEmpty(blobSasUrl))
                    {
                        response.AddAlert("error", "Blob SAS URL is null or empty");
                        return BadRequest(response);
                    }
                    BlobContainerClient containerClient = new(new Uri(blobSasUrl));

                    // Define the directory containing the post images
                    string postImagesDirectory = $"posts/{post.Id}/images/";
                    // List all blobs in the directory
                    await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: postImagesDirectory))
                    {
                        // Get a reference to the blob and delete it
                        BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                        await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
                    }
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                response.AddAlert("success", "Post deleted successfully");
                response.SuccessfulOperation = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", ex.Message);
                return BadRequest(response);
            }
        }
        [Authorize]
        [HttpPost(ApiEndPoints.ReactToPost)]
        public async Task<ActionResult<List<PostReactionDTO>>> LikePost(PostReactionDTO reactionDto)
        {
            //Extract User from claims
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();

            Post? post = await _context.Posts.Include(p => p.Reactions).FirstOrDefaultAsync(p => p.Id == reactionDto.PostId);
            if (post == null) return BadRequest();

            try
            {
                post.React(existingUser, reactionDto.ReactionType);
                await _context.SaveChangesAsync();
                PostDTO postDto = new(post);
                return Ok(postDto.Reactions);
            }
            catch
            {
                return BadRequest();
            }
        }
        [Authorize]
        [HttpPost(ApiEndPoints.AddCommentToPost)]
        public async Task<ActionResult<PostDTO>> AddCommentToPost(PostCommentDTO postCommentDto)
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();
            Post? post = await _context.Posts.Include(p => p.Reactions).Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == postCommentDto.PostId);
            if (post == null) return BadRequest();

            try
            {
                post.AddComment(existingUser, postCommentDto.Comment);
                await _context.SaveChangesAsync();
                PostDTO postDto = new(post);
                return Ok(postDto);
            }
            catch
            {
                return BadRequest();
            }
        }
        [Authorize]
        [HttpDelete(ApiEndPoints.DeleteComment)]
        public async Task<ActionResult<PDAPIResponse>> DeleteComment(PostCommentDTO postCommentDto)
        {
            PDAPIResponse response = new();
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) 
            {
                response.AddAlert("error", "Users from claims not found.");
                return BadRequest();
            }
            Post? post = await _context.Posts.Include(p => p.Author).Include(p => p.Reactions).Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == postCommentDto.PostId);
            if (post == null) return BadRequest();

            PostComment? comment = post.Comments.FirstOrDefault(c => c.Id == postCommentDto.Id);
            if (comment == null) return BadRequest();

            if (comment.Author.Id != existingUser.Id) return BadRequest();

            try
            {
                post.RemoveComment(comment);
                await _context.SaveChangesAsync();
                response.SuccessfulOperation = true;
                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }
        [Authorize]
        [HttpGet(ApiEndPoints.GetListOfAvatarUsersForACommunity)]
        public async Task<ActionResult<List<UserDTO>>> GetListOfAvatarUsersForACommunity([FromQuery] int communityId)
        {
            //You need to be admin, a citizen of this community or someone holding a role for this community
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest("Users from headers not found.");
            ResidentialCommunity? residentialCommunity = null;
            residentialCommunity = _context.ResidentialCommunities.Include(rc => rc.Homes).ThenInclude(h => h.Residents).Include(rc => rc.Homes).ThenInclude(h => h.Ownerships).Include(rc => rc.Roles).FirstOrDefault(rc => rc.Id == communityId);
            if (residentialCommunity == null) return BadRequest("Community not found.");
            //Only an admin, a citizen of this community or someone holding a role for this community can see the list of users
            //Is existingUser a citizen of this community, holds a role or is admin? 
            if (!existingUser.Citizenships.Any(c => c.Id == communityId) && !existingUser.Roles.Any(r => r.Community.Id == communityId) && !existingUser.Admin) return Forbid("User does not have permission to view this information.");
            //Make list of userAvatars            
            List<UserDTO> userAvatars = [];
            foreach (User user in residentialCommunity.Citizens)
            {
                userAvatars.Add(UserDTO.ReturnAvatarMinimumUserDTOFromUser(user));
            }
            return Ok(userAvatars);
        }
        [HttpGet(ApiEndPoints.GetCommunityAbout)]
        public async Task<ActionResult<ResidentialCommunityDTO>> GetCommunityAbout([FromQuery] int communityId)
        {
            //Include Homes, include roles
            ResidentialCommunity? community = await _context.ResidentialCommunities.Include(c => c.Homes).ThenInclude(h => h.Ownerships).Include(c => c.Homes).ThenInclude(h => h.Residents).Include(c => c.Roles).ThenInclude(r => r.Holder).Include(c => c.Petitions).FirstOrDefaultAsync(c => c.Id == communityId);
            if (community == null) return BadRequest("Community not found.");
            ResidentialCommunityDTO communityDTO = new(community);
            return Ok(communityDTO);
        }
        [Authorize]
        [HttpGet(ApiEndPoints.RolesGetListOfJCRequestsForGivenCommunity)]
        public async Task<ActionResult<List<JoinCommunityRequestDTO>>> RoleGetListOfJCRequestsForCommunity([FromQuery] int communityId)
        {
            //Make sure user has permissions to see this community. Does 
            //the user show up as a role for this community?
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();
            if (!existingUser.Roles.Any(r => r.Community.Id == communityId)) return Forbid();
            List<JoinCommunityRequest> joinCommunityRequests = await _context.JoinCommunityRequests.Include(jcr => jcr.User).Include(jcr => jcr.Home).Include(jcr => jcr.Community).Where(jcr => jcr.Community.Id == communityId).ToListAsync();
            List<JoinCommunityRequestDTO> joinCommunityRequestDTOs = [];
            foreach (JoinCommunityRequest joinRequest in joinCommunityRequests.Where(jcr => jcr.Approved == null))
            {
                JoinCommunityRequestDTO jcr = new()
                {
                    Id = joinRequest.Id,
                    UserDTO = UserDTO.ReturnAvatarMinimumUserDTOFromUser(joinRequest.User),
                    CommunityDTO = new()
                    {
                        Id = joinRequest.Community.Id,
                        Name = joinRequest.Community.Name,
                        FullName = joinRequest.Community.FullName
                    },
                    JoiningAsOwner = joinRequest.JoiningAsOwner,
                    JoiningAsResident = joinRequest.JoiningAsResident,
                    Approved = joinRequest.Approved,
                    DateRequested = joinRequest.DateRequested,
                    HomeDTO = new()
                    {
                        Id = joinRequest.Home.Id,
                        Number = joinRequest.Home.Number,
                        InternalAddress = joinRequest.Home.InternalAddress,
                        FullAddress = joinRequest.Home.FullAddress
                    }
                };

                joinCommunityRequestDTOs.Add(jcr);
            }
            return Ok(joinCommunityRequestDTOs);
        }
        [Authorize]
        [HttpGet(ApiEndPoints.GetPetition)]
        public async Task<ActionResult<PetitionDTO>> GetPetition([FromQuery] int? petitionId)
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Petition? petition = await _context.Petitions.Include(p => p.Authors).Include(p => p.Community).ThenInclude(c => c.Homes).ThenInclude(h => h.Ownerships).ThenInclude(ho => ho.Owner).Include(p => p.Signatures).ThenInclude(s => s.Signer).FirstOrDefaultAsync(p => p.Id == petitionId);

            if (petition == null) return BadRequest("Petition not found.");
            if (!petition.Published) return BadRequest("The petition has not been published.");
            //Get petitionDTO ready:
            PetitionDTO petitionDTO = new(petition);
            if (petition.Community == null) 
            {
                return BadRequest("Petition.Community is null");
            }
            petitionDTO.CommunityDTO = ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(petition.Community);
            //In order to display stats on the frontend, I will need information about the community
            foreach (Home home in petition.Community.Homes)
            {
                HomeDTO homeDTO = new()
                {
                    Id = home.Id,
                    Number = home.Number,
                    InternalAddress = home.InternalAddress,
                    FullAddress = home.FullAddress
                };
                //Get the residents of the house
                foreach (User resident in home.Residents)
                {
                    homeDTO.Residents.Add(UserDTO.ReturnAvatarMinimumUserDTOFromUser(resident));
                }
                //get the home ownerships of the house
                foreach (HomeOwnership homeOwnership in home.Ownerships)
                {
                    HomeOwnershipDTO homeOwnershipDTO = new()
                    {
                        Id = homeOwnership.Id,
                        Owner = UserDTO.ReturnAvatarMinimumUserDTOFromUser(homeOwnership.Owner),
                        OwnershipPercentage = homeOwnership.OwnershipPercentage
                    };
                    homeDTO.Ownerships.Add(homeOwnershipDTO);
                }
                //Add home to PetitionDTO.CommunityDTO
                petitionDTO.CommunityDTO.Homes.Add(homeDTO);
            }
            return Ok(petitionDTO);
        }
        //[Authorize]
        [HttpGet(ApiEndPoints.GeneratePDFOfPetition)]
        public async Task<FileResult?> GeneratePDFOfPetition([FromQuery] int? petitionId)
        {
            //User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            //if (existingUser == null) return BadRequest();
            Petition? petition = await _context.Petitions.Include(p => p.Authors).Include(p => p.Community).ThenInclude(c => c.Homes).ThenInclude(h => h.Residents).Include(p => p.Community).ThenInclude(c => c.Homes).ThenInclude(h => h.Ownerships).ThenInclude(ho => ho.Owner).Include(p => p.Signatures).ThenInclude(s => s.Signer).FirstOrDefaultAsync(p => p.Id == petitionId);
            
            if (petition == null) return null;
            if (!petition.Published) return null;
            if (petition.Community == null) return null;
            if (petition.Title == null) return null;
            if (petition.Description == null) return null;
            if (petition.ActionRequested == null) return null;
            if (petition.SupportingArguments == null) return null;

            //CREATE A MIGRADOC DOCUMENT
            Document document = new();
            document.Info.Title = petition.Title;
            document.Info.Subject = petition.Title;
            string authors = string.Empty;
            foreach (User author in petition.Authors) authors += $"{author.FullName}, ";
            authors = authors.TrimEnd(',', ' ');
            document.Info.Author = authors;
            List<string> tempFilesToDelete = [];
            //DEFINE STYLES
            // Get the predefined style Normal.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Style style = document.Styles["Normal"];
            style.ParagraphFormat.SpaceAfter = 2;
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Times New Roman";

            // Heading1 to Heading9 are predefined styles with an outline level. An outline level
            // other than OutlineLevel.BodyText automatically creates the outline (or bookmarks) 
            // in PDF.

            style = document.Styles["Heading1"];
            //style.Font.Name = "Tahoma"; //I dont have it on my system and it was crashing it
            style.Font.Size = 18;
            style.Font.Bold = true;
            style.Font.Color = Colors.DarkGreen;
            style.ParagraphFormat.PageBreakBefore = true;
            style.ParagraphFormat.SpaceAfter = 6;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            style = document.Styles["Heading2"];
            style.Font.Size = 12;
            style.Font.Bold = true;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading3"];
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 3;

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called TextBox based on style Normal
            style = document.Styles.AddStyle("TextBox", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            style.ParagraphFormat.Shading.Color = Colors.SkyBlue;

            // Create a new style called TOC based on style Normal
            style = document.Styles.AddStyle("TOC", "Normal");
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style.ParagraphFormat.Font.Color = Colors.DarkGreen;

            //DEFINE COVER PAGE
            //Get QR code for Petition
            // Step 1: Generate the QR code as a byte array using your existing utility method
            string urlToPetition = _utilityClass.WebAppBaseUrl + FrontEndPages.Petition + $"?petitionId={petition.Id}";
            byte[] qrCodeImage = _utilityClass.GenerateQRCode(urlToPetition);

            // convert the qr code image to ARGB format
            byte[] convertedQrCodeImage = _utilityClass.ConvertPngToArgb(qrCodeImage);

            // Step 2: Save the byte array as a temporary image file
            string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");
            System.IO.File.WriteAllBytes(tempFilePath, convertedQrCodeImage);
            tempFilesToDelete.Add(tempFilePath);

            Section section = document.AddSection();
            Paragraph paragraph = section.AddParagraph($"{petition.Title}" ,"Heading1");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph = section.AddParagraph($"This is a petition by the citizens of {petition.Community.FullName} for the board members of the Home Owners Association.", "Heading2");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph = section.AddParagraph($"Author(s): {authors}");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph = section.AddParagraph();
            Image qrCodeForPetition = paragraph.AddImage(tempFilePath);
            qrCodeForPetition.Height = Unit.FromCentimeter(3); // Set the height to 5 cm
            qrCodeForPetition.Width = Unit.FromCentimeter(3); // Set the width to 5 cm
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph = section.AddParagraph($"Date this document was rendered: {DateTime.UtcNow.Date}");
            #region TABLE OF CONTENTS
            //DEFINE TABLE OF CONTENTS
            section = document.LastSection;

            section.AddPageBreak();
            paragraph = section.AddParagraph("Table of Contents");
            paragraph.Format.Font.Size = 14;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.SpaceAfter = 24;
            paragraph.Format.OutlineLevel = OutlineLevel.Level1;

            paragraph = section.AddParagraph();
            paragraph.Style = "TOC";
            Hyperlink hyperlink = paragraph.AddHyperlink("Petition Statistics");
            hyperlink.AddText("Petition Statistics\t");
            hyperlink.AddPageRefField("Petition Statistics");

            paragraph = section.AddParagraph();
            paragraph.Style = "TOC";
            hyperlink = paragraph.AddHyperlink("Petition");
            hyperlink.AddText("Petition\t");
            hyperlink.AddPageRefField("Petition");

            paragraph = section.AddParagraph();
            paragraph.Style = "TOC";
            hyperlink = paragraph.AddHyperlink("Appendixes");
            hyperlink.AddText("Appendixes\t");
            hyperlink.AddPageRefField("Appendixes");

            paragraph = section.AddParagraph();
            paragraph.Style = "TOC";
            hyperlink = paragraph.AddHyperlink("Appendix 1: E-Signatures");
            hyperlink.AddText("Appendix 1: E-Signatures\t");
            hyperlink.AddPageRefField("Appendix 1: E-Signatures");
            
            int apIndex = 1;
            foreach (string link in petition.LinksToSupportingDocuments)
            {
                apIndex++;
                paragraph = section.AddParagraph();
                paragraph.Style = "TOC";
                hyperlink = paragraph.AddHyperlink($"Appendix {apIndex}: Attached Document");
                hyperlink.AddText($"Appendix {apIndex}: Attached Document\t");
                hyperlink.AddPageRefField($"Appendix {apIndex}: Attached Document");
            }
            paragraph = section.AddParagraph();
            paragraph.Style = "TOC";
            hyperlink = paragraph.AddHyperlink("Board Members Signatures");
            hyperlink.AddText("Board Members Signatures\t");
            hyperlink.AddPageRefField("Board Members Signatures");
            #endregion
            //DEFINE CONTENT SECTION
            section = document.AddSection();
            section.PageSetup.OddAndEvenPagesHeaderFooter = true;
            //section.PageSetup.StartingNumber = 1;
            section.PageSetup.PageFormat = PageFormat.A4;

            // Create a paragraph with centered page number. See definition of style "Footer".
            //paragraph = new Paragraph();
            paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddTab();
            paragraph.AddPageField();
            paragraph.AddText(" of ");
            paragraph.AddNumPagesField();
            // Add paragraph to footer for odd pages.
            //section.Footers.Primary.Add(paragraph);
            // Add clone of paragraph to footer for odd pages. Cloning is necessary because an object must
            // not belong to more than one other object. If you forget cloning an exception is thrown.
            section.Footers.EvenPage.Add(paragraph.Clone());

            #region STATS OF PETITION
            double amountOfHomesThatHaveSigned = 0;
            double amountOfHomesNeededForMajority = 0;
            double amountOfHomesThatHaventSigned = 0;
            double amountOfSignaturesFromHomeOwners = 0;
            double amountOfSignaturesFromNonOwningResidents = 0;
            bool majorityHomeOwnerSigned;
            //amountOfHomesThatHaveSigned
            double homesThatSigned = 0; 
            foreach (Home home in petition.Community.Homes)
            {
                foreach (KeyValuePair<User, double> ownersOwnerships in home.OwnersOwnerships)
                {
                    if (petition.Signatures.Any(s => s.Signer.Id == ownersOwnerships.Key.Id)) homesThatSigned += ownersOwnerships.Value;
                }
            }
            amountOfHomesThatHaveSigned = homesThatSigned / 100;
            //amountOfHomesNeededForMajority
            int totalHomes = petition.Community.Homes.Count;
            amountOfHomesNeededForMajority = totalHomes / 2 + 1;

            //amountOfHomesThatHaventSigned
            amountOfHomesThatHaventSigned = petition.Community.Homes.Count - amountOfHomesThatHaveSigned;

            //amountOfSignaturesFromHomeOwners &
            //amountOfSignaturesFromNonOwningResidents
            double signaFromHomeOwners = 0;
            double signaFromResidents = 0;
            foreach (ESignature eSignature in petition.Signatures)
            {
                if (petition.Community.Homes.Any(h => h.OwnersOwnerships.Any(o => o.Key.Id == eSignature.Signer.Id))) signaFromHomeOwners++;
                else if (petition.Community.Homes.Any(h => h.Residents.Any(r => r.Id == eSignature.Signer.Id))) signaFromResidents++;
            }
            amountOfSignaturesFromHomeOwners = signaFromHomeOwners;
            amountOfSignaturesFromNonOwningResidents = signaFromResidents;

            majorityHomeOwnerSigned = amountOfHomesThatHaveSigned >= amountOfHomesNeededForMajority;

            paragraph = document.LastSection.AddParagraph($"Petition Statistics", "Heading1");
            paragraph.AddBookmark("Petition Statistics");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            Table table = new();
            table.Borders.Width = 0.75;
            Column column = table.AddColumn(Unit.FromCentimeter(2));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(5));

            Row row = table.AddRow();
            row.Shading.Color = Colors.Green;
            Cell cell = row.Cells[0];
            cell.AddParagraph("Statistic");
            cell = row.Cells[1];
            cell.AddParagraph("Result");

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Majority homeowner signed:");
            cell = row.Cells[1];
            string result = majorityHomeOwnerSigned ? "Yes" : "No";
            cell.AddParagraph(result);

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Total Signatures:");
            cell = row.Cells[1];
            cell.AddParagraph(petition.Signatures.Count.ToString());

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Total Number of Homes in Community:");
            cell = row.Cells[1];
            cell.AddParagraph(petition.Community.Homes.Count.ToString());

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Amount of homes that have signed:");
            cell = row.Cells[1];
            cell.AddParagraph(amountOfHomesThatHaveSigned.ToString());

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Amount of homes needed for majority:");
            cell = row.Cells[1];
            cell.AddParagraph(amountOfHomesNeededForMajority.ToString());

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Amount of homes that haven't signed:");
            cell = row.Cells[1];
            cell.AddParagraph(amountOfHomesThatHaventSigned.ToString());

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Amount of signatures from home owners:");
            cell = row.Cells[1];
            cell.AddParagraph(amountOfSignaturesFromHomeOwners.ToString());

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Amount of signatures from non owning residents:");
            cell = row.Cells[1];
            cell.AddParagraph(amountOfSignaturesFromNonOwningResidents.ToString());

            table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            document.LastSection.Add(table);

            #endregion
            #region PETITION
            //DEFINE PARAGRAPHS
            section = document.AddSection();
            paragraph = document.LastSection.AddParagraph($"Petition: {petition.Title}", "Heading1");
            paragraph.AddBookmark("Petition");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph = document.LastSection.AddParagraph($"Petition for board members of HOA {petition.Community.FullName}/\nAuthor(s): {authors}.");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph = document.LastSection.AddParagraph("Description", "Heading2");
            paragraph.AddBookmark("Petition's Description");
            paragraph = document.LastSection.AddParagraph(petition.Description);
            paragraph.AddLineBreak();

            paragraph = document.LastSection.AddParagraph("Action Requested", "Heading2");
            paragraph.AddBookmark("Petition's Action Requested");
            paragraph = document.LastSection.AddParagraph(petition.ActionRequested);
            paragraph.AddLineBreak();

            paragraph = document.LastSection.AddParagraph("Petition's Supporting Arguments", "Heading2");
            paragraph.AddBookmark("Petition's Supporting Arguments");
            paragraph = document.LastSection.AddParagraph(petition.SupportingArguments);
            paragraph.AddLineBreak();

            if (petition.DeadlineForResponse != null)
            {
                paragraph = document.LastSection.AddParagraph("Deadline for Response", "Heading2");
                paragraph.AddBookmark("Petition's Deadline for Response");
                paragraph = document.LastSection.AddParagraph(((DateTime)petition.DeadlineForResponse).Date.ToString());
                paragraph.AddLineBreak();
            }
            #endregion
            #region APPENDIXES
            section = document.AddSection();
            paragraph = document.LastSection.AddParagraph("Appendixes", "Heading1");
            paragraph.AddBookmark("Appendixes");

            int appendixNumber = 1;
            //Signatures Appendix
            section = document.AddSection();
            paragraph = document.LastSection.AddParagraph($"Appendix {appendixNumber}: E-Signatures", "Heading1");
            paragraph.AddBookmark($"Appendix {appendixNumber}: E-Signatures");
            paragraph.AddLineBreak();
            paragraph = document.LastSection.AddParagraph();
            string eSignaturesDisclaimer = "The signatures included in this document were electronically signed by the residents of the community on the Plugin Democracy platform.These e-signatures are compliant with federal regulations governing electronic signatures, including the Electronic Signatures in Global and National Commerce(ESIGN) Act.As such, these signatures are legally binding and hold the same validity as handwritten signatures.All data associated with the e-signatures is securely stored and hashed to protect against tampering.Detailed proof of each signature's authenticity and the associated data can be obtained upon request, ensuring transparency and adherence to legal standards.";
            paragraph.AddFormattedText(eSignaturesDisclaimer, TextFormat.Bold | TextFormat.Italic);
            paragraph = document.LastSection.AddParagraph("To view the digital signatures for this petition, please visit the following link:");
            paragraph = document.LastSection.AddParagraph(urlToPetition);
            paragraph = document.LastSection.AddParagraph();
            qrCodeForPetition = paragraph.AddImage(tempFilePath);
            qrCodeForPetition.Height = Unit.FromCentimeter(3); // Set the height to 5 cm
            qrCodeForPetition.Width = Unit.FromCentimeter(3); // Set the width to 5 cm
            paragraph.AddLineBreak();

            foreach (Home home in petition.Community.Homes)
            {
                paragraph = document.LastSection.AddParagraph($"Home {home.Number} {home.InternalAddress}", "Heading2");
                paragraph = document.LastSection.AddParagraph("Home Owners ESignatures", "Heading3");
                paragraph.Format.Alignment = ParagraphAlignment.Left;
                foreach (User owner in home.Owners)
                {
                    ESignature? eSignature = petition.Signatures.FirstOrDefault(s => s.Signer.Id == owner.Id);

                    if (eSignature != null)
                    {
                        paragraph = document.LastSection.AddParagraph($"{owner.FullName}");
                        //TODO: I WAS UNABLE TO SHOW THE IMAGES ON THE PETITION DOCUMENT
                        #region UNABLE TO PRODUCE IMAGES OF ESIGNATURES
                        //// Attempt to retrieve the signature image from the incorrectly stored string
                        //try
                        //{
                        //    // Convert the corrupted UTF-8 string back to bytes

                        //    // Attempt to interpret the bytes as a Base64-encoded image
                        //    byte[] signatureImageBytes = Convert.FromBase64String(eSignature.SignatureImageBase64);

                        //    using (MemoryStream ms = new(signatureImageBytes))
                        //    {
                        //        string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");

                        //        // Ensure the directory exists
                        //        string? directory = Path.GetDirectoryName(tempFilePath);
                        //        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        //        {
                        //            Directory.CreateDirectory(directory);
                        //        }

                        //        // Save the image to the temporary file
                        //        await System.IO.File.WriteAllBytesAsync(tempFilePath, ms.ToArray());

                        //        // Add the image to the document using the temporary file path
                        //        paragraph = document.LastSection.AddParagraph();
                        //        Image imageElement = paragraph.AddImage(tempFilePath);
                        //        imageElement.Height = Unit.FromCentimeter(2); // Set the desired height

                        //        // Clean up the temporary file after adding the image
                        //        System.IO.File.Delete(tempFilePath);
                        //    }
                        //}
                        //catch (FormatException)
                        //{
                        //    // Handle cases where the string was not a valid Base64 string
                        //    paragraph = document.LastSection.AddParagraph("Signature image could not be retrieved due to invalid format.");
                        //}
                        //catch (Exception ex)
                        //{
                        //    // Handle any other exceptions that might occur
                        //    paragraph = document.LastSection.AddParagraph($"Error retrieving signature image: {ex.Message}");
                        //}
                        #endregion
                    }
                }
                paragraph = document.LastSection.AddParagraph("Non-owner Residents ESignatures", "Heading3");
                paragraph.Format.Alignment = ParagraphAlignment.Left;
                foreach (User resident in home.Residents)
                {
                    ESignature? eSignature = petition.Signatures.FirstOrDefault(s => s.Signer.Id == resident.Id);

                    if (eSignature != null)
                    {
                        paragraph = document.LastSection.AddParagraph($"{resident.FullName}");
                    }
                }
            }

            //Attached documents appendixes
            foreach (string link in petition.LinksToSupportingDocuments)
            {
                appendixNumber++;
                
                section = document.AddSection();
                string linkWithoutQuestion = string.Empty;
                int index = link.IndexOf('?');

                if (index >= 0) // If there is a '?' in the string
                {
                    linkWithoutQuestion = link.Substring(0, index); // Get the substring before the '?'
                }
                paragraph = document.LastSection.AddParagraph($"Appendix {appendixNumber}: Supporting Document:", "Heading1");
                paragraph.AddBookmark($"Appendix {appendixNumber}: Attached Document");
                paragraph.AddLineBreak();
                paragraph.AddLineBreak();
                paragraph = document.LastSection.AddParagraph($"{linkWithoutQuestion}", "Heading2");
                paragraph.AddLineBreak();
                paragraph = document.LastSection.AddParagraph($"To view the supporting document, please visit the following link:");
                paragraph.AddLineBreak();
                paragraph = document.LastSection.AddParagraph(link);
                paragraph.AddLineBreak();

                byte[] qrCode = _utilityClass.GenerateQRCode(link);
                byte[] convertedQr = _utilityClass.ConvertPngToArgb(qrCode);
                tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");
                System.IO.File.WriteAllBytes(tempFilePath, convertedQr);
                tempFilesToDelete.Add(tempFilePath);
                paragraph = document.LastSection.AddParagraph();
                Image qRCode = paragraph.AddImage(tempFilePath);
                qRCode.Height = Unit.FromCentimeter(6); // Set the height to 5 cm
                qRCode.Width = Unit.FromCentimeter(6); // Set the width to 5 cm
            }

            #endregion
            #region SIGNATUERS FOR BOARD MEMBERS
            section = document.AddSection();
            paragraph = document.LastSection.AddParagraph($"Board Members Signatures", "Heading1");
            paragraph.AddBookmark("Board Members Signatures");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            table = new();
            table.Borders.Width = 0.75;
            column = table.AddColumn(Unit.FromCentimeter(7));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(7));
            table.AddColumn(Unit.FromCentimeter(2));
            row = table.AddRow();
            row.Shading.Color = Colors.Green;
            row.Height = Unit.FromCentimeter(2);
            cell = row.Cells[0];
            cell.AddParagraph("Board Member FullName");
            cell = row.Cells[1];
            cell.AddParagraph("Signature");
            cell = row.Cells[2];
            cell.AddParagraph("Date");
            for (int i = 0; i < 9; i++)
            {
                row = table.AddRow();
                row.Height = Unit.FromCentimeter(2);
                cell = row.Cells[0];
                cell.AddParagraph();
                cell = row.Cells[1];
                cell.AddParagraph();
            }

            table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            document.LastSection.Add(table);
            #endregion

            PdfDocumentRenderer renderer = new()
            {
                Document = document
            };
            
            renderer.RenderDocument();

            using (MemoryStream memoryStream = new())
            {
                //Save the PDF to the memory stream
                renderer.PdfDocument.Save(memoryStream, false);

                foreach (string tempFile in tempFilesToDelete)
                {
                    if (System.IO.File.Exists(tempFile))
                    {
                        System.IO.File.Delete(tempFile);
                    }
                }
                
                //Convert the memory stream to a byte array
                byte[] pdfContent = memoryStream.ToArray();
                string fileName = $"Petition {petition.Title}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
                return File(pdfContent, "application/pdf", fileName);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            }
        }
        [Authorize]
        [HttpGet(ApiEndPoints.GetProposal)]
        public async Task<ActionResult<PDAPIResponse>> GetProposal([FromQuery] string proposalId)
        {
            PDAPIResponse response = new();

            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null)
            {
                response.AddAlert("error", "User from claims was not found.");
                return BadRequest(response);
            }

            Guid parsedGuid = Guid.Parse(proposalId);

            try
            {
                Proposal? proposal = await _context.Proposals
                .Include(p => p.Author)
                .Include(p => p.Community)
                    .ThenInclude(c => c.Homes)
                        .ThenInclude(h => h.Ownerships)
                .Include(p => p.Community)
                    .ThenInclude(c => c.Homes)
                        .ThenInclude(h => h.Residents)
                .Include(p => p.Community)
                    .ThenInclude(c => c.Citizens)
                .FirstOrDefaultAsync(p => p.Id == parsedGuid);

                if (proposal == null)
                {
                    response.AddAlert("error", "The proposal was not found.");
                    return BadRequest(response);
                }
                if (proposal.Status == ProposalStatus.Draft)
                {
                    response.AddAlert("error", "This proposal has not been published.");
                    return BadRequest(response);
                }
                //Is user a citizen of this community?
                if (!proposal.Community.Citizens.Any(c => c.Id == existingUser.Id))
                {
                    response.AddAlert("error", "You are not a citizen of this community");
                    return BadRequest(response);
                }

                ProposalDTO proposalDTO = new(proposal);
                response.ProposalDTO = proposalDTO;
                response.SuccessfulOperation = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }   
}