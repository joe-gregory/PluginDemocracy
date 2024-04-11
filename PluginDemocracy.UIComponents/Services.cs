using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        /// <summary>
        /// In order to include the session JWT in the headers, the request must be constructed manually using HttpRequestMessage.
        /// </summary>
        /// <param name="endpoint">The API endpoint to hit</param>
        /// <returns>PDAPIResponse from server or if none an empty PDAPIResponse</returns>
        public async Task<PDAPIResponse> GetDataAsync(string endpoint)
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + endpoint;

            using HttpRequestMessage request = new(HttpMethod.Get, url);
            // Add the JWT as a Bearer token in the Authorization header if it's available
            if(!string.IsNullOrEmpty(_appState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);
            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                return await CommunicationCommon(response);
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.IsLoading = false;
                return new();
            }
                    }
        /// <summary>
        /// This is a generic version of GetDataAsync. It is used to get data from the server and deserialize it into a generic type.
        /// This allows to obtain different types of data from the server than just PDAPIResponse.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public async Task<T?> GetDataAsyncGeneric<T>(string endpoint)
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + endpoint;

            using HttpRequestMessage request = new(HttpMethod.Get, url);
            // Add the JWT as a Bearer token in the Authorization header if it's available
            if(!string.IsNullOrEmpty(_appState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);
            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                _appState.IsLoading = false;
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.IsLoading = false;
                _appState.IsLoading = false;
                return default;
            }
        }
        public async Task<bool> PutDataAsync<T>(string endpoint, T data)
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + endpoint;

            using HttpRequestMessage request = new(HttpMethod.Put, url);
            // Add the JWT as a Bearer token in the Authorization header if it's available
            if(!string.IsNullOrEmpty(_appState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);
            // Add the data to the request
            if (data != null) request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            else request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                bool success = response.IsSuccessStatusCode;
                _appState.IsLoading = false;
                return success;
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.IsLoading = false;
                return false;
            }
        }
        /// <summary>
        /// In order to include the session JWT in the headers, the request must be constructed manually using HttpRequestMessage.
        /// </summary>
        /// <typeparam name="T">Type for optional data load</typeparam>
        /// <param name="endpoint">The API endpoint to hit</param>
        /// <param name="data">Data load in post request</param>
        /// <returns>PDAPIResponse from server or if none an empty PDAPIResponse</returns>
        public async Task<PDAPIResponse> PostDataAsync<T>(string endpoint, T? data = null) where T : class
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + endpoint;
            //Add JWT to request if available
            using HttpRequestMessage request = new(HttpMethod.Post, url);
            if(!string.IsNullOrEmpty(_appState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);
            //Add request content if the data is not null
            if (data != null) request.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            else request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                return await CommunicationCommon(response);
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.IsLoading = false;
                return new();
            }
        }
        public async Task<PDAPIResponse> UploadFileAsync(string apiEndpoint, IBrowserFile browserFile)
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + apiEndpoint;

            // Use MultipartFormDataContent to send files
            MultipartFormDataContent content = [];

            int maxMegaBytes = 10;
            long maxFileSize = maxMegaBytes * 1024 * 1024; //10MB
            if (browserFile.Size > maxFileSize)
            {
                AddSnackBarMessage("error", $"File size exceeds {maxMegaBytes}MB");
                _appState.IsLoading = false;
                return new PDAPIResponse(); // Ensure you exit the method if the file is too large
            }
            try
            {
                //Adding file to content
                await using Stream fileStream = browserFile.OpenReadStream(maxFileSize);
                StreamContent fileContent = new(fileStream);
                fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = browserFile.Name
                };
                content.Add(fileContent, "file");

                //Create HttpRequestMessage to include JWT in Authorization header
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };
                //Add SessionJWT as a Bearer token in the Authorization header if it's available
                if(!string.IsNullOrEmpty(_appState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);

                //Sending content to API
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                return await CommunicationCommon(response);
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.IsLoading = false;
                return new();
            }
        }
        /// <summary>
        /// Turns of _appState.IsLoading, extracts snackbar messages from the API response, and returns the API response. 
        /// </summary>
        /// <param name="response">The response sent from the server</param>
        /// <returns>PDAPIResponse extracted from response</returns>
        private async Task<PDAPIResponse> CommunicationCommon(HttpResponseMessage response)
        {
            #if DEBUG //Only show HTTP error in debug mode. In development, only show messages the API wants to send.
            if (!response.IsSuccessStatusCode) AddSnackBarMessage("error", $"HTTP Error: {response.StatusCode}");
            #endif
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
            _appState.PDAPIResponse = apiResponse;
            //If API response includes a RedirectTo page, navigate to it
            if (!string.IsNullOrEmpty(apiResponse.RedirectTo)) NavigateTo(apiResponse.RedirectTo);
            //Stop loading spinner
            _appState.IsLoading = false;
            // If apiResponse.User is sent, log in user.
            if (apiResponse.User != null) _appState.LogIn(apiResponse.User);
            //if apiResponse.SessionJWT is sent, set it in AppState
            if(apiResponse.SessionJWT != null) _appState.SessionJWT = apiResponse.SessionJWT;
            //if apiResponse.LogOut is sent, log out user and redirect to home
            if (apiResponse.LogOut == true)
            {
                _appState.LogOut();
                NavigateTo(FrontEndPages.Login);
            }
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
