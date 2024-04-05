using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PluginDemocracy.Data;
using PluginDemocracy.DTOs;
using Azure;
using PluginDemocracy.Models;
using Microsoft.EntityFrameworkCore;


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
        [HttpGet("getlistofjoincommunityrequests")]
        public async Task<ActionResult<List<JoinCommunityRequestDto>>> GetListOfJoinCommunityRequests([FromQuery] int communityId)
        {
            User? user = await _utilityClass.ReturnUserFromClaims(User);
            if (user == null) return Unauthorized();
            try
            {
                if(user == null) return BadRequest();
                if (user.Admin == false) return Unauthorized();
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
    }
}
