using WarmHouse.Shared.Infrastructure.Hosting;
using WarmHouse.TemperatureApi.Simulation;

const string ServiceTitle = "Temperature Sensor Simulator";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(ServiceTitle);
builder.Services.AddSingleton<TemperatureSimulator>();

var app = builder.Build();

app.MapServiceDefaults(ServiceTitle);

app.MapGet("/temperature", (
        TemperatureSimulator simulator,
        string? location,
        string? sensorId) => Results.Ok(simulator.Read(location, sensorId)))
    .WithName("GetTemperatureByLocation")
    .WithSummary("Current temperature for a room")
    .WithTags("Temperature");

app.MapGet("/temperature/{sensorId}", (
        TemperatureSimulator simulator,
        string sensorId) => Results.Ok(simulator.Read(location: null, sensorId: sensorId)))
    .WithName("GetTemperatureBySensorId")
    .WithSummary("Current temperature for a sensor id")
    .WithTags("Temperature");

app.Run();
