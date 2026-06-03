# SB01 — Current-state, pause reproducer, and proof integrity baseline

Priority: P0
Related findings: F01,F02,F09

## Objective

Audit latest pushed code, reproduce pause bug on /run-playback/performance route, capture failing-first proof, enumerate empty/weak transcripts.

## Required implementation rules

- Keep Components generic and domain-neutral.
- Prefer small cohesive changes over broad rewrites.
- Add or update tests before claiming the gate is closed.
- Capture failing-first proof for every P0/P1 behavioral bug.
- Update proof manifest and semantic invariants.

## Acceptance gate

No implementation until a failing-first pause/browser proof and proof-hygiene inventory exist.

## Required proof artifacts

- `proof/SB01/manifest.md`
- `proof/SB01/semantic-invariants.md`
- `proof/SB01/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant.
- Run focused tests for changed area.
- Run boundary audits.
- Run browser proof for playback/UI changes.
- Ensure no blank transcripts.
