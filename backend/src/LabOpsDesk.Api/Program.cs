using System.Text.Json.Serialization;
using LabOpsDesk.Api.Abstractions;
using LabOpsDesk.Api.Endpoints;
using LabOpsDesk.Api.Middleware;
using LabOpsDesk.Api.Options;
using LabOpsDesk.Api.Persistence;
using LabOpsDesk.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "data");

builder.Services.Configure<DataStoreOptions>(options =>
{
    options.AssetsJsonPath = Path.Combine(dataDirectory, "assets.json");
    options.PartsCsvPath = Path.Combine(dataDirectory, "parts.csv");
});

builder.Services.AddSingleton<IAssetStore, JsonAssetStore>();
builder.Services.AddSingleton<IPartStore, CsvPartStore>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IPartService, PartService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));
app.MapAssetEndpoints();
app.MapPartEndpoints();

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;
