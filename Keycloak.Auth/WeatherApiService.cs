using Keycloak.Auth.Models;

namespace Keycloak.Auth
{
    public class WeatherApiService
    {
        
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiAuthTokenManager _tokenManager;

        public WeatherApiService(IHttpClientFactory httpClientFactory, ApiAuthTokenManager tokenManager)
        {
            
            _httpClientFactory = httpClientFactory;
            _tokenManager = tokenManager;
        }


        internal async Task<WeatherDto?> GetLatestForecastAsync()
        {
            using var client = _httpClientFactory.CreateClient("weather");
            var token = await _tokenManager.GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var data =  await client.GetFromJsonAsync<WeatherDto>("/weatherforecast");

            return data;
        }
    }
}
