using WarmHouse.Identity.Application.Abstractions;
using WarmHouse.Identity.Application.Contracts.Requests;
using WarmHouse.Identity.Application.Contracts.Responses;
using WarmHouse.Identity.Application.Mapping;
using WarmHouse.Identity.Domain.Entities;
using WarmHouse.Identity.Domain.Errors;
using WarmHouse.Identity.Domain.Repositories;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Identity.Application.Handlers.Commands;

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

        return TokenMapper.ToDto(token, expiresAt);
    }
}
