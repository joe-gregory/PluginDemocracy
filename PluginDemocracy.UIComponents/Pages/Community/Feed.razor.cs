using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.UIComponents.Pages.Community
{
    public partial class Feed: IDisposable
    {
        public void Dispose()
        {
            AppState.OnChange -= StateHasChanged;
            GC.SuppressFinalize(this); // Prevents finalizer from being called
        }
    }
}
