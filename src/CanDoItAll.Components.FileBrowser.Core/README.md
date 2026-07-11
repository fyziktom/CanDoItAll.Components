# CanDoItAll.Components.FileBrowser.Core

Provider-neutral contracts and runtime state for browsing hierarchical file-like sources.

The package owns opaque occurrence identity, shallow paged browsing, immutable session snapshots, bounded inactive-query retention, atomic navigation/session transitions, exact failed-command retry, deterministic sorting, loaded/native/progressive search strategies, capabilities, action/content dispatch, and typed failures. Active results explicitly loaded by the user remain materialized even when they exceed the inactive-cache budget. It has no Blazor, BaseLib, CanvasLib, filesystem, IPFS, or CanDoItAll.Web dependency.

Provider paths and pages are validated before they can change visible state. Snapshots preserve browse completeness and consistency tokens. Progressive result continuations are served from bounded, expiring immutable snapshots rather than the evictable browse tree; this makes later pages deterministic, while deliberately spending the configured traversal budget before returning page one.

Renderers should use `IFileBrowserSession` for browsing and also for `GetActionsAsync`, `ExecuteActionAsync`, and `OpenReadAsync`. `RetryAsync` repeats the exact failed command against the prior stable view.

Use it when a file surface must share behavior across the first BaseLib renderer and future renderers such as CanvasLib.

See the repository [file browser architecture](https://github.com/fyziktom/CanDoItAll.Components/blob/main/docs/file-browser/architecture.md) and [provider authoring guide](https://github.com/fyziktom/CanDoItAll.Components/blob/main/docs/file-browser/provider-guide.md).

## Validate

```powershell
dotnet build src/CanDoItAll.Components.FileBrowser.Core/CanDoItAll.Components.FileBrowser.Core.csproj
dotnet test tests/CanDoItAll.Components.FileBrowser.Core.Tests/CanDoItAll.Components.FileBrowser.Core.Tests.csproj
```
