using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PluginDemocracy.Data;
using PluginDemocracy.DTOs;
using Azure;
using PluginDemocracy.Models;
using Microsoft.EntityFrameworkCore;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.API.Translations;


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
        [HttpGet("getlistofallsimplecommunitiesdtos")]
        public async Task<ActionResult<List<ResidentialCommunityDTO>>> GetListOfAllSimpleCommunitiesDtos()
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return Unauthorized();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == existingUser.Id);
                if(user == null) return BadRequest();
                if (user.Admin == false) return Unauthorized();
                List<ResidentialCommunity> communities = await _context.ResidentialCommunities.ToListAsync();
                List<ResidentialCommunityDTO> communityDtos = [];
                foreach (ResidentialCommunity community in communities) communityDtos.Add(ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(community));
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
        [HttpGet(ApiEndPoints.AdminGetPendingJoinCommunityRequestsIncludeCommunityRoles)]
        public async Task<ActionResult<List<JoinCommunityRequestDTO>>> GetJoinCommunityRequests([FromQuery] int? communityId)
        {
            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) return BadRequest();
            if (user.Admin == false) return Unauthorized();
            if (communityId == null) return BadRequest();
            try
            {
                List<JoinCommunityRequest> joinCommunityRequests = await _context.JoinCommunityRequests.Include(j => j.User).Include(j => j.Home).Include(j => j.Community).ThenInclude(c => c.Roles).Where(j => j.Community.Id == communityId).Where(r => r.Approved == null).ToListAsync();
                List<JoinCommunityRequestDTO> joinCommunityRequestDtos = [];
                foreach (JoinCommunityRequest joinCommunityRequest in joinCommunityRequests) joinCommunityRequestDtos.Add(new JoinCommunityRequestDTO(joinCommunityRequest));
                return Ok(joinCommunityRequestDtos);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpPost(ApiEndPoints.AdminAcceptJoinRequest)]
        public async Task<ActionResult<PDAPIResponse>> AcceptJoinCommunityRequest(JoinCommunityRequestDTO joinCommunityRequestDTO)
        {
            PDAPIResponse response = new();

            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) return BadRequest();
            if (user.Admin == false) return Unauthorized();
            //Get the Models.JoinCommunityRequest
            JoinCommunityRequest? joinCommunityRequest = _context.JoinCommunityRequests
                .Include(r => r.Community).ThenInclude(c => c.JoinCommunityRequests)
                .Include(r => r.Community).ThenInclude(c => c.Roles)
                .Include(r => r.Home)
                .Include(r => r.User)
                .FirstOrDefault(j => j.Id == joinCommunityRequestDTO.Id);

            
            if (joinCommunityRequest == null)
            {
                response.AddAlert("error", "Join request not found in DB");
                return BadRequest(response);
            }
            if (joinCommunityRequest.Approved != null)
            {
                response.AddAlert("error", "A decision had already been made on this request. Please submit a new request.");
                return BadRequest(response);
            }
            // I already have a method in community to deal with accepted join requests
            try
            {
                joinCommunityRequest.Community.ApproveJoinCommunityRequest(joinCommunityRequest, user);
                
                //Give a notification to the user
                joinCommunityRequest.User?.AddNotification($"{_utilityClass.Translate(ResourceKeys.YourJoinRequestHasBeenApprovedTitle, joinCommunityRequest.User.Culture)} {joinCommunityRequest.Community.FullName}", $"{_utilityClass.Translate(ResourceKeys.YourJoinRequestHasBeenApprovedBody, joinCommunityRequest.User.Culture)} {joinCommunityRequest.Community.FullName}");
                
                _context.SaveChanges();
                response.AddAlert("success", "Join request accepted");
                return Ok(response);
            }
            catch (Exception e)
            {
                response.AddAlert("error", e.Message);
                return StatusCode(500, response);
            }
            
        }
        [HttpPost(ApiEndPoints.AdminRejectJoinRequest)]
        public async Task<ActionResult<PDAPIResponse>> RejectJoinCommunityRequest(JoinCommunityRequestDTO requestDTO)
        {
            PDAPIResponse response = new();

            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) return BadRequest();
            if (user.Admin == false) return Unauthorized();

            //Get the Models.JoinCommunityRequest
            JoinCommunityRequest? joinCommunityRequest = _context.JoinCommunityRequests
                .Include(r => r.Community).ThenInclude(c => c.JoinCommunityRequests)
                .Include(r => r.Community).ThenInclude(c => c.Roles)
                .Include(r => r.Home)
                .Include(r => r.User)
                .FirstOrDefault(j => j.Id == requestDTO.Id);
            if (joinCommunityRequest == null)
            {
                response.AddAlert("error", "Join request not found in DB");
                return BadRequest(response);
            }
            if (joinCommunityRequest.Approved != null)
            {
                response.AddAlert("error", "A decision had already been made on this request. Please submit a new request.");
                return BadRequest(response);
            }
            
            try
            {
                joinCommunityRequest.Community.RejectJoinCommunityRequest(joinCommunityRequest, user);
                //Give a notification to the user
                joinCommunityRequest.User?.AddNotification($"{_utilityClass.Translate(ResourceKeys.YourJoinRequestHasBeenRejectedTitle, joinCommunityRequest.User.Culture)} {joinCommunityRequest.Community.FullName}", $"{_utilityClass.Translate(ResourceKeys.YourJoinRequestHasBeenRejectedBody, joinCommunityRequest.User.Culture)} {joinCommunityRequest.Community.FullName}");
                _context.SaveChanges();
                response.AddAlert("success", "Join request rejected");
                return Ok(response);
            }
            catch (Exception e)
            {
                response.AddAlert("error", e.Message);
                return StatusCode(500, response);
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
            if(string.IsNullOrEmpty(roleDTO.Title) || string.IsNullOrEmpty(roleDTO.Description))
            {
                apiResponse.AddAlert("error", "Title and description are required.");
                return BadRequest(apiResponse);
            }
            try
            {
                Role newRole = community.AddRole(roleDTO.Title, roleDTO.Description, roleDTO.Powers);
                community.AssignRole(newRole, holder);
                _context.SaveChanges();
                apiResponse.SuccessfulOperation = true;
                apiResponse.AddAlert("success", "Role created and assigned successfully.");
                return Ok(apiResponse);
            }
            catch(Exception e)
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
            ResidentialCommunity? community = _context.ResidentialCommunities.Include(c => c.Roles).FirstOrDefault(c => c.Roles.Any(r => r.Id == roleId));
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
                _context.SaveChanges();
                apiResponse.SuccessfulOperation = true;
                apiResponse.AddAlert("success", "Role deleted and unassigned successfully.");
                return Ok(apiResponse);
            }
            catch(Exception e)
            {
                apiResponse.AddAlert("error", e.Message);
                return StatusCode(500, apiResponse);
            }
        }
    }
}
