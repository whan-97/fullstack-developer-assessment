namespace LabOpsDesk.Api.Domain;

public sealed class InvalidAssetTransitionException : Exception
{
    public AssetStatus From { get; }
    public AssetStatus To { get; }

    public InvalidAssetTransitionException(AssetStatus from, AssetStatus to)
        : base($"Cannot transition asset from '{from}' to '{to}'.")
    {
        From = from;
        To = to;
    }
}
