using MudBlazor;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.UIComponents.Pages
{
    public partial class JoinCommunity
    {
        private List<CommunityDto> communitiesDtos = [];
        private CommunityDto? selectedCommunityDto;
        private List<HomeDto> homesDtosFromSelectedCommunity = [];
        private bool isJoinHomeDialogVisible = false;
        private DialogOptions dialogOptions = new()
        {
            CloseButton = true,
            DisableBackdropClick = true,
        };
        private HomeDto? selectedHomeDto;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //Make get request to get list of communities
            PDAPIResponse response = await Services.GetDataAsync(ApiEndPoints.GetListOfAllCommunities);
            communitiesDtos = response.AllCommunities;
        }

        private async Task OnSelectCommunityDtoChanged(CommunityDto newValue)
        {
            selectedCommunityDto = newValue;
            //Now load the homes for the selected community
            if(selectedCommunityDto!= null)
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
            if(response.Community != null) homesDtosFromSelectedCommunity = response.Community.Homes;
        }

        private void SelectedHome(int? homeId)
        {
            if (homeId == null) return;
            selectedHomeDto = homesDtosFromSelectedCommunity.FirstOrDefault(h => h.Id == homeId);
            isJoinHomeDialogVisible = true;
        }
    }
}
