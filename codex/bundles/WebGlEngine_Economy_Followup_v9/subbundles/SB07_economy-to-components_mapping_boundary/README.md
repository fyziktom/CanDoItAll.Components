# SB07 — Economy-to-Components mapping boundary

## Goal

Ensure Economy bridge maps domain terms only into generic visual/run primitives and allowed source provenance.

## Scope

Repository scope: **Both repos**  
Priority: **P1**

## Required implementation work

- Review every EconomyWebGl* projector metadata key.
- Reject domain semantics in action kind/stage id names consumed by Components.
- Document allowed generic mapping vocabulary.


## Required proof

- mapping-boundary-report.md
- WebGlRunDocumentValidator passing strict generic boundary


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
