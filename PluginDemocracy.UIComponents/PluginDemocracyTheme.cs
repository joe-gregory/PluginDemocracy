using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.UIComponents
{
    public static class PluginDemocracyTheme
    {
        public const string PrimaryColor = "008644";
        public static MudTheme Theme => new()
        {
            Palette = new PaletteLight()
            {
                Primary = PrimaryColor,
                AppbarBackground = PrimaryColor, 
                Background = "#ffffff", //white background
            }, 
            PaletteDark = new PaletteDark()
            {
                Primary = PrimaryColor,
                AppbarBackground = PrimaryColor,
                Background = "#ffffff", //white background currently for dark mode as well since the rest of the colors are not implemented for dark yet.
            }
        };
    }
}
