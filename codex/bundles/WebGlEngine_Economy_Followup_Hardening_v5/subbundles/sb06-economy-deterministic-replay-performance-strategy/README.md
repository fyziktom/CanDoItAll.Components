# SB06 — Economy deterministic replay performance strategy

Priority: P1
Related findings: F06

## Objective

Split Economy apply into forward incremental mode and full deterministic replay only for seek/backward/non-contiguous states. Prevent O(n²) repeated stepping.

## Required implementation rules

- Keep Components generic and domain-neutral.
- Prefer small cohesive changes over broad rewrites.
- Add or update tests before claiming the gate is closed.
- Capture failing-first proof for every P0/P1 behavioral bug.
- Update proof manifest and semantic invariants.

## Acceptance gate

Forward Step applies only necessary delta frames; Seek/Back uses full replay with reset. Browser proof covers both.

## Required proof artifacts

- `proof/SB06/manifest.md`
- `proof/SB06/semantic-invariants.md`
- `proof/SB06/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant.
- Run focused tests for changed area.
- Run boundary audits.
- Run browser proof for playback/UI changes.
- Ensure no blank transcripts.
