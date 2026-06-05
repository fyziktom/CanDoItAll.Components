# SB10 — Third scenario: multi-goods exchange/investment/elite formation

## Goal

Add structurally different scenario pack to expose genericity gaps.

## Scope

Repository scope: **Economy**  
Priority: **P0**

Status: **Completed**

## Required implementation work

- Create scenario with X goods categories, producers/consumers/rich investors/small entities.
- Represent exchange, investment/loan/equity-like funding and policy shocks with generic events.
- Define metrics: HHI, top wealth share, Gini-like proxy, liquidity, unmet demand, dependency on elite capital.


## Required proof

- scenario pack valid
- headless run valid or explicit gaps list
- readiness report

## Executed proof

- `proof/SB10/scenario-pack/multi-goods-elite/` preserves the strict-hashed committed scenario pack.
- `proof/SB10/headless-run/readiness-report.json` records a `headless-valid` readiness report.
- `proof/SB10/headless-run/metrics-invariants.json` records the six requested metrics and passing invariants.
- `proof/SB10/gaps-list.md` records the remaining bridge-bound visual mapping projection diagnostics owned by SB12.
- `proof/SB10/metrics-summary.json` summarizes HHI `0.330774`, top wealth share `0.489362`, Gini-like proxy `0.6250`, liquidity `14`, unmet demand `1`, and elite capital dependency `71`.
- `proof/SB10/scenario-pack-headless-tests.txt` passes 4/4 targeted pack/headless tests.
- `proof/SB10/scenario-pack-tamper-tests.txt` passes 6/6 strict tamper guardrail tests.
- `proof/SB10/simulationsandbox-build.txt` builds `CanDoItAll.Economy.SimulationSandbox` with 0 warnings and 0 errors.


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
