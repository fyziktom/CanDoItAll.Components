# SB12 — Visualization genericity for third scenario

## Goal

Render exchange/investment scenario without new domain-specific Components concepts.

## Scope

Repository scope: **Both repos**  
Priority: **P1**

## Required implementation work

- Map portfolios/entities/exchange edges to generic objects, links, symbols and stages.
- Avoid object kinds like buyer/seller/investor in WebGlRunLib; keep them in Economy input metadata only.
- Add visual load and idle browser proof.


## Required proof

- browser observer proof for third scenario
- generic validation passes


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
