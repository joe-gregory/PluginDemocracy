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
        private List<ResidentialCommunityDTO>? CommunitiesDTO = [];
        private ResidentialCommunityDTO? SelectedCommunity = null;
        private List<JoinCommunityRequestDTO>? PendingJoinCommunityRequests = null;
        private readonly RoleDTO roleToAdd = new()
        {
            Powers = new()
        };
        private List<UserDTO>? usersAvatarsOfCommunity;
        private bool disabledAll = false;
        private RoleDTO? roleToDelete;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //Make sure current user is admin if not, redirect to home page
            bool isUserAdmin = await Services.GetDataGenericAsync<bool>(ApiEndPoints.AdminIsCurrentUserAdmin);
            if (!isUserAdmin) Services.NavigateTo(FrontEndPages.Home);
            //Get a list of all the communities
            CommunitiesDTO = await Services.GetDataGenericAsync<List<ResidentialCommunityDTO>>(ApiEndPoints.AdminGetListOfAllSimpleCommunitiesDTOsWithRoles);
        }
        private async Task OnSelectCommunityDTOChanged(ResidentialCommunityDTO newValue)
        {
            disabledAll = true;
            SelectedCommunity = newValue;
            //Now load the requests for the selected community
            if (SelectedCommunity != null)
            {
                //Make a Get request for the JoinCommunityRequests for that community.
                PendingJoinCommunityRequests = await Services.GetDataGenericAsync<List<JoinCommunityRequestDTO>>(ApiEndPoints.AdminGetPendingJoinCommunityRequestsForACommunity + "?communityId=" + SelectedCommunity.Id);
                 await GetListOfUserAvatarsForCommunity();
            }
            disabledAll = false;
        }
        private async Task GetListOfUserAvatarsForCommunity()
        {
            usersAvatarsOfCommunity = await Services.GetDataGenericAsync<List<UserDTO>>(ApiEndPoints.GetListOfAvatarUsersForACommunity + "?communityId=" + SelectedCommunity?.Id);
        }
        private async void CreateAndAssignRole()
        {
            disabledAll = true;
            roleToAdd.Community = SelectedCommunity;
            await Services.PostDataAsync(ApiEndPoints.AdminCreateAndAssignRole, roleToAdd);
            disabledAll = false;
        }
        private async void UnassignAndDeleteRole()
        {
            if (roleToDelete != null)
            {
                disabledAll = true;
                await Services.PostDataAsync<object>($"{ApiEndPoints.AdminDeleteAndUnassignRole}?roleId={roleToDelete.Id}");
                disabledAll = false;
            }
            
        }
    }
}
