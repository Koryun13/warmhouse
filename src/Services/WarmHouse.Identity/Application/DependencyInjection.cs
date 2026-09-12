using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Identity.Application.Handlers.Commands;
using WarmHouse.Identity.Application.Handlers.Queries;

namespace WarmHouse.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<GetUserHandler>();
        services.AddScoped<IssueTokenHandler>();

        services.AddScoped<ListHousesHandler>();
        services.AddScoped<GetHouseHandler>();
        services.AddScoped<CreateHouseHandler>();
        services.AddScoped<GrantHouseAccessHandler>();

        return services;
    }
}
