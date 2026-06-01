# SB01 - Cross-repo inventory and branch guard

Codex must:

- work in the currently checked-out branch in both repositories,
- do not run `git switch -c`, `git checkout -b`, or create any new branch,
- record current branch and latest commit SHA for both repos,
- confirm the Components commit message typo does not affect branch selection,
- verify dependency direction:
  - Components -> no Economy
  - Economy.Simulation.WebGlBridge -> WebGlRunLib + Economy visualization/abstractions only
  - Economy.SimulationSandbox -> composition layer only

## Status

Completed.

## Goal

Capture branch, commit, and dependency-boundary evidence before implementation changes begin.

## Prerequisites

- Both local repositories exist at `C:\repositories\CanDoItAll.Components` and `C:\repositories\CanDoItAll.Economy`.
- No new branch creation is allowed.

## Owned Requirements

- R01 Branch And Boundary Guard.

## Dependency Impact

This subbundle is the entry gate for every later phase. If it fails, downstream implementation must stop.

## Validation Depth

Command transcript plus source/project reference scan.

## Proof Required

- Branch and commit SHA for both repositories.
- `git status --short` for both repositories.
- Components reference/domain scan.
- Economy project dependency scan for bridge and sandbox boundaries.

## Progression Gate

Pass when both repositories are on the current expected branch, no branch was created, and dependency direction is either clean or every baseline boundary finding is mapped to the downstream subbundle that must repair or explicitly close it.
