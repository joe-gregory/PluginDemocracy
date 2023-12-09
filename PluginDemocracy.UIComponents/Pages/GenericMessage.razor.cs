using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.UIComponents.Pages
{
    public partial class GenericMessage
    {
        [Parameter]
        public Dictionary<string, string>? Parameters { get; set; }

        public string? Title { get; set; }
        public string? Body { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if(Parameters != null)
            {
                Parameters.TryGetValue("Title", out string? title);
                Parameters.TryGetValue("Body", out string? body);

                Title = title;
                Body = body;
            }
        }
    }
}
