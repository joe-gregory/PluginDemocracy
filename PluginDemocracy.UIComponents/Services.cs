using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using PluginDemocracy.DTOs;  
using System.Net.Http.Json;

namespace PluginDemocracy.UIComponents
{
    /// <summary>
    /// Services for API calls, navigation, and snackbar messages
    /// </summary>
    /// <param name="navigation">Navigation Manager</param>
    /// <param name="snackbar">ISnackbar</param>
    /// <param name="httpClient">HttpClient for making API calls</param>
    /// <param name="appState">BaseAppState to track changes to App state</param>
    public class Services(NavigationManager navigation, ISnackbar snackbar, HttpClient httpClient, BaseAppState appState)
    {
        private readonly NavigationManager _navigation = navigation;
        private readonly ISnackbar _snackBar = snackbar;
        private readonly HttpClient _httpClient = httpClient;
        private readonly BaseAppState _appState = appState;

        public async Task<PDAPIResponse> GetDataAsync(string endpoint)
        {
            _appState.IsLoading = true;
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
                    if (apiResponse.User != null) _appState.LogIn(apiResponse.User);

                }
                _appState.ApiResponse = apiResponse;
                if (!string.IsNullOrEmpty(apiResponse.RedirectTo)) NavigateTo(apiResponse.RedirectTo);
                _appState.IsLoading = false;
                return _appState.ApiResponse;
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.ApiResponse = new();
                _appState.IsLoading = false;
                return _appState.ApiResponse;
            }
        }
        public async Task<PDAPIResponse> PostDataAsync<T>(string endpoint, T data)
        {
            _appState.IsLoading = true;
            try
            {
                string url = _appState.BaseUrl + endpoint;
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync<T>(url, data);
                if (!response.IsSuccessStatusCode)
                {
                    AddSnackBarMessage("error", $"HTTP Error: {response.StatusCode}");
                    _appState.IsLoading = false;
                    return new PDAPIResponse();
                }
                PDAPIResponse? apiResponse = await response.Content.ReadFromJsonAsync<PDAPIResponse>();
                if (apiResponse == null)
                {
                    AddSnackBarMessage("warning", "PDAPIResponse null");
                    _appState.IsLoading = false;
                    return new PDAPIResponse();
                }
                if (apiResponse.User != null) _appState.LogIn(apiResponse.User);
                AddSnackBarMessages(apiResponse.Alerts);
                _appState.ApiResponse = apiResponse;
                if (!string.IsNullOrEmpty(apiResponse.RedirectTo)) NavigateTo(apiResponse.RedirectTo);
                _appState.IsLoading = false;
                return apiResponse;
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.IsLoading = false;
                return new PDAPIResponse();
            }
        }
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
