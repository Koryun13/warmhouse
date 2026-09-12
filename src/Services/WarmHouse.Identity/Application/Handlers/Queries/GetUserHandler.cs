using WarmHouse.Identity.Application.Contracts.Responses;
using WarmHouse.Identity.Application.Mapping;
using WarmHouse.Identity.Domain.Errors;
using WarmHouse.Identity.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Application.Errors;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Identity.Application.Handlers.Queries;

/// <summary>Returns a profile. A user may read only their own.</summary>
public sealed class GetUserHandler(IUserRepository users, ICurrentUser currentUser)
{
    public async Task<Result<UserDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id != currentUser.Id)
        {
            return AccessErrors.Forbidden;
        }

        var user = await users.GetByIdAsync(id, cancellationToken);
        return user is null
            ? IdentityErrors.UserNotFound
            : Result<UserDto>.Success(UserMapper.ToDto(user));
    }
}
