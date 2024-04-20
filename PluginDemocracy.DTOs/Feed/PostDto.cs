using PluginDemocracy.DTOs.CommunitiesDto;
using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public UserDto? Author { get; set; }
        public string? Body { get; set; }
        public DateTime PublishedDate { get; set; }
        public List<PostCommentDto> Comments { get; set; } = [];
        public DateTime? LatestActivity { get; set; }
        public List<string> Images { get; set; } = [];
        public List<ReactionDto> Reactions { get; set; } = [];
        public PostDto()
        {
            Id = 0;
        }
        public PostDto(Post post)
        {
            Id = post.Id;
            if (post.Author != null) Author = new(post.Author);
            Body = post.Body;
            PublishedDate = post.PublishedDate;
            LatestActivity = post.LatestActivity;
        }

    }
}
