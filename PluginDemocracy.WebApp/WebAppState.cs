using PluginDemocracy.UIComponents;
namespace PluginDemocracy.WebApp
{
    public class WebAppState : BaseAppState
    {
        private bool _hasInternet = false;
        public override bool HasInternet { get => _hasInternet; protected set => _hasInternet = value; }
    }
}
