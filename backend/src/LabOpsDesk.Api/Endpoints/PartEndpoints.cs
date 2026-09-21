using LabOpsDesk.Api.Abstractions;
using LabOpsDesk.Api.Contracts;

namespace LabOpsDesk.Api.Endpoints;

public static class PartEndpoints
{
    public static RouteGroupBuilder MapPartEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/parts").WithTags("Parts");

        group.MapGet("/", async (IPartService service, CancellationToken cancellationToken) =>
        {
            var items = await service.ListAsync(cancellationToken);
            return Results.Ok(items);
        });

        group.MapGet("/{id:guid}", async (Guid id, IPartService service, CancellationToken cancellationToken) =>
        {
            var item = await service.GetAsync(id, cancellationToken);
            return Results.Ok(item);
        });

        group.MapPost("/", async (CreatePartRequest request, IPartService service, CancellationToken cancellationToken) =>
        {
            var created = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/parts/{created.Id}", created);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdatePartRequest request, IPartService service, CancellationToken cancellationToken) =>
        {
            var updated = await service.UpdateAsync(id, request, cancellationToken);
            return Results.Ok(updated);
        });

        group.MapDelete("/{id:guid}", async (Guid id, IPartService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        });

        group.MapPost("/{id:guid}/adjust", async (Guid id, AdjustPartQuantityRequest request, IPartService service, CancellationToken cancellationToken) =>
        {
            var updated = await service.AdjustQuantityAsync(id, request, cancellationToken);
            return Results.Ok(updated);
        });

        return group;
    }
}
