namespace PluginDemocracy.DTOs
{
    public class PetitionDTO
    {
        public int Id { get; set; }
        public bool Published { get; set; }
        public DateTime? PublishedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public CommunityDTO? CommunityDTO { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ActionRequested { get; set; }
        public string? SupportingArguments { get; set; }
        public DateTime? DeadlineForResponse { get; set; }
        public List<string> LinksToSupportingDocuments { get; set; } = [];
        public List<UserDTO> Authors { get; set; } = [];

    }
}
