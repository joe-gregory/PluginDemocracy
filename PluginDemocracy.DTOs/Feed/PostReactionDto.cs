using PluginDemocracy.Models;
using System.Text.Json.Serialization;

namespace PluginDemocracy.DTOs.CommunitiesDto
{

    [method: JsonConstructor]
    public class PostReactionDTO(int postId, UserDTO user, ReactionType reactionType)
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("postId")]

        public int PostId { get; set; } = postId;
        [JsonPropertyName("user")]

        public UserDTO User { get; private set; } = user;
        [JsonPropertyName("reactionType")]

        public ReactionType ReactionType { get; set; } = reactionType;
    }
}
