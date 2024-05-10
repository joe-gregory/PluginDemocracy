using PluginDemocracy.Models;
using System.Text.Json.Serialization;

namespace PluginDemocracy.DTOs.CommunitiesDto
{
   
    public class PostReactionDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("postId")]

        public int PostId { get; set; } 
        [JsonPropertyName("user")]

        public UserDto User { get; private set; } 
        [JsonPropertyName("reactionType")]

        public ReactionType ReactionType { get; set; } 
        [JsonConstructor]
        public PostReactionDto(int postId, UserDto user, ReactionType reactionType)
        {
            PostId = postId;
            User = user;
            ReactionType = reactionType;
        }
    }
}
