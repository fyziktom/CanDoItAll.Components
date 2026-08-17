using CanDoItAll.Components.BaseLib;
using CanDoItAll.Components.Charts;
using CanDoItAll.Components.Mermaid.Infrastructure;
using CanDoItAll.Components.QRCode;
using CanDoItAll.Components.Sandbox;
using CanDoItAll.Components.SandboxWasm.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

SandboxQueryLinks.UseHashRouting = true;
builder.RootComponents.Add<App>("#app");
builder.Services.AddCanDoItAllBaseLib();
builder.Services.AddCanDoItAllCharts();
builder.Services.AddCanDoItAllMermaid();
builder.Services.AddCanDoItAllQrCode();
await builder.Build().RunAsync();
