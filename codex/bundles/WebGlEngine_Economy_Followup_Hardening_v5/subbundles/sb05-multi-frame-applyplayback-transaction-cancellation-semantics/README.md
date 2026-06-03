# SB05 — Multi-frame ApplyPlayback transaction/cancellation semantics

Priority: P1
Related findings: F05
Status: Completed
Completed: 2026-06-03

## Objective

Harden ApplyPlaybackAsync for mid-stream cancellation, reset failure, frame failure, and partial application reporting. Add explicit transaction policy and failure snapshot.

## Required implementation rules

- Keep Components generic and domain-neutral.
- Prefer small cohesive changes over broad rewrites.
- Add or update tests before claiming the gate is closed.
- Capture failing-first proof for every P0/P1 behavioral bug.
- Update proof manifest and semantic invariants.

## Acceptance gate

Failures report target frame, last applied frame, cancellation reason, and do not continue applying later frames.

Gate result: Passed. Focused tests prove frame failure stops later frames and reports target/last-applied/failed-frame metadata; cancellation during frame 2 records cancellation reason and does not apply frame 3.

## Required proof artifacts

- `proof/SB05/manifest.md`
- `proof/SB05/semantic-invariants.md`
- `proof/SB05/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant. Completed: `proof/SB05/transcripts/components-build-after-applyplayback-transaction.txt`.
- Run focused tests for changed area. Completed: `proof/SB05/transcripts/webglrunlib-focused-tests.txt`.
- Run boundary audits. Completed: `proof/SB05/transcripts/source-assertion-applyplayback-transaction-scan.txt` and `proof/SB05/transcripts/domain-neutrality-scan.txt`.
- Run browser proof for playback/UI changes. Not required; SB05 changes WebGlRunLib transaction contracts only and adds no new UI/browser route.
- Ensure no blank transcripts. Completed: `proof/SB05/transcripts/proof-hygiene-inventory.txt`.
