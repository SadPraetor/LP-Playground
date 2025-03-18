using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Serialization;

namespace Keycloak.Auth.Models
{
    internal record WeatherDto(IEnumerable<WeatherForecast> Forecasts);

    internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
    {
        [JsonIgnore]
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
