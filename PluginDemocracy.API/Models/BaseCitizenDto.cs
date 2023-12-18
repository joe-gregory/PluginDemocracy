namespace PluginDemocracy.API.Models
{
    public abstract class BaseCitizenDto
    {
        public int? Id { get; set; }
        abstract public string? FullName { get; }
        virtual public string? Address { get; set; }
        public string? ProfilePicture { get; set; }
        //TODO: Add List<CommunityDto> Citizenships
        public List<CommunityDto> Citizenships { get; set; }
        //TODO: Add List<CommunityDto> AssociatedCommunities
        public BaseCitizenDto()
        {
            Citizenships = new();
        }
    }
}
