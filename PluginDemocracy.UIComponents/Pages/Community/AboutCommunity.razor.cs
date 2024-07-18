using Microsoft.AspNetCore.Components;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Community
{
    public class AboutCommunity : ComponentBase
    {
        [Inject]
        Services Services { get; set; } = default!;
        [Inject]
        BaseAppState AppState { get; set; } = default!;
        [SupplyParameterFromQuery]
        protected int? CommunityId { get; set; }
        protected ResidentialCommunityDTO? CommunityDTO;
        protected override async Task OnInitializedAsync()
        {
            if (CommunityId != null)
            {
                CommunityDTO = await Services.GetDataGenericAsync<ResidentialCommunityDTO>($"{ApiEndPoints.GetCommunityAbout}?communityId={CommunityId}");
            }
        }
    }

}
