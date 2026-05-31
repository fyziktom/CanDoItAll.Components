# SB04 â€” Components C# / JS batch normalizer parity

Repository scope: Components
Priority: High

## Goal

Add golden JSON fixtures that compare C# WebGlSceneCommandBatchNormalizer and JS normalizeCommandBatch for ordering, stages, duplicates, patch coalescing, and metrics.

## Entry gate

- Confirm current branch in every touched repository.
- Confirm no new branch is created.
- Re-read source files touched by this subbundle.
- Identify prerequisite subbundles and gate status.

## Implementation notes

- Keep abstractions generic.
- Do not add WebGL references to Economy projects.
- Do not add Economy/domain vocabulary to Components runtime.
- Do not build the shared-well demo.
- Keep WebGL large-screen only.

## Required proof

- `proof/SB04/manifest.md`
- `proof/SB04/semantic-invariants.md`
- changed-file hashes for all production files touched
- command transcripts for build/test/audit commands
- source assertions showing the implementation is not a stub


## Semantic Adequacy Gate

Before closure, name the shallow-pass trap, record adversarial negative proof, record semantic positive proof, run an anti-stub audit, and preserve raw-note closure wording without narrowing all/every/must/same-flow language.
## Closure checklist

- Tests pass.
- Boundary audit passes.
- Execution report row updated.
- Any partial work creates a concrete follow-up item.

