# SB06 — Store resolution and transfer ambiguity hardening

## Repository scope

Economy

## Goal

Remove hidden first-match/fallback store selection from research-mode economic behavior.

## Tasks

- Introduce store resolution policy with explicit strategies.
- Record source resolution reason in every flow metadata.
- Fail in strict mode if multiple candidate stores match without declared resolution.
- Add tests where two stores share owner/resource and scenario must choose explicitly.
- Add diagnostics for capacity rejection, stock rejection, and zero accepted transfer.

## Acceptance criteria

- Ambiguous store resolution is impossible in research-ready mode.
- Every flow can explain why source/target stores were selected.
- Transfer conservation/inventory changes are oracle-testable.

## Required proof artifacts

- `proof/SB06/transcripts/store-resolution-policy-tests.txt`
- `proof/SB06/artifacts/flow-resolution-sample.json`

## Gate

No economic result can depend on incidental StoreId ordering.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.
