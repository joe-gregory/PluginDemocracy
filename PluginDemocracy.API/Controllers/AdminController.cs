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
        public async Task<ActionResult<List<CommunityDTO>>> GetListOfAllSimpleCommunitiesDtos()
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return Unauthorized();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == existingUser.Id);
                if(user == null) return BadRequest();
                if (user.Admin == false) return Unauthorized();
                List<HOACommunity> communities = await _context.Communities.ToListAsync();
                List<CommunityDTO> communityDtos = [];
                foreach (HOACommunity community in communities) communityDtos.Add(CommunityDTO.ReturnSimpleCommunityDtoFromCommunity(community));
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
        [HttpGet(ApiEndPoints.AdminGetPendingJoinCommunityRequests)]
        public async Task<ActionResult<List<JoinCommunityRequestDTO>>> GetJoinCommunityRequests([FromQuery] int? communityId)
        {
            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) return BadRequest();
            if (user.Admin == false) return Unauthorized();
            if (communityId == null) return BadRequest();
            try
            {
                List<JoinCommunityRequest> joinCommunityRequests = await _context.JoinCommunityRequests.Include(j => j.User).Include(j => j.Home).Where(j => j.Community.Id == communityId).Where(r => r.Approved == null).ToListAsync();
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
        public async Task<ActionResult<PDAPIResponse>> AcceptJoinCommunityRequest(JoinCommunityRequestDTO request)
        {
            PDAPIResponse response = new();

            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) return BadRequest();
            if (user.Admin == false) return Unauthorized();
            //Get the original Models.JoinCommunityRequest
            JoinCommunityRequest? originalRequest = _context.JoinCommunityRequests.Include(r => r.Community).Include(r => r.Home).Include(r => r.User).FirstOrDefault(j => j.Id == request.Id);

            
            if (originalRequest == null)
            {
                response.AddAlert("error", "Join request not found in DB");
                return BadRequest(response);
            }
            if (originalRequest.Approved != null)
            {
                response.AddAlert("error", "A decision had already been made on this request. Please submit a new request.");
                return BadRequest(response);
            }
            // I already have a method in community to deal with accepted join requests
            try
            {
                originalRequest.Community.ApproveJoinCommunityRequest(originalRequest);
                
                //Give a notification to the user
                originalRequest.User?.AddNotification($"{_utilityClass.Translate(ResourceKeys.YourJoinRequestHasBeenApprovedTitle, originalRequest.User.Culture)} {originalRequest.Community.FullName}", $"{_utilityClass.Translate(ResourceKeys.YourJoinRequestHasBeenApprovedBody, originalRequest.User.Culture)} {originalRequest.Community.FullName}");
                
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
        public async Task<ActionResult<PDAPIResponse>> RejectJoinCommunityRequest(JoinCommunityRequestDTO request)
        {
            PDAPIResponse response = new();

            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) return BadRequest();
            if (user.Admin == false) return Unauthorized();

            JoinCommunityRequest? originalRequest = _context.JoinCommunityRequests.Include(r => r.Community).Include(r => r.User).FirstOrDefault(j => j.Id == request.Id);
            if (originalRequest == null)
            {
                response.AddAlert("error", "Join request not found in DB");
                return BadRequest(response);
            }
            if (originalRequest.Approved != null)
            {
                response.AddAlert("error", "A decision had already been made on this request. Please submit a new request.");
                return BadRequest(response);
            }
            
            try
            {
                originalRequest.Approved = false;
                //Give a notification to the user
                originalRequest.User?.AddNotification($"{_utilityClass.Translate(ResourceKeys.YourJoinRequestHasBeenRejectedTitle, originalRequest.User.Culture)} {originalRequest.Community.FullName}", $"{_utilityClass.Translate(ResourceKeys.YourJoinRequestHasBeenRejectedBody, originalRequest.User.Culture)} {originalRequest.Community.FullName}");
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

    }
}
