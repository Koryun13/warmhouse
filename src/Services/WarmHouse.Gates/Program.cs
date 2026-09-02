using System.Reflection;
using WarmHouse.Gates.Application;
using WarmHouse.Gates.Infrastructure;
using WarmHouse.Gates.Infrastructure.Persistence;
using WarmHouse.Shared.Infrastructure.Hosting;
using WarmHouse.Shared.Infrastructure.Messaging;
using WarmHouse.Shared.Infrastructure.Persistence;
using WarmHouse.Shared.Infrastructure.Security;

const string ServiceTitle = "Gate Control";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(ServiceTitle);
builder.AddServiceAuthentication();
builder.AddServicePostgres<GatesDbContext>("warmhouse_gates");
builder.AddServiceMessaging<GatesDbContext>("gates", Assembly.GetExecutingAssembly());

builder.Services.AddGatesApplication();
builder.Services.AddGatesInfrastructure();
builder.Services.AddEndpointModules(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapServiceDefaults(ServiceTitle);
app.MapEndpointModules();

app.Run();
