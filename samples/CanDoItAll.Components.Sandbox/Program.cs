using CanDoItAll.Components.Sandbox;
using CanDoItAll.Components.Sandbox.Components;
using CanDoItAll.Components.BaseLib;
using CanDoItAll.Components.Charts;
using CanDoItAll.Components.Mermaid.Infrastructure;
using CanDoItAll.Components.QRCode;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCanDoItAllBaseLib();
builder.Services.AddCanDoItAllCharts();
builder.Services.AddCanDoItAllMermaid();
builder.Services.AddCanDoItAllQrCode();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/api/pages.json", () =>
{
    var groupEntries = SandboxCatalogRegistry.Groups
        .Select(group => new CatalogEntry(group.Route, group.Title, "Components"))
        .Prepend(new CatalogEntry("/", "Home", "Home"));

    return Results.Json(groupEntries.ToArray());
});

app.Run();

public partial class Program;

internal sealed record CatalogEntry(string Path, string Title, string Group);
