using System.Text.Json;
using System.Text.Json.Serialization;
using LabOpsDesk.Api.Abstractions;
using LabOpsDesk.Api.Domain;
using LabOpsDesk.Api.Options;
using Microsoft.Extensions.Options;

namespace LabOpsDesk.Api.Persistence;

public sealed class JsonAssetStore : IAssetStore
{
    private static readonly SemaphoreSlim Lock = new(1, 1);
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _path;

    public JsonAssetStore(IOptions<DataStoreOptions> options)
    {
        _path = options.Value.AssetsJsonPath;
    }

    public async Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken)
    {
        await Lock.WaitAsync(cancellationToken);
        try
        {
            return await ReadAssetsFromFileAsync(cancellationToken);
        }
        finally
        {
            Lock.Release();
        }
    }

    public async Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await Lock.WaitAsync(cancellationToken);
        try
        {
            var assets = await ReadAssetsFromFileAsync(cancellationToken);
            return assets.FirstOrDefault(a => a.Id == id);
        }
        finally
        {
            Lock.Release();
        }
    }

    public async Task<Asset?> GetByTagAsync(string assetTag, CancellationToken cancellationToken)
    {
        await Lock.WaitAsync(cancellationToken);
        try
        {
            var assets = await ReadAssetsFromFileAsync(cancellationToken);
            return assets.FirstOrDefault(a => string.Equals(a.AssetTag, assetTag, StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            Lock.Release();
        }
    }

    public async Task SaveAsync(Asset asset, CancellationToken cancellationToken)
    {
        await Lock.WaitAsync(cancellationToken);
        try
        {
            var assets = await ReadAssetsFromFileAsync(cancellationToken);
            var index = assets.FindIndex(a => a.Id == asset.Id);

            if (index >= 0)
            {
                assets[index] = asset;
            }
            else
            {
                assets.Add(asset);
            }

            await WriteAssetsToFileAsync(assets, cancellationToken);
        }
        finally
        {
            Lock.Release();
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await Lock.WaitAsync(cancellationToken);
        try
        {
            var assets = await ReadAssetsFromFileAsync(cancellationToken);
            var removed = assets.RemoveAll(a => a.Id == id);

            if (removed > 0)
            {
                await WriteAssetsToFileAsync(assets, cancellationToken);
            }
        }
        finally
        {
            Lock.Release();
        }
    }

    private async Task<List<Asset>> ReadAssetsFromFileAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_path))
        {
            return new List<Asset>();
        }

        await using var stream = File.OpenRead(_path);
        var assets = await JsonSerializer.DeserializeAsync<List<Asset>>(stream, JsonOpts, cancellationToken);
        return assets ?? new List<Asset>();
    }

    private async Task WriteAssetsToFileAsync(List<Asset> assets, CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_path);
        await JsonSerializer.SerializeAsync(stream, assets, JsonOpts, cancellationToken);
    }
}