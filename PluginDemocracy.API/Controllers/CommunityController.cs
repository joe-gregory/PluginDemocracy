using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class CommunityController(PluginDemocracyContext context, APIUtilityClass utilityClass) : ControllerBase
    {
        private readonly PluginDemocracyContext _context = context;
        private readonly APIUtilityClass _utilityClass = utilityClass;

        [HttpPost("registercommunity")]
        public async Task<ActionResult<PDAPIResponse>> Register(CommunityDto communityDto)
        {
            PDAPIResponse response = new();
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
            Community newCommunity = new()
            {
                Name = communityDto.Name,
                Address = communityDto.Address,
                OfficialCurrency = communityDto.OfficialCurrency,
                Description = communityDto.Description,
                CanHaveHomes = true,
            };
            foreach (CultureInfo language in communityDto.OfficialLanguages) newCommunity.AddOfficialLanguage(language);
            foreach (HomeDto homeDTO in communityDto.Homes) newCommunity.AddHome(new Home()
            {
                ParentCommunity = newCommunity,
                Number = homeDTO.Number,
                InternalAddress = homeDTO.InternalAddress,
            });
            newCommunity.VotingStrategy = new HomeOwnersFractionalVotingStrategy();
            try
            {
                _context.Communities.Add(newCommunity);
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
                List<Community> communities = await _context.Communities.ToListAsync();
                foreach (Community community in communities) response.AllCommunities.Add(new CommunityDto()
                {
                    Id = community.Id,
                    Name = community.Name,
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
                Community? community = await _context.Communities.Include(c => c.Homes).FirstOrDefaultAsync(c => c.Id == communityId);
                if (community == null)
                {
                    response.AddAlert("error", "Community not found");
                    return BadRequest(response);
                }
                response.Community = CommunityDto.ReturnSimpleCommunityDtoFromCommunity(community);
                foreach (Home home in community.Homes) response.Community.Homes.Add(HomeDto.ReturnHomeDtoFromHome(home));
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.AddAlert("error", ex.Message);
                return BadRequest(response);
            }
        }
        /// <summary>
        /// Controller method by which a request to join a community is sent. 
        /// Community.AddJoinCommunityRequest makes validations. The controller needs to check that the user from claims 
        /// matches the user in the request and that the community matches but the rest of the validation is done in community
        /// If something doesn't work, the community will throw an exception and the controller will catch it and return a BadRequest. 
        /// Otherwise, save changes. The constructor for JoinCommunityReques also makes some verifications like you can only be joining 
        /// as owner or resident, not both.
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(ApiEndPoints.JoinCommunityRequest)]
        public async Task<ActionResult<PDAPIResponse>> JoinCommunityRequest(JoinCommunityRequestDto requestDto)
        {
            PDAPIResponse response = new();
            //Ensure User from claims is valid, that it matches the UserId in the request and that the Community exists.
            //All other verifications are done by the Community object. 
            //Extract User from claims
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User, response);
            if (existingUser == null)
            {
                response.AddAlert("error", "User from claims not found");
                return BadRequest(response);
            }
            //Does the user from claims match the user in the request?
            if (existingUser.Id != requestDto.UserId)
            {
                response.AddAlert("error", "User from claims does not match user in request");
                return BadRequest(response);
            }
            //Does the community exist?
            Community? community = await _context.Communities.Include(c => c.Homes).FirstOrDefaultAsync(c => c.Id == requestDto.CommunityId);
            if (community == null)
            {
                response.AddAlert("error", "Community not found");
                return BadRequest(response);
            }
            try
            {
                JoinCommunityRequest request = new(requestDto.HomeId, requestDto.UserId, requestDto.JoiningAsOwner, requestDto.OwnershipPercentage);
                community.AddJoinCommunityRequest(request);
                //At this point the request should be validated. It can be saved to database. 
                _context.SaveChanges();
                //Send notifications to the corresponding parties
                    //Send notification to role that has the capability to accept new home owners
                    //Send notification to app admin for now as well? 
                //How will notifications work? When you open up your app OnInitialized, lookup notifications for you.
                //In the common processing check if new notifications occurred, add bubble to topnavbar that you have notifications
                //Can notifications contain a link?
            }
            catch (Exception exception)
            {
                response.AddAlert("error", exception.Message);
                return BadRequest(response);
            }
        }
    }
}