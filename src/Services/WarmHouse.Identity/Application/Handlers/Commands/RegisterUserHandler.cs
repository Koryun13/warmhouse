using WarmHouse.Identity.Application.Abstractions;
using WarmHouse.Identity.Application.Contracts.Requests;
using WarmHouse.Identity.Application.Contracts.Responses;
using WarmHouse.Identity.Application.Mapping;
using WarmHouse.Identity.Domain.Entities;
using WarmHouse.Identity.Domain.Errors;
using WarmHouse.Identity.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Identity.Application.Handlers.Commands;

/// <summary>Creates an account. The password is hashed behind a port, never stored.</summary>
public sealed class RegisterUserHandler(
    IUserRepository users,
    IPasswordHasher hasher,
    IUnitOfWork unitOfWork,
    IDateTimeProvider clock)
{
    public async Task<Result<UserDto>> HandleAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email)
            || (request.Password?.Length ?? 0) < User.MinPasswordLength)
        {
            return IdentityErrors.InvalidRegistration;
        }

        var email = User.NormalizeEmail(request.Email);
        if (await users.ExistsAsync(email, cancellationToken))
        {
            return IdentityErrors.EmailTaken;
        }

        var user = User.Register(email, request.DisplayName, hasher.Hash(request.Password!), clock.UtcNow);

        users.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return UserMapper.ToDto(user);
    }
}
