# SB08 — Simple simulation mutation split

## Goal

Refactor SimpleSimulationStateTransitionEngine.Mutations into StoreResolutionService, TransferEngine, RejectionPolicy, FlowFactory, and StoreResolutionMetadataPolicy.

## Source references

- `E-MUTATIONS`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `mutation-split-build.txt`
- `mutation-focused-tests.txt`
- `line-count-audit.txt`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
