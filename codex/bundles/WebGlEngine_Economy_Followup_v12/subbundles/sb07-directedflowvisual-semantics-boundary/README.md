# SB07 — DirectedFlowVisual semantics boundary

## Goal

Document and enforce that DirectedFlowVisual is a visual-only generic flow, not economic resource movement. Economy driver owns all resource/economic meaning.

## Source references

- `C-WGRUN-ACTIONKINDS`
- `E-BRIDGE-DRIVER`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `directed-flow-boundary-tests.txt`
- `generic-action-docs.md`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
