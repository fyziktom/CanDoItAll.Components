# SB01 — Cross-Repo Inventory And Branch Guard

## Goal

Verify current branches, changed files, project references and forbidden dependency directions before modifying code.

## Required actions

- Do not create or switch to a new branch.
- Record `git branch --show-current` for both repos.
- Record `git status --short` for both repos before and after work.
- Record current project graph for relevant projects.
- Confirm `CanDoItAll.Components` has no Economy references.
- Confirm `CanDoItAll.Economy.Simulation.WebGlBridge` does not reference SimpleAccounts or Ledger.
- Confirm `SimulationSandbox` owns the joined composition.

## Proof

Create:

```text
proof/SB01/transcripts/components-inventory.txt
proof/SB01/transcripts/economy-inventory.txt
proof/SB01/transcripts/dependency-boundary-scan.txt
```
