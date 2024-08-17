using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.Data;
using PluginDemocracy.DTOs;
using PluginDemocracy.DTOs.CommunitiesDto;
using PluginDemocracy.Models;
using System.Globalization;

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
                foreach (ResidentialCommunity community in communities) response.AllCommunities.Add(new ResidentialCommunityDTO()
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
                response.Community = ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(community);
                foreach (Home home in community.Homes) response.Community.Homes.Add(new(home));
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

                foreach (Post post in community.PostsByLatestActivity) response.Posts.Add(new PostDTO(post));
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

            Post? post = await _context.Posts.Include(p => p.Author).Include(p => p.Reactions).FirstOrDefaultAsync(p => p.Id == reactionDto.PostId);
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
            Post? post = await _context.Posts.Include(p => p.Author).Include(p => p.Reactions).Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == postCommentDto.PostId);
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
            Petition? petition = await _context.Petitions.Include(p => p.Authors).Include(p => p.Community).Include(p => p.Signatures).ThenInclude(s => s.Signer).FirstOrDefaultAsync(p => p.Id == petitionId);
            if (petition == null) return BadRequest("Petition not found.");
            if (!petition.Published) return BadRequest("The petition has not been published.");
            PetitionDTO petitionDTO = new(petition);
            return Ok(petitionDTO);
        }
    }   
}