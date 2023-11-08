namespace PluginDemocracy.Models
{
    public class Post
    {
        public int Id { get; set; }
        public User Author { get; }
        public string Body { get; }
        public DateTime PublishedDate {get;}
        public Post(User user, string body)
        {
            Author = user;
            Body = body;
            PublishedDate = DateTime.UtcNow;
        }
    }
}
