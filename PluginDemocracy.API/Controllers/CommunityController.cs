using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
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
    public class CommunityController(PluginDemocracyContext context, APIUtilityClass utilityClass, IConfiguration configuration) : ControllerBase
    {
        private readonly PluginDemocracyContext _context = context;
        private readonly APIUtilityClass _utilityClass = utilityClass;
        private readonly IConfiguration _configuration = configuration;

        [Authorize]
        [HttpPost("registercommunity")]
        public async Task<ActionResult<PDAPIResponse>> Register(CommunityDTO communityDto)
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
            foreach (HomeDTO homeDTO in communityDto.Homes) newCommunity.AddHome(new Home()
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
                foreach (Community community in communities) response.AllCommunities.Add(new CommunityDTO()
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
                response.Community = CommunityDTO.ReturnSimpleCommunityDtoFromCommunity(community);
                foreach (Home home in community.Homes) response.Community.Homes.Add(HomeDTO.ReturnHomeDtoFromHome(home));
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
        public async Task<ActionResult<PDAPIResponse>> JoinCommunityRequest(JoinCommunityRequestDTO requestDto)
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
            if(requestDto.UserDto == null)
            {
                response.AddAlert("error", "UserDto is null");
                return BadRequest(response);
            }
            if (existingUser.Id != requestDto.UserDto.Id)
            {
                response.AddAlert("error", "User from claims does not match user in request");
                return BadRequest(response);
            }
            //Does the community exist?
            if(requestDto.CommunityDto == null)
            {
                response.AddAlert("error", "CommunityDto is null");
                return BadRequest(response);
            }
            Community? community = await _context.Communities.Include(c => c.Homes).FirstOrDefaultAsync(c => c.Id == requestDto.CommunityDto.Id);
            if (community == null)
            {
                response.AddAlert("error", "Community not found");
                return BadRequest(response);
            }
            if(requestDto.JoiningAsOwner == false && requestDto.JoiningAsResident == false)
            {
                response.AddAlert("error", "Both Request.JoiningAsOwner and Request.JoiningAsResident can't be false.");
                return BadRequest(response);
            }
            try
            {
                Home home = community.Homes.First(h => h.Id == requestDto.HomeDto?.Id);
                if(requestDto.JoiningAsResident == true) requestDto.JoiningAsOwner = false;
                //at this point requestDto.JoiningAsOwner should not be null, so I am going to go ahead and add this bool variable
                JoinCommunityRequest request = new(community, home, existingUser, requestDto.JoiningAsOwner, requestDto.OwnershipPercentage);
                community.AddJoinCommunityRequest(request);
                Home homeToJoin = community.Homes.First(h => h.Id == requestDto.HomeDto?.Id);

                //Send notifications to the corresponding parties
                //Send notification to role that has the capability to accept new home owners. Lookup all the Roles that have CanEditHomeOwnership and CanEditResidency
                //Search for all the roles that have CanEditHomeOwnership and CanEditResidency in the Community
                //If user is joining as home owner, send notification to Roles with corresponding powers. If role is not there, default send notification to app admin.
                List<User?> roleHoldersWithJoinPower = community.Roles.Where(r => r.Powers.CanEditHomeOwnership && r.Powers.CanEditResidency).Select(r => r.Holder).ToList();
                string body = $"{existingUser.FullName} has requested to join the community as a {(request.JoiningAsOwner ? $"home owner" : "resident")} for home {homeToJoin.FullName} in community {community.FullName}";
                string? appManagerEmail = _configuration["PluginDemocracy:AppManagerEmail"];
                if (request.JoiningAsOwner)
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
                    if (roleHoldersWithJoinPower.Count == 0 && appManagerEmail != null) await _utilityClass.SendEmailAsync(appManagerEmail, _utilityClass.Translate(ResourceKeys.NewJoinRequest), body);
                }
                //If user is joining as resident, send notification to home owner AND roles with powers, default to app admin otherwise. 
                else
                {
                    List<User> usersToEmail = [];
                    foreach (BaseCitizen baseCitizen in homeToJoin.Owners) if (baseCitizen is User user) usersToEmail.Add(user);
                    foreach (User? user in roleHoldersWithJoinPower) if (user != null) usersToEmail.Add(user);
                    //send the emails and add the notifications
                    foreach (User user in usersToEmail)
                    {
                        user.AddNotification(_utilityClass.Translate(ResourceKeys.NewJoinRequest, user.Culture), body);
                        await _utilityClass.SendEmailAsync(user.Email, _utilityClass.Translate(ResourceKeys.NewJoinRequest, user.Culture), body);
                    }
                    if (homeToJoin.Owners.Count == 0 && appManagerEmail != null) await _utilityClass.SendEmailAsync(appManagerEmail, _utilityClass.Translate(ResourceKeys.NewJoinRequest), body);
                }
                //The emails and notifications have been sent.

                //At this point the request should be validated and the notifications sent. It can be saved to database. 
                _context.SaveChanges();
                response.AddAlert("success", "Request sent successfully. La solicitud ha sido enviada.");
                return Ok(response);
            }
            catch (Exception exception)
            {
                response.AddAlert("error", exception.Message);
                return BadRequest(response);
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
            Community? community = await _context.Communities.FirstOrDefaultAsync(c => c.Id == request.CommunityId);
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

                foreach (IFormFile file in request.Files)
                {
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                    {
                        response.AddAlert("error", $"File type not supported. Only .jpg, .jpeg, and .png are supported. Filename: {file.Name}");
                        return BadRequest(response);
                    }

                    Guid guid = Guid.NewGuid();
                    string blobName = $"posts/{newPost.Id}/images/{guid}";

                    //Create a blob client for the image
                    BlobClient blobClient = containerClient.GetBlobClient(blobName);
                    await using Stream filestream = file.OpenReadStream();
                    //Upload the image
                    await blobClient.UploadAsync(filestream, new BlobHttpHeaders { ContentType = file.ContentType });
                    newPost.Images.Add(blobClient.Uri.ToString());
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
                Community? community = await _context.Communities
                    .Include(c => c.Posts)
                        .ThenInclude(p => p.Author)
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
                return Ok(response);
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
            if (post == null)
            {
                response.AddAlert("error", "Post not found");
                return BadRequest(response);
            }
            if (post?.Author?.Id != existingUser.Id)
            {
                response.AddAlert("error", "User does not have permission to delete this post");
                return BadRequest(response);
            }
            try
            {

                //If post has images in the blob storage, delete them
                if (post.Images.Count != 0)
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
        public async Task<ActionResult<List<PostReactionDto>>> LikePost(PostReactionDto reactionDto)
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
        public async Task<ActionResult<PostDTO>> AddCommentToPost(PostCommentDto postCommentDto)
        {
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();
            Post? post = await _context.Posts.Include(p => p.Author).Include(p => p.Reactions).Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == postCommentDto.PostId);
            if (post == null) return BadRequest();

            PostComment newPostComment = new(existingUser, postCommentDto.Comment);
            try
            {
                post.AddComment(newPostComment);
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
        public async Task<ActionResult<PDAPIResponse>> DeleteComment(PostCommentDto postCommentDto)
        {
            PDAPIResponse response = new();
            User? existingUser = await _utilityClass.ReturnUserFromClaims(User);
            if (existingUser == null) return BadRequest();
            Post? post = await _context.Posts.Include(p => p.Author).Include(p => p.Reactions).Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == postCommentDto.PostId);
            if (post == null) return BadRequest();

            PostComment? comment = post.Comments.FirstOrDefault(c => c.Id == postCommentDto.Id);
            if (comment == null) return BadRequest();

            if (comment.Author.Id != existingUser.Id) return BadRequest();

            try
            {
                post.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                response.SuccessfulOperation = true;
                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }

    }
}