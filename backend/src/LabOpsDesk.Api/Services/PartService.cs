using LabOpsDesk.Api.Abstractions;
using LabOpsDesk.Api.Contracts;

namespace LabOpsDesk.Api.Services;

public sealed class PartService : IPartService
{
    private readonly IPartStore _store;

    public PartService(IPartStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<PartResponse>> ListAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PartResponse> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PartResponse> CreateAsync(CreatePartRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PartResponse> UpdateAsync(Guid id, UpdatePartRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PartResponse> AdjustQuantityAsync(Guid id, AdjustPartQuantityRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
