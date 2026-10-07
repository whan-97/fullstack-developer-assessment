using LabOpsDesk.Api.Abstractions;
using LabOpsDesk.Api.Contracts;
using LabOpsDesk.Api.Domain;
using LabOpsDesk.Api.Mapping;

namespace LabOpsDesk.Api.Services;

public sealed class AssetService : IAssetService
{
    private readonly IAssetStore _store;

    public AssetService(IAssetStore store)
    {
        _store = store;
    }

    public async Task<IReadOnlyList<AssetResponse>> ListAsync(CancellationToken cancellationToken)
    {
        var getAssets = await _store.GetAllAsync(cancellationToken);
        return getAssets.Select(AssetMapper.ToResponse).ToList();
    }

    public async Task<AssetResponse> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var getAsset = await _store.GetByIdAsync(id, cancellationToken);
        if (getAsset is null)
        {
            throw new EntityNotFoundException(nameof(Asset), id);
        }

        return AssetMapper.ToResponse(getAsset);
    }

    public async Task<AssetResponse> CreateAsync(CreateAssetRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.AssetTag))
        {
            throw new ArgumentException("Asset Tag cannot be null or empty.", nameof(request.AssetTag));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(request.Name));
        }

        if (string.IsNullOrWhiteSpace(request.Platform))
        {
            throw new ArgumentException("Platform cannot be null or empty.", nameof(request.Platform));
        }

        if (string.IsNullOrWhiteSpace(request.Location))
        {
            throw new ArgumentException("Location cannot be null or empty.", nameof(request.Location));
        }

        var existing = await _store.GetByTagAsync(request.AssetTag.Trim(), cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException($"An asset with tag '{request.AssetTag}' already exists.");
        }

        var asset = new Asset
        {
            Id = Guid.NewGuid(),
            AssetTag = request.AssetTag.Trim(),
            Name = request.Name.Trim(),
            Platform = request.Platform.Trim(),
            Location = request.Location.Trim(),
            Status = AssetStatus.Available,
            Notes = request.Notes?.Trim() ?? string.Empty
        };

        await _store.SaveAsync(asset, cancellationToken);
        return AssetMapper.ToResponse(asset);
    }

    public async Task<AssetResponse> UpdateAsync(Guid id, UpdateAssetRequest request, CancellationToken cancellationToken)
    {
        var getAsset = await _store.GetByIdAsync(id, cancellationToken);
        if (getAsset is null)
        {
            throw new EntityNotFoundException(nameof(Asset), id);
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(request.Name));
        }

        if (string.IsNullOrWhiteSpace(request.Platform))
        {
            throw new ArgumentException("Platform cannot be null or empty.", nameof(request.Platform));
        }

        if (string.IsNullOrWhiteSpace(request.Location))
        {
            throw new ArgumentException("Location cannot be null or empty.", nameof(request.Location));
        }

        if (getAsset.Status != request.Status)
        {
            getAsset.TransitionTo(request.Status);
        }

        getAsset.Name = request.Name.Trim();
        getAsset.Platform = request.Platform.Trim();
        getAsset.Location = request.Location.Trim();
        getAsset.Notes = request.Notes?.Trim() ?? string.Empty;

        await _store.SaveAsync(getAsset, cancellationToken);
        return AssetMapper.ToResponse(getAsset);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var getAsset = await _store.GetByIdAsync(id, cancellationToken);
        if (getAsset is null)
        {
            throw new EntityNotFoundException(nameof(Asset), id);
        }

        await _store.DeleteAsync(id, cancellationToken);
    }

    public async Task<AssetResponse> CheckOutAsync(Guid id, CheckOutAssetRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Assignee))
        {
            throw new ArgumentException("Assignee cannot be null or empty for checkout.", nameof(request.Assignee));
        }

        var getAsset = await _store.GetByIdAsync(id, cancellationToken);
        if (getAsset is null)
        {
            throw new EntityNotFoundException(nameof(Asset), id);
        }

        getAsset.CheckOut(request.Assignee.Trim(), DateTimeOffset.UtcNow);

        await _store.SaveAsync(getAsset, cancellationToken);
        return AssetMapper.ToResponse(getAsset);
    }

    public async Task<AssetResponse> CheckInAsync(Guid id, CancellationToken cancellationToken)
    {
        var getAsset = await _store.GetByIdAsync(id, cancellationToken);
        if (getAsset is null)
        {
            throw new EntityNotFoundException(nameof(Asset), id);
        }

        getAsset.CheckIn();

        await _store.SaveAsync(getAsset, cancellationToken);
        return AssetMapper.ToResponse(getAsset);
    }
}