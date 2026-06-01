# Assumptions, Risks and Reopen Triggers

## Assumptions

- Codex can access both repositories locally.
- `CanDoItAll.Components` should be checked out on `webgl-engine`.
- `CanDoItAll.Economy` should use its current working branch unless the operator specifies another branch.
- Both repositories target `net10.0`.
- The implementation agent can run `dotnet`, `npm`, browser/Playwright proof, and local package feed operations.
- Some observations may already be partially fixed. SB01 must update the bundle current-state notes before editing code.

## Critical Path Risks

| Risk | Why it matters | Mitigation |
| --- | --- | --- |
| WebGlLib becomes too heavy | Simple apps should still use it for lightweight 3D model/scene visualization | SB07 hard boundary refactor gate |
| JS runtime passes build but fails in browser | Vanilla JS module reference errors are not caught by C# build | SB02 static import/export audit plus browser proof |
| Patch failure leaves partial state | C# model, JS model and proof snapshots diverge | SB03 transactional patching |
| Transform patches still rebuild whole scene | Large simulations become too slow | SB04 incremental render diagnostics and stress proof |
| Texture disposal breaks shared GLB models | Removing one instance can corrupt other instances | SB05 ownership model |
| WebGlRunLib overfits Economy | Future production-line simulator cannot reuse it cleanly | SB08 generic fixture and domain-leak scan |
| Economy bridge hides bad mappings with fallbacks | Simulations look like they work but render wrong objects/actions | SB10 strict mode negative tests |
| Package/project-reference split breaks | Parallel repo development becomes fragile | SB12 package and project-reference proof |
| Browser proof becomes shallow | Screenshots/logs are attached but not reviewed | SB13 explicit visual and diagnostic QA questions |

## Validation Risks

- Unit tests alone cannot prove browser render lifecycle or WebGL resource disposal.
- Browser proof alone cannot prove patch transaction semantics or source provenance.
- Positive fixture tests alone cannot prove genericity.
- Stress proof without diagnostics can hide full-scene rebuilds.
- A package build can pass while hidden project references or circular assumptions remain.

## Reopen Triggers

Reopen the owning earlier subbundle if any downstream work discovers:

- a `WebGlLib` reference to `WebGlRunLib`, Economy, production-line, or simulator-specific namespace;
- a failed patch that changes scene object/link counts or revision;
- a transform-only patch that increments full rebuild diagnostics;
- a disposed material/texture causing another GLB instance to become invisible or throw;
- a fallback mapping used in strict Economy mode;
- a WebGlRunLib API that includes economy-specific names;
- a package-mode build that requires local project references;
- browser console errors not classified in the proof manifest;
- any critical subbundle without proof manifest, semantic invariants, changed-file hashes, negative proof and positive proof.
