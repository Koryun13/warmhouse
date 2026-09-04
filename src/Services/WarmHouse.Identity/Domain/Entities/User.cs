using WarmHouse.Shared.Kernel;

namespace WarmHouse.Identity.Domain.Entities;

/// <summary>
/// An account holder.
///
/// The entity stores only the password hash and never the password itself;
/// hashing is an infrastructure concern behind a port, so the algorithm can be
/// changed without touching the domain.
/// </summary>
public sealed class User : AggregateRoot
{
    private User()
    {
        // Required by EF Core.
    }

    private User(Guid id, string email, string displayName, string passwordHash, DateTimeOffset now)
        : base(id)
    {
        Email = email;
        DisplayName = displayName;
        PasswordHash = passwordHash;
        CreatedAt = now;
    }

    public string Email { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public static User Register(string email, string displayName, string passwordHash, DateTimeOffset now)
        => new(Guid.CreateVersion7(), NormalizeEmail(email), displayName.Trim(), passwordHash, now);

    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    public const int MinPasswordLength = 8;
}
