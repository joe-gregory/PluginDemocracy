using Microsoft.AspNetCore.Components.Web;
using PluginDemocracy.Models;
using System.Globalization;
using PluginDemocracy.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using System.Net.Http.Json;

namespace PluginDemocracy.UIComponents
{
    public abstract class BaseAppState
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IConfiguration _configuration;
        protected readonly IHttpClientFactory _httpClientFactory;
        private bool _isLoading = false;
        public bool IsLoading 
        { 
            get => _isLoading;
            set
            {
                _isLoading = value;
                NotifyStateChanged();
            } 
        } 
        public PDAPIResponse ApiResponse { get; set; }
        public abstract string BaseUrl { get; protected set; }
        //PROPERTIES:
        public event Action? OnChange;
        public bool HasInternet { get; protected set; }
        public UserDto? User { get; protected set; }
        protected TranslationResourceManager TranslationResourceManager { get; } = TranslationResourceManager.Instance;
        public CultureInfo Culture { get => TranslationResourceManager.Culture; }
        //METHODS:
        public BaseAppState(IConfiguration configuration, IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            
            ApiResponse = new();
        }
        protected void NotifyStateChanged() => OnChange?.Invoke();
        public async Task<PDAPIResponse> GetDataAsync(string endpoint)
        {
            try
            {
                //In ASP.NET Core and Blazor applications, HttpClient should typically be provided through the built-in IHttpClientFactory rather than injected directly. This factory helps manage the lifetimes of HttpClient instances and avoid common pitfalls like socket exhaustion.
                var httpClient = _httpClientFactory.CreateClient();
                var apiResponse = await httpClient.GetFromJsonAsync<PDAPIResponse>($"{BaseUrl}{endpoint}");
                if (apiResponse != null) AddSnackBarMessages(apiResponse.Alerts);
                else apiResponse = new();
                NotifyStateChanged();
                ApiResponse = apiResponse;
                if (!string.IsNullOrEmpty(ApiResponse.RedirectTo))
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var navigationManager = scope.ServiceProvider.GetRequiredService<NavigationManager>();
                        navigationManager.NavigateTo(ApiResponse.RedirectTo);
                    }
                }
                return apiResponse;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                
                
                PDAPIResponse apiResponse = new();
                apiResponse.AddAlert("error", $"Error: {ex.Message}");
                AddSnackBarMessages(apiResponse.Alerts);
                NotifyStateChanged(); // Notify UI about the error
                return apiResponse;
            }
        }
        public void AddSnackBarMessages(List<PDAPIResponse.Alert> alerts)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var snackbar = scope.ServiceProvider.GetRequiredService<ISnackbar>();
                foreach (PDAPIResponse.Alert alert in alerts)
                {
                    if (Enum.TryParse(alert.Severity.ToString(), true, out MudBlazor.Severity mudBlazorSeverity)) snackbar.Add(alert.Message, mudBlazorSeverity);
                }
            }

        }
        public void LogIn(UserDto user)
        {
            User = user;
            SetCulture(user.Culture);
            NotifyStateChanged();
        }
        public void LogOut()
        {
            User = null;
            NotifyStateChanged();
        }
        //TODO: Change to protected later on cuando este implementando como checar el internet en diferentes devices
        public void SetInternetState(bool internetState)
        {
            HasInternet = internetState;
            NotifyStateChanged();
        }
        public void SetCulture(CultureInfo cultureInfo)
        {
            cultureInfo ??= new CultureInfo("en-US");
            TranslationResourceManager.SetCulture(cultureInfo);
            NotifyStateChanged();
        }
        public string Translate(string key)
        {
            return TranslationResourceManager[key];
        }

    }
}
