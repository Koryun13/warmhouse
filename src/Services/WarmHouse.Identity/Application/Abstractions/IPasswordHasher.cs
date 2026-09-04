namespace WarmHouse.Identity.Application.Abstractions;

/// <summary>Password hashing, kept behind a port so the algorithm can be replaced.</summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string hash);
}
