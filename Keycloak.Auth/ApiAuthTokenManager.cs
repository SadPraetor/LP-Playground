using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Keycloak.Auth
{
    public class ApiAuthTokenManager
    {      
        private string? _accessToken;
        private string? _refreshToken;
        private DateTime _accessTokenValidTo;
        private bool AccessTokenExpired => DateTime.UtcNow > _accessTokenValidTo;
        private DateTime _refreshTokenValidTo;
        private bool RefreshTokenExpired => DateTime.UtcNow > _refreshTokenValidTo;

        private JwtSecurityTokenHandler _jwtHandler = new JwtSecurityTokenHandler();

        private readonly TokenApiClient _tokenApiClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<ApiAuthTokenManager> _logger;


        public ApiAuthTokenManager(
            TokenApiClient tokenApiClient, 
            IHttpContextAccessor contextAccessor,
            ILogger<ApiAuthTokenManager> logger)
        {
            _tokenApiClient = tokenApiClient;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public async Task SetValuesAsync(HttpContext currentContext)
        {
            var accessToken = await currentContext.GetTokenAsync("access_token");
            var refreshToken = await currentContext.GetTokenAsync("refresh_token");
            FillTokens(accessToken, refreshToken);
        }

        private void FillTokens (string? accessToken, string? refreshToken)
        {
            if(accessToken is null || refreshToken is null)
            {
                _logger.LogWarning("Unable to fill token information, one of the tokens is null");
                return;
            }

            _accessToken = accessToken;
            _refreshToken = refreshToken;
            _accessTokenValidTo = _jwtHandler.ReadJwtToken(accessToken).ValidTo;
            _refreshTokenValidTo = _jwtHandler.ReadJwtToken(refreshToken).ValidTo;
        }


        public async Task<string> GetTokenAsync()
        {
            _ = _accessToken ?? throw new InvalidOperationException("Access token is null");

            if (!AccessTokenExpired)
            {
                return _accessToken;
            }

            _ = _refreshToken ?? throw new InvalidOperationException("Refresh token is null");
            var refreshDto = await _tokenApiClient.RefreshTokenAsync(_refreshToken!);

            if(refreshDto is null)
            {
                throw new InvalidOperationException("Failed to refresh token");
            }

            FillTokens(refreshDto.AccessToken, refreshDto.RefreshToken);
           
            await RefreshCookieAsync();

            return _accessToken.ToString();
        }


        private async Task RefreshCookieAsync()
        {
            var context = _contextAccessor.HttpContext;
            if(context is null)
            {
                throw new InvalidOperationException("Context is null");
            }

            var authResult = await context.AuthenticateAsync("Cookies");

            if (authResult?.Principal is not null && authResult.Properties is not null)
            {
                var authProperties = authResult.Properties;

                // Get the existing tokens, update them with the new values
                var tokens = authProperties.GetTokens().ToList();

                // Replace or add the access token
                tokens.RemoveAll(t => t.Name == "access_token");
                tokens.Add(new AuthenticationToken { Name = "access_token", Value = _accessToken! });

                // Replace or add the refresh token
                tokens.RemoveAll(t => t.Name == "refresh_token");
                tokens.Add(new AuthenticationToken { Name = "refresh_token", Value = _refreshToken! });

              
                authProperties.StoreTokens(tokens);

                // Re-issue the cookie with updated tokens
                await context.SignInAsync("Cookies", authResult.Principal, authProperties);

                _logger.LogInformation("Cookie refreshed");
            }
        }
    }
}
