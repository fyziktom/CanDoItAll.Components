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

## Execution status

Completed.

Implemented in Economy:
- Store-resolution metadata recording for resolved source/target/shared/effect roles.
- Flow metadata propagation for `storeResolution.<role>.*`.
- Strict/research zero-accepted transfer diagnostics after full rejection.
- Accepted and rejected flow provenance that preserves policy, store id, reason, and candidate count.

Proof captured:
- Failing-first metadata/diagnostic proof: `proof/SB06/transcripts/store-resolution-policy-tests-failing-first.txt`.
- Focused store-resolution policy test proof: `proof/SB06/transcripts/store-resolution-policy-tests.txt`.
- Broader trust-hardening regression proof: `proof/SB06/transcripts/store-resolution-hardening-tests.txt`.
- Flow sample export proof: `proof/SB06/transcripts/flow-resolution-sample-export.txt`.
- Machine-readable accepted/rejected flow sample: `proof/SB06/artifacts/flow-resolution-sample.json`.

## Gate

No economic result can depend on incidental StoreId ordering.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.
