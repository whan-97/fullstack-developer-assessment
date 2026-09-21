namespace LabOpsDesk.Api.Domain;

public sealed class Part
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public int QuantityOnHand { get; set; }
    public int ReorderThreshold { get; set; }
    public string Location { get; set; } = string.Empty;

    public bool IsLowStock => QuantityOnHand <= ReorderThreshold;

    public void AdjustQuantity(int delta)
    {
        throw new NotImplementedException();
    }
}
