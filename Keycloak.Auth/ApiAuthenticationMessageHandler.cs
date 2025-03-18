namespace Keycloak.Auth
{
    public class ApiAuthenticationMessageHandler : DelegatingHandler
    {
        private readonly ApiAuthTokenManager _tokenHandler;

        public ApiAuthenticationMessageHandler(ApiAuthTokenManager tokenManager)
        {
            _tokenHandler = tokenManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
        {
            var token = await _tokenHandler.GetTokenAsync();
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);            
        }
    }
}
