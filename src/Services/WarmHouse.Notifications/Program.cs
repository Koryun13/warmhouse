using System.Reflection;
using WarmHouse.Notifications.Application;
using WarmHouse.Notifications.Infrastructure;
using WarmHouse.Notifications.Infrastructure.Persistence;
using WarmHouse.Shared.Infrastructure.Hosting;
using WarmHouse.Shared.Infrastructure.Messaging;
using WarmHouse.Shared.Infrastructure.Persistence;
using WarmHouse.Shared.Infrastructure.Security;

const string ServiceTitle = "Notifications";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(ServiceTitle);
builder.AddServiceAuthentication();
builder.AddServicePostgres<NotificationsDbContext>("warmhouse_notifications");
builder.AddServiceMessaging<NotificationsDbContext>("notifications", Assembly.GetExecutingAssembly());

builder.Services.AddNotificationsApplication();
builder.Services.AddNotificationsInfrastructure();
builder.Services.AddEndpointModules(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapServiceDefaults(ServiceTitle);
app.MapEndpointModules();

app.Run();
