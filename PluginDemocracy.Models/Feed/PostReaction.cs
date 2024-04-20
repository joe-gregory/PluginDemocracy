using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class PostReaction
    {
        public int Id { get; set; }
        public User User { get; private set; }
        public ReactionType ReactionType { get; set; }
        public DateTime CreatedDate { get; protected set; } = DateTime.UtcNow;
        protected PostReaction()
        {
            User = new();
            ReactionType = ReactionType.Like;
        }
        public PostReaction(User user, ReactionType reaction)
        {
            User = user;
            ReactionType = reaction;
        }
    }
    public enum ReactionType
    {
        Like,
        Dislike,
    }
}
