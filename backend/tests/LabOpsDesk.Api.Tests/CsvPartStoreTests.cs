using LabOpsDesk.Api.Domain;
using LabOpsDesk.Api.Options;
using LabOpsDesk.Api.Persistence;
using Microsoft.Extensions.Options;

namespace LabOpsDesk.Api.Tests;

public class CsvPartStoreTests : IDisposable
{
    private readonly string _path;
    private readonly CsvPartStore _store;

    public CsvPartStoreTests()
    {
        _path = Path.Combine(Path.GetTempPath(), $"parts-{Guid.NewGuid():N}.csv");
        File.WriteAllText(_path, "Id,Sku,Name,Category,UnitOfMeasure,QuantityOnHand,ReorderThreshold,Location\n");
        _store = new CsvPartStore(Microsoft.Extensions.Options.Options.Create(new DataStoreOptions { PartsCsvPath = _path }));
    }

    [Fact]
    public async Task Save_and_list_round_trip()
    {
        var part = new Part
        {
            Id = Guid.NewGuid(),
            Sku = "SKU-T-1",
            Name = "Test Part",
            Category = "Consumable",
            UnitOfMeasure = "each",
            QuantityOnHand = 4,
            ReorderThreshold = 2,
            Location = "Cage"
        };

        await _store.SaveAsync(part, CancellationToken.None);
        var all = await _store.GetAllAsync(CancellationToken.None);

        Assert.Contains(all, p => p.Sku == "SKU-T-1" && p.QuantityOnHand == 4);
    }

    [Fact]
    public async Task Delete_removes_row()
    {
        var part = new Part
        {
            Id = Guid.NewGuid(),
            Sku = "SKU-DEL",
            Name = "Delete Me",
            Category = "Accessory",
            UnitOfMeasure = "each",
            QuantityOnHand = 1,
            ReorderThreshold = 1,
            Location = "Cage"
        };

        await _store.SaveAsync(part, CancellationToken.None);
        await _store.DeleteAsync(part.Id, CancellationToken.None);
        var loaded = await _store.GetByIdAsync(part.Id, CancellationToken.None);

        Assert.Null(loaded);
    }

    public void Dispose()
    {
        if (File.Exists(_path))
        {
            File.Delete(_path);
        }
    }
}
