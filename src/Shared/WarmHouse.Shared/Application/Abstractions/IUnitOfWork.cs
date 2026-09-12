namespace WarmHouse.Shared.Application.Abstractions;

/// <summary>
/// Commits the changes made to aggregates within a single handler.
/// Implemented by the persistence layer over the service's DbContext.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
