using LabOpsDesk.Api.Contracts;
using LabOpsDesk.Api.Domain;

namespace LabOpsDesk.Api.Mapping;

public static class AssetMapper
{
    public static AssetResponse ToResponse(Asset asset) =>
        new(
            asset.Id,
            asset.AssetTag,
            asset.Name,
            asset.Platform,
            asset.Location,
            asset.Status.ToString(),
            asset.CheckedOutTo,
            asset.CheckedOutAt,
            asset.Notes);
}

public static class PartMapper
{
    public static PartResponse ToResponse(Part part) =>
        new(
            part.Id,
            part.Sku,
            part.Name,
            part.Category,
            part.UnitOfMeasure,
            part.QuantityOnHand,
            part.ReorderThreshold,
            part.Location,
            part.IsLowStock);
}
