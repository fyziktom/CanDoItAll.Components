# SB05 — Multi-frame ApplyPlayback transaction/cancellation semantics

Priority: P1
Related findings: F05

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

## Required proof artifacts

- `proof/SB05/manifest.md`
- `proof/SB05/semantic-invariants.md`
- `proof/SB05/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant.
- Run focused tests for changed area.
- Run boundary audits.
- Run browser proof for playback/UI changes.
- Ensure no blank transcripts.
