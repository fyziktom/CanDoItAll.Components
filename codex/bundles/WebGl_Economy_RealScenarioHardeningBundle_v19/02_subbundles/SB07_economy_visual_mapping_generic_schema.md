# SB07 - Economy visual mapping generic schema

Codex must ensure visual mapping remains renderer-neutral in abstractions.

Renderer-neutral:

- visual category
- action kind
- pose key
- symbol category
- anchor alias
- relationship category

WebGL-specific:

- concrete WebGL asset id
- WebGL fallback object id
- WebGL symbol asset id
- WebGL anchor key if it is runtime-specific

If WebGL-specific fields remain in abstraction for now, mark them as bridge-bound and add a follow-up note to split them later.

## Status

Completed.

## Goal

Preserve renderer-neutral Economy abstractions and clearly isolate any remaining WebGL-specific fields as bridge-bound follow-up work.

## Prerequisites

- SB06 bridge refactor gate must pass.
- Boundary policy must be re-read before source edits.

## Owned Requirements

- R07 Renderer-Neutral Mapping.

## Dependency Impact

Protects SB08 and later sandbox composition from generic abstraction leakage.

## Validation Depth

Boundary audit plus source scan for project references and domain/runtime coupling.

## Proof Required

- Economy boundary audit transcript.
- Source scan transcript for renderer-specific fields and domain leakage.
- Proof manifest.

## Progression Gate

Pass only when generic abstractions remain renderer-neutral or every remaining WebGL field is explicitly marked bridge-bound with a follow-up.
