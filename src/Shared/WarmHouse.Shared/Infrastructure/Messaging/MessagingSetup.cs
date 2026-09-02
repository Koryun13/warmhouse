using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;

namespace WarmHouse.Shared.Infrastructure.Messaging;

/// <summary>
/// Registers the message broker and the consumers of a service.
///
/// A broker is used rather than direct calls because telemetry and commands
/// are a stream: the publisher must not wait for, or even know about, its
/// subscribers. That is what allows new device types and new consumers to be
/// added without touching the services that already exist.
///
/// Publishing and consuming both go through the service's own database:
///
/// <list type="bullet">
///   <item>
///     <description>
///       <b>Outbox.</b> A publish is written to the outbox table inside the same
///       transaction as the state change that caused it, and delivered to RabbitMQ
///       afterwards by the delivery service. Without it a crash between
///       <c>SaveChanges</c> and the publish would leave a device registered that
///       no other service ever hears about.
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>Inbox.</b> Delivery is at-least-once, so a consumer can see the same
///       message twice. The inbox records the message id per consumer and skips
///       the redelivery, which makes side effects — not just database writes —
///       happen once.
///     </description>
///   </item>
/// </list>
///
/// Because the outbox holds the publish until the transaction commits, use cases
/// must publish <i>before</i> saving; a publish after <c>SaveChangesAsync</c>
/// would sit in the outbox until some later save flushed it.
/// </summary>
public static class MessagingSetup
{
    public static WebApplicationBuilder AddServiceMessaging<TDbContext>(
        this WebApplicationBuilder builder,
        string serviceName,
        Assembly? consumerAssembly = null)
        where TDbContext : DbContext
    {
        var host = builder.Configuration["RABBITMQ_HOST"] ?? "localhost";
        var user = builder.Configuration["RABBITMQ_USER"] ?? "guest";
        var password = builder.Configuration["RABBITMQ_PASSWORD"] ?? "guest";

        builder.Services.AddMassTransit(registration =>
        {
            registration.SetKebabCaseEndpointNameFormatter();

            if (consumerAssembly is not null)
            {
                registration.AddConsumers(consumerAssembly);
            }

            registration.AddEntityFrameworkOutbox<TDbContext>(outbox =>
            {
                outbox.UsePostgres();

                // Route IPublishEndpoint through the outbox table instead of
                // straight to the broker.
                outbox.UseBusOutbox();

                outbox.QueryDelay = TimeSpan.FromSeconds(1);
            });

            // Every receive endpoint gets the inbox: consumption and the writes
            // it performs commit together, and a duplicate delivery is skipped.
            registration.AddConfigureEndpointsCallback((context, _, cfg) =>
                cfg.UseEntityFrameworkOutbox<TDbContext>(context));

            registration.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host, "/", h =>
                {
                    h.Username(user);
                    h.Password(password);
                });

                // Retry a temporarily failing consumer before the message is
                // moved to its error queue.
                cfg.UseMessageRetry(retry => retry.Intervals(
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(15)));

                cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceName, false));
            });
        });

        builder.Services.AddScoped<IIntegrationEventPublisher, MassTransitEventPublisher>();

        return builder;
    }
}

/// <summary>
/// Adapter that implements the application's publishing port over MassTransit.
/// It is the only type in the service that knows the broker exists.
/// </summary>
internal sealed class MassTransitEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    public Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken)
        where TEvent : class, IIntegrationEvent
        => publishEndpoint.Publish(integrationEvent, cancellationToken);
}
