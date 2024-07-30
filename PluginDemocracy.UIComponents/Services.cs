using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Newtonsoft.Json;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static PluginDemocracy.UIComponents.Components.BottomBar;

namespace PluginDemocracy.UIComponents
{
    /// <summary>
    /// Services for API calls, navigation, and snackbar messages
    /// </summary>
    /// <param name="navigation">Navigation Manager</param>
    /// <param name="snackbar">ISnackbar</param>
    /// <param name="httpClient">HttpClient for making API calls</param>
    /// <param name="appState">BaseAppState to track changes to App state</param>
    public class Services(NavigationManager navigation, ISnackbar snackbar, HttpClient httpClient, BaseAppState appState, IHttpClientFactory httpClientFactory)
    {
        private readonly NavigationManager _navigation = navigation;
        private readonly ISnackbar _snackBar = snackbar;
        public readonly HttpClient _httpClient = httpClient;
        private readonly BaseAppState _appState = appState;
        protected readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

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
            if (!string.IsNullOrEmpty(_appState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);
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
        public async Task<T?> GetDataGenericAsync<T>(string endpoint)
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + endpoint;

            using HttpRequestMessage request = new(HttpMethod.Get, url);
            // Add the JWT as a Bearer token in the Authorization header if it's available
            if (!string.IsNullOrEmpty(_appState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);
            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    AddSnackBarMessage("error", $"HTTP Error: {response.StatusCode}");
                    return default;
                }
                string responseBody = await response.Content.ReadAsStringAsync();
                var settings = new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                T? returnObject =  JsonConvert.DeserializeObject<T>(responseBody, settings);
                return returnObject;
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.IsLoading = false;
                _appState.IsLoading = false;
                return default;
            }
            finally
            {
                _appState.IsLoading = false;
            }
        }
        public async Task<bool> PutDataAsync<T>(string endpoint, T data)
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + endpoint;

            using HttpRequestMessage request = new(HttpMethod.Put, url);
            // Add the JWT as a Bearer token in the Authorization header if it's available
            if (!string.IsNullOrEmpty(_appState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);
            // Add the data to the request
            if (data != null) request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
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
        /// This makes POST requests with a content-type header of application/json, so it only works with <paramref name="data"/> 
        /// that can be formatted as JSON.
        /// In order to include the session JWT in the headers, the request must be constructed manually using HttpRequestMessage.
        /// </summary>
        /// <typeparam name="T">Type for optional data load</typeparam>
        /// <param name="endpoint">The API endpoint to hit</param>
        /// <param name="data">Data load in post request</param>
        /// <returns>PDAPIResponse from server or if none an empty PDAPIResponse</returns>
        public async Task<PDAPIResponse> PostDataAsync<T>(string endpoint, T? data = default, bool referenceHandlerPreserve = true) 
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + endpoint;
            //Add JWT to request if available
            using HttpRequestMessage request = new(HttpMethod.Post, url);
            if (!string.IsNullOrEmpty(_appState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);
            try
            {
                //Add request content if the data is not null
                JsonSerializerOptions options = new();
                if (referenceHandlerPreserve) options.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                if (data != null)
                {
                    string jsonData = System.Text.Json.JsonSerializer.Serialize(data, options);
                    request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                }
                else request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
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
        public async Task<TResult?> PostDataGenericAsync<TInput, TResult>(string endpoint, TInput? data = null)
            where TResult : class
            where TInput : class
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + endpoint;
            //Add JWT to request if available
            using HttpRequestMessage request = new(HttpMethod.Post, url);
            if (!string.IsNullOrEmpty(_appState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);
            //Add request content if the data is not null
            try
            {
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                };
                if (data != null) request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(data, options), Encoding.UTF8, "application/json");
                else request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.IsLoading = false;
                return default;
            }
            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                _appState.IsLoading = false;
                TResult? result;
                try
                {
                    result = await response.Content.ReadFromJsonAsync<TResult>();
                    return result;
                }
                catch
                {
                    try
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var settings = new JsonSerializerSettings
                        {
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                            TypeNameHandling = TypeNameHandling.Auto,

                        };
                        TResult? returnObject = JsonConvert.DeserializeObject<TResult>(responseBody, settings);
                        return returnObject;
                    }
                    catch(Exception e)
                    {
                        AddSnackBarMessage("error", $"{e.Message}");
                        return default;
                    }
                }
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.IsLoading = false;
                return default;
            }
        }
        public async Task<PDAPIResponse> DeleteDataAsync(string endpoint)
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + endpoint;
            //Add JWT to request if available
            using HttpRequestMessage request = new(HttpMethod.Delete, url);
            if (!string.IsNullOrEmpty(_appState.SessionJWT))
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);

            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                return await CommunicationCommon(response);
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.IsLoading = false;
                return new PDAPIResponse();
            }
        }
        public async Task<PDAPIResponse> DeleteDataAsyncGeneric<TInput>(string endpoint, TInput data)
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + endpoint;
            //Add JWT to request if available
            using HttpRequestMessage request = new(HttpMethod.Delete, url);
            if (!string.IsNullOrEmpty(_appState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);
            //Add request content if the data is not null
            try
            {
                if (data != null) request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                else request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
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
                HttpRequestMessage request = new(HttpMethod.Post, url)
                {
                    Content = content
                };
                //Add SessionJWT as a Bearer token in the Authorization header if it's available
                if (!string.IsNullOrEmpty(_appState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);

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
        public async Task<PDAPIResponse> UploadMultitpleFilesAsync(string apiEndpoint, IList<IBrowserFile> files)
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + apiEndpoint;
            MultipartFormDataContent content = [];
            try
            {
                foreach (IBrowserFile file in files)
                {
                    await using Stream fileStream = file.OpenReadStream();
                    StreamContent fileContent = new(fileStream);
                    fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "file",
                        FileName = file.Name
                    };
                    content.Add(fileContent, "file");
                }
                //Create HttpRequestMessage to include JWT in Authorization header
                HttpRequestMessage request = new(HttpMethod.Post, url)
                {
                    Content = content
                };
                //Add SessionJWT as a Bearer token in the Authorization header if it's available
                if (!string.IsNullOrEmpty(_appState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);
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
        /// Method for uploading posts with images.
        /// TODO: might delete this method and change to use a generic version in the razor component to keep the code cleaner 
        /// and just have a method per http verb and perhaps another generic version as well. 
        /// </summary>
        /// <param name="apiEndpoint"></param>
        /// <param name="postBody"></param>
        /// <param name="fileDataDictionary"></param>
        /// <param name="communityId"></param>
        /// <returns></returns>
        public async Task<bool> UploadPostAsync(string apiEndpoint, string postBody, Dictionary<string, FileData> fileDataDictionary, int communityId)
        {
            _appState.IsLoading = true;
            string url = _appState.BaseUrl + apiEndpoint;
            MultipartFormDataContent content = [];

            //Add post text to the content
            content.Add(new StringContent(postBody), "Body");
            content.Add(new StringContent(communityId.ToString()), "CommunityId");

            foreach (FileData fileDataValue in fileDataDictionary.Values)
            {
                StreamContent fileContent = new(fileDataValue.Stream);
                fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "Files",
                    FileName = fileDataValue.FileName
                };
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(fileDataValue.ContentType);
                content.Add(fileContent, "Files", fileDataValue.FileName);
            }
            HttpRequestMessage request = new(HttpMethod.Post, url) { Content = content };
            if (!string.IsNullOrEmpty(_appState.SessionJWT))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);
            }

            //sending content to API
            try
            {
                HttpClient newHttpClient = CreateHttpClient();
                //on this line the task is canceled if there are images
                newHttpClient.Timeout = TimeSpan.FromMinutes(5);
                HttpResponseMessage response = await newHttpClient.SendAsync(request);
                bool isSuccessStatusCode = response.IsSuccessStatusCode;
                await CommunicationCommon(response);
                return isSuccessStatusCode;
            }
            catch (Exception ex)
            {
                #if DEBUG
                AddSnackBarMessage("error", ex.Message);
                #endif
                AddSnackBarMessage("error", $"Network or server error");
                _appState.IsLoading = false;
                return false;
            }
        }
        /// <summary>
        /// Method for deleting posts. 
        /// TODO: Might replace for generic DeleteDataAsync
        /// </summary>
        /// <param name="postId">The Id of the post to delete</param>
        /// <returns>bool indicating if the operation was successful</returns>
        public async Task<bool> DeletePostAsync(int postId)
        {
            HttpClient httpClient = CreateHttpClient();
            string url = _appState.BaseUrl + ApiEndPoints.DeletePost + $"?postId={postId}";

            HttpRequestMessage request = new(HttpMethod.Delete, url);
            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);
                bool isSuccessStatusCode = response.IsSuccessStatusCode;
                await CommunicationCommon(response);
                return _appState.PDAPIResponse.SuccessfulOperation;
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                _appState.IsLoading = false;
                return false;
            }
        }

        /// <summary>
        /// Turns of _appState.IsLoading, extracts snackbar messages from the API response, and returns the API response. 
        /// </summary>
        /// <param name="response">The response sent from the server</param>
        /// <returns>PDAPIResponse extracted from response</returns>
        public async Task<PDAPIResponse> CommunicationCommon(HttpResponseMessage response)
        {
            #if DEBUG //Only show HTTP error in debug mode. In development, only show messages the API wants to send.
            if (!response.IsSuccessStatusCode)
            {
                AddSnackBarMessage("error", $"HTTP Error: {response.StatusCode}");
            }
            #endif
            string jsonString = await response.Content.ReadAsStringAsync();
            PDAPIResponse? apiResponse = null;
            try
            {
                var settings = new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Auto,
                    MissingMemberHandling = MissingMemberHandling.Ignore // Ignores missing members in the JSON data
                };
                apiResponse = JsonConvert.DeserializeObject<PDAPIResponse>(jsonString, settings);
            }
            catch(Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                #if DEBUG
                Console.WriteLine(ex.Message);
                #endif
            }

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
            // If apiResponse.User is sent, log in user.
            if (apiResponse.User != null) _appState.LogIn(apiResponse.User);
            //if apiResponse.SessionJWT is sent, set it in AppState
            if (apiResponse.SessionJWT != null) _appState.SessionJWT = apiResponse.SessionJWT;
            //posts
            if (apiResponse.Posts.Count > 0) _appState.Posts = apiResponse.Posts;
            //if apiResponse.LogOut is sent, log out user and redirect to home
            if (apiResponse.LogOut == true)
            {
                _appState.LogOut();
                NavigateTo(FrontEndPages.Login);
            }
            //Stop loading spinner
            _appState.IsLoading = false;
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
        public static IEnumerable<string> PasswordStrength(string pw)
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
        public async Task<PDAPIResponse> SendRequestAsync(HttpRequestMessage request)
        {
            try
            {
                // Send the request
                HttpClient newHttpClient = CreateHttpClient();
                HttpResponseMessage response = await newHttpClient.SendAsync(request);
                return await CommunicationCommon(response);
            }
            catch (Exception ex)
            {
                AddSnackBarMessage("error", ex.Message);
                return new PDAPIResponse { SuccessfulOperation = false };
            }
        }
        internal HttpClient CreateHttpClient()
        {
            HttpClient client = _httpClientFactory.CreateClient("MyHttpClient");
            if (!string.IsNullOrEmpty(_appState.SessionJWT)) client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _appState.SessionJWT);
            return client;
        }
    }
}
