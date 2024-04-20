using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class PostCommentDto
    {
        public int Id { get; set; } 
        public UserDto Author { get; set; } 
        public string Comment { get; set; } 
        public DateTime PublishedDate { get; set; } 
        public PostCommentDto()
        {
            Id = 0;
            Author = new();
            Comment = string.Empty;
            PublishedDate = DateTime.UtcNow;
        }
        public PostCommentDto(PostComment comment)
        {
            Id = comment.Id;
            Author = new UserDto(comment.Author);
            Comment = comment.Comment;
            PublishedDate = comment.PublishedDate;
        }
    }
}
