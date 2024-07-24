using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Pages.Roles
{
    public partial class RolesPendingJCRequests
    {
        private List<ResidentialCommunityDTO> possibleCommunities = [];
        private List<JoinCommunityRequestDTO> joinCommunityRequestsForSelectedCommunity = [];
        private ResidentialCommunityDTO? selectedCommunity;
        private int selectedCommunityId;
        protected override async Task OnInitializedAsync()
        {
            possibleCommunities = AppState.User?.Roles
                .Select(role => role.Community)
                .Where(community => community != null)
                .Distinct()
                .Select(community => community!) // Use the null-forgiving operator
                .ToList() ?? []; 
            if (possibleCommunities.Count == 1) selectedCommunity = possibleCommunities[0];
            string url = AppState.BaseUrl + ApiEndPoints.RolesGetListOfJCRequestsForGivenCommunity + $"?communityId={selectedCommunity?.Id}";
            if (selectedCommunity != null) joinCommunityRequestsForSelectedCommunity = await Services.GetDataGenericAsync<List<JoinCommunityRequestDTO>>(url) ?? [];
        }
        private async void OnSelectedCommunityChanged()
        {
            //Get new list of joinCommunityRequestsForSelectedCommunity
            string url = AppState.BaseUrl + ApiEndPoints.RolesGetListOfJCRequestsForGivenCommunity + $"?communityId={selectedCommunityId}";
            joinCommunityRequestsForSelectedCommunity = await Services.GetDataGenericAsync<List<JoinCommunityRequestDTO>>(url) ?? [];
            StateHasChanged();
        }
    }
}
