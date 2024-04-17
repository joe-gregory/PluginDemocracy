using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class PostCommentDto(PostComment comment)
    {
        public int Id { get; set; } = comment.Id;
        public UserDto Author { get; set; } = new(comment.Author);
        public string Comment { get; set; } = comment.Comment;
        public DateTime PublishedDate { get; set; } = comment.PublishedDate;
    }
}
