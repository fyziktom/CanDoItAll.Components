# File Browser Implementation Plan

This is a lightweight phase record inspired by CanDoItAll bundles. It intentionally avoids the older hard-gate/proof ceremony while retaining durable scope, dependencies, checkpoints, and test expectations.

## Dependency map

```text
P1 Architecture and contracts
  -> P2 Tree store, navigation, ordering, session
       -> P3 Search strategies
       -> P4 FileSystem provider
       -> P5 BaseLib renderer
            -> P6 Dedicated sandbox and realistic providers
                 -> P7 Browser, architecture, packaging, and docs closure
```

P1-P3 are critical foundations. UI work cannot change their dependency direction to solve a rendering problem.

## P1: Contracts and boundaries

Deliver:

- Core and test projects.
- Opaque item identity, item/source capabilities, queries, pages, snapshots, typed errors.
- Base provider, optional native-search provider, search strategy, catalog, and session interfaces.
- Architecture docs and public XML documentation.

Checkpoint:

- Core builds with no Blazor, BaseLib, CanvasLib, filesystem-provider, IPFS, or host reference.
- Contract tests reject invalid and ambiguous state.

## P2: Lazy tree and navigation runtime

Deliver:

- Query-aware paged tree store with deduplication and bounded retention.
- Deterministic comparer shared by loaded search and provider mocks.
- Navigation history and breadcrumb state.
- Session initialization, source switch, navigate, load more, refresh, sort/filter/recursive changes, selection, and immutable snapshots.
- Atomic renderer-visible command transitions, complete state/history checkpoints, exact failed-command retry, and disposal-safe cancellation.
- Strict provider path/page conformance checks before state mutation.
- Browse completeness and consistency propagation into snapshots.

Checkpoint:

- Revisit uses cache; refresh reloads only active query; any failed transition restores the prior stable view; load-more failure preserves rows.
- `RetryAsync` repeats the exact failed source/location/query/page command rather than reconstructing it from current state.
- No type becomes a broad manager or service locator.

## P3: Search strategies

Deliver:

- Loaded-folder strategy.
- Loaded-descendant strategy.
- Native-provider strategy.
- Bounded progressive traversal strategy with retained immutable continuation snapshots.
- Strategy catalog/coordinator and search diagnostics.

Checkpoint:

- Tests prove loaded scopes make no provider calls.
- Traversal is cycle-safe, cancellable, budgeted, and marks partial results.
- Continuations survive tree-cache eviction and source mutation, reject cross-query/expired tokens, and make no provider call after page one.
- The documented tradeoff is a slower first page because the bounded traversal and global ordering complete before it is returned.

## P4: FileSystem provider

Deliver:

- Dedicated provider project and README.
- Configured root, shallow page listing, metadata, path resolution, filtering, sorting, and hidden-entry policy.
- Root-containment and traversal defenses.
- Isolated integration tests using temporary directories.

Checkpoint:

- Provider cannot resolve or enumerate outside its configured root.
- Provider project depends inward only on Core.
- "Shallow" means no descendant traversal. Stable sorting and consistency still require a full visible direct-child scan before the in-memory page slice; this is not claimed as native filesystem I/O paging.

## P5: BaseLib renderer

Deliver:

- Razor class library and package README.
- Decomposed component tree from `architecture.md`.
- List and grid modes, source rail/mobile selector, breadcrumbs, search modes, filters, sort, include descendants, status, loading/empty/error, load more.
- Compact accessible inline actions and BaseLib context menu.
- Host callbacks for file invocation and actions.
- Session-backed custom action discovery and capability-checked built-in/provider action execution; host callbacks apply returned URI/clipboard/editor effects.
- Dedicated style asset built from repository Tailwind conventions.
- Pure UI projection tests for icon, size, action, and display state logic.

Checkpoint:

- Renderer references Core and BaseLib only.
- No child component calls a provider.
- Main coordinator remains cohesive; code-behind partials are used only for cohesive Razor component logic.

## P6: Dedicated sandbox

Deliver:

- Interactive Server sandbox with `ThemeHost`, full-height `Layout`, one `Body` scroll/workspace owner, and overlay hosts.
- Mock project provider with subprojects, paging, recursive listing, mixed file-storage/IPFS resources, and native search.
- Mock IPFS provider with shallow pages, CIDs, known-hash roots, native search, and error states.
- FileSystem provider scoped to a safe sandbox root.
- Empty and retryable-error scenarios, with real operation telemetry making loading transitions observable.
- Provider call telemetry visible in the sandbox for proof.

Checkpoint:

- Mock behavior uses the real provider contracts rather than seeding session snapshots.
- The sandbox proves both full-page and compact/floating-window compositions.

## P7: Closure

Deliver:

- Scoped builds and tests, then a clean full-solution build and the CI release-focus test matrix.
- Playwright CLI proof at large desktop and narrow mobile viewports.
- Open context-menu proof and interaction screenshots under `output/playwright/`.
- Refreshed CodeAnalytics dashboard/dependencies/findings and architecture review.
- Solution, CI, packaging, root docs, release checklist, and component catalog updates.
- Provider authoring guide, IPFS integration gap, and test report.

Checkpoint:

- No project cycle or forbidden dependency.
- All FileBrowser, shared BaseLib, and CI release-focus tests pass and browser states agree with the documented user stories.
- Any production IPFS adapter remains explicitly unshipped until the shallow API exists.

## UI decomposition record

| Component | State owned | Main inputs/events | Target size |
|---|---|---|---|
| `FileBrowser` | view mode, search debounce, menu coordinates and discovered menu actions; session subscription | session; snapshot/view/item/action callbacks | reusable work-area coordinator |
| `FileBrowserSourceNavigation` | none | sources/current source; source changed; composed beside the work area | source rail |
| `FileBrowserToolbar` | search input draft | snapshot/options; query/sort/filter/view events | toolbar projection |
| `FileBrowserBreadcrumbs` | none | breadcrumb/history; navigation events | path/history projection |
| `FileBrowserListView` | none | items/selection/supported sorts; select/invoke/action events | dense semantic list projection |
| `FileBrowserCardView` | none | items/selection; select/invoke/action events | responsive card projection |
| `FileBrowserItemActions` | JS menu anchor module lifecycle | item capabilities/custom actions; action/menu events | shared inline/overflow actions |
| `FileBrowserItemIdentity` | none | item | list identity/metadata projection |
| `FileBrowserCardIdentity` | none | item | card identity/metadata projection |
| `FileBrowserStatusBar` | none | snapshot | completeness/count/search diagnostics |

## Final raw-request closure checklist

- [x] Separate reusable C# logic and BaseLib renderer projects.
- [x] Architecture supports a later CanvasLib renderer.
- [x] Standard filesystem, project/subproject, project-resource, and IPFS scenarios represented.
- [x] Lazy page loading and retained loaded structure implemented.
- [x] Include-subprojects option implemented only where supported.
- [x] Multiple search strategies implemented and visible.
- [x] Sorting and type filtering implemented.
- [x] Compact copy/open-new-tab/download actions implemented by capability.
- [x] Dedicated sandbox with project/IPFS/filesystem/empty/retryable-error scenarios implemented.
- [x] Final build/test counts, refreshed architecture snapshot, and latest desktop/mobile browser re-proof recorded at closure.
- [x] Consumer and provider documentation completed.
