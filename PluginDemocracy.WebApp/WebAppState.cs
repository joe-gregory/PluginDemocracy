using PluginDemocracy.API;
using PluginDemocracy.UIComponents;

namespace PluginDemocracy.WebApp
{
    public class WebAppState : BaseAppState 
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public string BaseUrl { get; private set; }
        public WebAppState(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            BaseUrl = _configuration["ApiSettings:BaseUrl"]?? string.Empty;

        }

        public async Task<PDAPIResponse> GetDataAsync(string endpoint)
        {
            try
            {
                var apiResponse = await _httpClient.GetFromJsonAsync<PDAPIResponse>($"{BaseUrl}{endpoint}");
                NotifyStateChanged();
                return apiResponse;
            }
            catch (Exception ex)
            {
                // Handle exceptions


                NotifyStateChanged(); // Notify UI about the error
                return new PDAPIResponse
                {
                    Messages = new List<PDAPIResponse.Message>
                {
                    new PDAPIResponse.Message(PDAPIResponse.SeverityLevel.Error, $"Error: {ex.Message}")
                }
                };
            }
    }
}
