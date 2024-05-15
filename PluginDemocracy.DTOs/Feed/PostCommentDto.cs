using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class PostCommentDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public UserDto Author { get; set; } 
        public string Comment { get; set; } 
        public DateTime PublishedDate { get; set; } 
        public PostCommentDto()
        {
            Comment = string.Empty;
            Author = new UserDto();
        }
        public PostCommentDto(int postId, UserDto author, string comment)
        {
            PostId = postId;
            Author = author;
            Comment = comment;
            PublishedDate = DateTime.UtcNow;
        }
        public PostCommentDto(PostComment comment)
        {
            Id = comment.Id;
            PostId = comment.PostId;
            Author = new UserDto(comment.Author);
            Comment = comment.Comment;
            PublishedDate = comment.PublishedDate;
        }
    }
}
