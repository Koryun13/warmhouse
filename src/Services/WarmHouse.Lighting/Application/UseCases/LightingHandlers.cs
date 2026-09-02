using WarmHouse.Lighting.Application.Contracts;
using WarmHouse.Lighting.Domain;
using WarmHouse.Lighting.Domain.Abstractions;
using WarmHouse.Lighting.Domain.Fixtures;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Lighting.Application.UseCases;

public sealed class ListLightFixturesHandler(ILightFixtureRepository fixtures, ICurrentUser currentUser)
{
    public async Task<Result<IReadOnlyList<LightFixtureDto>>> HandleAsync(
        Guid houseId,
        CancellationToken cancellationToken)
    {
        if (!currentUser.CanAccess(houseId))
        {
            return AccessErrors.HouseForbidden(houseId);
        }

        var found = await fixtures.ListAsync(houseId, cancellationToken);
        return Result<IReadOnlyList<LightFixtureDto>>.Success([.. found.Select(LightingMapper.ToDto)]);
    }
}

public sealed class GetLightFixtureHandler(ILightFixtureRepository fixtures, ICurrentUser currentUser)
{
    public async Task<Result<LightFixtureDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var fixture = await fixtures.GetByIdAsync(id, cancellationToken);
        return fixture is null || !currentUser.CanAccess(fixture.HouseId)
            ? LightingErrors.FixtureNotFound
            : Result<LightFixtureDto>.Success(LightingMapper.ToDto(fixture));
    }
}

public sealed class SwitchLightHandler(
    ILightFixtureRepository fixtures,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    IDateTimeProvider clock)
{
    public async Task<Result<LightFixtureDto>> HandleAsync(
        Guid id,
        SwitchLightRequest request,
        CancellationToken cancellationToken)
    {
        var fixture = await fixtures.GetByIdAsync(id, cancellationToken);
        if (fixture is null || !currentUser.CanAccess(fixture.HouseId))
        {
            return LightingErrors.FixtureNotFound;
        }

        fixture.Switch(request.On, clock.UtcNow);

        await publisher.PublishAsync(
            LightingMapper.Command(fixture.DeviceId, "lighting.switch", request.On ? "on" : "off",
                [], currentUser.Id, clock.UtcNow),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return LightingMapper.ToDto(fixture);
    }
}

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
            LightingMapper.Command(fixture.DeviceId, "lighting.brightness", "set",
                new Dictionary<string, string> { ["brightness"] = request.Brightness.ToString() },
                currentUser.Id, clock.UtcNow),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return LightingMapper.ToDto(fixture);
    }
}

internal static class LightingMapper
{
    public static LightFixtureDto ToDto(LightFixture fixture) => new(
        fixture.Id, fixture.HouseId, fixture.DeviceId, fixture.Name, fixture.Location,
        fixture.IsOn, fixture.Brightness, fixture.IsDimmable, fixture.UpdatedAt);

    public static DeviceCommandRequested Command(
        Guid deviceId,
        string capability,
        string action,
        Dictionary<string, string> payload,
        Guid requestedBy,
        DateTimeOffset now)
        => new(Guid.CreateVersion7(), now, Guid.CreateVersion7(), deviceId,
            capability, action, payload, requestedBy, Guid.CreateVersion7());
}
