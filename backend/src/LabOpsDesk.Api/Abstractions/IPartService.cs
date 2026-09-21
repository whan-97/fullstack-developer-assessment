using LabOpsDesk.Api.Contracts;

namespace LabOpsDesk.Api.Abstractions;

public interface IPartService
{
    Task<IReadOnlyList<PartResponse>> ListAsync(CancellationToken cancellationToken);
    Task<PartResponse> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<PartResponse> CreateAsync(CreatePartRequest request, CancellationToken cancellationToken);
    Task<PartResponse> UpdateAsync(Guid id, UpdatePartRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<PartResponse> AdjustQuantityAsync(Guid id, AdjustPartQuantityRequest request, CancellationToken cancellationToken);
}
