using Microsoft.AspNetCore.Routing;

namespace WarmHouse.Shared.Presentation;

/// <summary>
/// A slice of the presentation layer. Each module maps the endpoints of one
/// resource group, so routing never collects in a single large file.
/// </summary>
public interface IEndpointModule
{
    void MapEndpoints(IEndpointRouteBuilder app);
}
