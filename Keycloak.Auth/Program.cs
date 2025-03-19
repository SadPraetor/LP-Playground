using Keycloak.Auth;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddScoped<ApiAuthTokenManager>();
builder.Services.AddTransient<ApiAuthenticationMessageHandler>();
builder.Services.AddTransient<WeatherApiService>();

builder.Services.AddHttpClient<TokenApiClient>(nameof(TokenApiClient),client =>
{
    client.BaseAddress = new Uri("http://localhost:8071/realms/drivalia/protocol/openid-connect/token");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient("weather", client =>
{
    client.BaseAddress = new Uri("https://localhost:7236");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});



builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddKeycloakWebApp(builder.Configuration.GetSection("Keycloak"),
   

    configureOpenIdConnectOptions: options =>
    {
        options.SaveTokens = true;
        options.ResponseType = OpenIdConnectResponseType.Code;
        //options.Scope.Add("offline_access");    //this will generate refresh token that does not expire

        options.Events = new OpenIdConnectEvents
        {
            OnSignedOutCallbackRedirect = context =>
            {
                context.Response.Redirect("/Home/Logout");
                context.HandleResponse();

                return Task.CompletedTask;
            }
        };
    });



var test = builder.Services.Where(x => x.ServiceType == typeof(ApiAuthTokenManager))
    .ToList();
Debug.Assert(test.Count == 1);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAuthTokensAccessor();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .RequireAuthorization();

app.Run();
