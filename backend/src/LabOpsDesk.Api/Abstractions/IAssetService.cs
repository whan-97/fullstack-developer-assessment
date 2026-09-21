using LabOpsDesk.Api.Contracts;

namespace LabOpsDesk.Api.Abstractions;

public interface IAssetService
{
    Task<IReadOnlyList<AssetResponse>> ListAsync(CancellationToken cancellationToken);
    Task<AssetResponse> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<AssetResponse> CreateAsync(CreateAssetRequest request, CancellationToken cancellationToken);
    Task<AssetResponse> UpdateAsync(Guid id, UpdateAssetRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<AssetResponse> CheckOutAsync(Guid id, CheckOutAssetRequest request, CancellationToken cancellationToken);
    Task<AssetResponse> CheckInAsync(Guid id, CancellationToken cancellationToken);
}
