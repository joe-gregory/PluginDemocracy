using Microsoft.AspNetCore.Components;
using PluginDemocracy.API.UrlRegistry;

namespace PluginDemocracy.UIComponents.Pages.Community
{
    public partial class Feed: IDisposable
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //if AppState.PDAPIResponse.LastRefreshed is older than 30 seconds, refresh user data
            if (AppState.PDAPIResponse.LastRefreshed < DateTime.UtcNow.AddSeconds(-10)) await Services.GetDataAsync(ApiEndPoints.RefreshUserData);
            //If user doesn't have any communities, redirect to JoinOrRegisterCommunity page
            AppState.Posts.Clear();
            AppState.SelectedCommunityInFeed = null;
            if (AppState.User?.Citizenships.Count == 0) Services.NavigateTo(FrontEndPages.JoinOrRegisterCommunity);

            //If user has only one community, set it as selected. By setting it automatically, GetFeed should be called in the property setter
            if (AppState.User?.Citizenships.Count == 1) AppState.SelectedCommunityInFeed = AppState.User.Citizenships[0].Id;
            //If user has multiple communities, let the user select a community
            //If a community had been selected previously, default to that one
            if (AppState.SelectedCommunityInFeed != null) await OnCommunityChanged(AppState.SelectedCommunityInFeed.Value);
            AppState.OnChange += StateHasChangedSafe;
            AppState.OnPostCreatedAsync += RefreshFeed;
        }
        private async Task RefreshFeed()
        {
            if (IsCurrentPageVisible()) // Assuming you have or will implement a method to check visibility
            {
                AppState.Posts.Clear();
                // Code to refresh the feed
                await GetFeed();
            }
        }
        private bool IsCurrentPageVisible()
        {
            // Code to check if the current page is visible
            Uri uri = new(NavigationManager.Uri);
            string path = uri.AbsolutePath;

            return path.Equals(FrontEndPages.Feed, StringComparison.OrdinalIgnoreCase);
        }
        private async Task OnCommunityChanged(int? newCommunityId)
        {
            AppState.SelectedCommunityInFeed = newCommunityId;
            await RefreshFeed();
        }
        private async Task GetFeed()
        {
            string url = ApiEndPoints.GetFeed + $"?communityId={AppState.SelectedCommunityInFeed}";
            await Services.GetDataAsync(url);
            StateHasChanged();
        }
        private void StateHasChangedSafe()
        {
            InvokeAsync(StateHasChanged);
        }
        public void Dispose()
        {
            AppState.OnChange -= StateHasChangedSafe;
            AppState.OnChange -= StateHasChanged;
            AppState.OnPostCreatedAsync -= RefreshFeed;
            GC.SuppressFinalize(this); // Prevents finalizer from being called
        }
    }
}
