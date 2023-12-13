using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using PluginDemocracy.API.Models;
using System.Net.Http.Json;

namespace PluginDemocracy.UIComponents
{
    public class Services
    {
        private NavigationManager _navigation;
        private ISnackbar _snackBar;
        private HttpClient _httpClient;
        private BaseAppState _appState;
        public Services(NavigationManager navigation, ISnackbar snackbar, HttpClient httpClient, BaseAppState appState)
        {
            _navigation = navigation;
            _snackBar = snackbar;
            _httpClient = httpClient;
            _appState = appState;
        }
        public async Task<PDAPIResponse> GetDataAsync(string endpoint)
        {
            try
            {
                PDAPIResponse? apiResponse = await _httpClient.GetFromJsonAsync<PDAPIResponse>($"{_appState.BaseUrl}{endpoint}");
                if (apiResponse == null)
                {
                    AddSnackBarMessage("error", "PDAPIResponse null");
                    apiResponse = new();
                }
                else
                {
                    AddSnackBarMessages(apiResponse.Alerts);
                }
                _appState.ApiResponse = apiResponse;
                if (!string.IsNullOrEmpty(apiResponse.RedirectTo)) NavigateTo(apiResponse.RedirectTo);
                return _appState.ApiResponse;
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.ApiResponse = new();
                return _appState.ApiResponse;
            }
        }
        //public async Task<PDAPIResponse> GetDataAsync(string endpoint)
        //{
        //    try
        //    {
        //        //In ASP.NET Core and Blazor applications, HttpClient should typically be provided through the built-in IHttpClientFactory rather than injected directly. This factory helps manage the lifetimes of HttpClient instances and avoid common pitfalls like socket exhaustion.
        //        var httpClient = _httpClientFactory.CreateClient();
        //        var apiResponse = await httpClient.GetFromJsonAsync<PDAPIResponse>($"{BaseUrl}{endpoint}");
        //        if (apiResponse != null) AddSnackBarMessages(apiResponse.Alerts);
        //        else apiResponse = new();
        //        NotifyStateChanged();
        //        ApiResponse = apiResponse;
        //        if (!string.IsNullOrEmpty(ApiResponse.RedirectTo))
        //        {
        //            using (var scope = _serviceProvider.CreateScope())
        //            {
        //                var navigationManager = scope.ServiceProvider.GetRequiredService<NavigationManager>();
        //                navigationManager.NavigateTo(ApiResponse.RedirectTo);
        //            }
        //        }
        //        return apiResponse;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions


        //        PDAPIResponse apiResponse = new();
        //        apiResponse.AddAlert("error", $"Error: {ex.Message}");
        //        AddSnackBarMessages(apiResponse.Alerts);
        //        NotifyStateChanged(); // Notify UI about the error
        //        return apiResponse;
        //    }
        //}
        public void NavigateTo(string page)
        {
            _navigation.NavigateTo(page);
        }
        public void AddSnackBarMessages(List<PDAPIResponse.Alert> alerts)
        {
            foreach (PDAPIResponse.Alert alert in alerts)
            {
                if (Enum.TryParse(alert.Severity.ToString(), true, out MudBlazor.Severity mudBlazorSeverity)) _snackBar.Add(alert.Message, mudBlazorSeverity);
            }
        }
        public void AddSnackBarMessage(string severity, string message)
        {
            if (Enum.TryParse(severity, true, out MudBlazor.Severity mudBlazorSeverity)) _snackBar.Add(message, mudBlazorSeverity);
        }

    }
}
