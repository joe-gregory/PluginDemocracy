using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class PostReaction
    {
        public int Id { get; init; }
        public User User { get; init; }
        public ReactionType ReactionType { get; init; }
        public DateTime CreatedDate { get; init; }
        //Disabling CS8618 as this is a parameterless constructor for the benefit of EF Core
        #pragma warning disable CS8618
        private PostReaction() { }
        #pragma warning restore CS8618
        public PostReaction(User user, ReactionType reaction)
        {
            User = user;
            ReactionType = reaction;
            CreatedDate = DateTime.UtcNow;
        }
    }
    public enum ReactionType
    {
        Like,
        Dislike,
    }
}
