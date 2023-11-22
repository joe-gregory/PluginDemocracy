using PluginDemocracy.Models;
using System.Globalization;

namespace PluginDemocracy.UIComponents
{
    public abstract class BaseAppState
    {
        //PROPERTIES:
        public event Action? OnChange;
        abstract public bool HasInternet { get; protected set; }
        public void SetInternetState(bool hasInternet)
        {
            if (HasInternet != hasInternet)
            {
                HasInternet = hasInternet;
                NotifyStateChanged();
            }
        }
        public User? User { get; protected set; }
        private TranslationResourceManager TranslationResourceManager { get; } = TranslationResourceManager.Instance;
        public CultureInfo Culture { get => TranslationResourceManager.Culture; }
        protected void NotifyStateChanged() => OnChange?.Invoke();
        //METHODS:
        public void LogIn(User user)
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
