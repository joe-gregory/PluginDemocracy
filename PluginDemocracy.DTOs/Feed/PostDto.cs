using PluginDemocracy.DTOs.CommunitiesDto;
using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class PostDTO
    {
        public int Id { get; set; }
        public IAvatar? Author { get; set; }
        public string? Body { get; set; }
        public DateTime PublishedDate { get; set; }
        public List<PostCommentDTO> Comments { get; set; } = [];
        public DateTime? LatestActivity { get; set; }
        public List<string> Images { get; set; } = [];
        public List<PostReactionDto> Reactions { get; set; } = [];
        public int NumberOfLikeReactions { get => Reactions.Where(r => r.ReactionType == ReactionType.Like).Count(); }
        public int NumberOfDislikeReactions { get => Reactions.Where(r => r.ReactionType == ReactionType.Dislike).Count(); }
        public PostDTO()
        {
            Id = 0;
        }
        public PostDTO(Post post)
        {
            Id = post.Id;
            if (post.Author is User author) Author = UserDTO.ReturnSimpleUserDTOFromUser(author);
            else if(post.Author is ResidentialCommunity community) Author = ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(community);
            Body = post.Body;
            PublishedDate = post.PublishedDate;
            LatestActivity = post.LatestActivity;
            Images = [..post.Images];
            foreach(PostReaction reaction in post.Reactions)
            {
                Reactions.Add(new(post.Id, new(reaction.User), reaction.ReactionType));
            }
            foreach(PostComment comment in post.Comments)
            {
                Comments.Add(new(comment));
            }
        }

    }
}
