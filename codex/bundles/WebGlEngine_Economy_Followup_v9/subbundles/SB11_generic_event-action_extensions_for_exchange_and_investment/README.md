# SB11 — Generic event/action extensions for exchange and investment

## Goal

Add only generic primitives needed by third scenario; avoid hardcoding monopoly/elite domain terms into core.

## Scope

Repository scope: **Economy**  
Priority: **P1**

Status: **Completed**

## Required implementation work

- Prefer generic resource-flow, obligation, contribution, claim, return, fee and relationship events.
- Add registry entries only if no existing event kind can express the behavior cleanly.
- No new UI-specific economics in Components.


## Required proof

- event registry tests
- semantic oracle for investment flow

## Executed proof

- `proof/SB11/failing-first-generic-contribution.txt` records the pre-change `unknown-event-handler` result for `actor.resource.contribution`.
- `proof/SB11/event-registry-and-investment-flow-tests.txt` passes 2/2 registry and semantic-flow tests.
- `proof/SB11/investment-flow-oracle.json` records conserved totals (`credit=110`, `equityClaim=10`), contribution/claim/return flows, and obligation relationship strength `40`.
- `proof/SB11/abstractions-build.txt` builds `CanDoItAll.Economy.Simulation.Abstractions` with 0 warnings and 0 errors.
- `proof/SB11/simpleaccounts-build.txt` builds `CanDoItAll.Economy.Simulation.SimpleAccounts` with 0 warnings and 0 errors.


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
