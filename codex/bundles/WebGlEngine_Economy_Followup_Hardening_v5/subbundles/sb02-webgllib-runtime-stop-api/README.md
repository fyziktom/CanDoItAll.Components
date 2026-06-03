# SB02 — WebGlLib runtime stop API

Priority: P0
Related findings: F01,F03

## Objective

Add public stopRuntimeActivity/cancelCommandStages API that cancels command stage runner, clears active+queued motions, updates diagnostics, and is exposed in WebGlSceneView.

## Required implementation rules

- Keep Components generic and domain-neutral.
- Prefer small cohesive changes over broad rewrites.
- Add or update tests before claiming the gate is closed.
- Capture failing-first proof for every P0/P1 behavioral bug.
- Update proof manifest and semantic invariants.

## Acceptance gate

Calling stop twice is idempotent; queued stages and active/queued motions drop to zero.

## Required proof artifacts

- `proof/SB02/manifest.md`
- `proof/SB02/semantic-invariants.md`
- `proof/SB02/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant.
- Run focused tests for changed area.
- Run boundary audits.
- Run browser proof for playback/UI changes.
- Ensure no blank transcripts.
