# SB03 — RunPlayback playback state-machine and pause fix

Priority: P0
Related findings: F01,F02,F04

## Objective

Refactor Play/Pause/Cancel into a short-returning component command model with background playback loop, CTS lifecycle, runtime stop on pause/cancel, stale callback suppression, and dispose safety.

## Required implementation rules

- Keep Components generic and domain-neutral.
- Prefer small cohesive changes over broad rewrites.
- Add or update tests before claiming the gate is closed.
- Capture failing-first proof for every P0/P1 behavioral bug.
- Update proof manifest and semantic invariants.

## Acceptance gate

Browser proof: click Play, wait for motion, click Pause, scene stops within bounded time and no further frame/stage/motion progress occurs.

## Required proof artifacts

- `proof/SB03/manifest.md`
- `proof/SB03/semantic-invariants.md`
- `proof/SB03/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant.
- Run focused tests for changed area.
- Run boundary audits.
- Run browser proof for playback/UI changes.
- Ensure no blank transcripts.
