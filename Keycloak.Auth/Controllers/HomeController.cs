using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Keycloak.Auth.Models;
using Microsoft.AspNetCore.Authorization;

namespace Keycloak.Auth.Controllers;

public class HomeController : Controller
{
    private readonly WeatherApiService _weatherClient;    
    private readonly ILogger<HomeController> _logger;

    public HomeController(WeatherApiService weatherClient,       
        ILogger<HomeController> logger)
    {
        _weatherClient = weatherClient;       
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Logout()
    {
        return View();
    }
    
    public async Task<IActionResult> CallApi()
    {
        var context = this.HttpContext;        
        var data = await _weatherClient.GetLatestForecastAsync();


        return View(data);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
