using Microsoft.AspNetCore.Components;

namespace PluginDemocracy.UIComponents.Components
{
    public partial class TopNavBar
    {
        private bool CommunityMenuButtonDisabled => AppState != null && !AppState.HasInternet;
        private bool UserMenuButtonDisabled => AppState != null && !AppState.HasInternet;
        private bool UserLoggedIn => AppState.User != null;
        private bool UserHasCommunity => AppState?.User?.Citizenships.Count >= 1;
        private bool communityMenuOpen;
        private bool userMenuOpen;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            AppState.OnChange += () => InvokeAsync(StateHasChanged);
        }
        public void Dispose()
        {
            AppState.OnChange -= () => InvokeAsync(StateHasChanged);
        }
        void ToggleCommunityMenu()
        {
            if (CommunityMenuButtonDisabled) return;
            communityMenuOpen = !communityMenuOpen;
        }

        void ToggleUserMenu()
        {
            if (UserMenuButtonDisabled) return;
            userMenuOpen = !userMenuOpen;
        }
    }
}
