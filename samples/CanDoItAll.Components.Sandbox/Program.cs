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

if (app.Environment.IsDevelopment())
{
    // Fingerprinted static assets are served with long-lived, immutable cache headers by
    // MapStaticAssets, which is correct for production but makes it easy to chase a stale
    // cached response while iterating locally (especially after restarting dotnet watch or
    // switching branches). Force no-store in Development so every request always hits disk.
    app.Use(async (context, next) =>
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.CacheControl = "no-store";
            context.Response.Headers.Remove("ETag");
            return Task.CompletedTask;
        });

        await next();
    });
}

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(Routes).Assembly)
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
