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
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public PDAPIResponse ApiResponse { get; private set; }
        public string BaseUrl { get; private set; }
        public WebAppState(IConfiguration configuration, IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            BaseUrl = _configuration["ApiSettings:BaseUrl"] ?? string.Empty;
            ApiResponse = new();
        }

        public async Task<PDAPIResponse> GetDataAsync(string endpoint)
        {
            try
            {
                //In ASP.NET Core and Blazor applications, HttpClient should typically be provided through the built-in IHttpClientFactory rather than injected directly. This factory helps manage the lifetimes of HttpClient instances and avoid common pitfalls like socket exhaustion.
                var httpClient = _httpClientFactory.CreateClient();
                var apiResponse = await httpClient.GetFromJsonAsync<PDAPIResponse>($"{BaseUrl}{endpoint}");
                if (apiResponse != null) AddSnackBarMessages(apiResponse.Alerts);
                else apiResponse = new();
                NotifyStateChanged();
                ApiResponse = apiResponse;
                if (!string.IsNullOrEmpty(ApiResponse.RedirectTo))
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var navigationManager = scope.ServiceProvider.GetRequiredService<NavigationManager>();
                        navigationManager.NavigateTo(ApiResponse.RedirectTo);
                    }
                }
                return apiResponse;
            }
            catch (Exception ex)
            {
                // Handle exceptions


                NotifyStateChanged(); // Notify UI about the error
                return new PDAPIResponse
                {
                    Alerts = new List<PDAPIResponse.Alert>
                {
                    new PDAPIResponse.Alert(PDAPIResponse.Severity.Error, $"Error: {ex.Message}")
                }
                };
            }
        }
        public void AddSnackBarMessages(List<PDAPIResponse.Alert> alerts)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var snackbar = scope.ServiceProvider.GetRequiredService<ISnackbar>();
                foreach (PDAPIResponse.Alert alert in alerts)
                {
                    if (Enum.TryParse(alert.Severity.ToString(), true, out MudBlazor.Severity mudBlazorSeverity)) snackbar.Add(alert.Message, mudBlazorSeverity);
                }
            }
                
        }
    }
}
