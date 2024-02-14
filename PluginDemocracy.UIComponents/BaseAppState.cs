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
    /// <summary>
    /// TODO: a loading method wherever I am storing data on web or maui that obtains this class. The SessionJWT is particuarly important for keeping session. 
    /// TODO: When a user opens up the app, the app needs to make sure that its token is still valid and if not, it should logout the user to ask for credentials again. Maybe the app needs to make a get request when just booting up to make sure the JWT is still valid. 
    /// </summary>
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
        public string? SessionJWT { get; set; }
        //METHODS:
        #region METHODS
        public BaseAppState(IConfiguration configuration, IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            ApiResponse = new();
        }
        #region PROTECTED METHODS
        protected void NotifyStateChanged() => OnChange?.Invoke();
        /// <summary>
        /// TODO: THIS CAN PROBABLY BE DELETED
        /// </summary>
        /// <param name="alerts"></param>
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
        #endregion
        #region PUBLIC METHODS
        public void LogIn(UserDto user)
        {
            User = user;
            SetCulture(user.Culture);
            NotifyStateChanged();
        }
        public void LogOut()
        {
            User = null;
            SessionJWT = null;
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
        /// <summary>
        /// This might need to be protected? 
        /// </summary>
        public abstract void SaveState();
        #endregion region
        #endregion
    }
}
