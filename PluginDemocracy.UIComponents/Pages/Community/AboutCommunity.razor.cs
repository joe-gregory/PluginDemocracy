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
            CommunityDTO = await Services.GetDataGenericAsync<ResidentialCommunityDTO>($"{ApiEndPoints.GetCommunityAbout}?communityId={CommunityId}");
        }
    }
}
