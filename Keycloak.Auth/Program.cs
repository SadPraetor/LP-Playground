using Keycloak.Auth;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console(
        outputTemplate: "[{Level:u4}] | {Message:lj}{NewLine}{Exception}",
        restrictedToMinimumLevel: LogEventLevel.Information,
        formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddScoped<ApiAuthTokenManager>();
builder.Services.AddTransient<ApiAuthenticationMessageHandler>();
builder.Services.AddTransient<WeatherApiService>();

builder.Services.AddHttpContextAccessor();

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
        options.ResponseType = OpenIdConnectResponseType.Code; //required for refresh token to come with initial login
        //options.Scope.Add("offline_access");    //this will generate refresh token that does not expire

        options.Events = new OpenIdConnectEvents
        {
            OnSignedOutCallbackRedirect = context =>
            {
                context.Response.Redirect("/");
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
