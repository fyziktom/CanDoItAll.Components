# SB10 — Proof integrity validator

Priority: P1
Related findings: F09
Status: Completed

## Objective

Add validator that rejects empty transcripts, screenshot-only browser proof, missing assertions, stale package feeds, and missing failing-first proof for critical subbundles.

## Required implementation rules

- Keep Components generic and domain-neutral.
- Prefer small cohesive changes over broad rewrites.
- Add or update tests before claiming the gate is closed.
- Capture failing-first proof for every P0/P1 behavioral bug.
- Update proof manifest and semantic invariants.

## Acceptance gate

Bundle cannot close with zero-byte/blank transcripts or browser screenshots without JSON assertions.

## Required proof artifacts

- `proof/SB10/manifest.md`
- `proof/SB10/semantic-invariants.md`
- `proof/SB10/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant.
- Run focused tests for changed area.
- Run boundary audits.
- Run browser proof for playback/UI changes.
- Ensure no blank transcripts.

## Completion summary

Gate result: Passed. `scripts/validate_bundle.py` now rejects completed proof with blank transcripts, screenshot-only browser proof, invalid/missing browser assertion JSON, missing P0/P1 failing-first evidence, stale package/feed markers, or missing source-assertion transcripts while still allowing unfinished future subbundles during prepared-stage validation.

- `bundle://proof/SB10/manifest.md`
- `bundle://proof/SB10/semantic-invariants.md`
- `bundle://proof/SB10/transcripts/failing-first-proof-validator-gap.txt`
- `bundle://proof/SB10/transcripts/proof-validator-unit-tests.txt`
- `bundle://proof/SB10/transcripts/bundle-validator-after-hardening-pre-docs.txt`
- `bundle://proof/SB10/transcripts/source-assertion-proof-validator-scan.txt`
