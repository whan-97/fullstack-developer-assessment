using System.ComponentModel.DataAnnotations;
using LabOpsDesk.Api.Domain;

namespace LabOpsDesk.Api.Contracts;

public sealed record AssetResponse(
    Guid Id,
    string AssetTag,
    string Name,
    string Platform,
    string Location,
    string Status,
    string? CheckedOutTo,
    DateTimeOffset? CheckedOutAt,
    string? Notes);

public sealed class CreateAssetRequest
{
    [Required, MaxLength(64)]
    public string AssetTag { get; set; } = string.Empty;

    [Required, MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(64)]
    public string Platform { get; set; } = string.Empty;

    [Required, MaxLength(128)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(512)]
    public string? Notes { get; set; }
}

public sealed class UpdateAssetRequest
{
    [Required, MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(64)]
    public string Platform { get; set; } = string.Empty;

    [Required, MaxLength(128)]
    public string Location { get; set; } = string.Empty;

    [Required]
    public AssetStatus Status { get; set; }

    [MaxLength(512)]
    public string? Notes { get; set; }
}

public sealed class CheckOutAssetRequest
{
    [Required, MaxLength(128)]
    public string Assignee { get; set; } = string.Empty;
}

public sealed record PartResponse(
    Guid Id,
    string Sku,
    string Name,
    string Category,
    string UnitOfMeasure,
    int QuantityOnHand,
    int ReorderThreshold,
    string Location,
    bool IsLowStock);

public sealed class CreatePartRequest
{
    [Required, MaxLength(64)]
    public string Sku { get; set; } = string.Empty;

    [Required, MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(64)]
    public string Category { get; set; } = string.Empty;

    [Required, MaxLength(32)]
    public string UnitOfMeasure { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int QuantityOnHand { get; set; }

    [Range(0, int.MaxValue)]
    public int ReorderThreshold { get; set; }

    [Required, MaxLength(128)]
    public string Location { get; set; } = string.Empty;
}

public sealed class UpdatePartRequest
{
    [Required, MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(64)]
    public string Category { get; set; } = string.Empty;

    [Required, MaxLength(32)]
    public string UnitOfMeasure { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int QuantityOnHand { get; set; }

    [Range(0, int.MaxValue)]
    public int ReorderThreshold { get; set; }

    [Required, MaxLength(128)]
    public string Location { get; set; } = string.Empty;
}

public sealed class AdjustPartQuantityRequest
{
    public int Delta { get; set; }
}

public sealed record ApiErrorResponse(
    string Code,
    string Message,
    IDictionary<string, string[]>? Errors = null);
