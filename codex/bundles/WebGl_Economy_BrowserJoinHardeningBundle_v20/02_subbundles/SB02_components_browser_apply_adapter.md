# SB02 - Components generic browser apply adapter

## Status

Completed. Entry and closure gates passed.

## Goal

Provide a generic bridge from `WebGlRunFrameApplyResult` to actual `WebGlSceneView`/JS runtime calls.

## Tasks

- Add a generic adapter in `CanDoItAll.Components.WebGlRunLib` or `WebGlLib`, without Economy references.
- It must accept a `WebGlRunFrameApplyResult`.
- It must apply:
  - scene reset if required,
  - patches,
  - motions,
  - command batches/stages,
  - stage barriers.
- It must return a typed result with:
  - applied frame index,
  - stage count,
  - motion count,
  - patch count,
  - runtime diagnostic snapshot.

## Acceptance

- Unit tests prove the adapter converts a frame apply result into expected runtime calls using a fake runtime interface.
- No Economy references in Components.
- No browser UI work in Components beyond generic primitives.

## Prerequisites

- SB01 completed or explicitly blocked with safe branch/boundary baseline.
- Components source references still match `07_references/source_references.md`.

## Owned Requirements

- R02 Browser apply adapter.

## Dependency Impact

This is the Components foundation for SB03, SB04, SB05, and SB11. A fake adapter or domain-specific adapter would invalidate all later browser smoke proof.

## Validation Depth

Unit tests must use a fake runtime interface and verify actual calls/counts for reset, patches, motions, command stages, barriers, and diagnostics.

## Proof Required

- `dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj`
- `bundle://proof/SB02/transcripts/webglrunlib-browser-adapter-tests.txt`
- `bundle://proof/SB02/transcripts/source-assertions.txt`
- `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- `bundle://proof/SB02/manifest.md`
- `bundle://proof/SB02/semantic-invariants.md`

## Browser Validation Logging

N/A for Components only. Browser-visible proof is downstream SB05/SB11.

## Semantic Adequacy Gate

- Shallow-pass trap: adapter returns counts without invoking a runtime.
- Adversarial negative proof: fake runtime surfaces failed diagnostic/error state and the adapter reports failure.
- Semantic positive proof: fake runtime call log proves reset, patches, motions, stages/barriers, and diagnostic snapshot are applied from a real `WebGlRunFrameApplyResult`.
- Anti-stub audit: Components contains no Economy terms and no TODO/NotImplemented adapter paths.

## Progression Gate

Pass only when typed adapter API and fake-runtime tests prove behavior. SB03 and Economy UI work must re-check this proof before relying on it.
