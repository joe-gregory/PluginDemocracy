using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class Reaction
    {
        public int Id { get; set; }
        public User Author { get; private set; }
        public ReactionType ReactionType { get; set; }
        protected Reaction()
        {
            Author = new();
            ReactionType = ReactionType.Like;
        }
        public Reaction(User user, ReactionType reaction)
        {
            Author = user;
            ReactionType = reaction;
        }
    }
    public enum ReactionType
    {
        Like,
        Dislike,
    }
}
