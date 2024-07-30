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
        /// <summary>
        /// This gets the pending requests, so requests where request.Approved == null
        /// </summary>
        /// <param name="communityId"></param>
        /// <returns></returns>
        [HttpGet(ApiEndPoints.AdminGetPendingJoinCommunityRequestsForACommunity)]
        public async Task<ActionResult<List<JoinCommunityRequestDTO>>> AdminGetPendingJoinCommunityRequestsIncludeCommunityRoles([FromQuery] int? communityId)
        {
            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) return BadRequest();
            if (user.Admin == false) return Unauthorized();
            if (communityId == null) return BadRequest();
            try
            {
                List<JoinCommunityRequest> joinCommunityRequests = await _context.JoinCommunityRequests.Include(j => j.User).Include(j => j.Home).Include(j => j.Community).Where(j => j.Community.Id == communityId).Where(r => r.Approved == null).ToListAsync();
                List<JoinCommunityRequestDTO> joinCommunityRequestDtos = [];
                foreach (JoinCommunityRequest joinCommunityRequest in joinCommunityRequests) joinCommunityRequestDtos.Add(new JoinCommunityRequestDTO(joinCommunityRequest));
                return Ok(joinCommunityRequestDtos);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
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
            foreach(CultureInfo language in existingCommunity.OfficialLanguages) if (!newLanguages.Contains(language)) existingCommunity.RemoveOfficialLanguage(language);
            foreach (CultureInfo language in newLanguages) if (!existingCommunity.OfficialLanguages.Contains(language)) existingCommunity.AddOfficialLanguage(language);

            try
            {
                _context.SaveChanges();
                return Ok(new ResidentialCommunityDTO(existingCommunity));
            }
            catch(Exception e)
            {
                return StatusCode(500, $"Internal error: {e.Message}");
            }

        }
        [HttpPost(ApiEndPoints.AdminUpdateCommunityPicture)]
        public async Task<ActionResult<bool>> UpdateCommunityPicture([FromQuery] int communityId, [FromForm] IFormFile file)
        {
            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) return BadRequest("User null");
            if (user.Admin == false) return Unauthorized("Nice try. You are not admin.");
            ResidentialCommunity? community = await _context.ResidentialCommunities.FirstOrDefaultAsync(c => c.Id == communityId);
            if (community == null) return BadRequest("Community null");
            try
            {
                //Save file to blob storage
                //Remove write SAS
                //Add read SAS
                //Add link to community object
                //Save changes
                //Return true if successful
                return Ok(true);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
