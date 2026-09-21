using LabOpsDesk.Api.Abstractions;
using LabOpsDesk.Api.Contracts;
using LabOpsDesk.Api.Domain;
using LabOpsDesk.Api.Services;
using Moq;

namespace LabOpsDesk.Api.Tests;

public class AssetServiceTests
{
    private readonly Mock<IAssetStore> _store = new();
    private readonly AssetService _service;

    public AssetServiceTests()
    {
        _service = new AssetService(_store.Object);
    }

    [Fact]
    public async Task CreateAsync_rejects_duplicate_asset_tag()
    {
        _store.Setup(s => s.GetByTagAsync("SEQ-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Asset { Id = Guid.NewGuid(), AssetTag = "SEQ-001" });

        var request = new CreateAssetRequest
        {
            AssetTag = "SEQ-001",
            Name = "Dup",
            Platform = "NovaSeq",
            Location = "Lab"
        };

        await Assert.ThrowsAsync<ConflictException>(() =>
            _service.CreateAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CheckOutAsync_persists_updated_asset()
    {
        var id = Guid.NewGuid();
        var asset = new Asset
        {
            Id = id,
            AssetTag = "SEQ-010",
            Name = "Bay B",
            Platform = "NextSeq",
            Location = "Lab 2",
            Status = AssetStatus.Available
        };

        _store.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        var result = await _service.CheckOutAsync(
            id,
            new CheckOutAssetRequest { Assignee = "morgan.park" },
            CancellationToken.None);

        Assert.Equal("InUse", result.Status);
        Assert.Equal("morgan.park", result.CheckedOutTo);
        _store.Verify(s => s.SaveAsync(It.Is<Asset>(a => a.Status == AssetStatus.InUse), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_missing_asset_throws_not_found()
    {
        var id = Guid.NewGuid();
        _store.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asset?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _service.GetAsync(id, CancellationToken.None));
    }
}
