using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
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
                ResidentialCommunity? community = await _context.ResidentialCommunities.Include(c => c.Homes).FirstOrDefaultAsync(c => c.Id == communityId);
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
            if(joinCommunityRequestUploadDTO.CommunityId == 0)
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
            if(joinCommunityRequestUploadDTO.JoiningAsOwner == joinCommunityRequestUploadDTO.JoiningAsResident)
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
            if(joinCommunityRequestUploadDTO.JoiningAsOwner == false && joinCommunityRequestUploadDTO.JoiningAsResident == false)
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
                string link = $"{_utilityClass.WebAppBaseUrl}{FrontEndPages.JoinCommunityRequest}?requestId={joinCommunityRequest.Id}";
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
                pdApiresponse.RedirectTo = $"{FrontEndPages.JoinCommunityRequest}?requestId={joinCommunityRequest.Id}";
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
                JoinCommunityRequestDTO jcr = new();
                jcr.Id = joinRequest.Id;
                jcr.CommunityDTO = new()
                {
                    Name = joinRequest.Community.Name
                };
                jcr.JoiningAsOwner = joinRequest.JoiningAsOwner;
                jcr.JoiningAsResident = joinRequest.JoiningAsResident;
                jcr.Approved = joinRequest.Approved;
                jcr.DateRequested = joinRequest.DateRequested;

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
            JoinCommunityRequest? joinRequest = await _context.JoinCommunityRequests.Include(j => j.Community).Include(j => j.Home).Include(j => j.User).FirstOrDefaultAsync(j => j.Id == requestId);
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
            if (!canThisPersonViewIt) return Forbid();
            return Ok(new JoinCommunityRequestDTO(joinRequest));
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

                    newPost.AddImage(blobClient.Uri.ToString());
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
            if (existingUser == null) return BadRequest();
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

    }
}