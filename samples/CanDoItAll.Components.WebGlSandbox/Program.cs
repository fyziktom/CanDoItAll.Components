using CanDoItAll.Components.BaseLib;
using CanDoItAll.Components.WebGlSandbox.Components;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = builder.Environment.IsDevelopment();
    });
builder.Services.AddCanDoItAllBaseLib();

var app = builder.Build();

app.UseHttpsRedirection();
var externalModelRoot = FindExternalModelRoot(app.Environment.ContentRootPath);
if (Directory.Exists(externalModelRoot))
{
    var modelContentTypes = new FileExtensionContentTypeProvider();
    modelContentTypes.Mappings[".glb"] = "model/gltf-binary";
    modelContentTypes.Mappings[".gltf"] = "model/gltf+json";
    app.UseStaticFiles(new StaticFileOptions
    {
        ContentTypeProvider = modelContentTypes,
        FileProvider = new PhysicalFileProvider(externalModelRoot),
        RequestPath = "/assets/external-models"
    });
}

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static string? FindExternalModelRoot(string startDirectory)
{
    var directory = new DirectoryInfo(startDirectory);
    while (directory is not null)
    {
        var candidate = Path.Combine(directory.FullName, "3DModels");
        if (Directory.Exists(candidate))
        {
            return candidate;
        }

        directory = directory.Parent;
    }

    return null;
}

public partial class Program;
