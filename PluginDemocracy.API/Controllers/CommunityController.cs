using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PluginDemocracy.Data;
using PluginDemocracy.DTOs;
using PluginDemocracy.Models;

namespace PluginDemocracy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityController(PluginDemocracyContext context, APIUtilityClass utilityClass) : ControllerBase
    {
        private readonly PluginDemocracyContext _context = context;
        private readonly APIUtilityClass _utilityClass = utilityClass;

        [HttpPost("register")]
        public async Task<ActionResult<PDAPIResponse>> Register(CommunityDto communityDto)
        {
            PDAPIResponse response = new();
            try
            {
                if (ModelState.IsValid)
                {
                    var community = new Community
                    {
                        Name = communityDto.Name,
                        Description = communityDto.Description,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Communities.Add(community);
                    await _context.SaveChangesAsync();
                    response.Success = true;
                    response.Message = "Community created successfully";
                    response.Data = community;
                    return Ok(response);
                }
                response.Success = false;
                response.Message = "Invalid model state";
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

    }
}
