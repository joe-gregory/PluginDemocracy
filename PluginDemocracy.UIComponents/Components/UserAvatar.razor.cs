using Microsoft.AspNetCore.Components;
using MudBlazor;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Components
{
    /// <summary>
    /// It takes a UserDTO parameter. It automatically sets the image to the user's 
    /// profile picture or initials if there is no profile picture. 
    /// If you click on the avatar, it takes you to the users profile page. 
    /// </summary>
    public partial class UserAvatar
    {
        [Inject]
        Services Services { get; set; } = default!;
        [Parameter]
        public UserDTO? User { get; set; }
        [Parameter]
        public ResidentialCommunityDTO? Community { get; set; }
        [Parameter]
        public MudBlazor.Size Size { get; set; }
        private void NavigateToProfile()
        {
            if (User != null) Services.NavigateTo($"{FrontEndPages.AboutUser}?userId={User.Id}");
            else if (Community != null) Services.NavigateTo($"{FrontEndPages.AboutCommunity}?communityId={Community.Id}");
        }
    }
}
