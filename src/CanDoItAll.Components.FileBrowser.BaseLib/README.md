# CanDoItAll.Components.FileBrowser.BaseLib

Enterprise Blazor rendering for `CanDoItAll.Components.FileBrowser.Core`, composed from CanDoItAll BaseLib controls.

The renderer supports provider-driven shallow paging, breadcrumbs and navigation history, list and card views, source-aware search scopes, sort and type filters, capability-driven compact actions, selection, and explicit loading, empty, partial, and failed states. The same component can fill a page, sit in a dialog, or run inside a narrow Canvas floating window because responsive behavior follows the component container.

The host owns `ThemeHost`, application `Layout`, source navigation, and overlay hosts. This package owns only the reusable file work area and optional companion source navigation.

## Host assets

Register BaseLib and include both style packages in the host document:

```csharp
builder.Services.AddCanDoItAllBaseLib();
```

```razor
<link rel="stylesheet" href="_content/CanDoItAll.Components.BaseLib/css/material-icons.css" />
<link rel="stylesheet" href="_content/CanDoItAll.Components.BaseLib/css/output.css" />
<link rel="stylesheet" href="_content/CanDoItAll.Components.FileBrowser.BaseLib/css/file-browser.css" />
```

Compose providers and one session in the host, then pass only the session to the reusable renderer:

```csharp
IFileBrowserSession session = new FileBrowserSession(
    providers,
    new FileBrowserSessionOptions(pageSize: 50));
```

```razor
<FileBrowser Session="@session"
             InitialSourceId="@sourceId"
             ItemInvoked="OpenFileAsync"
             ActionRequested="HandleActionAsync" />
```

`FileBrowserSourceNavigation` is an optional companion for a full-page Drive-like composition. Omit it when the browser is scoped to a floating project window. Exact shell and asset composition is shown by the dedicated sandbox.

The renderer discovers custom actions through `IFileBrowserSession.GetActionsAsync`; it never reaches into a concrete provider. `ActionRequested` remains the host-effect seam. A host can call `ExecuteActionAsync` and then apply the returned navigation URI or clipboard value, while content consumers call `OpenReadAsync` and dispose the returned lease. Retry UI calls `RetryAsync`, which repeats the exact failed session command without losing the prior stable view.

Core navigation targets accept only well-formed `http`/`https` URLs and safe same-host relative targets. Active and local schemes such as `javascript:`, `data:`, `file:`, and `ipfs:` are rejected. Before applying an external navigation result, the host should additionally enforce its own trusted-origin allowlist and opening/download policy.

## Validate

```powershell
dotnet build src/CanDoItAll.Components.FileBrowser.BaseLib/CanDoItAll.Components.FileBrowser.BaseLib.csproj
dotnet test tests/CanDoItAll.Components.FileBrowser.BaseLib.Tests/CanDoItAll.Components.FileBrowser.BaseLib.Tests.csproj
```
