using PluginDemocracy.Models;
using System.Text.Json.Serialization;

namespace PluginDemocracy.DTOs.CommunitiesDto
{

    public class PostReactionDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("postId")]

        public int PostId { get; set; }
        [JsonPropertyName("user")]

        public UserDTO User { get; private set; }
        [JsonPropertyName("reactionType")]

        public ReactionType ReactionType { get; set; }
        public PostReactionDTO() 
        {
            User = new();
        }
        public PostReactionDTO(int postId, UserDTO user, ReactionType reactionType)
        {
            PostId = postId;
            User = user;
            ReactionType = reactionType;
        }
    }
}
