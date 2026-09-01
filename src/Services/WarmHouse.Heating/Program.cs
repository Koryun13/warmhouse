using System.Reflection;
using WarmHouse.Heating.Application;
using WarmHouse.Heating.Infrastructure;
using WarmHouse.Heating.Infrastructure.Persistence;
using WarmHouse.Shared.Infrastructure.Hosting;
using WarmHouse.Shared.Infrastructure.Messaging;
using WarmHouse.Shared.Infrastructure.Persistence;

const string ServiceTitle = "Heating Control";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(ServiceTitle);
builder.AddServicePostgres<HeatingDbContext>("warmhouse_heating");
builder.AddServiceMessaging("heating", Assembly.GetExecutingAssembly());

builder.Services.AddHeatingApplication();
builder.Services.AddHeatingInfrastructure();
builder.Services.AddEndpointModules(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapServiceDefaults(ServiceTitle);
app.MapEndpointModules();

app.Run();
