
namespace PluginDemocracy.DTOs
{
    public class CommunityDto : BaseCitizenDto
    {
        public string? Name { get; set; }
        public override string? FullName => string.Join(" ", Name, Address);
        public string OfficialCurrency { get; set; } = "USD";
        public List<string> OfficialLanguages { get; set; } = [];
        public override List<CommunityDto> Citizenships
        {
            get
            {
                return [];
            }
        }
        //    virtual public List<BaseCitizenDto> Citizens
    }
}
