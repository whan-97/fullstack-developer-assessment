using LabOpsDesk.Api.Domain;

namespace LabOpsDesk.Api.Tests;

public class PartQuantityTests
{
    [Fact]
    public void AdjustQuantity_increments()
    {
        var part = new Part { QuantityOnHand = 5, ReorderThreshold = 3 };
        part.AdjustQuantity(2);
        Assert.Equal(7, part.QuantityOnHand);
        Assert.False(part.IsLowStock);
    }

    [Fact]
    public void AdjustQuantity_rejects_negative_balance()
    {
        var part = new Part { QuantityOnHand = 1 };
        Assert.Throws<InvalidOperationException>(() => part.AdjustQuantity(-2));
    }

    [Fact]
    public void IsLowStock_when_at_or_below_threshold()
    {
        var part = new Part { QuantityOnHand = 3, ReorderThreshold = 3 };
        Assert.True(part.IsLowStock);
    }
}
