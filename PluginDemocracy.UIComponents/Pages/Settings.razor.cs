using System.Drawing;

namespace PluginDemocracy.UIComponents.Pages
{
    public partial class Settings
    {
        private bool _checked;
        private MudBlazor.Color thumbIconColor;
        private bool disabled = false;
        private string selectedFlag;
        private string usaFlag = "https://upload.wikimedia.org/wikipedia/en/thumb/a/a4/Flag_of_the_United_States.svg/2880px-Flag_of_the_United_States.svg.png";
        private string mxnFlag = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Flag_of_Mexico.svg/2880px-Flag_of_Mexico.svg.png";
        /// <summary>
        /// Checked true equals es-MX, not checked false = USA
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (AppState.Culture.Name == "es-MX") _checked = true;
            else if (AppState.Culture.Name == "en-US") _checked = false;
            else disabled = true;
            SetLook();
        }
        private void SetLook()
        {
            //true == Mexican
            if (_checked)
            {
                thumbIconColor = MudBlazor.Color.Success;
                selectedFlag = mxnFlag;
            }
            else 
            {
                thumbIconColor = MudBlazor.Color.Info;
                selectedFlag = usaFlag;
            } 
        }
        private void OnSwitchToggled()
        {
            _checked = !_checked;
            if (_checked) AppState.SetCulture(new System.Globalization.CultureInfo("es-MX"));
            else AppState.SetCulture(new System.Globalization.CultureInfo("en-US"));
            SetLook();
        }
    }
}
