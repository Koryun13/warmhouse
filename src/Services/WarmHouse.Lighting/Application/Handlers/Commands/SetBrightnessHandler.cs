using WarmHouse.Lighting.Application.Contracts.Requests;
using WarmHouse.Lighting.Application.Contracts.Responses;
using WarmHouse.Lighting.Application.Mapping;
using WarmHouse.Lighting.Domain.Entities;
using WarmHouse.Lighting.Domain.Errors;
using WarmHouse.Lighting.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Lighting.Application.Handlers.Commands;

/// <summary>Dims a light, provided the fixture declared that it can dim.</summary>
public sealed class SetBrightnessHandler(
    ILightFixtureRepository fixtures,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task<Result<LightFixtureDto>> HandleAsync(
        Guid id,
        SetBrightnessRequest request,
        CancellationToken cancellationToken)
    {
        if (!LightFixture.IsBrightnessAllowed(request.Brightness))
        {
            return LightingErrors.BrightnessOutOfRange;
        }

        var fixture = await fixtures.GetByIdAsync(id, cancellationToken);
        if (fixture is null || !currentUser.CanAccess(fixture.HouseId))
        {
            return LightingErrors.FixtureNotFound;
        }

        if (!fixture.IsDimmable)
        {
            return LightingErrors.NotDimmable;
        }

        fixture.SetBrightness(request.Brightness, clock.UtcNow);

        await publisher.PublishAsync(
            LightingCommandFactory.Command(fixture.DeviceId, "lighting.brightness", "set",
                new Dictionary<string, string> { ["brightness"] = request.Brightness.ToString() },
                currentUser.Id, clock.UtcNow),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return LightFixtureMapper.ToDto(fixture);
    }
}
