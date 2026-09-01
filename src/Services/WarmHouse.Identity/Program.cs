using System.Reflection;
using WarmHouse.Identity.Application;
using WarmHouse.Identity.Infrastructure;
using WarmHouse.Identity.Infrastructure.Persistence;
using WarmHouse.Shared.Infrastructure.Hosting;
using WarmHouse.Shared.Infrastructure.Persistence;

const string ServiceTitle = "Users and Houses";

var builder = WebApplication.CreateBuilder(args);

// This service publishes no integration events, so it needs no broker.
builder.AddServiceDefaults(ServiceTitle);
builder.AddServicePostgres<IdentityDbContext>("warmhouse_identity");

builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure();
builder.Services.AddEndpointModules(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapServiceDefaults(ServiceTitle);
app.MapEndpointModules();

app.Run();
