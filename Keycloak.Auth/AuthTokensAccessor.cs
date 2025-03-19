using Microsoft.AspNetCore.Authentication;
using System.Runtime.CompilerServices;


namespace Keycloak.Auth
{
    public static class AuthTokensAccessDependencyInjection
    {
        public static IApplicationBuilder UseAuthTokensAccessor(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<AuthTokensAccessor>();
            return builder;
        }
    }

    public class AuthTokensAccessor
    {
        private readonly RequestDelegate _next;
        

        public AuthTokensAccessor(RequestDelegate next)
        {
            _next = next;           
        }

        public async Task InvokeAsync(HttpContext context, ApiAuthTokenManager tokenHandler)
        {           
            if(context.User?.Identity?.IsAuthenticated ?? false)
            {
                await tokenHandler.SetValuesAsync(context);
            }
            await _next(context);           
        }
    }
}
