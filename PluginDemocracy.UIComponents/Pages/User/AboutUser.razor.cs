using Microsoft.AspNetCore.Components;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Pages.User
{
    public partial class AboutUser
    {
        [SupplyParameterFromQuery(Name = "userId")]
        public int? UserId { get; set; }
        UserDTO? User { get; set; }
        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
            //If no user id is supplied, use the current user

            if (UserId == null)
            {
                User = await Services.GetDataGenericAsync<UserDTO>($"{ApiEndPoints.GetUserAbout}?userId={AppState.User?.Id}");
                return;
            }
            //Make get request for user base of user id
            else
            {
                User = await Services.GetDataGenericAsync<UserDTO>($"{ApiEndPoints.GetUserAbout}?userId={UserId}");
                return;
            }
        }
    }
}
