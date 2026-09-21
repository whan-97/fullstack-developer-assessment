using LabOpsDesk.Api.Domain;
using LabOpsDesk.Api.Options;
using LabOpsDesk.Api.Persistence;
using Microsoft.Extensions.Options;

namespace LabOpsDesk.Api.Tests;

public class JsonAssetStoreTests : IDisposable
{
    private readonly string _path;
    private readonly JsonAssetStore _store;

    public JsonAssetStoreTests()
    {
        _path = Path.Combine(Path.GetTempPath(), $"assets-{Guid.NewGuid():N}.json");
        File.WriteAllText(_path, "[]");
        _store = new JsonAssetStore(Microsoft.Extensions.Options.Options.Create(new DataStoreOptions { AssetsJsonPath = _path }));
    }

    [Fact]
    public async Task Save_and_get_round_trip()
    {
        var asset = new Asset
        {
            Id = Guid.NewGuid(),
            AssetTag = "SEQ-100",
            Name = "Test Sequencer",
            Platform = "NovaSeq",
            Location = "Lab 1",
            Status = AssetStatus.Available
        };

        await _store.SaveAsync(asset, CancellationToken.None);
        var loaded = await _store.GetByIdAsync(asset.Id, CancellationToken.None);

        Assert.NotNull(loaded);
        Assert.Equal("SEQ-100", loaded!.AssetTag);
        Assert.Equal(AssetStatus.Available, loaded.Status);
    }

    [Fact]
    public async Task GetByTag_is_case_insensitive()
    {
        var asset = new Asset
        {
            Id = Guid.NewGuid(),
            AssetTag = "SEQ-200",
            Name = "Case Test",
            Platform = "MiSeq",
            Location = "Lab 2",
            Status = AssetStatus.Available
        };

        await _store.SaveAsync(asset, CancellationToken.None);
        var loaded = await _store.GetByTagAsync("seq-200", CancellationToken.None);

        Assert.NotNull(loaded);
        Assert.Equal(asset.Id, loaded!.Id);
    }

    public void Dispose()
    {
        if (File.Exists(_path))
        {
            File.Delete(_path);
        }
    }
}
