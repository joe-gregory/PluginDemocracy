using MudBlazor;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Pages.Community
{
    public partial class JoinCommunity
    {
        private List<CommunityDTO> communitiesDtos = [];
        private CommunityDTO? selectedCommunityDto;
        private List<HomeDTO> homesDtosFromSelectedCommunity = [];
        private bool isJoinHomeDialogVisible = false;
        private readonly DialogOptions dialogOptions = new()
        {
            CloseButton = true,
            DisableBackdropClick = true,
            CloseOnEscapeKey = true,
        };
        private HomeDTO? selectedHomeDto;
        //false is resident, true is owner
        private bool residentOrOwner = false;
        private bool displayDialogErrorMessage = false;
        private string dialogErrorMessage = string.Empty;
        private double selectedOwnershipPercentage = 0;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //Make get request to get list of communities
            PDAPIResponse response = await Services.GetDataAsync(ApiEndPoints.GetListOfAllCommunities);
            communitiesDtos = response.AllCommunities;
        }

        private async Task OnSelectCommunityDtoChanged(CommunityDTO newValue)
        {
            selectedCommunityDto = newValue;
            //Now load the homes for the selected community
            if (selectedCommunityDto != null)
            {
                await LoadHomesForSelectedCommunity();
            }
        }
        private async Task LoadHomesForSelectedCommunity()
        {
            //Make get request to get list of homes for the selected community
            if (selectedCommunityDto == null) return;
            string fullUrl = ApiEndPoints.GetListOfHomesForCommunity + $"?communityId={selectedCommunityDto.Id}";
            PDAPIResponse response = await Services.GetDataAsync(fullUrl);
            if (response.Community != null) homesDtosFromSelectedCommunity = response.Community.Homes;
        }

        private void SelectedHome(int? homeId)
        {
            if (homeId == null) return;
            selectedHomeDto = homesDtosFromSelectedCommunity.FirstOrDefault(h => h.Id == homeId);
            displayDialogErrorMessage = false;
            dialogErrorMessage = string.Empty;
            isJoinHomeDialogVisible = true;
        }

        private async Task SendRequest()
        {
            displayDialogErrorMessage = false;
            dialogErrorMessage = string.Empty;
            JoinCommunityRequestDTO joinRequest;
            if (AppState.User != null && selectedHomeDto != null)
            {
                try
                {
                    selectedHomeDto.ParentCommunity = selectedCommunityDto;
                    joinRequest = selectedHomeDto.JoinHome(AppState.User, residentOrOwner, selectedOwnershipPercentage);
                    displayDialogErrorMessage = false;
                    dialogErrorMessage = string.Empty;
                    await Services.PostDataAsync(ApiEndPoints.JoinCommunityRequest, joinRequest);
                    isJoinHomeDialogVisible = false;
                }
                catch
                {
                    displayDialogErrorMessage = true;
                    dialogErrorMessage = AppState.Translate(Translations.ResourceKeys.ErrorMessageJoinHomeWrongPercentage);
                    return;
                }

            }
        }
    }
}
