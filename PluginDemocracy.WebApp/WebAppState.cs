using Microsoft.AspNetCore.Components;
using MudBlazor;
using PluginDemocracy.API;
using PluginDemocracy.UIComponents;
using System;
using System.Net.Http;

namespace PluginDemocracy.WebApp
{
    public class WebAppState : BaseAppState
    {
        public override string ApiBaseUrl { get; protected set; }
        public override bool HasInternet { get; protected set; } = true;
        public WebAppState(IConfiguration configuration, IServiceProvider serviceProvider) : base(configuration, serviceProvider)
        {
#if DEBUG
            // Use the local development URL when in Debug mode
            ApiBaseUrl = "https://localhost:7042"; // Replace with your actual local API URL
#else
        // Use the production URL from appsettings.json in Release mode
        ApiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? string.Empty;
#endif
        }
        /// <summary>
        /// TODO: Save the state of the app to local storage in browser
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void SaveState()
        {
        }
    }
}
