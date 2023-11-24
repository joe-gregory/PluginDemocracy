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
        public static string PrimaryColor { get => "008644"; }
        public static MudTheme Theme => new MudTheme()
        {
            Palette = new PaletteLight()
            {
                Primary = PrimaryColor,
                AppbarBackground = PrimaryColor
            }, 
            PaletteDark = new PaletteDark()
            {
                Primary = PrimaryColor,
                AppbarBackground = PrimaryColor
            }
        };
    }
}
