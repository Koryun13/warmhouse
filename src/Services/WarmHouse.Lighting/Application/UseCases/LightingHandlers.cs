using WarmHouse.Lighting.Application.Contracts;
using WarmHouse.Lighting.Domain;
using WarmHouse.Lighting.Domain.Abstractions;
using WarmHouse.Lighting.Domain.Fixtures;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Contracts.Events;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Lighting.Application.UseCases;

public sealed class ListLightFixturesHandler(ILightFixtureRepository fixtures)
{
    public async Task<Result<IReadOnlyList<LightFixtureDto>>> HandleAsync(
        Guid? houseId,
        CancellationToken cancellationToken)
    {
        var found = await fixtures.ListAsync(houseId, cancellationToken);
        return Result<IReadOnlyList<LightFixtureDto>>.Success([.. found.Select(LightingMapper.ToDto)]);
    }
}

public sealed class GetLightFixtureHandler(ILightFixtureRepository fixtures)
{
    public async Task<Result<LightFixtureDto>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var fixture = await fixtures.GetByIdAsync(id, cancellationToken);
        return fixture is null
            ? LightingErrors.FixtureNotFound
            : Result<LightFixtureDto>.Success(LightingMapper.ToDto(fixture));
    }
}

public sealed class SwitchLightHandler(
    ILightFixtureRepository fixtures,
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
        if (fixture is null)
        {
            return LightingErrors.FixtureNotFound;
        }

        fixture.Switch(request.On, clock.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publisher.PublishAsync(
            LightingMapper.Command(fixture.DeviceId, "lighting.switch", request.On ? "on" : "off",
                [], request.RequestedBy, clock.UtcNow),
            cancellationToken);

        return LightingMapper.ToDto(fixture);
    }
}

public sealed class SetBrightnessHandler(
    ILightFixtureRepository fixtures,
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
        if (fixture is null)
        {
            return LightingErrors.FixtureNotFound;
        }

        if (!fixture.IsDimmable)
        {
            return LightingErrors.NotDimmable;
        }

        fixture.SetBrightness(request.Brightness, clock.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publisher.PublishAsync(
            LightingMapper.Command(fixture.DeviceId, "lighting.brightness", "set",
                new Dictionary<string, string> { ["brightness"] = request.Brightness.ToString() },
                request.RequestedBy, clock.UtcNow),
            cancellationToken);

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
