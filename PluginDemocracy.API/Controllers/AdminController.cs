using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PluginDemocracy.Data;
using PluginDemocracy.DTOs;
using Azure;
using PluginDemocracy.Models;
using Microsoft.EntityFrameworkCore;
using PluginDemocracy.API.UrlRegistry;


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
        public async Task<ActionResult<List<CommunityDto>>> GetListOfAllSimpleCommunitiesDtos()
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return Unauthorized();
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == existingUser.Id);
                if(user == null) return BadRequest();
                if (user.Admin == false) return Unauthorized();
                List<Community> communities = await _context.Communities.ToListAsync();
                List<CommunityDto> communityDtos = [];
                foreach (Community community in communities) communityDtos.Add(CommunityDto.ReturnSimpleCommunityDtoFromCommunity(community));
                return Ok(communityDtos);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpGet("getjoincommunityrequests")]
        public async Task<ActionResult<List<JoinCommunityRequestDto>>> GetJoinCommunityRequests([FromQuery] int? communityId)
        {
            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) return BadRequest();
            if (user.Admin == false) return Unauthorized();
            if (communityId == null) return BadRequest();
            try
            {
                List<JoinCommunityRequest> joinCommunityRequests = await _context.JoinCommunityRequests.Where(j => j.Community.Id == communityId).ToListAsync();
                List<JoinCommunityRequestDto> joinCommunityRequestDtos = [];
                foreach (JoinCommunityRequest joinCommunityRequest in joinCommunityRequests) joinCommunityRequestDtos.Add(new JoinCommunityRequestDto(joinCommunityRequest));
                return Ok(joinCommunityRequestDtos);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpPost(ApiEndPoints.AdminAcceptJoinRequest)]
        public async Task<ActionResult<PDAPIResponse>> AcceptJoinCommunityRequest(JoinCommunityRequestDto request)
        {
            PDAPIResponse response = new();

            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) return BadRequest();
            if (user.Admin == false) return Unauthorized();
            //Get the original Models.JoinCommunityRequest
            JoinCommunityRequest? originalRequest = _context.JoinCommunityRequests.FirstOrDefault(j => j.Id == request.Id);
            if (originalRequest == null)
            {
                response.AddAlert("error", "Join request not found in DB");
                return BadRequest(response);
            }
            // I already have a method in community to deal with accepted join requests
            try
            {
                originalRequest.Community.ApproveJoinCommunityRequest(originalRequest);
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
        public async Task<ActionResult<PDAPIResponse>> RejectJoinCommunityRequest(JoinCommunityRequestDto request)
        {
            PDAPIResponse response = new();

            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) return BadRequest();
            if (user.Admin == false) return Unauthorized();

            JoinCommunityRequest? originalRequest = _context.JoinCommunityRequests.FirstOrDefault(j => j.Id == request.Id);
            if (originalRequest == null)
            {
                response.AddAlert("error", "Join request not found in DB");
                return BadRequest(response);
            }

            originalRequest.Approved = false;
            try
            {
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
