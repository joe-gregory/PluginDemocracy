using System.ComponentModel;
using System.Globalization;
using PluginDemocracy.Translations;

namespace PluginDemocracy.Translations
{
    public class TranslationResourceManager : INotifyPropertyChanged
    {
        public static CultureInfo Culture { get => TranslationResources.Culture; }
        private TranslationResourceManager()
        {
            TranslationResources.Culture ??= CultureInfo.GetCultureInfo("en-US");
            TranslationResources.Culture = CultureInfo.CurrentCulture;
        }
        public static TranslationResourceManager Instance { get; } = new();
        public string this[string resourceKey] => TranslationResources.ResourceManager.GetObject(resourceKey, TranslationResources.Culture) as string ?? "No matching translation found";

        public event PropertyChangedEventHandler? PropertyChanged;
        public void SetCulture(CultureInfo culture)
        {
            TranslationResources.Culture = culture;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
