# SB03 - Settled runtime idle semantics

## Status

Completed on 2026-06-04.

## Closure

SB03 implemented semantic idle vs visual idle, added the `semanticIdle`, `visualIdle`, and `finalRenderDrained` diagnostics, and hardened final scheduled render handling so `waitForRuntimeIdle` accepts a scheduled-only final paint only after an explicit final drain or two consecutive semantic-idle probes.

Executed proof:

- `../../proof/SB03/manifest.md`
- `../../proof/SB03/semantic-invariants.md`
- `../../proof/SB03/js/runtime-idle-final-render-assertions.json`
- `../../proof/SB03/browser/runtime-idle-semantics-assertions.json`
- `../../proof/SB03/browser/runtime-idle-semantics-after.png`
- `../../proof/SB03/transcripts/webgllib-tests.txt`
- `../../proof/SB03/transcripts/webglrunlib-tests.txt`
- `../../proof/SB03/transcripts/webglsandbox-build.txt`
- `../../proof/SB03/transcripts/runtime-idle-semantics-playwright.txt`

## Goal

Harden runtime idle so final render scheduling does not create false timeouts and semantic work is distinct from render work.

## Scope

Repository scope: **Components**  
Priority: **P0**

## Required implementation work

- Define semantic idle vs visual idle.
- Require two consecutive idle probes or explicit final render drain.
- Add diagnostic fields: semanticIdle, visualIdle, finalRenderDrained.

## Required proof

- JS tests for idle with final scheduled render
- browser proof with active/queued counts zero

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
