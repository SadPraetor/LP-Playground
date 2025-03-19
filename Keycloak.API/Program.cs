using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//lines below also work
//builder.Services.AddAuthentication("keycloak")    
//     .AddKeycloakWebApi(builder.Configuration, jwtBearerScheme: "keycloak");

////with different schema
//builder
//.Services
//.AddKeycloakWebApiAuthentication(builder.Configuration,jwtBearerScheme:"keycloak");

////default
//builder
//.Services
//.AddKeycloakWebApiAuthentication(builder.Configuration);

builder
.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    //options.MetadataAddress = "http://localhost:8071/realms/drivalia/.well-known/openid-configuration";
    options.Authority = "http://localhost:8071/realms/drivalia";
    options.TokenValidationParameters.ValidateAudience = true;
    options.RequireHttpsMetadata = false;
    options.Audience = "cc3_test";
})

;
builder.Services.AddAuthorization();
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("cc3_test_group", policy =>
//    {
//        policy.RequireClaim("groups_membership", "cc3_test");
//    });
//    options.DefaultPolicy = options.GetPolicy("cc3_test_group")!;
//})
//    .AddKeycloakAuthorization(builder.Configuration);




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/weatherforecast", (HttpContext context) =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return new WeatherDto(forecast);
})
.WithName("GetWeatherForecast")
.RequireAuthorization();

app.Run();

internal record WeatherDto (IEnumerable<WeatherForecast> Forecasts);

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    [JsonIgnore]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}