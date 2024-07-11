using Microsoft.AspNetCore.Components;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Pages.Community
{
    public partial class JoinCommunityRequests
    {
        [SupplyParameterFromQuery]
        public int? RequestId { get; set; }
        private MudBlazor.Color statusColor = MudBlazor.Color.Primary;
        private string statusText = "Pending";
        private JoinCommunityRequestDTO? joinCommunityRequestDTO;
        protected override async Task OnInitializedAsync()
        {
            if (Services != null && RequestId != null) joinCommunityRequestDTO = await Services.GetDataGenericAsync<JoinCommunityRequestDTO>($"{ApiEndPoints.GetJoinCommunityRequest}?requestId={RequestId}");
            if (joinCommunityRequestDTO != null)
            {
                if (joinCommunityRequestDTO.Approved == null)
                {
                    statusColor = MudBlazor.Color.Info;
                    statusText = "Pending";
                }
                else if (joinCommunityRequestDTO.Approved == true)
                {
                    statusColor = MudBlazor.Color.Success;
                    statusText = "Approved";
                }
                else
                {
                    statusColor = MudBlazor.Color.Error;
                    statusText = "Denied";
                }
                StateHasChanged();
            }
        }
    }
}
