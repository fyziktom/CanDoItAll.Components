# Current-State Analysis

## Cross-repo observations

`CanDoItAll.Components` already contains a meaningful split between `WebGlLib` and a newer `WebGlRunLib` project. The solution includes both `src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj` and `src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj`, plus test projects for both. This means the desired higher layer is no longer just a future idea; Codex must audit what has already landed before moving code.

`WebGlLib` currently owns generic scene models, assets, symbols, interaction, patching, document serialization, diagnostics and browser runtime modules. Its README still states that simulation clocks, run lifecycle, pathfinding, physics, persistence providers, economy rules and domain semantics belong outside WebGlLib.

`CanDoItAll.Economy` already contains `CanDoItAll.Economy.Simulation.WebGlBridge`. The project references `CanDoItAll.Components.WebGlRunLib` by local project reference when `ComponentsRepoRoot` is configured and by package reference when `UseComponentsWebGlRunLibPackage=true`.

## Important existing strengths

- Typed C# scene contracts exist.
- JS runtime is modular and uses Three.js as a pragmatic rendering layer.
- Scene document serialization/hashing and tests already exist.
- Economy bridge already preserves source metadata for frames/actions in several places.
- Runtime diagnostics and browser proof artifacts exist from earlier work.
- The architecture has already started moving run semantics into `WebGlRunLib`.

## Confirmed or likely gaps to close

1. JS runtime import safety needs a hard audit. One inspected file calls `resolveObjectPosition` without showing it in the import list.
2. JS patching needs preflight/transactional behavior. It currently mutates before full validation in several paths.
3. Transform-only patching appears to still call `rebuildScene` after patch application, which contradicts the desired incremental update behavior.
4. Texture ownership is too coarse. Owned cloned materials may still reference shared template textures.
5. Revision semantics are split between `Scene.Revision` and `UiState.Revision`.
6. Layer membership can go stale when objects are removed.
7. JS diagnostics are broader than the C# diagnostics DTO.
8. `WebGlRunLib` exists, but its exact boundary and proof must be audited against the current code, not assumed.
9. Economy bridge must remain a consumer, not a backdoor for adding economy-specific semantics to Components.
10. Browser proof must be expanded from a demo proof into a hardening proof that includes stress, negative cases, repeated dispose/import and cross-repo consumption.

## Current-state source list

See `inputs/repository-source-references.md`.
