using Microsoft.AspNetCore.Components;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Pages.Community
{
    public partial class AboutCommunity
    {
        [SupplyParameterFromQuery]
        protected int? CommunityId { get; set; }
        protected ResidentialCommunityDTO? CommunityDTO;
        protected override async Task OnInitializedAsync()
        {
            if (AppState.User?.Citizenships.Count == 1 && CommunityId == null ) CommunityId = AppState.User.Citizenships[0].Id;
            if (CommunityId != null) CommunityDTO = await Services.GetDataGenericAsync<ResidentialCommunityDTO>($"{ApiEndPoints.GetCommunityAbout}?communityId={CommunityId}");
        }
        private async Task OnSelectedCommunityChanged(int communityId)
        {
            CommunityDTO = await Services.GetDataGenericAsync<ResidentialCommunityDTO>($"{ApiEndPoints.GetCommunityAbout}?communityId={communityId}");
        }
    }
}
