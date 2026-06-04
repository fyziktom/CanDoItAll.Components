# SB10 — Third scenario: multi-goods exchange/investment/elite formation

## Goal

Add structurally different scenario pack to expose genericity gaps.

## Scope

Repository scope: **Economy**  
Priority: **P0**

## Required implementation work

- Create scenario with X goods categories, producers/consumers/rich investors/small entities.
- Represent exchange, investment/loan/equity-like funding and policy shocks with generic events.
- Define metrics: HHI, top wealth share, Gini-like proxy, liquidity, unmet demand, dependency on elite capital.


## Required proof

- scenario pack valid
- headless run valid or explicit gaps list
- readiness report


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
