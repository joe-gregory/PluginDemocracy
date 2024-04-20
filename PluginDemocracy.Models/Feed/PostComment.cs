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
        public User Author { get; private set; }
        public string Comment { get; private set; }
        public DateTime PublishedDate { get; protected set; } = DateTime.UtcNow;
        protected PostComment()
        {
            Comment = string.Empty;
            Author = new();
        }
        public PostComment(User user, string body)
        {
            Author = user;
            Comment = body;
        }
    }
}
