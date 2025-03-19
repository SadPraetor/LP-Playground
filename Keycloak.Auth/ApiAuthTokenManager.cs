using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;

namespace Keycloak.Auth
{
    public class ApiAuthTokenManager
    {      
        private DateTime _expiration;
        

        private JwtSecurityTokenHandler _jwtHandler = new JwtSecurityTokenHandler();
        private string? _accessToken;
        private string? _refreshToken;
        private readonly TokenApiClient _tokenApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<ApiAuthTokenManager> _logger;

        public string? AccessToken { get => _accessToken; }

        public string? RefreshToken { get => _refreshToken; }

        private bool _accessTokenExpired => DateTime.UtcNow > _expiration;


        public ApiAuthTokenManager(
            TokenApiClient tokenApiClient, 
            IHttpContextAccessor contextAccessor,
            ILogger<ApiAuthTokenManager> logger)
        {
            _tokenApiClient = tokenApiClient;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public async Task SetValuesAsync (HttpContext currentContext)
        {
            var accessToken = await currentContext.GetTokenAsync("access_token");
            var jwtToken = _jwtHandler.ReadJwtToken(accessToken);
            _expiration = jwtToken.ValidTo.ToUniversalTime();
            _accessToken = accessToken;

            _refreshToken = await currentContext.GetTokenAsync("refresh_token");
            jwtToken = _jwtHandler.ReadJwtToken(_refreshToken);
            _logger.LogInformation("Refresh token expiration {ValidTo}", jwtToken.ValidTo.ToLocalTime());
        }

        public async Task<string> GetTokenAsync()
        {            
            _ = AccessToken ?? throw new InvalidOperationException("Access token is null");
            
            if (!_accessTokenExpired)
            {
                return AccessToken;
            }

            _ = _refreshToken ?? throw new InvalidOperationException("Refresh token is null");

            var data = await _tokenApiClient.RefreshTokenAsync(_refreshToken);

            _accessToken = data.AccessToken;
            _refreshToken = data.RefreshToken;
            _expiration = _jwtHandler.ReadJwtToken(_accessToken).ValidTo;

            var refreshTokenExpiration = _jwtHandler.ReadJwtToken(_refreshToken).ValidTo.ToLocalTime();
            
            _logger.LogInformation("New Refresh token expiration {ValidTo}", refreshTokenExpiration);

            await RefreshCookieAsync();

            return _accessToken;
        }

        private async Task RefreshCookieAsync()
        {
            var context = _contextAccessor.HttpContext;
            var authResult = await context.AuthenticateAsync("Cookies");
            if (authResult?.Principal != null)
            {
                var authProperties = authResult.Properties;

                // Get the existing tokens, update them with the new values
                var tokens = authProperties.GetTokens().ToList();

                // Replace or add the access token
                tokens.RemoveAll(t => t.Name == "access_token");
                tokens.Add(new AuthenticationToken { Name = "access_token", Value = _accessToken });

                // Replace or add the refresh token
                tokens.RemoveAll(t => t.Name == "refresh_token");
                tokens.Add(new AuthenticationToken { Name = "refresh_token", Value = _refreshToken });

              
                authProperties.StoreTokens(tokens);

                // Re-issue the cookie with updated tokens
                await context.SignInAsync("Cookies", authResult.Principal, authProperties);
            }
        }


        
    }
}
