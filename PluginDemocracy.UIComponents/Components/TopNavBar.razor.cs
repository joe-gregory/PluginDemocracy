using Microsoft.AspNetCore.Components;

namespace PluginDemocracy.UIComponents.Components
{
    public partial class TopNavBar
    {
        private bool CommunityMenuButtonDisabled => AppState != null ? !AppState.HasInternet : false;
        private bool UserMenuButtonDisabled => AppState != null ? !AppState.HasInternet : false;
        private bool UserLoggedIn => AppState.User != null;
        private bool UserHasCommunity => AppState.User != null && AppState.User.Citizenships.Count != 0;
        private bool communityMenuOpen;
        private bool userMenuOpen;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            AppState.OnChange += StateHasChanged;
        }
        public void Dispose()
        {
            AppState.OnChange -= StateHasChanged;
        }
        void ToggleCommunityMenu()
        {
            communityMenuOpen = !communityMenuOpen;
        }

        void ToggleUserMenu()
        {
            if (UserMenuButtonDisabled) return;
            userMenuOpen = !userMenuOpen;
        }
    }
}
