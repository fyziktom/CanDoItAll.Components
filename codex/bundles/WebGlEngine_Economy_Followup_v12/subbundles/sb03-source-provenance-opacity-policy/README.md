# SB03 — Source provenance opacity policy

## Goal

Prevent raw domain identifiers from leaking through source.* metadata in generic WebGlRunDocument. Domain driver must emit opaque source hashes and write a domain-side trace map artifact.

## Source references

- `C-WGRUN-VALIDATOR`
- `C-DOMAIN-DRIVER`
- `E-BRIDGE-DRIVER`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `provenance-opacity-tests.txt`
- `trace-map-artifact.json`
- `generic-run-document-no-raw-domain-ids.txt`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
