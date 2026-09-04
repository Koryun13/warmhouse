using WarmHouse.Identity.Application.Contracts.Responses;
using WarmHouse.Identity.Domain.Entities;

namespace WarmHouse.Identity.Application.Mapping;

/// <summary>Projects the user aggregate onto its published shape.</summary>
public static class UserMapper
{
    public static UserDto ToDto(User user) => new(
        user.Id, user.Email, user.DisplayName, user.CreatedAt);
}
