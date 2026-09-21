using LabOpsDesk.Api.Abstractions;
using LabOpsDesk.Api.Domain;
using LabOpsDesk.Api.Options;
using Microsoft.Extensions.Options;

namespace LabOpsDesk.Api.Persistence;

public sealed class CsvPartStore : IPartStore
{
    private readonly string _path;

    public CsvPartStore(IOptions<DataStoreOptions> options)
    {
        _path = options.Value.PartsCsvPath;
    }

    public Task<IReadOnlyList<Part>> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Part?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Part?> GetBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(Part part, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
