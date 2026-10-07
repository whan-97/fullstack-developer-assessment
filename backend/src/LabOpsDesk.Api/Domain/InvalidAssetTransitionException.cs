namespace LabOpsDesk.Api.Domain;

public sealed class InvalidAssetTransitionException : Exception
{
    public AssetStatus From { get; }
    public AssetStatus To { get; }

    public InvalidAssetTransitionException(AssetStatus assetFrom, AssetStatus assetTo)
        : base($"Cannot transition asset from '{assetFrom}' to '{assetTo}'.")
    {
        From = assetFrom;
        To = assetTo;
    }
}
