using LabOpsDesk.Api.Domain;

namespace LabOpsDesk.Api.Abstractions;

public interface IPartStore
{
    Task<IReadOnlyList<Part>> GetAllAsync(CancellationToken cancellationToken);
    Task<Part?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Part?> GetBySkuAsync(string sku, CancellationToken cancellationToken);
    Task SaveAsync(Part part, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
