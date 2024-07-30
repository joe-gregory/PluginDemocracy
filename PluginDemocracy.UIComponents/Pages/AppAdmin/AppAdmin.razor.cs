using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore.Metadata;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;
using PluginDemocracy.Models;

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
        private RolePowers powers = new();
        private List<UserDTO>? usersAvatarsOfCommunity;
        private bool disabledAll = false;
        private RoleDTO? roleToDelete;

        private const string usaFlag = "https://upload.wikimedia.org/wikipedia/en/thumb/a/a4/Flag_of_the_United_States.svg/2880px-Flag_of_the_United_States.svg.png";
        private const string mxnFlag = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Flag_of_Mexico.svg/2880px-Flag_of_Mexico.svg.png";
        private bool english = false;
        private bool spanish = false;
        private IBrowserFile? file;
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
            english = false;
            spanish = false;
            if (SelectedCommunity.OfficialLanguages.Contains(new System.Globalization.CultureInfo("en-US"))) english = true;
            if (SelectedCommunity.OfficialLanguages.Contains(new System.Globalization.CultureInfo("es-MX"))) spanish = true;
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
            roleToAdd.Powers = powers;
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
        
        private async void UpdateCommunityInfo()
        {
            if (SelectedCommunity != null)
            {
                SelectedCommunity.OfficialLanguagesCodes = [];
                if (english) SelectedCommunity.OfficialLanguagesCodes.Add("en-US");
                if (spanish) SelectedCommunity.OfficialLanguagesCodes.Add("es-MX");
                ResidentialCommunityDTO? updatedCommunity = await Services.PostDataGenericAsync<ResidentialCommunityDTO, ResidentialCommunityDTO>(ApiEndPoints.AdminUpdateCommunityInfo, SelectedCommunity);
                if (updatedCommunity != null)
                { 
                    SelectedCommunity = updatedCommunity;
                    StateHasChanged();
                }
                else Services.AddSnackBarMessage("error", "Expected a community object but got null");
            }
            else
            {
                Services.AddSnackBarMessage("error", "Error. Post not sent. SelectedCommunity null.");
            }

        }
        private async void UpdateCommunityPicture()
        {
            if (file == null)
            {
                Services.AddSnackBarMessage("warning", AppState.Translate(Translations.ResourceKeys.PleaseSelectAFile));
                return;
            }
            string endpoint = $"{ApiEndPoints.AdminUpdateCommunityPicture}?communityId={SelectedCommunity?.Id}";
            await Services.UploadFileAsync(endpoint, file);
            file = null;
        }
    }
}
