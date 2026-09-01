using System.Reflection;
using WarmHouse.Shared.Infrastructure.Hosting;
using WarmHouse.Shared.Infrastructure.Messaging;
using WarmHouse.Shared.Infrastructure.Persistence;
using WarmHouse.Telemetry.Application;
using WarmHouse.Telemetry.Infrastructure;
using WarmHouse.Telemetry.Infrastructure.Persistence;

const string ServiceTitle = "Telemetry";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(ServiceTitle);
builder.AddServicePostgres<TelemetryDbContext>("warmhouse_telemetry");
builder.AddServiceMessaging("telemetry", Assembly.GetExecutingAssembly());

builder.Services.AddTelemetryApplication();
builder.Services.AddTelemetryInfrastructure();
builder.Services.AddEndpointModules(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapServiceDefaults(ServiceTitle);
app.MapEndpointModules();

app.Run();
