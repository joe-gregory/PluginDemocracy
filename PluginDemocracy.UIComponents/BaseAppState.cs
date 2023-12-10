using Microsoft.AspNetCore.Components.Web;
using PluginDemocracy.Models;
using System.Globalization;
using PluginDemocracy.API.Models;
using Microsoft.AspNetCore.Components;

namespace PluginDemocracy.UIComponents
{
    public abstract class BaseAppState
    {
        
        //PROPERTIES:
        public event Action? OnChange;
        public bool HasInternet { get; protected set; }
        public UserDto? User { get; protected set; }
        private TranslationResourceManager TranslationResourceManager { get; } = TranslationResourceManager.Instance;
        public CultureInfo Culture { get => TranslationResourceManager.Culture; }
        //METHODS:
        protected void NotifyStateChanged() => OnChange?.Invoke();
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
        //Change to protected later on cuando este implementando como checar el internet en diferentes devices
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
