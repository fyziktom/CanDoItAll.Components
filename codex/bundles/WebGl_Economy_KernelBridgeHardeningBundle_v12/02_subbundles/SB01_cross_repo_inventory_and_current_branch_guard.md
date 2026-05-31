# SB01 - Cross-Repo Inventory and Current Branch Guard

## Goal

Before editing, prove the current branch and the current state of both repos.

## Tasks

- In both local clones, run:
  - `git status --short`
  - `git branch --show-current`
  - `git log -5 --oneline`
- Confirm Codex is not creating or switching to a new branch.
- Record changed-file inventory for:
  - Components WebGL runtime
  - Components WebGlRunLib
  - Economy Simulation.Abstractions
  - Economy Simulation.SimpleAccounts
  - Economy Simulation.Visualization
  - Economy tests and fixtures

## Hard rule

Do not run `git checkout -b`, `git switch -c`, or `git branch <new>`.

## Proof

Create:

```text
proof/SB01/manifest.md
proof/SB01/semantic-invariants.md
proof/SB01/transcripts/components-inventory.txt
proof/SB01/transcripts/economy-inventory.txt
```
