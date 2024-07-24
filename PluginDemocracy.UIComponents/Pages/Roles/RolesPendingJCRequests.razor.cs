using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Pages.Roles
{
    public partial class RolesPendingJCRequests
    {
        private List<ResidentialCommunityDTO> possibleCommunities = [];
        private List<JoinCommunityRequestDTO> joinCommunityRequestsForSelectedCommunity = [];
        private ResidentialCommunityDTO? selectedCommunity;
        protected override async Task OnInitializedAsync()
        {
            possibleCommunities = AppState.User?.Roles
                .Select(role => role.Community)
                .Where(community => community != null)
                .Distinct()
                .Select(community => community!) // Use the null-forgiving operator
                .ToList() ?? [];
            if (possibleCommunities.Count == 1) selectedCommunity = possibleCommunities[0];
            string url = ApiEndPoints.RolesGetListOfJCRequestsForGivenCommunity + $"?communityId={selectedCommunity?.Id}";
            if (selectedCommunity != null) joinCommunityRequestsForSelectedCommunity = await Services.GetDataGenericAsync<List<JoinCommunityRequestDTO>>(url) ?? [];
        }
        private async Task OnSelectedCommunityChanged(int? value)
        {
            if (value != null)
            {
                selectedCommunity = possibleCommunities.FirstOrDefault(community => community.Id == value);
                //Get new list of joinCommunityRequestsForSelectedCommunity
                string url = ApiEndPoints.RolesGetListOfJCRequestsForGivenCommunity + $"?communityId={value.Value}";
                joinCommunityRequestsForSelectedCommunity = await Services.GetDataGenericAsync<List<JoinCommunityRequestDTO>>(url) ?? [];
                StateHasChanged();
            }

        }
    }
}
