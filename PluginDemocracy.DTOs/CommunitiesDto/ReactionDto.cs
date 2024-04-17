using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs.CommunitiesDto
{
    public class ReactionDto
    {
        public int Id { get; set; }
        public UserDto Author { get; private set; }
        public ReactionType ReactionType { get; set; }
        protected ReactionDto()
        {
            Author = new();
            ReactionType = ReactionType.Like;
        }
        public ReactionDto(UserDto user, ReactionType reaction)
        {
            Author = user;
            ReactionType = reaction;
        }
        public ReactionDto(Reaction reaction)
        {
            Id = reaction.Id;
            Author = new UserDto(reaction.Author);
            ReactionType = reaction.ReactionType;
        }
    }
}
