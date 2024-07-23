using Microsoft.AspNetCore.Components;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Pages.User
{
    public partial class AboutUser
    {
        [Inject]
        private Services Services { get; set; } = default!;
        [Inject]
        private BaseAppState AppState { get; set; } = default!;
        [SupplyParameterFromQuery(Name = "userId")]
        public int? UserId { get; set; }
        UserDTO? User { get; set; }
        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
            //Make get request for user base of user id
           await Services.GetDataGenericAsync<UserDTO>(ApiEndPoints.RefreshUserData);
            User = AppState.User;

        }
    }
}
