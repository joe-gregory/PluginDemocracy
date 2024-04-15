using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class PostComment
    {
        public int Id { get; set; }
        public User? Author { get; private set; }
        public string? Comment { get; private set; }
        public DateTime PublishedDate { get; private set; }
        protected PostComment()
        {
            PublishedDate = DateTime.UtcNow;
        }
        public PostComment(User user, string body)
        {
            Author = user;
            Comment = body;
            PublishedDate = DateTime.UtcNow;
        }
    }
}
