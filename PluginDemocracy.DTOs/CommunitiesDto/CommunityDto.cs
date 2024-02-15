
namespace PluginDemocracy.DTOs
{
    public class CommunityDto : BaseCitizenDto
    {
        public override string? FullName => throw new NotImplementedException();

        public override List<CommunityDto> Citizenships => throw new NotImplementedException();
    }
}
