using Microsoft.AspNetCore.Components;

namespace PluginDemocracy.UIComponents
{
    public partial class TopNavBar
    {
        public bool CommunityMenuButtonDisabled => !HasInternet;
        public bool UserMenuButtonDisabled => !HasInternet;

        private bool communityMenuOpen = false;
        private bool userMenuOpen = false;

        [Parameter]
        public bool IsLoggedIn { get; set;}
        [Parameter]
        public bool HasInternet { get;set; }

        void ToggleCommunityMenu()
        {
            communityMenuOpen = !communityMenuOpen;
        }

        void ToggleUserMenu()
        {
            if (!HasInternet) return;
            userMenuOpen = !userMenuOpen;
        }
    }
}
