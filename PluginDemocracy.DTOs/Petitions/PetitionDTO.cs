namespace PluginDemocracy.DTOs
{
    public class PetitionDTO
    {
        public int Id { get; set; }
        public CommunityDTO? CommunityDTO { get; set; }
        public bool Published { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<UserDTO> Authors { get; set; } = [];
        public string? ActionRequested { get; set; }
        public List<string> LinksToSupportingDocuments { get; set; } = [];
    }
}
