using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;  
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            string url = _appState.BaseUrl + endpoint;
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                return await CommunicationCommon(response);
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.ApiResponse = new();
                _appState.IsLoading = false;
                return _appState.ApiResponse;
            }
        }
        public async Task<PDAPIResponse> PostDataAsync<T>(string endpoint, T data) where T : class 
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + endpoint;
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync<T>(url, data);
                return await CommunicationCommon(response);
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.ApiResponse = new();
                _appState.IsLoading = false;
                return _appState.ApiResponse;
            }
        }
        public async Task<PDAPIResponse> UploadFileAsync(string apiEndpoint, IBrowserFile browserFile)
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + apiEndpoint;

            // Use MultipartFormDataContent to send files
            using var content = new MultipartFormDataContent();
            await using var fileStream = browserFile.OpenReadStream();

            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
            {
                Name = "file",
                FileName = browserFile.Name
            };

            content.Add(fileContent, "file");

            string userIdValue = _appState.User?.Id?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(userIdValue)) content.Add(new StringContent(userIdValue), "userId");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                return await CommunicationCommon(response);
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.ApiResponse = new();
                _appState.IsLoading = false;
                return _appState.ApiResponse;
            }
        }
        private async Task<PDAPIResponse> CommunicationCommon(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                AddSnackBarMessage("error", $"HTTP Error: {response.StatusCode}");
                _appState.IsLoading = false;
            }

            PDAPIResponse? apiResponse = await response.Content.ReadFromJsonAsync<PDAPIResponse>();

            if (apiResponse == null)
            {
                AddSnackBarMessage("warning", "PDAPIResponse null");
                _appState.IsLoading = false;
                return new PDAPIResponse();
            }


            //Add snackbar messages sent from API
            AddSnackBarMessages(apiResponse.Alerts);
            //Add latest API response to AppState
            _appState.ApiResponse = apiResponse;
            //If API response includes a RedirectTo page, navigate to it
            if (!string.IsNullOrEmpty(apiResponse.RedirectTo)) NavigateTo(apiResponse.RedirectTo);
            //Stop loading spinner
            _appState.IsLoading = false;
            // If apiResponse.User is sent, log in user.
            if (apiResponse.User != null) _appState.LogIn(apiResponse.User);

            return apiResponse;
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
        public void LogOutAndRedirectHome()
        {
            _appState.LogOut();
            NavigateTo(FrontEndPages.Home);
        }
       public IEnumerable<string> PasswordStrength(string pw)
        {
            if (string.IsNullOrWhiteSpace(pw))
            {
                yield return "Password is required!";
                yield break;
            }
            if (pw.Length < 7)
                yield return "Password must be at least of length 7";
            // if (!Regex.IsMatch(pw, @"[A-Z]"))
            //     yield return "Password must contain at least one capital letter";
            // if (!Regex.IsMatch(pw, @"[a-z]"))
            //     yield return "Password must contain at least one lowercase letter";
            // if (!Regex.IsMatch(pw, @"[0-9]"))
            //     yield return "Password must contain at least one digit";
        }
    }
}
