// See https://aka.ms/new-console-template for more information
using Keycloak.Migration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sharprompt;
using System.ComponentModel.DataAnnotations;
using Keycloak.AuthServices.Sdk;
using Keycloak.AuthServices.Common;
using Duende.AccessTokenManagement;
using Serilog.Events;
using Serilog;
using System.Globalization;

Console.WriteLine("Hello, World!");

//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Verbose()
//    .WriteTo.Console(
//        outputTemplate: "[{Level:u4}] | {Message:lj}{NewLine}{Exception}",
//        restrictedToMinimumLevel: LogEventLevel.Information,
//        formatProvider: CultureInfo.InvariantCulture)
//    .CreateBootstrapLogger();



var configurationBuilder = new ConfigurationBuilder();


configurationBuilder.AddJsonFile("appsettings.json",false);


var configuration = configurationBuilder.Build();

var services = new ServiceCollection();

services.AddSerilog(options =>
{
    options
        .MinimumLevel.Verbose()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{Exception}",
            restrictedToMinimumLevel: LogEventLevel.Information,
            formatProvider: CultureInfo.InvariantCulture)
        .WriteTo.File(
            "logs/log-.log",
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{Exception}",
            rollingInterval: RollingInterval.Day,
            restrictedToMinimumLevel: LogEventLevel.Information);
});

services.AddSingleton<IConfiguration>(configuration);
services.AddDbContext<CC3DbContext>(options => options.UseSqlServer(configuration.GetConnectionString("cc3")),optionsLifetime: ServiceLifetime.Singleton);
services.AddSingleton<AccountDownloader>();
services.AddSingleton<SetPasswordHandler>();

var options = configuration.GetKeycloakOptions<KeycloakAdminClientOptions>();

services.AddDistributedMemoryCache();
services.AddClientCredentialsTokenManagement()
    .AddClient(
        "keycloak",
        client =>
        {
            client.ClientId = options.Resource;
            client.ClientSecret = options.Credentials.Secret;
            client.TokenEndpoint = options.KeycloakTokenEndpoint;
        });

//services.AddClientCredentialsHttpClient("cc3_migration", "keycloak", client =>
//{
//    client.BaseAddress = new Uri(options.AuthServerUrl);
//});


//services.AddKeycloakAdminHttpClient(configuration)
//    .AddClientCredentialsTokenHandler("keycloak");


services.AddTransient<Keycloak.Net.KeycloakClient>(sp=>
{
    var handler = sp.GetRequiredService<IClientCredentialsTokenManagementService>();

    return new Keycloak.Net.KeycloakClient(options.AuthServerUrl, ()=> handler.GetAccessTokenAsync("keycloak").GetAwaiter().GetResult().AccessToken);
});

services.AddTransient<KeycloakSetup>();
services.AddTransient<UserMigrationService>();
services.AddSingleton<TemporaryPasswordGenerator>();
services.AddSingleton<UpdatePasswordActionHandler>();
services.AddSingleton<FindUsers>();


var serviceProvider = services.BuildServiceProvider();

//var findUsersService = serviceProvider.GetRequiredService<FindUsers>();

//findUsersService.FilterNewProdUsers();

//return;

while (true)
{
    var action = Prompt.Select<ActionEnum>("What you wanna do?", [ActionEnum.ExportAccounts, ActionEnum.SetupKeycloakGroup, ActionEnum.MigrateUsers, ActionEnum.UpdatePassword,ActionEnum.SetPassword, ActionEnum.Exit]);

    if(action is ActionEnum.Exit)
    {
        break;
    }

    switch (action)
    {        
        case ActionEnum.ExportAccounts:
            var accountDownloader = serviceProvider.GetRequiredService<AccountDownloader>();
            await accountDownloader.ExportAccounts("accounts");

            break;
        case ActionEnum.SetupKeycloakGroup:
            var client = serviceProvider.GetRequiredService<KeycloakSetup>();
            await client.SetupKeycloakAsync("prod");
            break;
        case ActionEnum.MigrateUsers:
            var migrationService = serviceProvider.GetRequiredService<UserMigrationService>();
            await migrationService.MigrateUsersFromFile("test");
            break;
        case ActionEnum.UpdatePassword:
            var handler = serviceProvider.GetRequiredService<UpdatePasswordActionHandler>();
            await handler.SetActionUpdatePasswordAsync();
            break;
        case ActionEnum.SetPassword:
            var passwordHandler = serviceProvider.GetRequiredService<SetPasswordHandler>();
            await passwordHandler.SetNewPasswordAsync("Auticko2024@@");
            break;
        default:
            break;
    }
    Console.WriteLine();
}



public enum ActionEnum
{
    [Display(Name = "Exit application")]
    Exit,
    [Display(Name = "Export PortalUserNames from CC3 database into text file for review")]
    ExportAccounts,
    [Display(Name ="Setup in Keycloak group, role, attribute mapping")]
    SetupKeycloakGroup,
    [Display(Name ="Migrate users")]
    MigrateUsers,
    [Display(Name ="Set Update_Password")]
    UpdatePassword,
    [Display(Name = "Set new user password")]
    SetPassword
}