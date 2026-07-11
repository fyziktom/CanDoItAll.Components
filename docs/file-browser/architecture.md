# File Browser Architecture

## Decision summary

Create independent packages rather than putting provider state into BaseLib:

| Project | Role | Allowed dependencies |
|---|---|---|
| `CanDoItAll.Components.FileBrowser.Core` | Contracts, immutable models, cache/tree state, navigation session, search strategies, deterministic ordering | BCL only unless a narrowly justified abstractions package is needed |
| `CanDoItAll.Components.FileBrowser.Providers.FileSystem` | Safe local-filesystem adapter | Core, BCL filesystem APIs |
| `CanDoItAll.Components.FileBrowser.BaseLib` | Blazor renderer and UI projections | Core, BaseLib |
| `CanDoItAll.Components.FileBrowser.Sandbox` | Composition root, project/IPFS mocks, realistic scenarios, browser proof | Core, FileSystem provider, BaseLib renderer, BaseLib |
| Core/FileSystem/BaseLib test projects | Isolated behavior and integration proof | Only the implementation under test and test framework packages |

Future direction:

```text
FileBrowser.CanvasLib -> FileBrowser.Core + CanvasLib
CanDoItAll project adapter -> FileBrowser.Core + CanDoItAll project contracts
CanDoItAll IPFS adapter -> FileBrowser.Core + shallow IPFS client contracts
```

Forbidden references:

```text
FileBrowser.Core -X-> BaseLib / CanvasLib / Blazor / IPFS / CanDoItAll.Web
FileBrowser.BaseLib -X-> FileSystem provider / IPFS / CanDoItAll.Web
Provider implementations -X-> FileBrowser.BaseLib
```

The sandbox is the only project that knows all concrete implementations.

## Current-state evidence

CodeAnalytics snapshot `snap-20260710213317-b9914504` established the baseline:

- `CanDoItAll.Components.BaseLib -> CanDoItAll.Components.Common`
- `CanvasLib -> BaseLib + Common + OverlayLib`
- Sandboxes are outer composition roots and reference product libraries.
- No project-level cycle was reported.
- The new core has no existing owner; placing it in BaseLib would make a future Canvas renderer depend on the first renderer.

Final focused snapshot `snap-20260711025115-c15334bd` contains the four shipped runtime/composition projects, 54 documents, 116 types, and 909 members. It reports no dependency cycle, error finding, open question, or blocking diagnostic. Core has no project reference; FileSystem and the renderer depend inward on Core; Sandbox remains the outer composition root. The remaining warnings are generated-name duplicates and large-file heuristics, not boundary violations.

Relevant source evidence includes:

- `CanDoItAll.Components.slnx`
- `src/CanDoItAll.Components.BaseLib/CanDoItAll.Components.BaseLib.csproj`
- `src/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj`
- `src/CanDoItAll.Components.Sandbox/SandboxCatalogRegistry.cs`
- `docs/standard-components-tailwind-policy.md`

## Responsibility inventory

| Responsibility | Owner |
|---|---|
| Provider-neutral item and query contracts | Core models |
| Provider selection | Provider catalog |
| Lazy child-page storage and deduplication | Tree store |
| Back/forward/up and active location | Navigation state |
| Browse/search orchestration and immutable snapshots | Session |
| Built-in/provider action discovery and execution | Session facade + optional action provider |
| Range-aware content access | Session facade + optional content provider |
| Search algorithm selection | Search strategy catalog/coordinator |
| Loaded-folder matching | Loaded-folder strategy |
| Loaded-tree matching | Loaded-tree strategy |
| Native backend search | Native-provider strategy + optional provider interface |
| Bounded on-the-fly traversal | Progressive traversal strategy |
| Filesystem path normalization and containment | FileSystem provider |
| Icons, display formatting, rows/cards, controls | BaseLib renderer |
| Applying navigation URIs, clipboard values, editors/windows, and other host effects | Host callback/action handler |
| Project and IPFS SDK mapping | Separate future adapter projects |
| Concrete provider registration | Sandbox or consuming app composition root |

No broad `Manager`, service locator, nested provider, or runtime partial-class cluster is introduced.

## Core model

### Identity

`FileBrowserItemKey` is `(SourceId, opaque occurrence value, optional revision)`. The value identifies a browser occurrence; the core never parses a filesystem path, CID, database key, or project identifier. Providers must not use content identity alone as occurrence identity: the same CID or hash can legitimately appear under multiple names and parents. `FileBrowserContentIdentity` separately carries a scheme/value identity such as a CID.

Items carry:

- stable key and optional parent key
- display name and logical/copy path
- kind (`Container`, `File`, or `Link`) and display category
- explicit child-state tri-state so unknown/unloaded is different from empty
- optional MIME type, size, owner, timestamps, open/download links
- optional scheme/value content identity such as `cid:<value>`
- metadata completeness so exact, partial, approximate, unknown, and expensive fields are not conflated
- item capabilities and provider-owned string metadata

Provider metadata is bounded and presentation-safe. External SDK objects never enter core records.

### Provider contracts

The base provider owns:

- source descriptor and capabilities
- root resolution
- shallow paged child listing
- root-to-item path resolution for breadcrumbs and deep initialization

Native deep search is a separate optional interface. This keeps ordinary providers small and prevents a `NotSupportedException` method from becoming the extension model.

Browse requests include parent key, page size, opaque continuation token, requested metadata fields, sort, filter, consistency token, and the explicit include-descendants option. Pages include items, next continuation token, optional total count, completeness, warnings, and revision/diagnostic metadata when supplied. A continuation token is valid only for the source, parent, revision, filter, and sort that produced it.

Content reads and provider-specific commands are segregated optional contracts rather than responsibilities of the hierarchy provider. This leaves room for range-limited preview streams, directory archives, pin/unpin, share, topology, and future write commands without making every provider or renderer fake those capabilities.

The session is the renderer-facing facade for those optional contracts. `GetActionsAsync`, `ExecuteActionAsync`, and `OpenReadAsync` resolve only loaded items in the active source, serialize access with session operations, validate built-in capability requirements, and delegate custom behavior to `IFileBrowserActionProvider` or `IFileBrowserContentProvider`. Hosts still own platform effects such as opening a URI, copying a returned value, or placing a content lease into an editor.

Provider responses are treated as untrusted boundary data. A resolved path must start at a parentless root, contain only containers from the requested source, have exact adjacent parent links, contain no duplicate/cyclic key, and end at the requested key. Browse and search pages are validated before mutation for requested page size, source mixing, self-parenting, shallow-parent violations, conflicting or repeated keys, visible parent cycles, impossible totals, cursor progress, and consistency-token mismatches. Optional action lists and content leases are validated before use, and custom actions require matching source/item capabilities. Contract violations become typed `CorruptProviderResponse` or retryable `StaleCursor` errors.

Open/download navigation values cross another trust boundary. Core accepts only well-formed HTTP(S) and safe same-host relative targets; active/local schemes, protocol-relative targets, backslashes, controls, and malformed values are rejected. The host remains responsible for a deployment-specific trusted-origin allowlist before applying an external effect.

### Tree store

The tree store keeps immutable item descriptors and per-container query pages:

- page application is replace or append
- item keys deduplicate overlapping backend pages
- each container records continuation, total count, query fingerprint, completeness, last access, and error state
- loaded-descendant traversal is breadth-first and cycle-safe
- inactive-query retention is bounded by options and protects the active breadcrumb path; active pages explicitly loaded by the user remain materialized and can exceed that inactive-cache budget
- page conformance is checked before any store mutation
- a container is considered fully cached only when completeness is `Complete` and it has neither continuation nor error

The store contains no provider calls and is unit-testable independently.

### Session and snapshots

`IFileBrowserSession` is the renderer-facing facade. It coordinates one operation at a time, delegates to providers and strategies, and publishes immutable `FileBrowserSnapshot` instances through a change event.

Renderer-visible transitions are atomic. Before a command starts, the session checkpoints the active provider, browse/search state, sort/filter options, selection, warnings, and complete navigation history. Initialization, source/location changes, history moves, query changes, refresh, and load-more prepare provider data before committing. Failure restores the checkpoint, publishes a typed error on the prior stable view, and records the exact command for `RetryAsync`; retry is not reconstructed from whatever location happens to be current. Disposal cancels and drains in-flight serialized work before synchronization primitives are released.

Snapshots include:

- available and current source
- current container and breadcrumb path
- visible browse or search items
- selected keys
- sort, filter, include-descendants, and search state
- back/forward/up capability
- continuation and optional total count
- loading phase, browse completeness, consistency token, partial-result information, and typed error
- cache/scanning diagnostics useful to the sandbox and telemetry

UI state such as list-versus-grid mode stays in the renderer. Provider and navigation state stays in the core.

## Search strategy records

### Strategy: loaded folder

- Force: instant filtering of the current materialized page with no I/O.
- Selected: strategy over the tree snapshot.
- Rejected: routing every search to the provider, which makes a local filter slow and surprising.
- Test seam: fake tree snapshot; assert zero provider calls.

### Strategy: loaded descendants

- Force: inspect only visited content and be honest about scope.
- Selected: cycle-safe cache traversal strategy.
- Rejected: silently fetching missing descendants, which changes the user's chosen scope.
- Test seam: partially loaded tree with a cycle/duplicate key.

### Strategy: native provider search

- Force: large stores may have indexed backend search and provider-specific continuation.
- Selected: adapter strategy over `IFileBrowserSearchProvider`.
- Rejected: adding search to the base provider and forcing unsupported implementations.
- Test seam: recording native provider with pages, errors, and cancellation.

### Strategy: progressive traversal

- Force: some hierarchical sources have only shallow listing but users may explicitly need deeper search.
- Selected: bounded breadth-first strategy with cancellation, visited-key deduplication, partial-result diagnostics, and a bounded expiring continuation store.
- Rejected: unbounded recursion or `Task.WhenAll`, especially because the legacy IPFS flow already exhibits that failure mode.
- Continuation behavior: the first request captures an immutable, globally ordered result snapshot behind a random query-bound token. Later pages read that snapshot without provider calls or dependency on the evictable browse tree. Expired, evicted, revision-mismatched, or cross-query tokens fail as retryable stale cursors.
- Tradeoff: traversal runs through the configured container/item budget before page one is returned. This makes ordering and later pages deterministic, at the cost of a slower first page and bounded in-memory snapshot retention until completion, eviction, or expiry.
- Isolation: traversal pages use an operation-local tree store. A canceled scan can warm neither the session's navigational query nor its continuation cursor, so ordinary browse load-more remains replay-safe after search rollback.
- Test seam: fake hierarchy larger than the budget plus cache eviction, source mutation, cross-query token reuse, expiry, and cancellation.

### Adapter: providers

- Force: filesystem, project, and IPFS APIs use incompatible identifiers and response shapes.
- Selected: provider adapters mapping to repository-owned records.
- Rejected: renderer conditionals on provider names and SDK types in shared contracts.
- Test seam: provider-specific tests and generic session conformance tests.

### Factory/catalog: session composition

- Force: a browser instance needs a validated source catalog and a complete set of search strategies.
- Selected: typed catalog plus narrow session factory/default composition helper.
- Rejected: `IServiceProvider` lookup inside core behavior and a switch over concrete providers.
- Test seam: duplicate/unknown source and missing-strategy negative tests.

Observer-style snapshot events use the ordinary .NET event model; no separate event-bus abstraction is justified.

## IPFS architecture constraint

The current legacy path cannot back true lazy loading:

- `CanDoItAll.IPFS.NodeControl/wwwroot/js/filesExplorer.js` calls the existing file-list API.
- The IPFS engine's `ReadFileSystemNodeAsync` recursively describes child links.
- `DescribeLinkAsync` calls back into `ReadFileSystemNodeAsync`.
- `NodeExplorerWorkflowService` separately walks pinned directory subtrees when identifying visible roots.

Therefore the production adapter is blocked on a shallow/page-aware IPFS API that returns only direct links and an opaque continuation (or another bounded cursor). The sandbox IPFS provider models that target contract; it must not claim that the legacy endpoint is lazy.

The shipped local-filesystem adapter is shallow in hierarchy depth but not a backend-paged filesystem API. For each requested directory it enumerates and captures visible direct-child metadata, computes a directory consistency token, applies deterministic filter/sort, and only then slices the requested page. It never descends into unopened directories. This deliberately favors stable paging and mutation detection; very large single directories still incur one direct-directory scan per browse request. Providers backed by databases, project APIs, or a future IPFS endpoint should use native server-side paging when available.

## BaseLib renderer plan

Component tree:

```text
Host shell                         optional full-page or compact composition
|-- FileBrowserSourceNavigation    source rail or compact selector
`-- FileBrowser                    reusable browser; owns renderer state and session subscription
    |-- FileBrowserToolbar         search, scope, type, recursive toggle, sort, view
    |-- FileBrowserBreadcrumbs     back/forward/up and path navigation
    |-- FileBrowserListView        semantic dense-list projection
    |   |-- FileBrowserItemIdentity
    |   `-- FileBrowserItemActions
    |-- FileBrowserCardView        responsive card projection
    |   |-- FileBrowserCardIdentity
    |   `-- FileBrowserItemActions
    |-- FileBrowserStatusBar       counts, cache/search scope, selection
    `-- BaseLib ContextMenu        action parity for row/card overflow
```

The reusable `FileBrowser` does not own `ThemeHost`, `Layout`, `SideMenu`, `PageScaffold`, or overlay hosts. A host may compose the companion source navigation beside it for a Drive-like full page, omit it in a floating window, or show a compact source picker. The screenshot's left rail informs the full-page sandbox rather than becoming a mandatory component contract.

State flows down through immutable snapshots. User events flow into session methods or up through `EventCallback<T>` for host-owned effects. Action discovery uses the session facade; components do not fetch providers independently.

BaseLib components selected from repository evidence:

- `Stack`, `Grid`, `Cluster`, `Split`, `Layout`, `Body`, and `PageScaffold`
- `Button`, `CopyButton`, `Icon`
- `TextBox`, `DropDown`, `CheckBox`, `FilterBar`
- `LoadingState`, `EmptyState`, `Alert`, `StatusBadge`
- `ContextMenu`, `Tooltip`, and `Notification` in the host

BaseLib's current `DataGrid` owns client paging and has no server-sort contract, so it is not the correct owner for provider continuation. The renderer uses semantic table markup while reusing BaseLib controls and tokens.

The browser root establishes `container-type: inline-size`. Toolbar stacking, metadata visibility, quick-action collapse, density, and card flow use package-local container queries because a narrow Canvas floating window can exist inside a wide desktop viewport. Viewport breakpoints alone are not sufficient.

### Visual thesis

A calm graphite-and-sky enterprise workspace: dense but readable rows, one blue accent for selection/action, small material file icons, quiet dividers, and no dashboard-card mosaic.

### Interaction thesis

- Loading state transitions into content without layout jump.
- Hover, focus, selection, and list/grid changes use short restrained transitions.
- View changes and source changes remain motion-safe under `prefers-reduced-motion`.

## Testability gate

Required proof:

- Unit tests target the tree store, ordering, navigation state, strategies, catalog, and session without Blazor or a real provider.
- Negative tests cover duplicate sources, invalid keys/tokens, unsupported native search, provider errors, traversal budgets, and path escape attempts.
- Filesystem adapter integration tests use an isolated temporary root and never the developer's filesystem outside it.
- Composition smoke creates a session with project, IPFS, and filesystem providers.
- Browser proof covers desktop, mobile, list, grid, search scopes, recursive listing, load more, empty, loading, and error/retry.
- A refreshed CodeAnalytics snapshot proves `Core` has no UI/provider-implementation references and no new project cycle.

## Simpler options rejected

- Put everything in BaseLib: rejected because CanvasLib and non-Blazor consumers would inherit the first renderer.
- One `IFileService` returning recursive trees: rejected because it cannot express paging, provider search, or honest partial state.
- One provider interface with every optional method: rejected because unsupported operations become runtime exceptions.
- Renderer-owned dictionaries and lazy loading: rejected because behavior would be duplicated in the future Canvas renderer and hard to test.
- Generic `object` metadata or SDK DTOs: rejected because contracts would leak infrastructure and lose compile-time safety.
- Treat the legacy IPFS list endpoint as lazy: rejected because current engine behavior recursively walks descendants.
