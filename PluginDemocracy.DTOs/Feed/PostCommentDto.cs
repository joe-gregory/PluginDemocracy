using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class PostCommentDTO
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public UserDTO Author { get; set; } 
        public string Comment { get; set; } 
        public DateTime PublishedDate { get; set; } 
        public PostCommentDTO()
        {
            Comment = string.Empty;
            Author = new UserDTO();
        }
        public PostCommentDTO(int postId, UserDTO author, string comment)
        {
            PostId = postId;
            Author = author;
            Comment = comment;
            PublishedDate = DateTime.UtcNow;
        }
        public PostCommentDTO(PostComment comment)
        {
            Id = comment.Id;
            Author = new UserDTO(comment.Author);
            Comment = comment.Comment;
            PublishedDate = comment.PublishedDate;
        }
    }
}
