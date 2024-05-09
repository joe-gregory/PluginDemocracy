using PluginDemocracy.API.UrlRegistry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.UIComponents.Pages.Community
{
    public partial class Feed: IDisposable
    {
        public event Action? OnCommunitySelectChange;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //if AppState.PDAPIResponse.LastRefreshed is older than 30 seconds, refresh user data
            if (AppState.PDAPIResponse.LastRefreshed < DateTime.UtcNow.AddSeconds(-30)) await Services.GetDataAsync(ApiEndPoints.RefreshUserData);
            //If user doesn't have any communities, redirect to JoinOrRegisterCommunity page
            if (AppState.User?.Citizenships.Count == 0) Services.NavigateTo(FrontEndPages.JoinOrRegisterCommunity);

            //If user has only one community, set it as selected. By setting it automatically, GetFeed should be called in the property setter
            if (AppState.User?.Citizenships.Count == 1) AppState.SelectedCommunityInFeed = AppState.User.Citizenships[0].Id;
            //If user has multiple communities, let the user select a community
            //If a community had been selected previously, default to that one
            if (AppState.SelectedCommunityInFeed != null) await OnCommunityChanged(AppState.SelectedCommunityInFeed);
            AppState.OnChange += StateHasChangedSafe;
        }

        private async Task OnCommunityChanged(int? newCommunityId)
        {
            if (newCommunityId != null) AppState.SelectedCommunityInFeed = newCommunityId;
            await GetFeed();
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
            AppState.OnChange -= StateHasChanged;
            GC.SuppressFinalize(this); // Prevents finalizer from being called
        }
    }
}
