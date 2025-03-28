// See https://aka.ms/new-console-template for more information
using Keycloak.Migration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sharprompt;
using System.ComponentModel.DataAnnotations;

Console.WriteLine("Hello, World!");


var configurationBuilder = new ConfigurationBuilder();

configurationBuilder.AddJsonFile("appsettings.json",false);

var configuration = configurationBuilder.Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);
services.AddDbContext<CC3DbContext>(options => options.UseSqlServer(configuration.GetConnectionString("cc3")),optionsLifetime: ServiceLifetime.Singleton);
services.AddSingleton<AccountDownloader>();

var serviceProvider = services.BuildServiceProvider();


while (true)
{
    var action = Prompt.Select<ActionEnum>("What you wanna do?", [ActionEnum.ExportAccounts, ActionEnum.SetupKeycloakGroup, ActionEnum.MigrateUsers, ActionEnum.Exit]);

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
            break;
        case ActionEnum.MigrateUsers:
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
    MigrateUsers
}