# SB05 — Browser observer proof must use real browser state

## Goal

Replace self-referential observer proof with artifact-backed comparison of expected vs actual browser state.

## Scope

Repository scope: **Components**  
Priority: **P0**

## Required implementation work

- Stop comparing runDocument to itself.
- Export browser scene/document hash or proof snapshot hash.
- Compare expected final positions, completed stage ids, idle diagnostics and console errors.


## Required proof

- observer-proof.json
- screenshot
- console logs
- hash comparison


## Hard gates

- No placeholder proof files.
- No empty transcript may be referenced as passing proof.
- Every changed production behavior must have failing-first or negative proof where feasible.
- Browser proof must include screenshot, console logs, diagnostics JSON and explicit assertions when the subbundle touches UI/runtime behavior.
- If any gate cannot be completed, stop and write a `REOPEN.md` with exact remaining work.

## QA review prompts

- Does the change reduce simulator noise or merely document it?
- Does the change keep Components generic?
- Does the change separate headless economic truth from browser observer evidence?
- Could a scenario pass because of fallback/default behavior instead of intended economics?

## Closure

Status: completed on 2026-06-04.

Implemented in Components:

- `RunPlayback` now compares `runDocument` to a distinct browser-loaded document instead of comparing `runDocument` to itself.
- RunPlayback diagnostics now expose expected/browser document hashes, browser-loaded scene content hash, browser proof snapshot hash, idle diagnostics, completed stage ids, and final object positions.
- Browser-loaded document capture records the exact imported scene payload after a successful browser import and avoids the previous fragile full-scene export path.
- The SB05 Playwright proof drives `/run-playback` in a real browser, records screenshot and console logs, and asserts hash parity, observer validity, idle blockers, completed stages, final positions, and source-level absence of self-compare.

Proof:

- `proof/SB05/manifest.md`
- `proof/SB05/browser/observer-proof-assertions.json`
- `proof/SB05/browser/observer-proof.json`
- `proof/SB05/browser/observer-proof-after.png`
- `proof/SB05/browser/observer-proof-console.log`
- `proof/SB05/transcripts/source-assertions.txt`
- `proof/SB05/transcripts/webglsandbox-build.txt`
