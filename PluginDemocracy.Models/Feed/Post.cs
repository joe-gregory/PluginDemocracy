namespace PluginDemocracy.Models
{
    public class Post
    {
        public int Id { get; set; }
        public User? Author { get; private set; }
        public string? Body { get; private set; }
        public DateTime? PublishedDate { get; private set; }
        protected Post() { }
        public Post(User user, string body)
        {
            Author = user;
            Body = body;
            PublishedDate = DateTime.UtcNow;
        }
    }
}
