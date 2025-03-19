using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;

namespace Keycloak.Auth
{
    public class ApiAuthTokenManager
    {      
        private DateTime _expiration;
        

        private JwtSecurityTokenHandler _jwtHandler = new JwtSecurityTokenHandler();
        private string? _accessToken;
        private string? _refreshToken;
        private readonly TokenApiClient _tokenApiClient;

        public string? AccessToken { get => _accessToken; }

        public string? RefreshToken { get => _refreshToken; }

        private bool _accessTokenExpired => DateTime.UtcNow > _expiration;


        public ApiAuthTokenManager(TokenApiClient tokenApiClient)
        {
            _tokenApiClient = tokenApiClient;
        }

        public async Task SetValuesAsync (HttpContext currentContext)
        {
            var accessToken = await currentContext.GetTokenAsync("access_token");
            var jwtToken = _jwtHandler.ReadJwtToken(accessToken);

            _accessToken = accessToken;

            _refreshToken = await currentContext.GetTokenAsync("refresh_token");


            _expiration = jwtToken.ValidTo.ToUniversalTime();

            jwtToken = _jwtHandler.ReadJwtToken(_refreshToken);


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

            return _accessToken;
        }


        
    }
}
