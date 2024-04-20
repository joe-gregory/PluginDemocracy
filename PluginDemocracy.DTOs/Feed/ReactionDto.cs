using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs.CommunitiesDto
{
    public class ReactionDto
    {
        public int Id { get; set; }
        public UserDto User { get; private set; }
        public ReactionType ReactionType { get; set; }
        protected ReactionDto()
        {
            User = new();
            ReactionType = ReactionType.Like;
        }
        public ReactionDto(UserDto user, ReactionType reaction)
        {
            User = user;
            ReactionType = reaction;
        }
        public ReactionDto(PostReaction reaction)
        {
            Id = reaction.Id;
            User = new UserDto(reaction.User);
            ReactionType = reaction.ReactionType;
        }
    }
}
