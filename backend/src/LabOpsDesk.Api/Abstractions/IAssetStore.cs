using LabOpsDesk.Api.Domain;

namespace LabOpsDesk.Api.Abstractions;

public interface IAssetStore
{
    Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken);
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Asset?> GetByTagAsync(string assetTag, CancellationToken cancellationToken);
    Task SaveAsync(Asset asset, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
