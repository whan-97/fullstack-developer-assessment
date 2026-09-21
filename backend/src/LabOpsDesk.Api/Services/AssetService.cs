using LabOpsDesk.Api.Abstractions;
using LabOpsDesk.Api.Contracts;

namespace LabOpsDesk.Api.Services;

public sealed class AssetService : IAssetService
{
    private readonly IAssetStore _store;

    public AssetService(IAssetStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<AssetResponse>> ListAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<AssetResponse> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<AssetResponse> CreateAsync(CreateAssetRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<AssetResponse> UpdateAsync(Guid id, UpdateAssetRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<AssetResponse> CheckOutAsync(Guid id, CheckOutAssetRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<AssetResponse> CheckInAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
