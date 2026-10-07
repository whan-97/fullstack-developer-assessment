using System.Globalization;
using System.Text;
using LabOpsDesk.Api.Abstractions;
using LabOpsDesk.Api.Domain;
using LabOpsDesk.Api.Options;
using Microsoft.Extensions.Options;

namespace LabOpsDesk.Api.Persistence;

public sealed class CsvPartStore : IPartStore
{
    private static readonly SemaphoreSlim Lock = new(1, 1);
    private const string CsvHeader = "Id,Sku,Name,Category,UnitOfMeasure,QuantityOnHand,ReorderThreshold,Location";

    private readonly string _path;

    public CsvPartStore(IOptions<DataStoreOptions> options)
    {
        _path = options.Value.PartsCsvPath;
    }

    public async Task<IReadOnlyList<Part>> GetAllAsync(CancellationToken cancellationToken)
    {
        await Lock.WaitAsync(cancellationToken);
        try
        {
            return await ReadPartsFromFileAsync(cancellationToken);
        }
        finally
        {
            Lock.Release();
        }
    }

    public async Task<Part?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await Lock.WaitAsync(cancellationToken);
        try
        {
            var parts = await ReadPartsFromFileAsync(cancellationToken);
            return parts.FirstOrDefault(p => p.Id == id);
        }
        finally
        {
            Lock.Release();
        }
    }

    public async Task<Part?> GetBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        await Lock.WaitAsync(cancellationToken);
        try
        {
            var parts = await ReadPartsFromFileAsync(cancellationToken);
            return parts.FirstOrDefault(p => string.Equals(p.Sku, sku, StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            Lock.Release();
        }
    }

    public async Task SaveAsync(Part part, CancellationToken cancellationToken)
    {
        await Lock.WaitAsync(cancellationToken);
        try
        {
            var parts = await ReadPartsFromFileAsync(cancellationToken);
            var index = parts.FindIndex(p => p.Id == part.Id);

            if (index >= 0)
            {
                parts[index] = part;
            }
            else
            {
                parts.Add(part);
            }

            await WritePartsToFileAsync(parts, cancellationToken);
        }
        finally
        {
            Lock.Release();
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await Lock.WaitAsync(cancellationToken);
        try
        {
            var parts = await ReadPartsFromFileAsync(cancellationToken);
            var removed = parts.RemoveAll(p => p.Id == id);

            if (removed > 0)
            {
                await WritePartsToFileAsync(parts, cancellationToken);
            }
        }
        finally
        {
            Lock.Release();
        }
    }

    private async Task<List<Part>> ReadPartsFromFileAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_path))
        {
            return new List<Part>();
        }

        var lines = await File.ReadAllLinesAsync(_path, cancellationToken);
        var parts = new List<Part>();

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var fields = ParseCsvLine(line);
            if (fields.Length < 8) continue;

            if (Guid.TryParse(fields[0], out var id) &&
                int.TryParse(fields[5], NumberStyles.Integer, CultureInfo.InvariantCulture, out var qty) &&
                int.TryParse(fields[6], NumberStyles.Integer, CultureInfo.InvariantCulture, out var reorder))
            {
                parts.Add(new Part
                {
                    Id = id,
                    Sku = fields[1],
                    Name = fields[2],
                    Category = fields[3],
                    UnitOfMeasure = fields[4],
                    QuantityOnHand = qty,
                    ReorderThreshold = reorder,
                    Location = fields[7]
                });
            }
        }

        return parts;
    }

    private async Task WritePartsToFileAsync(List<Part> parts, CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var lines = new List<string> { CsvHeader };
        lines.AddRange(parts.Select(p =>
            $"{p.Id},{EscapeCsv(p.Sku)},{EscapeCsv(p.Name)},{EscapeCsv(p.Category)},{EscapeCsv(p.UnitOfMeasure)},{p.QuantityOnHand},{p.ReorderThreshold},{EscapeCsv(p.Location)}"
        ));

        await File.WriteAllLinesAsync(_path, lines, Encoding.UTF8, cancellationToken);
    }

    private static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(current.ToString());
        return result.ToArray();
    }

    private static string EscapeCsv(string field)
    {
        if (string.IsNullOrEmpty(field)) return string.Empty;

        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}