using LabOpsDesk.Api.Abstractions;
using LabOpsDesk.Api.Contracts;
using LabOpsDesk.Api.Domain;
using LabOpsDesk.Api.Mapping;

namespace LabOpsDesk.Api.Services;

public sealed class PartService : IPartService
{
    private readonly IPartStore _store;

    public PartService(IPartStore store)
    {
        _store = store;
    }

    public async Task<IReadOnlyList<PartResponse>> ListAsync(CancellationToken cancellationToken)
    {
        var getParts = await _store.GetAllAsync(cancellationToken);
        return getParts.Select(PartMapper.ToResponse).ToList();
    }

    public async Task<PartResponse> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var getPart = await _store.GetByIdAsync(id, cancellationToken);
        if (getPart is null)
        {
            throw new EntityNotFoundException(nameof(Part), id);
        }

        return PartMapper.ToResponse(getPart);
    }

    public async Task<PartResponse> CreateAsync(CreatePartRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Sku))
        {
            throw new ArgumentException("SKU cannot be null or empty.", nameof(request.Sku));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(request.Name));
        }

        if (string.IsNullOrWhiteSpace(request.Category))
        {
            throw new ArgumentException("Category cannot be null or empty.", nameof(request.Category));
        }

        if (string.IsNullOrWhiteSpace(request.Location))
        {
            throw new ArgumentException("Location cannot be null or empty.", nameof(request.Location));
        }

        if (string.IsNullOrWhiteSpace(request.UnitOfMeasure))
        {
            throw new ArgumentException("Unit of Measure cannot be null or empty.", nameof(request.UnitOfMeasure));
        }

        if (request.QuantityOnHand < 0)
        {
            throw new ArgumentException("Quantity on hand cannot be negative.", nameof(request.QuantityOnHand));
        }

        if (request.ReorderThreshold < 0)
        {
            throw new ArgumentException("Reorder threshold cannot be negative.", nameof(request.ReorderThreshold));
        }

        var getItem = await _store.GetBySkuAsync(request.Sku.Trim(), cancellationToken);
        if (getItem is not null)
        {
            throw new ConflictException($"A part with SKU '{request.Sku}' already exists.");
        }

        var part = new Part
        {
            Id = Guid.NewGuid(),
            Sku = request.Sku.Trim(),
            Name = request.Name.Trim(),
            Category = request.Category.Trim(),
            UnitOfMeasure = request.UnitOfMeasure.Trim(),
            QuantityOnHand = request.QuantityOnHand,
            ReorderThreshold = request.ReorderThreshold,
            Location = request.Location.Trim()
        };

        await _store.SaveAsync(part, cancellationToken);
        return PartMapper.ToResponse(part);
    }

    public async Task<PartResponse> UpdateAsync(Guid id, UpdatePartRequest request, CancellationToken cancellationToken)
    {
        var getPart = await _store.GetByIdAsync(id, cancellationToken);
        if (getPart is null)
        {
            throw new EntityNotFoundException(nameof(Part), id);
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(request.Name));
        }

        if (string.IsNullOrWhiteSpace(request.Category))
        {
            throw new ArgumentException("Category cannot be null or empty.", nameof(request.Category));
        }

        if (string.IsNullOrWhiteSpace(request.Location))
        {
            throw new ArgumentException("Location cannot be null or empty.", nameof(request.Location));
        }

        if (string.IsNullOrWhiteSpace(request.UnitOfMeasure))
        {
            throw new ArgumentException("Unit of Measure cannot be null or empty.", nameof(request.UnitOfMeasure));
        }

        if (request.QuantityOnHand < 0)
        {
            throw new ArgumentException("Quantity on hand cannot be negative.", nameof(request.QuantityOnHand));
        }

        if (request.ReorderThreshold < 0)
        {
            throw new ArgumentException("Reorder threshold cannot be negative.", nameof(request.ReorderThreshold));
        }

        getPart.Name = request.Name.Trim();
        getPart.Category = request.Category.Trim();
        getPart.UnitOfMeasure = request.UnitOfMeasure.Trim();
        getPart.QuantityOnHand = request.QuantityOnHand;
        getPart.ReorderThreshold = request.ReorderThreshold;
        getPart.Location = request.Location.Trim();

        await _store.SaveAsync(getPart, cancellationToken);
        return PartMapper.ToResponse(getPart);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var getPart = await _store.GetByIdAsync(id, cancellationToken);
        if (getPart is null)
        {
            throw new EntityNotFoundException(nameof(Part), id);
        }

        await _store.DeleteAsync(id, cancellationToken);
    }

    public async Task<PartResponse> AdjustQuantityAsync(Guid id, AdjustPartQuantityRequest request, CancellationToken cancellationToken)
    {
        var getPart = await _store.GetByIdAsync(id, cancellationToken);
        if (getPart is null)
        {
            throw new EntityNotFoundException(nameof(Part), id);
        }

        getPart.AdjustQuantity(request.Delta);

        await _store.SaveAsync(getPart, cancellationToken);
        return PartMapper.ToResponse(getPart);
    }
}