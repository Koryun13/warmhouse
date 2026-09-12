using WarmHouse.Shared.Infrastructure.Hosting;

const string ServiceTitle = "API Gateway";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(ServiceTitle);

// Single entry point for web and mobile clients. Routing lives in
// configuration so a service can be moved or scaled without a code change.
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapServiceDefaults(ServiceTitle);
app.MapReverseProxy();

app.Run();
