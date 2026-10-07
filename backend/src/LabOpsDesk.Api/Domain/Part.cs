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

    public void AdjustQuantity(int quantity)
    {
        if (QuantityOnHand + quantity < 0)
            throw new InvalidOperationException($"Adjusting quantity by {quantity} would result in a stock deficit of ({QuantityOnHand + quantity}).");

        QuantityOnHand += quantity;
    }
}
