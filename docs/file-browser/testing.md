# File Browser Testing and Verification

## Automated suites

| Scope | What is pinned |
|---|---|
| Core | contracts, ordering, validated tree retention, atomic navigation/session state, exact retry, catalog, four search strategies, paging, errors, lifecycle, actions/content, and cancellation |
| FileSystem provider | isolated roots, shallow paging, all advertised sorts/filters, stale cursors, traversal containment, hidden/reparse policies, and metadata |
| BaseLib renderer projections | formatting, icon mapping, public component parameters, capability-driven menus/events, search input/debounce races, and safe defaults |
| Shared BaseLib | tooltip interop/disposal lifecycle and existing package/component approvals |

Final Release results: Core 156, FileSystem 47, FileBrowser.BaseLib 51, and shared BaseLib 69 tests. That is 254 FileBrowser-specific tests and 323 tests including the shared Tooltip regressions, with zero failures or skips. The complete CI release-focus matrix also passed Common 5 and QRCode 9 for 337 total tests. No suite requires network access or credentials, and filesystem tests use disposable isolated roots.

The core suite contains explicit regressions for source protection cleanup, item-pressure LRU eviction, orphan append rejection, search/browse cursor isolation, strict malformed path/search/page rejection, page-size and repeated-cursor defenses, browse completeness, renderer-visible atomic rollback, exact failed-command retry, cancellation commit points, progressive traversal cache isolation, disposal races, capability-gated and response-validated action/content behavior, URI scheme rejection, progressive budgets, per-container consistency tokens, idempotent continuation replay, continuation durability across tree eviction/source mutation, continuation expiry/cross-query rejection, and invalid default request keys.

## Commands

```powershell
dotnet build CanDoItAll.Components.slnx -c Release --no-restore --no-incremental
dotnet test tests/CanDoItAll.Components.FileBrowser.Core.Tests/CanDoItAll.Components.FileBrowser.Core.Tests.csproj -c Release --no-build --no-restore
dotnet test tests/CanDoItAll.Components.FileBrowser.Providers.FileSystem.Tests/CanDoItAll.Components.FileBrowser.Providers.FileSystem.Tests.csproj -c Release --no-build --no-restore
dotnet test tests/CanDoItAll.Components.FileBrowser.BaseLib.Tests/CanDoItAll.Components.FileBrowser.BaseLib.Tests.csproj -c Release --no-build --no-restore
dotnet test tests/CanDoItAll.Components.BaseLib.Tests/CanDoItAll.Components.BaseLib.Tests.csproj -c Release --no-build --no-restore
dotnet build src/CanDoItAll.Components.FileBrowser.Sandbox/CanDoItAll.Components.FileBrowser.Sandbox.csproj -c Release --no-restore -warnaserror
powershell -ExecutionPolicy Bypass -File scripts/pack-release.ps1 -NoBuild
```

## Browser acceptance matrix

The final Release assemblies were exercised in the dedicated Interactive Server sandbox with real Chromium at 1600×1000 and 390×844. Final proof artifacts are under `output/playwright/` (`desktop-final-closure.png`, `context-menu-final-closure.png`, `projects-recursive-final-closure.png`, `ipfs-native-search-final-closure.png`, `retry-success-final-closure.png`, `progressive-search-final-closure.png`, `mobile-dark-final-closure.png`, and `empty-folder-mobile-final-closure.png`).

| Scenario | Acceptance assertion |
|---|---|
| Project root first page and load-more | 8 rows grew to 12 of 12 with no duplicate key |
| Project/subproject navigation | Context open and keyboard Enter navigated; breadcrumb/history worked; cached back retained 12 rows with mock-call count unchanged at 5 |
| Include subprojects | Paged recursive result reached 24 of 56 and exposed paths such as `/projects/CanDoItAll.Web/modules` |
| IPFS card view | Card projection and CID copy action completed |
| Duplicate IPFS content | Native `handbook` search returned two occurrence paths (`Recent imports` and `Pinned collections`) sharing the tested CID identity |
| Search concurrency | Slow input preserved every prefix through `handbook`; an immediately superseded query left `handbook` and its two results current |
| Progressive filesystem search | Found `FileBrowserSession.cs`; stopped honestly at 250 containers / 1,639 items and rendered partial diagnostics |
| Retryable source | Failed source switch retained the prior IPFS query/results; exact Retry loaded `provider-recovered.txt` without recreating the session |
| Empty folder | Keyboard navigation opened `New project (empty)` and retained breadcrumb/history controls |
| Overflow/context actions | Open/copy/download menu stayed inside the 1600×1000 viewport |
| Compact dark frame | Theme host computed `rgb(2, 6, 23)` with the compact source selector |
| Mobile | 390px client and scroll widths matched; no horizontal overflow |

The closure pass reported zero browser-console warnings and zero errors. It also caught and fixed a slow-typing race: the toolbar now retains each immediate BaseLib `TextBox` draft before forwarding the non-blocking debounce callback. Tooltip interop is single-flight/disposal-aware and tolerates disconnected elements during overflow-menu rerenders.

## Architecture verification

The initial CodeAnalytics snapshot is `snap-20260710213317-b9914504`. Final focused snapshot `snap-20260711025115-c15334bd` was rebuilt after the last UI regression fix:

- 4 scoped projects, 54 documents, 116 types, and 909 members;
- no dependency cycle, error finding, open question, or blocking diagnostic;
- Core has no project reference; FileSystem and FileBrowser.BaseLib point inward to Core; Sandbox remains the outer composition root;
- two diagnostics are duplicate compiler-generated attribute display names;
- three warning findings are large-file heuristics for `FileBrowser.razor.cs`, `FileBrowserSession.cs`, and `FileBrowserTreeStore.cs`, retained as future extraction opportunities rather than dependency defects.

All three `0.1.0` NuGet archives and symbol packages were produced. The pack validator confirmed README, XML documentation, DLLs, Core/BaseLib dependency declarations, `file-browser.css`, collocated action JavaScript, and transitive static-web-asset props. Public publication is still blocked by the repository-wide unresolved final `LICENSE` decision; local proof packing is complete.

The Components MCP discovery endpoint was attempted repeatedly, including after final closure, and the root session returned `Transport closed`; this is recorded as tooling unavailability, not silently treated as a successful component-catalog proof. A separate audit session could reach it and confirmed the canonical BaseLib shell/assets.
