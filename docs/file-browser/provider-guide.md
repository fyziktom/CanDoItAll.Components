# File Browser Provider Authoring Guide

## Start with source semantics

A provider maps one configured hierarchy into `CanDoItAll.Components.FileBrowser.Core`. It does not render controls and it must not expose SDK objects. Decide these points before implementing:

- What identifies an occurrence? Use a path/link/project-node identity, not a content hash alone.
- What identifies immutable content? Put that in `FileBrowserContentIdentity` with a scheme such as `cid` or `sha256`.
- Can the backend list direct children without recursively reading descendants?
- Is paging stable, and what source revision makes a continuation stale?
- Which metadata is exact, approximate, expensive, or unavailable?
- Which searches are loaded-only, provider-indexed, or explicit bounded traversal?

## Required contract

Implement `IFileBrowserProvider` with three bounded operations:

1. `GetRootAsync` resolves one container root.
2. `GetPathAsync` resolves a root-to-container breadcrumb path without loading sibling content.
3. `BrowseAsync` returns one shallow page for one parent occurrence.

Advertise only behavior the adapter can actually execute in `FileBrowserSourceDescriptor`. Native search, content reads, and custom commands use the separate `IFileBrowserSearchProvider`, `IFileBrowserContentProvider`, and `IFileBrowserActionProvider` contracts.

Renderers reach those optional contracts through `IFileBrowserSession.GetActionsAsync`, `ExecuteActionAsync`, and `OpenReadAsync`. Do not inject a concrete provider into a renderer. The session verifies the active source and loaded occurrence, handles built-in capability-gated actions, serializes provider access, and delegates only custom/provider behavior.

Optional contracts have strict capability and response rules:

- Custom action discovery runs only when both the source and item advertise `CustomActions`.
- Return a non-null action list containing only non-null descriptors. Custom identifiers must not collide with a reserved built-in identifier unless that built-in is already supported by the item; an advertised built-in may still delegate to the provider when it has no URI implementation.
- Advertise `ContentRead` only when `OpenReadAsync` returns a non-null lease with a readable stream. Advertise `RangeRead` as well before accepting a non-zero offset or bounded length.
- Treat every continuation token as an immutable, replay-safe checkpoint. A retry with the same token must describe the same page or fail explicitly as stale; a consumptive stream pointer is not a valid cursor.

```csharp
public sealed class ExampleProvider : IFileBrowserProvider
{
    public FileBrowserSourceDescriptor Descriptor { get; } = new(
        new FileBrowserSourceId("example"),
        "Example source",
        capabilities: FileBrowserSourceCapabilities.PagedBrowse,
        supportedSearchScopes:
        [
            FileBrowserSearchScope.LoadedFolder,
            FileBrowserSearchScope.LoadedDescendants,
            FileBrowserSearchScope.Progressive
        ]);

    public ValueTask<FileBrowserItem> GetRootAsync(
        FileBrowserMetadataRequest metadata,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public ValueTask<IReadOnlyList<FileBrowserItem>> GetPathAsync(
        FileBrowserItemKey itemKey,
        FileBrowserMetadataRequest metadata,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public ValueTask<FileBrowserPage> BrowseAsync(
        FileBrowserBrowseRequest request,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
```

## Paging and consistency rules

- Treat continuation tokens as provider-owned opaque values.
- Bind a cursor to source, parent occurrence, revision, sort, filter, page shape, and requested metadata when those change the result.
- Return a consistency token on the first page and validate it on later pages.
- Report `FileBrowserCompleteness` honestly as the fidelity of the returned data. It is separate from pagination exhaustion: the core considers a container fully cached only when completeness is `Complete`, the accepted page set has no continuation, and it has no error.
- Reject a mismatched or orphaned cursor as retryable `StaleCursor`; never reinterpret it as page one.
- Keep `BrowseAsync` shallow even when a container reports `HasChildren`.
- If recursive project listing is supported, implement the typed `IncludeDescendants` request explicitly and keep the flattened result paged.

The core store deduplicates overlapping pages and rejects append operations when page one is no longer cached. Providers still own cursor integrity.

The core also rejects malformed browse and search responses before changing retained or renderer-visible state: pages larger than requested, foreign-source items, self-parenting, shallow results whose parent is not the requested container, conflicting or repeated result keys, visible parent cycles, impossible totals, consistency-token mismatches, and repeated/non-advancing cursors. `GetPathAsync` must return an exact parentless-root-to-requested-container chain with adjacent parent links, one source, and no repeated key. These checks are a safety net, not permission for an adapter to emit ambiguous data.

## Identity and metadata

`FileBrowserItemKey.Value` is opaque to the core but must be stable for the occurrence. The same CID can appear at `/imports/manual.pdf` and `/knowledge/manual.pdf`; those are different item keys with the same `FileBrowserContentIdentity`.

Set `FileBrowserChildState.Unknown` when determining children would require an expensive read. Use `Empty` only when emptiness is known. Build `FileBrowserMetadataState` honestly so renderers and future preview logic do not mistake a missing value for a verified zero.

Open/download targets must be well-formed `http`/`https` URLs or safe same-host relative targets. The core rejects active/local schemes, protocol-relative URLs, backslashes, control characters, and malformed targets. Hosts must still enforce an origin allowlist before applying any external navigation. For IPFS, expose a trusted HTTP(S) gateway target and keep the CID separately in `FileBrowserContentIdentity`; an `ipfs:` navigation URI is rejected.

## Errors and cancellation

Map backend failures to renderer-safe `FileBrowserErrorCode` values through `FileBrowserProviderException`. Preserve technical detail only in the technical field. Mark only transient failures retryable. Check the cancellation token before I/O and throughout long enumerations; cancellation must propagate as `OperationCanceledException`, not a rendered provider error.

## Filesystem adapter example

The shipped adapter is root-confined and shallow:

```csharp
var provider = new FileSystemFileBrowserProvider(
    new FileSystemFileBrowserOptions(
        new FileBrowserSourceId("workspace"),
        rootPath: @"C:\repositories\MyProject",
        displayName: "Workspace files",
        includeHidden: false,
        followDirectoryReparsePoints: false,
        recommendedPageSize: 50,
        maximumPageSize: 250));
```

It rejects path escape and source mixing, does not follow reparse points by default, binds cursors to directory consistency, and advertises no native or recursive behavior.

The adapter never descends into child directories, but a page request does enumerate all visible direct children and capture the metadata needed to compute the consistency token and deterministic filter/sort before slicing the in-memory page. This is shallow lazy hierarchy loading, not native filesystem I/O paging. Hosts expecting extremely large single directories should account for that scan; database/project/IPFS adapters should preserve backend paging rather than copy this implementation detail.

## IPFS adapter prerequisite

Do not adapt the legacy recursive `ReadFileSystemNodeAsync` flow as if it were lazy. That flow recursively reaches child links through `DescribeLinkAsync` and performs unbounded descendant work with `Task.WhenAll`; the legacy node-explorer workflow also walks pinned subtrees. A production IPFS provider requires a shallow direct-link API with bounded paging or another bounded cursor, honest HAMT/DAG behavior, revision-aware IPNS resolution, partial-link failures, and range-aware content reads. Until that API exists, use the sandbox provider as a contract mock only.

## Provider acceptance tests

Every adapter should test:

- root and deep path resolution;
- shallow direct-child behavior;
- deterministic sort/filter/page combinations;
- cursor replay, cross-query reuse, mutation, and stale revisions;
- invalid/default/foreign keys and containment;
- empty, inaccessible, missing, and partially readable containers;
- cancellation before and during work;
- metadata completeness, child state, and capability honesty;
- provider-specific link, symlink, DAG, or project-boundary policy.
