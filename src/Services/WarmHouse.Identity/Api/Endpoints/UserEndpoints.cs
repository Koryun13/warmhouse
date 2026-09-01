using WarmHouse.Identity.Application.Contracts;
using WarmHouse.Identity.Application.UseCases;
using WarmHouse.Shared.Infrastructure.Presentation;

namespace WarmHouse.Identity.Api.Endpoints;

internal sealed class UserEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/api/v1/users").WithTags("Users");

        users.MapPost("", async (
                RegisterUserRequest request, RegisterUserHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(request, ct))
                    .Match(dto => Results.Created($"/api/v1/users/{dto.Id}", dto)))
            .WithName("RegisterUser")
            .WithSummary("Register a user");

        users.MapGet("/{id:guid}", async (Guid id, GetUserHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(id, ct)).Match(Results.Ok))
            .WithName("GetUser")
            .WithSummary("User profile");

        app.MapPost("/api/v1/auth/token", async (
                TokenRequest request, IssueTokenHandler handler, CancellationToken ct) =>
                (await handler.HandleAsync(request, ct)).Match(Results.Ok))
            .WithTags("Auth")
            .WithName("IssueToken")
            .WithSummary("Obtain an access token");
    }
}
