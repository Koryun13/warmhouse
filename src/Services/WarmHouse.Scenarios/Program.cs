using System.Reflection;
using WarmHouse.Scenarios.Application;
using WarmHouse.Scenarios.Infrastructure;
using WarmHouse.Scenarios.Infrastructure.Persistence;
using WarmHouse.Shared.Infrastructure.Hosting;
using WarmHouse.Shared.Infrastructure.Messaging;
using WarmHouse.Shared.Infrastructure.Persistence;
using WarmHouse.Shared.Infrastructure.Security;

const string ServiceTitle = "Automation Scenarios";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(ServiceTitle);
builder.AddServiceAuthentication();
builder.AddServicePostgres<ScenariosDbContext>("warmhouse_scenarios");
builder.AddServiceMessaging<ScenariosDbContext>("scenarios", Assembly.GetExecutingAssembly());

builder.Services.AddScenariosApplication();
builder.Services.AddScenariosInfrastructure();
builder.Services.AddEndpointModules(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapServiceDefaults(ServiceTitle);
app.MapEndpointModules();

app.Run();
