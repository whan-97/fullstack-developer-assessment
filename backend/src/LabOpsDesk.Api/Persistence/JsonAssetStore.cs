using LabOpsDesk.Api.Abstractions;
using LabOpsDesk.Api.Domain;
using LabOpsDesk.Api.Options;
using Microsoft.Extensions.Options;

namespace LabOpsDesk.Api.Persistence;

public sealed class JsonAssetStore : IAssetStore
{
    private readonly string _path;

    public JsonAssetStore(IOptions<DataStoreOptions> options)
    {
        _path = options.Value.AssetsJsonPath;
    }

    public Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Asset?> GetByTagAsync(string assetTag, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(Asset asset, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
