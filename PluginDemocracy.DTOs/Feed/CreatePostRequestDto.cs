using Microsoft.AspNetCore.Http;

namespace PluginDemocracy.DTOs
{
    public class CreatePostRequestDto
    {
        public int CommunityId { get; set; }
        public string? Body { get; set; }
        public List<IFormFile> Files { get; set; } = [];
    }
}
