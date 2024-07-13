using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.UIComponents.Pages.AppAdmin
{
    public partial class AppAdmin
    {
        private List<ResidentialCommunityDTO>? CommunitiesDto = [];
        private ResidentialCommunityDTO? SelectedCommunity = null;
        private List<JoinCommunityRequestDTO>? PendingJoinCommunityRequests = null;
        private bool IsDialogOpen = false;
        private JoinCommunityRequestDTO? SelectedJoinRequest = null;
        private readonly RoleDTO roleToAdd = new()
        {
            Powers = new()
        };
        private List<UserDTO>? usersAvatarsOfCommunity;
        private bool disabledAll = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //Make sure current user is admin if not, redirect to home page
            bool isUserAdmin = await Services.GetDataGenericAsync<bool>(ApiEndPoints.AdminIsCurrentUserAdmin);
            if (!isUserAdmin) Services.NavigateTo(FrontEndPages.Home);
            //Get a list of all the communities
            CommunitiesDto = await Services.GetDataGenericAsync<List<ResidentialCommunityDTO>>(ApiEndPoints.AdminGetListOfAllSimpleCommunitiesDtos);
        }
        private async Task OnSelectCommunityDtoChanged(ResidentialCommunityDTO newValue)
        {
            disabledAll = true;
            SelectedCommunity = newValue;
            //Now load the requests for the selected community
            if (SelectedCommunity != null)
            {
                //Make a Get request for the JoinCommunityRequests for that community.
                PendingJoinCommunityRequests = await Services.GetDataGenericAsync<List<JoinCommunityRequestDTO>>(ApiEndPoints.AdminGetPendingJoinCommunityRequests + "?communityId=" + SelectedCommunity.Id);
                await GetListOfUserAvatarsForCommunity();
            }
            disabledAll = false;
        }
        private async Task ApproveJoinRequest()
        {
            //Make a post request to approve the request
            await Services.PostDataAsync<JoinCommunityRequestDTO>(ApiEndPoints.AdminAcceptJoinRequest, SelectedJoinRequest);
            //Reload the requests

            await CommonDecision();
        }
        private async Task RejectJoinRequest()
        {
            //Make a post request to reject the request
            await Services.PostDataAsync<JoinCommunityRequestDTO>(ApiEndPoints.AdminRejectJoinRequest, SelectedJoinRequest);
            //Reload the requests
            await CommonDecision();
        }
        private async Task CommonDecision()
        {
            PendingJoinCommunityRequests = await Services.GetDataGenericAsync<List<JoinCommunityRequestDTO>>(ApiEndPoints.AdminGetPendingJoinCommunityRequests + "?communityId=" + SelectedCommunity?.Id);
            IsDialogOpen = false;
        }
        private void OnRowClicked(JoinCommunityRequestDTO request)
        {
            SelectedJoinRequest = request;
            IsDialogOpen = true;
        }
        private async Task GetListOfUserAvatarsForCommunity()
        {
            usersAvatarsOfCommunity = await Services.GetDataGenericAsync<List<UserDTO>>(ApiEndPoints.GetListOfAvatarUsersForACommunity + "?communityId=" + SelectedCommunity?.Id);
        }
        private async void CreateAndAssignRole()
        {
            disabledAll = true;
            await Services.PostDataAsync(ApiEndPoints.AdminCreateAndAssignRole, roleToAdd);
            disabledAll = false;
        }
    }
}
