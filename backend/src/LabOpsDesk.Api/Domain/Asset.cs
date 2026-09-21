namespace LabOpsDesk.Api.Domain;

public sealed class Asset
{
    public Guid Id { get; set; }
    public string AssetTag { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public AssetStatus Status { get; set; } = AssetStatus.Available;
    public string? CheckedOutTo { get; set; }
    public DateTimeOffset? CheckedOutAt { get; set; }
    public string? Notes { get; set; }

    public void TransitionTo(AssetStatus next)
    {
        throw new NotImplementedException();
    }

    public bool CanTransitionTo(AssetStatus next)
    {
        throw new NotImplementedException();
    }

    public void CheckOut(string assignee, DateTimeOffset at)
    {
        throw new NotImplementedException();
    }

    public void CheckIn()
    {
        throw new NotImplementedException();
    }
}
