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
        public override string BaseUrl { get; protected set; }
        public WebAppState(IConfiguration configuration, IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory) : base(configuration, serviceProvider, httpClientFactory)
        {
            BaseUrl = _configuration["ApiSettings:BaseUrl"] ?? string.Empty;
        }
    }
}
