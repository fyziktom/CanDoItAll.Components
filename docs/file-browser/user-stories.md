# File Browser User Stories

## Personas

- CanDoItAll user browsing project files and resources.
- Agent or application author opening a browser scoped to a project-structure node.
- Storage operator browsing IPFS roots or known hashes.
- Developer embedding the BaseLib renderer or a future CanvasLib renderer.
- Provider author integrating a new hierarchy or search backend.

## Navigation and loading

### US-01: Browse a provider root

As a user, I can choose an available source and see only its first page so opening a large source never loads its complete hierarchy.

Acceptance criteria:

- Source identity and root item are provider-owned.
- The first request carries a bounded page size and no continuation token.
- The UI differentiates initial loading, loaded, empty, and failed states.
- Unsupported sources or duplicate source identifiers fail during composition, not during a click.

### US-02: Navigate folders and virtual containers

As a user, I can open folders, projects, subprojects, IPFS directories, and other virtual containers using the same interaction.

Acceptance criteria:

- Item identifiers are opaque to the core and stable within a source.
- Double-click and Enter navigate a container.
- Breadcrumb, back, forward, and up navigation work.
- A host can initialize the session at a deep item, which enables a floating window scoped from a project-structure node.
- Revisiting an unchanged, fully cached container does not call the provider again.

### US-03: Load additional pages

As a user, I can request more items when a container has a continuation token.

Acceptance criteria:

- Additional items append without duplicates.
- The provider continuation token remains opaque.
- Completeness and consistency state remain visible after each accepted page.
- Concurrent or repeated requests do not corrupt ordering or selection.
- A failed additional page preserves the already loaded items and offers retry.

### US-04: Refresh intentionally

As a user, I can refresh the active container without discarding unrelated visited containers.

Acceptance criteria:

- Refresh replaces the active query result from page one.
- Stale selections that no longer exist are removed.
- The cache has an explicit bounded retention policy.

## Project and resource scenarios

### US-05: Browse CanDoItAll projects lazily

As a CanDoItAll user, I see projects as virtual folders. Opening a project loads only that project's direct files and direct subproject nodes.

Acceptance criteria:

- Subprojects appear as navigable containers.
- Files inside unopened subprojects are not fetched.
- Project-local storage and IPFS-backed resource containers can appear beside ordinary project files.
- Provider metadata can expose owner, project kind, origin, and logical path without leaking CanDoItAll implementation types into the core.

### US-06: Include descendant project files

As a user, I can opt into a flattened view of a project including its subprojects when the source supports recursive listing.

Acceptance criteria:

- The option is shown only for sources that advertise recursive listing.
- The option is passed as a typed browse query flag.
- The provider still pages results; the core never assumes recursion is small.
- Relative paths disambiguate files with the same name.

### US-07: Embed in a floating workbench window

As an application author, I can render the browser without its source rail and initialize it at a supplied item key.

Acceptance criteria:

- The compact renderer uses the same session contracts as the full-page renderer.
- The core has no dependency on BaseLib, CanvasLib, OverlayLib, Blazor, JavaScript, or a host application.
- The future CanvasLib renderer can subscribe to immutable session snapshots rather than reimplement browsing behavior.

## IPFS scenarios

### US-08: Browse IPFS folders and hashes

As a storage user, I can browse a configured IPFS root, node, or list of known hashes and see content type, size, CID, and path where available.

Acceptance criteria:

- A directory listing is shallow and page-aware at the source API; wrapping an eager recursive API does not count as lazy loading.
- CIDs and links remain provider metadata or opaque IDs.
- Files and directories sort predictably, including type sorting.
- Gateway/open/download actions appear only when supported.
- Unavailable content, malformed DAG nodes, and gateway failures produce retryable or terminal typed errors.

### US-09: Avoid the legacy eager IPFS traversal

As an operator, opening one IPFS directory must not recursively materialize its descendant DAG.

Acceptance criteria:

- The future IPFS adapter uses a new shallow/page-aware endpoint.
- The existing `ListFileAsync` / `file/ls` recursive behavior is documented as incompatible with production lazy loading.
- Search that traverses IPFS is bounded or native; it is never an unbounded `Task.WhenAll` walk.

## Search, filter, and sort

### US-10: Search only the loaded folder

As a user, I can filter the currently loaded folder instantly without a provider call.

Acceptance criteria:

- Name and logical path matching are case-insensitive by default.
- Type filters and sort apply to the result.
- Clearing search restores the current browse page and selection rules.

### US-11: Search loaded descendants

As a user, I can search only the portion of the hierarchy already present in the cache.

Acceptance criteria:

- No provider call occurs.
- Results clearly say that scope is limited to loaded content.
- Cache traversal is cycle-safe and deduplicated.

### US-12: Use provider-native deep search

As a user, I can search a full project or IPFS source when the provider has an efficient native search API.

Acceptance criteria:

- Native search is an optional provider interface and advertised capability.
- Query, filter, sort, page size, and continuation token remain typed and bounded.
- Search results retain stable item keys and display paths so users can navigate to their container.

### US-13: Progressively search a hierarchy

As a user, I can explicitly request an on-the-fly traversal when native search is unavailable.

Acceptance criteria:

- Traversal is breadth-first, cancellable, cycle-safe, and constrained by maximum containers and items.
- The result reports scanned counts and whether it is partial.
- A budget-limited result never pretends to cover the whole source.
- Additional result pages come from a bounded retained snapshot, so tree-cache eviction or source mutation cannot silently change an in-progress result set.
- An expired, evicted, or cross-query continuation fails as a retryable stale cursor.

### US-14: Sort and filter consistently

As a user, I can sort by name, modified time, size, type, owner, or path and filter by item category.

Acceptance criteria:

- Providers receive sort and filter instructions for browse and native search.
- Loaded-only strategies use the same deterministic comparer.
- Folders-first is explicit and stable, with item key as the final tie-breaker.

## Selection and actions

### US-15: Select and invoke items

As a user, I can select an item, open a container, or invoke a file without confusing those states.

Acceptance criteria:

- Single click selects; double-click or Enter invokes.
- Container invocation navigates by default.
- File invocation is raised to the host as a typed event.
- The component never guesses how a host application opens an editor or floating window.

### US-16: Use compact file actions

As a user, I can copy a logical path, copy a distinct content identity such as a CID, open in a new tab, download, refresh, or use provider/host actions when available.

Acceptance criteria:

- Small icon actions have accessible names and tooltips or equivalent labels.
- Copy path uses BaseLib's copy affordance.
- Copy path and copy content ID are separate commands; a CID is never mislabeled as a path.
- Open-in-new-tab and download require a provider/host link or callback capability.
- Unsupported actions are absent rather than failing after selection.
- The row/card overflow menu exposes the same action set as inline actions.

## Operational quality

### US-17: Recover from failures

As a user, I retain context when a source, page, or search request fails.

Acceptance criteria:

- Cancellation is not rendered as an error.
- Errors have stable codes, safe user messages, retryability, and optional technical detail for diagnostics.
- Loading-more failures preserve prior rows.
- A failed source, location, history, sort/filter, or search transition restores the previously published session state instead of exposing a mixed location and result set.
- Retry repeats the exact failed command, including its target source, continuation, and query inputs.

### US-18: Use the browser with keyboard and narrow screens

As a user, I can operate the browser on desktop and mobile without clipped controls or inaccessible rows.

Acceptance criteria:

- Search, source, scope, filter, sort, view toggle, breadcrumbs, rows/cards, actions, and load-more are keyboard reachable.
- Focus is visible.
- The source rail becomes a compact mobile selector.
- Low-priority metadata hides or reflows before primary identity and actions.
- Context menus and overlays remain within the viewport.

### US-19: Validate extension architecture

As a provider author, I can add a provider without editing the core runtime or renderer.

Acceptance criteria:

- Provider SDK types stay in adapter projects.
- A fake provider proves paging, errors, and native search without filesystem, network, or credentials.
- Composition tests prove providers and search strategies are discovered through typed catalogs.
- The core and provider tests include negative and cancellation cases, not only non-null assertions.

## Deliberate first-release exclusions

- Upload, create folder, rename, move, delete, sharing, and permissions editing.
- Drag-and-drop, thumbnail generation, inline preview, and bulk commands.
- Offline synchronization and persisted cross-device preferences.
- A production CanDoItAll project adapter and production IPFS adapter; this repository ships their contracts and realistic mocks. The IPFS adapter additionally depends on a new shallow IPFS API.
- CanvasLib rendering. The boundary is created now; the renderer is a later package.
