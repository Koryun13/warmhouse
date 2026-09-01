using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Builder;
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
/// </summary>
public static class MessagingSetup
{
    public static WebApplicationBuilder AddServiceMessaging(
        this WebApplicationBuilder builder,
        string serviceName,
        Assembly? consumerAssembly = null)
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
