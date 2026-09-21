using LabOpsDesk.Api.Domain;

namespace LabOpsDesk.Api.Tests;

public class AssetTransitionTests
{
    [Theory]
    [InlineData(AssetStatus.Available, AssetStatus.InUse)]
    [InlineData(AssetStatus.Available, AssetStatus.Maintenance)]
    [InlineData(AssetStatus.Available, AssetStatus.Retired)]
    [InlineData(AssetStatus.InUse, AssetStatus.Available)]
    [InlineData(AssetStatus.InUse, AssetStatus.Maintenance)]
    [InlineData(AssetStatus.Maintenance, AssetStatus.Available)]
    [InlineData(AssetStatus.Maintenance, AssetStatus.Retired)]
    public void CanTransitionTo_allows_valid_paths(AssetStatus from, AssetStatus to)
    {
        var asset = new Asset { Status = from };
        Assert.True(asset.CanTransitionTo(to));
    }

    [Theory]
    [InlineData(AssetStatus.Available, AssetStatus.Available)]
    [InlineData(AssetStatus.InUse, AssetStatus.Retired)]
    [InlineData(AssetStatus.Retired, AssetStatus.Available)]
    [InlineData(AssetStatus.Retired, AssetStatus.InUse)]
    [InlineData(AssetStatus.Maintenance, AssetStatus.InUse)]
    public void CanTransitionTo_blocks_illegal_paths(AssetStatus from, AssetStatus to)
    {
        var asset = new Asset { Status = from };
        Assert.False(asset.CanTransitionTo(to));
    }

    [Fact]
    public void CheckOut_from_available_sets_assignee_and_status()
    {
        var asset = new Asset { Status = AssetStatus.Available };
        var at = DateTimeOffset.Parse("2026-09-21T02:00:00Z");

        asset.CheckOut("jordan.lee", at);

        Assert.Equal(AssetStatus.InUse, asset.Status);
        Assert.Equal("jordan.lee", asset.CheckedOutTo);
        Assert.Equal(at, asset.CheckedOutAt);
    }

    [Fact]
    public void CheckOut_from_in_use_throws()
    {
        var asset = new Asset { Status = AssetStatus.InUse, CheckedOutTo = "alex.nguyen" };

        Assert.Throws<InvalidAssetTransitionException>(() =>
            asset.CheckOut("jordan.lee", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void CheckIn_clears_checkout_fields()
    {
        var asset = new Asset
        {
            Status = AssetStatus.InUse,
            CheckedOutTo = "alex.nguyen",
            CheckedOutAt = DateTimeOffset.UtcNow
        };

        asset.CheckIn();

        Assert.Equal(AssetStatus.Available, asset.Status);
        Assert.Null(asset.CheckedOutTo);
        Assert.Null(asset.CheckedOutAt);
    }
}
