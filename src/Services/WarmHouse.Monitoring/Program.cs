using System.Reflection;
using WarmHouse.Monitoring.Application;
using WarmHouse.Monitoring.Infrastructure;
using WarmHouse.Monitoring.Infrastructure.Persistence;
using WarmHouse.Shared.Infrastructure.Hosting;
using WarmHouse.Shared.Infrastructure.Messaging;
using WarmHouse.Shared.Infrastructure.Persistence;
using WarmHouse.Shared.Infrastructure.Security;

const string ServiceTitle = "Video Monitoring";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(ServiceTitle);
builder.AddServiceAuthentication();
builder.AddServicePostgres<MonitoringDbContext>("warmhouse_monitoring");
builder.AddServiceMessaging<MonitoringDbContext>("monitoring", Assembly.GetExecutingAssembly());

builder.Services.AddMonitoringApplication();
builder.Services.AddMonitoringInfrastructure();
builder.Services.AddEndpointModules(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapServiceDefaults(ServiceTitle);
app.MapEndpointModules();

app.Run();
