using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class PostCommentDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public UserDTO Author { get; set; } 
        public string Comment { get; set; } 
        public DateTime PublishedDate { get; set; } 
        public PostCommentDto()
        {
            Comment = string.Empty;
            Author = new UserDTO();
        }
        public PostCommentDto(int postId, UserDTO author, string comment)
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
            Author = new UserDTO(comment.Author);
            Comment = comment.Comment;
            PublishedDate = comment.PublishedDate;
        }
    }
}
