namespace PluginDemocracy.Models
{
    public class Post
    {
        public int Id { get; set; }
        public User? Author { get; private set; }
        public string? Body { get; private set; }
        public DateTime PublishedDate { get; private set; }
        public readonly List<PostComment> Comments;
        public DateTime? LatestActivity { get; private set; } = null;
        public List<string> Images { get; set; }
        public List<Reaction> Reactions { get; set; }
        protected Post() 
        {
            PublishedDate = DateTime.UtcNow;
            Comments = [];
            Images = [];
            Reactions = [];
        }
        public Post(User user, string body)
        {
            Author = user;
            Body = body;
            PublishedDate = DateTime.UtcNow;
            Comments = [];
            Images = [];
            Reactions = [];
        }
        public void AddComment(PostComment comment)
        {
            Comments.Add(comment);
            LatestActivity = DateTime.UtcNow;
        }
    }
}
