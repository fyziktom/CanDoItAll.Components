# SB03 — RunPlayback playback state-machine and pause fix

Priority: P0
Related findings: F01,F02,F04
Status: Completed
Completed: 2026-06-03

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

Gate result: Passed. `proof/SB03/browser/runplayback-pause-assertions.json` records Pause returning in 150 ms, frame remaining at 1 after the deadline, queued command stages clearing from 1 to 0, runtime stop reason `Paused.`, and no stage/motion progress after Pause.

## Required proof artifacts

- `proof/SB03/manifest.md`
- `proof/SB03/semantic-invariants.md`
- `proof/SB03/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant. Completed: `proof/SB03/transcripts/components-build-after-runplayback-pause.txt`.
- Run focused tests for changed area. Completed: `proof/SB03/transcripts/webglrunlib-focused-tests.txt`.
- Run boundary audits. Completed: source contract scan `proof/SB03/transcripts/source-assertion-runplayback-pause-scan.txt`.
- Run browser proof for playback/UI changes. Completed: `proof/SB03/transcripts/runplayback-pause-playwright.txt`, `proof/SB03/browser/runplayback-pause-assertions.json`, and `proof/SB03/browser/runplayback-pause-after.png`.
- Ensure no blank transcripts. Completed: `proof/SB03/transcripts/proof-hygiene-inventory.txt`.
