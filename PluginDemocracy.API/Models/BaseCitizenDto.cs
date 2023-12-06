namespace PluginDemocracy.API.Models
{
    public abstract class BaseCitizenDto
    {
        public int? Id { get; set; }
        public Guid? Guid { get; set; }
        abstract public string? FullName { get; }
        virtual public string? Address { get; set; }
        public string? ProfilePicture { get; set; }
        //TODO: Add List<CommunityDto> Citizenships
        //TODO: Add List<CommunityDto> AssociatedCommunities
    }
}
