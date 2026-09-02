using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace WarmHouse.Shared.Infrastructure.Persistence;

/// <summary>
/// Adds the outbox and inbox tables to a service's own database.
///
/// They belong to the service, not to the broker: that is the whole point of
/// the pattern — the state change and the message about it are written in one
/// transaction, so neither can exist without the other.
/// </summary>
public static class OutboxModel
{
    public static ModelBuilder AddMessagingOutbox(this ModelBuilder modelBuilder)
    {
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        return modelBuilder;
    }
}
