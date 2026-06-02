# Scene Revision Policy

Status: Accepted during SB03 execution

## Decision

`WebGlSceneModel.Revision` is the canonical scene content and patch revision.

`WebGlSceneUiState.Revision` is a compatibility/runtime mirror for patch results and exported browser scenes. Patch reducers in C# and JavaScript resolve the current revision from `Scene.Revision` first and fall back to `UiState.Revision` only for older payloads where the top-level value is absent or zero.

Successful mutating patches commit exactly one next revision and mirror that value into `UiState.Revision`. Failed patches do not mutate objects, links, layers, diagnostics that describe successful mutation, or either revision.

## Hash Policy

`WebGlSceneDocument.SceneContentHash` includes `Scene.Revision` because revision is part of scene content identity.

`SceneContentHash` ignores `UiState.Revision`, hover state, and selection state. These are runtime UI state, not scene content.

`WebGlSceneDocument.DocumentHash` still covers the normalized document as saved. When `IncludeUiState` is true, a UI-only revision drift can change `DocumentHash` without changing `SceneContentHash`.

## Patch Transaction Policy

Patch validation must run before mutation for failure modes that can invalidate the whole patch, including:

- Scene id mismatch.
- Strict base-revision mismatch.
- Missing add-object ids.
- Object patches that target objects removed by the same patch or absent after add/remove resolution.
- Added links with missing endpoints, unless the patch explicitly requests warning mode with `missingLinkEndpointMode=warn`.

Object removal also removes links that reference the object and removes the object id from scene layers. This keeps runtime graph, model links, layers, and revision accounting consistent.

## Boundary

This policy belongs to `WebGlLib` because it defines generic scene mutation and save/load semantics. It does not introduce run clocks, stage semantics, Economy concepts, production-line concepts, persistence providers, or domain provenance rules.

## Proof

- C# reducer/document tests: `proof/SB03/transcripts/passing-dotnet-patch-document-revision.txt`
- Browser patch transaction proof: `proof/SB03/transcripts/passing-browser-bad-link-transaction-proof.json`
- Runtime import parity audit: `proof/SB03/transcripts/passing-audit-scene-runtime-imports.txt`
- Runtime graph audit: `proof/SB03/transcripts/passing-audit-scene-runtime.txt`
