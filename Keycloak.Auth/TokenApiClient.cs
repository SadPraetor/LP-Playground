using System.Text.Json.Serialization;

namespace Keycloak.Auth
{
    public class TokenApiClient
    {
        
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public TokenApiClient(HttpClient client, 
            IConfiguration configuration)
        {
            
            _client = client;
            _configuration = configuration;
        }

        internal async Task<RefreshTokenDto> RefreshTokenAsync(string refreshToken)
        {
            var clientId= _configuration.GetValue<string>("Keycloak:resource");
            var secret = _configuration.GetValue<string>("Keycloak:credentials:secret");

            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
            { "client_id", clientId },
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken },
            { "client_secret", secret }
            });

            
            var response = await _client.PostAsync("token", formContent);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<RefreshTokenDto>();

            return data;
        }
    }

    internal record RefreshTokenDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = default!;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; init; } = default!;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }

        [JsonPropertyName("refresh_expires_in")]
        public int RefreshExpiresIn { get; init; }
    }
}
