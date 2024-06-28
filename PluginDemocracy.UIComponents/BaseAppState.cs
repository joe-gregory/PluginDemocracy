using System.Globalization;
using PluginDemocracy.DTOs;
using Microsoft.Extensions.Configuration;
using PluginDemocracy.Translations;

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
        public PDAPIResponse PDAPIResponse { get; set; }
        public abstract string BaseUrl { get; protected set; }
        //PROPERTIES:
        /// <summary>
        /// Subscribe to this event whenever you want to know when the state of AppState has changed. This event is internally
        /// triggered by AppState with the <see cref="NotifyStateChanged"/> method.
        /// </summary>
        public event Action? OnChange;
        public event Func<Task>? OnPostCreatedAsync;
        public abstract bool HasInternet { get; protected set; }
        public UserDTO? User { get; protected set; }
        protected int? _selectedCommunityInFeed;
        public int? SelectedCommunityInFeed {
            get
            {
                return _selectedCommunityInFeed;
            }
            set 
            {
                if (_selectedCommunityInFeed != value) // Only notify if the value actually changes
                {
                    _selectedCommunityInFeed = value;
                    NotifyStateChanged(); // This will trigger any subscribed components to update
                }
            } 
        }
        public bool IsLoggedIn { get => User != null; }
        protected TranslationResourceManager TranslationResourceManager { get; } = TranslationResourceManager.Instance;
        public CultureInfo Culture { get => TranslationResourceManager.Culture; }
        public string? SessionJWT { get; set; }
        public List<PostDTO> Posts { get; set; } = [];
        //METHODS:
        #region METHODS
        public BaseAppState(IConfiguration configuration, IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            PDAPIResponse = new();
        }
        #region PROTECTED METHODS
        /// <summary>
        /// Notify that changes have been made to the App State
        /// </summary>
        protected void NotifyStateChanged() => OnChange?.Invoke();
        public void NotifyPostCreation() => OnPostCreatedAsync?.Invoke();
        #endregion
        #region PUBLIC METHODS
        public void LogIn(UserDTO user)
        {
            User = user;
            SetCulture(user.Culture);
            NotifyStateChanged();
            Posts.Clear();
        }
        public void LogOut()
        {
            User = null;
            SessionJWT = null;
            NotifyStateChanged();
            Posts.Clear();
        }
        public void DeletePost(PostDTO post)
        {
            if (Posts.Contains(post)) Posts.Remove(post);
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
