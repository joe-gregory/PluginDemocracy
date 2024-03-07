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
        private List<CommunityDto> communitiesDtos = new();
        private CommunityDto? selectedCommunityDto;
        private List<HomeDto> homesDtosFromSelectedCommunity = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //Make get request to get list of communities
            PDAPIResponse response = await Services.GetDataAsync(ApiEndPoints.GetListOfAllCommunities);
            communitiesDtos = response.AllCommunities;
        }

        private void OnSelectCommunityDtoChanged(CommunityDto newValue)
        {
            selectedCommunityDto = newValue;
            //Now load the homes for the selected community
            if(selectedCommunityDto!= null)
            {

            }
        }
        private async void LoadHomesForSelectedCommunity()
        {
            //Make get request to get list of homes for the selected community
            PDAPIResponse response = await Services.GetDataAsync(ApiEndPoints.GetListOfHomesForCommunity, selectedCommunityDto.Id);
            if(response.Community != null) homesDtosFromSelectedCommunity = response.Community.Homes;
        }
    }
}
