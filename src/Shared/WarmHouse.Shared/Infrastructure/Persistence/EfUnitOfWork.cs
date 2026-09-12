using Microsoft.EntityFrameworkCore;
using WarmHouse.Shared.Application.Abstractions;

namespace WarmHouse.Shared.Infrastructure.Persistence;

/// <summary>Commits a handler's aggregate changes through the service's DbContext.</summary>
internal sealed class EfUnitOfWork(DbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => context.SaveChangesAsync(cancellationToken);
}
