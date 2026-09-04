using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Identity.Application.Abstractions;
using WarmHouse.Identity.Domain.Repositories;
using WarmHouse.Identity.Infrastructure.Persistence.Repositories;
using WarmHouse.Identity.Infrastructure.Security;

namespace WarmHouse.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IHouseRepository, HouseRepository>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<ITokenIssuer, JwtTokenIssuer>();

        return services;
    }
}
