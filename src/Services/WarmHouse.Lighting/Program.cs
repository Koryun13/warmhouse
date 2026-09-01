using System.Reflection;
using WarmHouse.Lighting.Application;
using WarmHouse.Lighting.Infrastructure;
using WarmHouse.Lighting.Infrastructure.Persistence;
using WarmHouse.Shared.Infrastructure.Hosting;
using WarmHouse.Shared.Infrastructure.Messaging;
using WarmHouse.Shared.Infrastructure.Persistence;

const string ServiceTitle = "Lighting Control";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(ServiceTitle);
builder.AddServicePostgres<LightingDbContext>("warmhouse_lighting");
builder.AddServiceMessaging("lighting", Assembly.GetExecutingAssembly());

builder.Services.AddLightingApplication();
builder.Services.AddLightingInfrastructure();
builder.Services.AddEndpointModules(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapServiceDefaults(ServiceTitle);
app.MapEndpointModules();

app.Run();
