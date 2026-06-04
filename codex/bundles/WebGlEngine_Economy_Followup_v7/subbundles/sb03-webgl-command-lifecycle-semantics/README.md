# SB03 — WebGL command lifecycle semantics

## Repository scope

Components

## Goal

Separate accepted, scheduled, active, settled, cancelled, and failed command states across JS and C#.

## Tasks

- Extend WebGlSceneCommandResult/WebGlSceneCommandBatchResult metadata or typed diagnostics with lifecycle state.
- Ensure applyCommandBatch result clearly says whether staged commands are only scheduled or fully settled.
- Add `ApplyCommandBatchAndWaitAsync` or equivalent for proof paths.
- Document that normal apply can be non-blocking but proof apply must wait.
- Add tests for long barrier + motion + pause + idle wait.

## Acceptance criteria

- A batch with staged motions returns lifecycle state `scheduled` or `settled`, never ambiguous success.
- Proof routes use the settled command path.
- Diagnostics expose stage runner lifecycle consistently.

## Required proof artifacts

- `proof/SB03/transcripts/command-lifecycle-tests.txt`
- `proof/SB03/browser/staged-batch-settled-proof.json`

## Gate

Forced refactor review before continuing to economic readiness work.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.
