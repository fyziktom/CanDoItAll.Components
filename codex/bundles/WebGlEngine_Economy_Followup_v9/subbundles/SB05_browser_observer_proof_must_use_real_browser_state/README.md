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
