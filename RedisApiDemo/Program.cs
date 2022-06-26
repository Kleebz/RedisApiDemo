using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using RedisApiDemo.Api.Endpoints;
using RedisApiDemo.Domain.Models;
using RedisApiDemo.Domain.Repository;
using RedisApiDemo.Domain.Repository.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<RedisDemoDbContext>();
builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    //options.Configuration = builder.Configuration.GetConnectionString("CacheConnection");
    options.Configuration = builder.Configuration["Redis:CacheConnection"];
    options.InstanceName = "RedisApiDemo";
});
builder.Services.AddSingleton<IDistributedCache, RedisCache>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Global options for Distributed cache
//app.Lifetime.ApplicationStarted.Register(() =>
//{
//    var currentTimeUTC = DateTime.UtcNow.ToString();
//    byte[] encodedCurrentTimeUTC = System.Text.Encoding.UTF8.GetBytes(currentTimeUTC);
//    var options = new DistributedCacheEntryOptions()
//        .SetSlidingExpiration(TimeSpan.FromSeconds(20));
//    app.Services.GetService<IDistributedCache>()
//    .Set("cachedTimeUTC", encodedCurrentTimeUTC, options);
//});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

CustomerEndpoints.MapCustomerEndpoints(app);

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}