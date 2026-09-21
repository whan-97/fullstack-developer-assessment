namespace LabOpsDesk.Api.Options;

public sealed class DataStoreOptions
{
    public const string SectionName = "DataStore";

    public string AssetsJsonPath { get; set; } = string.Empty;
    public string PartsCsvPath { get; set; } = string.Empty;
}
