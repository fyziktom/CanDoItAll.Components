# SB06 — Economy deterministic replay performance strategy

Priority: P1
Related findings: F06
Status: Completed
Completed: 2026-06-03

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

Gate result: Passed. The Economy sandbox now uses a stable-browser-frame replay planner: manual/seek/back/non-contiguous paths use full deterministic replay with reset, while contiguous forward Step after a stable browser frame applies only the delta frame without reset. Browser proof validates full apply through frame 0, incremental Step through frame 1, and full Last seek through frames 0,1,2.

## Required proof artifacts

- `proof/SB06/manifest.md`
- `proof/SB06/semantic-invariants.md`
- `proof/SB06/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant. Completed: `proof/SB06/transcripts/economy-build-after-replay-strategy.txt`.
- Run focused tests for changed area. Completed: `proof/SB06/transcripts/economy-component-focused-tests.txt`.
- Run boundary audits. Completed: `proof/SB06/transcripts/source-assertion-economy-replay-scan.txt`, `proof/SB06/transcripts/components-domain-boundary-scan.txt`, and `proof/SB06/transcripts/anti-stub-economy-replay-scan.txt`.
- Run browser proof for playback/UI changes. Completed: `proof/SB06/browser/economy-replay-mode-assertions.json`, `proof/SB06/browser/economy-replay-mode-after.png`, and `proof/SB06/transcripts/economy-replay-mode-playwright.txt`.
- Ensure no blank transcripts. Completed: `proof/SB06/transcripts/proof-hygiene-inventory.txt`.
