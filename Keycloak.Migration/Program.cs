// See https://aka.ms/new-console-template for more information
using Keycloak.Migration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");


var configurationBuilder = new ConfigurationBuilder();

configurationBuilder.AddJsonFile("appsettings.json",false);

var configuration = configurationBuilder.Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);
services.AddDbContext<CC3DbContext>(options => options.UseSqlServer(configuration.GetConnectionString("cc3")));

