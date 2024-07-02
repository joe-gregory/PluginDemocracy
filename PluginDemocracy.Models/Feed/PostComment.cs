using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class PostComment
    {
        public int Id { get; init; }
        public Post Post { get; init; }
        public User Author { get; init; }
        public string Comment { get; init; }
        public DateTime PublishedDate { get; init; }
        //Disabling CS8618 as this is a parameterless constructor for the benefit of EF Core
        #pragma warning disable CS8618
        private PostComment() { }
        #pragma warning restore CS8618
        public PostComment(User user, Post post, string body)
        {
            Author = user;
            Post = post;
            Comment = body;
            PublishedDate = DateTime.UtcNow;
        }
    }
}
