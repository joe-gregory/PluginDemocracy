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
        public abstract bool HasInternet { get; protected set; }
        public UserDto? User { get; protected set; }
        public bool IsLoggedIn { get => User != null; }
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
        protected void AddSnackBarMessages(List<PDAPIResponse.Alert> alerts)
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
