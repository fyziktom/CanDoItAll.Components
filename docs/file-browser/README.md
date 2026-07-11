# File Browser Initiative

This folder is the durable architecture and delivery record for the provider-neutral CanDoItAll file browser.

The first renderer uses BaseLib. A later CanvasLib renderer must reuse the same core session, provider, cache, search, and action contracts without referencing the BaseLib renderer.

## Documents

- [User stories and scope](user-stories.md)
- [Architecture and decisions](architecture.md)
- [Phased implementation plan](implementation-plan.md)
- [Provider authoring guide](provider-guide.md)
- [Testing and verification](testing.md)

## Working status

| Phase | Status |
|---|---|
| Discovery and architecture | Complete |
| Core contracts and runtime | Complete |
| Provider implementations and mocks | Complete for FileSystem and sandbox sources; production project/IPFS adapters intentionally deferred |
| BaseLib renderer | Complete |
| Dedicated sandbox | Complete |
| Verification and documentation | Complete |

## Evidence baseline

- Branch: `file-explorer`
- Initial CodeAnalytics snapshot: `snap-20260710213317-b9914504`
- Snapshot health: 18 projects, 392 documents, 678 types, 4,836 members, no project-level dependency cycle.
- Final focused snapshot: `snap-20260711025115-c15334bd` — 4 projects, 54 documents, 116 types, 909 members, no cycle, no error finding, and no blocking diagnostic.
- The final snapshot's two diagnostics are duplicate compiler-generated attribute display names. Its three warning findings are size heuristics for the renderer coordinator, session coordinator, and tree store; dependency direction remains clean.
- The Components MCP was attempted repeatedly during discovery, implementation, and closure; the root session returned `Transport closed`. Source components and sandbox examples were used as the documented fallback, while a separate audit session confirmed the canonical BaseLib shell and asset composition.

Local build, test, browser, and NuGet archive proof is complete. Public package publication remains subject to the repository-wide release checklist, including the unresolved final license file. The production IPFS adapter remains intentionally unshipped until a shallow/page-aware IPFS API exists.

## Design promise

The file browser is not a filesystem wrapper. It is a reusable browsing runtime over opaque provider items. Local files, CanDoItAll projects and subprojects, project resources, IPFS roots, and future stores plug into the same bounded lazy-loading and search model.
