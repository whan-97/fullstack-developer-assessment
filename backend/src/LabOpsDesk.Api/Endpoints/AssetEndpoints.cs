using LabOpsDesk.Api.Abstractions;
using LabOpsDesk.Api.Contracts;

namespace LabOpsDesk.Api.Endpoints;

public static class AssetEndpoints
{
    public static RouteGroupBuilder MapAssetEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/assets").WithTags("Assets");

        group.MapGet("/", async (IAssetService service, CancellationToken cancellationToken) =>
        {
            var items = await service.ListAsync(cancellationToken);
            return Results.Ok(items);
        });

        group.MapGet("/{id:guid}", async (Guid id, IAssetService service, CancellationToken cancellationToken) =>
        {
            var item = await service.GetAsync(id, cancellationToken);
            return Results.Ok(item);
        });

        group.MapPost("/", async (CreateAssetRequest request, IAssetService service, CancellationToken cancellationToken) =>
        {
            var created = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/assets/{created.Id}", created);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateAssetRequest request, IAssetService service, CancellationToken cancellationToken) =>
        {
            var updated = await service.UpdateAsync(id, request, cancellationToken);
            return Results.Ok(updated);
        });

        group.MapDelete("/{id:guid}", async (Guid id, IAssetService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        });

        group.MapPost("/{id:guid}/checkout", async (Guid id, CheckOutAssetRequest request, IAssetService service, CancellationToken cancellationToken) =>
        {
            var updated = await service.CheckOutAsync(id, request, cancellationToken);
            return Results.Ok(updated);
        });

        group.MapPost("/{id:guid}/checkin", async (Guid id, IAssetService service, CancellationToken cancellationToken) =>
        {
            var updated = await service.CheckInAsync(id, cancellationToken);
            return Results.Ok(updated);
        });

        return group;
    }
}
