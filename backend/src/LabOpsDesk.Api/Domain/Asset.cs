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

    public void TransitionTo(AssetStatus updatedStatus)
    {
        if (!CanTransitionTo(updatedStatus))
            throw new InvalidAssetTransitionException(Status, updatedStatus);

        Status = updatedStatus;
    }

    public bool CanTransitionTo(AssetStatus updatedStatus)
    {
        if (Status == updatedStatus)
            return false;

        switch (Status)
        {
            case AssetStatus.Available:
                return updatedStatus == AssetStatus.InUse ||
                       updatedStatus == AssetStatus.Maintenance ||
                       updatedStatus == AssetStatus.Retired;

            case AssetStatus.InUse:
                return updatedStatus == AssetStatus.Available ||
                       updatedStatus == AssetStatus.Maintenance;

            case AssetStatus.Maintenance:
                return updatedStatus == AssetStatus.Available ||
                       updatedStatus == AssetStatus.Retired;

            case AssetStatus.Retired:
                return false;

            default:
                return false;
        }
    }

    public void CheckOut(string assignee, DateTimeOffset currentTime)
    {
        if (string.IsNullOrWhiteSpace(assignee))
            throw new ArgumentException("Assignee cannot be empty.", nameof(assignee));

        TransitionTo(AssetStatus.InUse);

        CheckedOutTo = assignee;
        CheckedOutAt = currentTime;
    }

    public void CheckIn()
    {
        TransitionTo(AssetStatus.Available);

        CheckedOutTo = null;
        CheckedOutAt = null;
    }
}