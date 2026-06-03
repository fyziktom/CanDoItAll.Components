# SB04 — WebGlRun runner lifecycle contracts

Priority: P1
Related findings: F11
Status: Completed
Completed: 2026-06-03

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

Gate result: Passed. Focused tests prove explicit Pause/Cancel/Stop state, diagnostics, counters, active/pending-stage clearing, and cancellation during frame apply leaves `CompletedStageIds` empty.

## Required proof artifacts

- `proof/SB04/manifest.md`
- `proof/SB04/semantic-invariants.md`
- `proof/SB04/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant. Completed: `proof/SB04/transcripts/components-build-after-runner-lifecycle.txt`.
- Run focused tests for changed area. Completed: `proof/SB04/transcripts/webglrunlib-focused-tests.txt`.
- Run boundary audits. Completed: `proof/SB04/transcripts/source-assertion-runner-lifecycle-scan.txt` and `proof/SB04/transcripts/domain-neutrality-scan.txt`.
- Run browser proof for playback/UI changes. Not required; SB04 changes runner contracts only and adds no new UI/browser runtime surface.
- Ensure no blank transcripts. Completed: `proof/SB04/transcripts/proof-hygiene-inventory.txt`.
