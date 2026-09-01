using System.Reflection;
using WarmHouse.Devices.Application;
using WarmHouse.Devices.Infrastructure;
using WarmHouse.Devices.Infrastructure.Persistence;
using WarmHouse.Shared.Infrastructure.Hosting;
using WarmHouse.Shared.Infrastructure.Messaging;
using WarmHouse.Shared.Infrastructure.Persistence;

const string ServiceTitle = "Device Management";

var builder = WebApplication.CreateBuilder(args);

// Composition root: the only place that knows every layer.
builder.AddServiceDefaults(ServiceTitle);
builder.AddServicePostgres<DevicesDbContext>("warmhouse_devices");
builder.AddServiceMessaging("devices", Assembly.GetExecutingAssembly());

builder.Services.AddDevicesApplication();
builder.Services.AddDevicesInfrastructure();
builder.Services.AddEndpointModules(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapServiceDefaults(ServiceTitle);
app.MapEndpointModules();

app.Run();
