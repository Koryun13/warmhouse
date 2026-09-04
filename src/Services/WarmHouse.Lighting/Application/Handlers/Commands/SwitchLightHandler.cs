using WarmHouse.Lighting.Application.Contracts.Requests;
using WarmHouse.Lighting.Application.Contracts.Responses;
using WarmHouse.Lighting.Application.Mapping;
using WarmHouse.Lighting.Domain.Errors;
using WarmHouse.Lighting.Domain.Repositories;
using WarmHouse.Shared.Application.Abstractions;
using WarmHouse.Shared.Kernel;

namespace WarmHouse.Lighting.Application.Handlers.Commands;

/// <summary>Switches a light on or off and tells the device.</summary>
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
            LightingCommandFactory.Command(fixture.DeviceId, "lighting.switch", request.On ? "on" : "off",
                [], currentUser.Id, clock.UtcNow),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return LightFixtureMapper.ToDto(fixture);
    }
}
