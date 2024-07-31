using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PluginDemocracy.Data;
using PluginDemocracy.DTOs;
using Azure;
using PluginDemocracy.Models;
using Microsoft.EntityFrameworkCore;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.API.Translations;
using System.Globalization;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


namespace PluginDemocracy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(PluginDemocracyContext context, APIUtilityClass utilityClass) : ControllerBase
    {
        private readonly PluginDemocracyContext _context = context;
        private readonly APIUtilityClass _utilityClass = utilityClass;

        [HttpGet("iscurrentuseradmin")]
        public async Task<ActionResult<bool>> IsCurrentUserAdmin()
        {
            User? userFromRequest = await _utilityClass.ReturnUserFromClaims(User);
            if (userFromRequest == null) return BadRequest();
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userFromRequest.Id);
            if (user == null) return BadRequest();
            return Ok(user.Admin);
        }
        [HttpGet(ApiEndPoints.AdminGetListOfAllSimpleCommunitiesDTOsWithRoles)]
        public async Task<ActionResult<List<ResidentialCommunityDTO>>> GetListOfAllSimpleCommunitiesDTOsWithRoles()
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return Unauthorized();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == existingUser.Id);
                if (user == null) return BadRequest();
                if (user.Admin == false) return Unauthorized();
                List<ResidentialCommunity> communities = await _context.ResidentialCommunities.Include(c => c.Roles).ToListAsync();
                List<ResidentialCommunityDTO> communityDtos = [];
                foreach (ResidentialCommunity community in communities)
                {
                    ResidentialCommunityDTO communityDTO = ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(community);
                    foreach (Role role in community.Roles) communityDTO.Roles.Add(new RoleDTO(role));
                    communityDtos.Add(communityDTO);

                }
                return Ok(communityDtos);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpGet(ApiEndPoints.AdminGetFullCommunityDTOObject)]
        public async Task<ActionResult<ResidentialCommunityDTO>> GetFullCommunityDTOObject([FromQuery] int? communityId)
        {
            if (communityId == null) return BadRequest("CommunityId is null");
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest("No user found with given credentials");
            if (existingUser.Admin == false) return Unauthorized("This user is not admin");
            ResidentialCommunity? community = await _context.ResidentialCommunities.Include(c => c.Roles).Include(c => c.Homes).ThenInclude(h => h.Residents).Include(c => c.Homes).ThenInclude(h => h.Ownerships).Include(c => c.JoinCommunityRequests.Where(jcr => jcr.Approved == null)).FirstOrDefaultAsync(c => c.Id == communityId);
            if (community == null) return BadRequest("No community found with given id");
            return Ok(new ResidentialCommunityDTO(community));
        }
        [HttpPost(ApiEndPoints.AdminCreateAndAssignRole)]
        public async Task<ActionResult<PDAPIResponse>> CreateAndAssignRole(RoleDTO roleDTO)
        {
            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) return BadRequest();
            if (user.Admin == false) return Unauthorized();
            PDAPIResponse apiResponse = new();
            //Get the community.
            if (roleDTO.Community == null)
            {
                apiResponse.AddAlert("error", "The community provided is null.");
                return BadRequest(apiResponse);
            }
            if (roleDTO.Holder == null)
            {
                apiResponse.AddAlert("error", "No user specified for the role.");
                return BadRequest(apiResponse);
            }
            User? holder = _context.Users.FirstOrDefault(u => u.Id == roleDTO.Holder.Id);
            if (holder == null)
            {
                apiResponse.AddAlert("error", "No user found with the given holders id information.");
                return BadRequest(apiResponse);
            }
            ResidentialCommunity? community = _context.ResidentialCommunities.FirstOrDefault(c => c.Id == roleDTO.Community.Id);
            if (community == null)
            {
                apiResponse.AddAlert("error", "No community found with the given community id information.");
                return BadRequest(apiResponse);
            }
            if (string.IsNullOrEmpty(roleDTO.Title) || string.IsNullOrEmpty(roleDTO.Description))
            {
                apiResponse.AddAlert("error", "Title and description are required.");
                return BadRequest(apiResponse);
            }
            try
            {
                Role newRole = community.AddRole(roleDTO.Title, roleDTO.Description, roleDTO.Powers);
                community.AssignRole(newRole, holder);
                string title = "Role assigned to you.";
                string messageBody = $"You have been assigned the role of {newRole.Title} in {community.FullName} with permissions to:\n";
                if (newRole.Powers.CanEditHomeOwnership) messageBody += "Edit home ownerships\n";
                if (newRole.Powers.CanEditResidency) messageBody += "Edit residency\n";
                if (newRole.Powers.CanModifyAccounting) messageBody += "Modify accounting\n";
                messageBody += "\n\nWith Great Power Comes Great Responsibility.";
                holder.AddNotification(title, messageBody);
                await _utilityClass.SendEmailAsync(holder.Email, title, messageBody);
                _context.SaveChanges();
                apiResponse.SuccessfulOperation = true;
                apiResponse.AddAlert("success", "Role created and assigned successfully.");
                return Ok(apiResponse);
            }
            catch (Exception e)
            {
                apiResponse.AddAlert("error", e.Message);
                return StatusCode(500, apiResponse);
            }
        }
        [HttpPost(ApiEndPoints.AdminDeleteAndUnassignRole)]
        public async Task<ActionResult<PDAPIResponse>> DeleteAndUnassignRole([FromQuery] int roleId)
        {
            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) return BadRequest();
            if (user.Admin == false) return Unauthorized();
            PDAPIResponse apiResponse = new();
            //Get the community.
            ResidentialCommunity? community = _context.ResidentialCommunities.Include(c => c.Roles).ThenInclude(r => r.Holder).FirstOrDefault(c => c.Roles.Any(r => r.Id == roleId));
            if (community == null)
            {
                apiResponse.AddAlert("error", "No community was found that had a role Id as provided");
                return BadRequest(apiResponse);
            }
            try
            {
                Role? role = community.Roles.FirstOrDefault(r => r.Id == roleId);
                if (role == null)
                {
                    apiResponse.AddAlert("error", "No role found with the given title and description.");
                    return BadRequest(apiResponse);
                }
                community.RemoveRole(role);
                string title = $"Your role {role.Title} has been removed.";
                string body = $"Your role {role.Title} in {community.FullName} has been removed. You no longer have the permissions granted by this role. If you feel this is a mistake, contact app administrator.\n";
                if (role.Holder != null)
                {
                    role.Holder.AddNotification(title, body);
                    await _utilityClass.SendEmailAsync(role.Holder.Email, title, body);
                }
                _context.Remove(role);
                _context.SaveChanges();
                apiResponse.SuccessfulOperation = true;
                apiResponse.AddAlert("success", "Role deleted and unassigned successfully.");
                return Ok(apiResponse);
            }
            catch (Exception e)
            {
                apiResponse.AddAlert("error", e.Message);
                return StatusCode(500, apiResponse);
            }
        }
        [HttpPost(ApiEndPoints.AdminUpdateCommunityInfo)]
        public async Task<ActionResult<ResidentialCommunityDTO>> UpdateCommunityInfo(ResidentialCommunityDTO community)
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest("User not found");
            if (!existingUser.Admin) return Forbid("You are not an admin.");
            ResidentialCommunity? existingCommunity = await _context.ResidentialCommunities.Include(c => c.Roles).FirstOrDefaultAsync(c => c.Id == community.Id);
            if (existingCommunity == null) return BadRequest("Community not found");
            //Update info
            if (community.Name != null) existingCommunity.Name = community.Name;
            if (community.Address != null) existingCommunity.Address = community.Address;
            if (community.Description != null) existingCommunity.Description = community.Description;
            if (community.OfficialCurrency != null) existingCommunity.OfficialCurrency = community.OfficialCurrency;
            List<CultureInfo> newLanguages = [];
            foreach (string languageCode in community.OfficialLanguagesCodes) newLanguages.Add(new(languageCode));
            foreach (CultureInfo language in existingCommunity.OfficialLanguages) if (!newLanguages.Contains(language)) existingCommunity.RemoveOfficialLanguage(language);
            foreach (CultureInfo language in newLanguages) if (!existingCommunity.OfficialLanguages.Contains(language)) existingCommunity.AddOfficialLanguage(language);

            try
            {
                _context.SaveChanges();
                return Ok(new ResidentialCommunityDTO(existingCommunity));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal error: {e.Message}");
            }

        }
        [HttpPost(ApiEndPoints.AdminUpdateCommunityPicture)]
        public async Task<ActionResult<PDAPIResponse>> UpdateCommunityPicture([FromQuery] int communityId, [FromForm] IFormFile file)
        {
            User? user = await _utilityClass.ReturnUserFromClaims(User);
            PDAPIResponse response = new();

            if (user == null)
            {
                response.AddAlert("error", "User null");
                return BadRequest(response);
            }
            if (user.Admin == false)
            {
                response.AddAlert("error", "You are not admin.");
                return Unauthorized(response);
            }
            ResidentialCommunity? community = await _context.ResidentialCommunities.FirstOrDefaultAsync(c => c.Id == communityId);
            if (community == null)
            {
                response.AddAlert("error", "Community not found");
                return BadRequest(response);
            }
            try
            {
                //SAVE FILE TO BLOB STORAGE:
                //Get the blob container URL and SAS token from environment variables and create a BlobContainerClient
                string blobContainerURL = Environment.GetEnvironmentVariable("BlobContainerURL") ?? string.Empty;
                if (string.IsNullOrEmpty(blobContainerURL))
                {
                    response.AddAlert("error", "Blob container URL not found in environment variables.");
                    return BadRequest(response);
                }
                string blobSASToken = Environment.GetEnvironmentVariable("BlobSASToken") ?? string.Empty;
                if (string.IsNullOrEmpty(blobSASToken))
                {

                    response.AddAlert("error", "Blob SAS token not found in environment variables.");
                    return BadRequest(response);
                }
                string readOnlyBlobSASToken = Environment.GetEnvironmentVariable("ReadOnlyBlobSASToken") ?? string.Empty;
                if (string.IsNullOrEmpty(readOnlyBlobSASToken))
                {
                    response.AddAlert("error", "Read only Blob SAS token not found in environment variables.");
                    return BadRequest(response);
                }
                try
                {
                    BlobContainerClient blobContainerClient = new(new Uri($"{blobContainerURL}?{blobSASToken}"));
                    //Add the file to the blob


                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (!fileExtension.StartsWith('.')) fileExtension = "." + fileExtension;
                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                    {
                        response.AddAlert("error", "File extension is not jpg, jpeg, or png");
                        return BadRequest(response);
                    }
                    string blobName = $"community/{community.Id}/profileimage/{community.FullName}{fileExtension}";
                    BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
                    await using Stream fileStream = file.OpenReadStream();
                    //Upload the document
                    await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = file.ContentType });
                    //Remove Sas write token: 
                    UriBuilder uriBuilder = new(blobClient.Uri);
                    System.Collections.Specialized.NameValueCollection query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                    query.Clear();
                    uriBuilder.Query = query.ToString();
                    //Add read SAS
                    string blobUrlWithoutSas = uriBuilder.ToString();
                    //Add link to community object
                    community.ProfilePicture = $"{blobUrlWithoutSas}?{readOnlyBlobSASToken}";
                    //Save changes
                    _context.SaveChanges();
                    //Return true if successful
                    response.SuccessfulOperation = true;
                    response.AddAlert("success", "Community's profile picture successfully updated.");
                    return Ok(response);
                }
                catch (Exception e)
                {
                    response.AddAlert("error", $"Error: {e.Message}");
                    return response;
                }

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpPost(ApiEndPoints.AdminRemoveHomeOwnership)]
        public async Task<ActionResult<PDAPIResponse>> RemoveHomeOnwership([FromQuery] int? communityId, [FromQuery] int? homeId, [FromQuery] int? homeOwnershipId)
        {
            PDAPIResponse response = new();
            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) 
            {
                response.AddAlert("error", "User from claims not found");
                return BadRequest(response); 
            }
            if (user.Admin == false)
            {
                response.AddAlert("error", "Nice try, user is not admin.");
                return Unauthorized(response);
            }
            if (communityId == null)
            {
                response.AddAlert("error", "communityId is null");
                return BadRequest(response);
            }
            if (homeId == null)
            {
                response.AddAlert("error", "homeId is null");
                return BadRequest(response);
            }
            if (homeOwnershipId == null)
            {
                response.AddAlert("error", "homeOwnershipId is null");
                return BadRequest(response);
            }
            ResidentialCommunity? community = await _context.ResidentialCommunities.Include(c => c.Homes).ThenInclude(h => h.Residents).Include(c => c.Homes).ThenInclude(h => h.Ownerships).ThenInclude(hw => hw.Owner).FirstOrDefaultAsync(c => c.Id == communityId);
            if (community == null)
            {
                response.AddAlert("error", "Community not found");
                return BadRequest(response);
            }
            Home? home = community.Homes.FirstOrDefault(h => h.Id == homeId);
            if (home == null)
            {
                response.AddAlert("error", "Home not found");
                return BadRequest(response);
            }
            HomeOwnership? homeOwnership = home.Ownerships.FirstOrDefault(o => o.Id == homeOwnershipId);
            if (homeOwnership == null)
            {
                response.AddAlert("error", "Home ownership not found");
                return BadRequest(response);
            }
            try
            {
                home.RemoveOwner(homeOwnership.Owner);
                _context.SaveChanges();
                response.SuccessfulOperation = true;
                response.AddAlert("success", "Home ownership removed from home");
                return response;
            }
            catch (Exception e)
            {
                response.AddAlert("error", $"{e.Message}");
                return response;
            }
        }
        [HttpDelete(ApiEndPoints.AdminRemoveResidencyFromHome)]
        public async Task<ActionResult<PDAPIResponse>> RemoveResidencyFromHome([FromQuery] int? communityId, [FromQuery] int? homeId, [FromQuery] int? residentId)
        {
            PDAPIResponse response = new();
            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null)
            {
                response.AddAlert("error", "User from claims not found");
                return BadRequest(response);
            }
            if (user.Admin == false)
            {
                response.AddAlert("error", "Nice try, user is not admin.");
                return Unauthorized(response);
            }
            if (communityId == null)
            {
                response.AddAlert("error", "communityId is null");
                return BadRequest(response);
            }
            if (homeId == null)
            {
                response.AddAlert("error", "homeId is null");
                return BadRequest(response);
            }
            if (residentId == null)
            {
                response.AddAlert("error", "residentId is null");
                return BadRequest(response);
            }
            ResidentialCommunity? community = await _context.ResidentialCommunities.Include(c => c.Homes).ThenInclude(h => h.Residents).FirstOrDefaultAsync(c => c.Id == communityId);
            if (community == null)
            {
                response.AddAlert("error", "Community not found");
                return BadRequest(response);
            }
            Home? home = community.Homes.FirstOrDefault(h => h.Id == homeId);
            if (home == null)
            {
                response.AddAlert("error", "Home not found");
                return BadRequest(response);
            }
            User? resident = await _context.Users.Include(u => u.ResidentOfHomes).FirstOrDefaultAsync(u => u.Id == residentId);
            if (resident == null)
            {
                response.AddAlert("error", "Resident not found");
                return BadRequest(response);
            }
            try
            {
                home.RemoveResident(resident);
                _context.SaveChanges();
                response.SuccessfulOperation = true;
                response.AddAlert("success", "Residency removed from home");
                return response;
            }
            catch (Exception e)
            {
                response.AddAlert("error", $"{e.Message}");
                return response;
            }
        }
        [HttpDelete(ApiEndPoints.AdminDeleteHome)]
        public async Task<ActionResult<PDAPIResponse>> DeleteHome([FromQuery] int? communityId, [FromQuery] int? homeId)
        {
            PDAPIResponse response = new();
            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null)
            {
                response.AddAlert("error", "User from claims not found");
                return BadRequest(response);
            }
            if (user.Admin == false)
            {
                response.AddAlert("error", "Nice try, user is not admin.");
                return Unauthorized(response);
            }
            if (communityId == null)
            {
                response.AddAlert("error", "communityId is null");
                return BadRequest(response);
            }
            if (homeId == null)
            {
                response.AddAlert("error", "homeId is null");
                return BadRequest(response);
            }
            ResidentialCommunity? community = await _context.ResidentialCommunities.Include(c => c.Homes).FirstOrDefaultAsync(c => c.Id == communityId);
            if (community == null)
            {
                response.AddAlert("error", "Community not found");
                return BadRequest(response);
            }
            Home? home = community.Homes.FirstOrDefault(h => h.Id == homeId);
            if (home == null)
            {
                response.AddAlert("error", "Home not found");
                return BadRequest(response);
            }
            try
            {
                List<JoinCommunityRequest> joinCommunityRequests = await _context.JoinCommunityRequests.Include(jcr => jcr.Home).Where(jcr => jcr.Home.Id == home.Id).ToListAsync();
                foreach (JoinCommunityRequest jcr in joinCommunityRequests)
                { 
                    community.RemoveJoinCommunityRequest(jcr); 
                    _context.Remove(jcr);
                }
                community.RemoveHome(home);
                _context.Remove(home);
                _context.SaveChanges();
                response.SuccessfulOperation = true;
                response.AddAlert("success", "Home removed from community");
                return response;
            }
            catch (Exception e)
            {
                response.AddAlert("error", $"{e.Message}");
                return response;
            }
        }
        [HttpPost(ApiEndPoints.AdminEditHome)]
        public async Task<ActionResult<PDAPIResponse>> EditHome([FromQuery] int? communityId, [FromBody] HomeDTO? home)
        {
            PDAPIResponse response = new();
            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null)
            {
                response.AddAlert("error", "User from claims not found");
                return BadRequest(response);
            }
            if (user.Admin == false)
            {
                response.AddAlert("error", "Nice try, user is not admin.");
                return Unauthorized(response);
            }
            if (communityId == null)
            {
                response.AddAlert("error", "communityId is null");
                return BadRequest(response);
            }
            if (home == null)
            {
                response.AddAlert("error", "home is null");
                return BadRequest(response);
            }
            ResidentialCommunity? community = await _context.ResidentialCommunities.Include(c => c.Homes).FirstOrDefaultAsync(c => c.Id == communityId);
            if (community == null)
            {
                response.AddAlert("error", "Community not found");
                return BadRequest(response);
            }
            Home? existingHome = community.Homes.FirstOrDefault(h => h.Id == home.Id);
            if (existingHome == null)
            {
                response.AddAlert("error", "Home not found in community list.");
                return BadRequest(response);
            }
            try
            {
                if (home.Number != existingHome.Number) existingHome.Number = home.Number;
                if (home.InternalAddress != existingHome.InternalAddress) existingHome.InternalAddress = home.InternalAddress;
                _context.SaveChanges();
                response.SuccessfulOperation = true;
                response.AddAlert("success","Home updated successfully");
                return response;
            }
            catch (Exception e)
            {
                response.AddAlert("error", $"{e.Message}");
                return response;
            }
        }
        [HttpPost(ApiEndPoints.AdminAddHome)]
        public async Task<ActionResult<PDAPIResponse>> AddHomeToCommunity([FromQuery] int? communityId, [FromBody] HomeDTO? homeToAdd)
        {
            PDAPIResponse response = new();
            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null)
            {
                response.AddAlert("error", "User from claims not found");
                return BadRequest(response);
            }
            if (user.Admin == false)
            {
                response.AddAlert("error", "Nice try, user is not admin.");
                return Unauthorized(response);
            }
            if (communityId == null)
            {
                response.AddAlert("error", "communityId is null");
                return BadRequest(response);
            }
            if (homeToAdd == null)
            {
                response.AddAlert("error", "home is null");
                return BadRequest(response);
            }
            if (homeToAdd.Number < 1)
            {
                response.AddAlert("error", "Home number must be greater than 0");
                return BadRequest(response);
            }
            ResidentialCommunity? community = await _context.ResidentialCommunities.Include(c => c.Homes).FirstOrDefaultAsync(c => c.Id == communityId);
            if (community == null)
            {
                response.AddAlert("error", "Community not found");
                return BadRequest(response);
            }
            try
            {
                Home newHome = new(community, homeToAdd.Number, homeToAdd.InternalAddress);
                community.AddHome(newHome);
                _context.SaveChanges();
                response.SuccessfulOperation = true;
                response.AddAlert("success", "Home added to community");
                return response;
            }
            catch (Exception e)
            {
                response.AddAlert("error", e.Message);
                return response;
            }
            
        }
    }
}

