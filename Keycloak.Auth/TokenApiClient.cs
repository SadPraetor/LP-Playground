using System.Text.Json.Serialization;

namespace Keycloak.Auth
{
    public class TokenApiClient
    {
        
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenApiClient> _logger;

        public TokenApiClient(HttpClient client, 
            IConfiguration configuration,
            ILogger<TokenApiClient> logger)
        {
            
            _client = client;
            _configuration = configuration;
            _logger = logger;
        }

        internal async Task<RefreshTokenDto?> RefreshTokenAsync(string refreshToken)
        {
            _logger.LogInformation("Token refresh required");
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

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Token refresh API called with success");
                var data = await response.Content.ReadFromJsonAsync<RefreshTokenDto>();
                return data;
            }

            _logger.LogError("Refresh request returned status {HttpStatus}", response.StatusCode);
            var errorDto = await response.Content.ReadFromJsonAsync<ErrorDto>();
            _logger.LogInformation("Error message: {message}", errorDto);

            return null;
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

    internal record ErrorDto
    {
        public string Error { get; init; } = default!;
        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; init; } = default!;
    }
}
