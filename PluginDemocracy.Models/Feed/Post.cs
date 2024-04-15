namespace PluginDemocracy.Models
{
    public class Post
    {
        public int Id { get; set; }
        public User? Author { get; private set; }
        public string? Body { get; private set; }
        public DateTime PublishedDate { get; private set; }
        public readonly List<PostComment> Comments;
        public DateTime? LatestComment { get; private set; } = null;
        public List<string> Images { get; set; }
        protected Post() 
        {
            PublishedDate = DateTime.UtcNow;
            Comments = [];
            Images = [];
        }
        public Post(User user, string body)
        {
            Author = user;
            Body = body;
            PublishedDate = DateTime.UtcNow;
            Comments = [];
            Images = [];
        }
        public void AddComment(PostComment comment)
        {
            Comments.Add(comment);
            LatestComment = DateTime.UtcNow;
        }
    }
}
