using WarmHouse.Identity.Application.Abstractions;
using WarmHouse.Identity.Application.Contracts;
using WarmHouse.Identity.Domain;
using WarmHouse.Identity.Domain.Abstractions;
using WarmHouse.Identity.Domain.Users;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Identity.Application.UseCases;

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

        return new UserDto(user.Id, user.Email, user.DisplayName, user.CreatedAt);
    }
}

public sealed class GetUserHandler(IUserRepository users)
{
    public async Task<Result<UserDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(id, cancellationToken);
        return user is null
            ? IdentityErrors.UserNotFound
            : Result<UserDto>.Success(new UserDto(user.Id, user.Email, user.DisplayName, user.CreatedAt));
    }
}

/// <summary>
/// Authenticates a user and issues a token.
///
/// The token carries the houses the user may reach, so the domain services can
/// authorise a request without calling back into this service on every hop.
/// </summary>
public sealed class IssueTokenHandler(
    IUserRepository users,
    IHouseRepository houses,
    IPasswordHasher hasher,
    ITokenIssuer issuer)
{
    public async Task<Result<TokenDto>> HandleAsync(TokenRequest request, CancellationToken cancellationToken)
    {
        var user = await users.GetByEmailAsync(User.NormalizeEmail(request.Email), cancellationToken);

        if (user is null || !hasher.Verify(request.Password, user.PasswordHash))
        {
            return IdentityErrors.InvalidCredentials;
        }

        var houseIds = await houses.ListAccessibleHouseIdsAsync(user.Id, cancellationToken);
        var (token, expiresAt) = issuer.Issue(user, houseIds);

        return new TokenDto(token, "Bearer", expiresAt);
    }
}
