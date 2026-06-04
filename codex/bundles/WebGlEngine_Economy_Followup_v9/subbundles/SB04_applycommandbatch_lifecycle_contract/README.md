# SB04 — ApplyCommandBatch lifecycle contract

## Goal

Clarify scheduled/applied/settled lifecycle across JS and C# adapters.

## Scope

Repository scope: **Components**  
Priority: **P0**

## Required implementation work

- Default browser observer applies should use ApplyCommandBatchAndWait or configured wait policy.
- Non-waiting ApplyCommandBatch must return lifecycle=scheduled if work remains.
- C# command result must surface lifecycle and idle blockers.


## Required proof

- adapter tests for scheduled vs settled
- browser proof with wait-for-motion barrier


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
