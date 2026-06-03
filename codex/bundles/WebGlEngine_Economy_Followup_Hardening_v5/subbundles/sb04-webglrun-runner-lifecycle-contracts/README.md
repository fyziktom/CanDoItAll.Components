# SB04 — WebGlRun runner lifecycle contracts

Priority: P1
Related findings: F11

## Objective

Add optional runner Pause/Cancel/Stop semantics and diagnostics so hosts do not need ad-hoc cancellation logic. Keep generic and not browser-specific.

## Required implementation rules

- Keep Components generic and domain-neutral.
- Prefer small cohesive changes over broad rewrites.
- Add or update tests before claiming the gate is closed.
- Capture failing-first proof for every P0/P1 behavioral bug.
- Update proof manifest and semantic invariants.

## Acceptance gate

Runner state reflects canceled/paused/stopped operation and does not mark canceled frames completed.

## Required proof artifacts

- `proof/SB04/manifest.md`
- `proof/SB04/semantic-invariants.md`
- `proof/SB04/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant.
- Run focused tests for changed area.
- Run boundary audits.
- Run browser proof for playback/UI changes.
- Ensure no blank transcripts.
