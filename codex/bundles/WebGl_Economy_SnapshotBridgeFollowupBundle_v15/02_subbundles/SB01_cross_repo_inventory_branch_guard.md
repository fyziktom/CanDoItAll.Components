# SB01 — Cross-repo inventory and branch guard

## Goal
Confirm current branches, changed files, and dependency boundaries before any implementation.

## Required
- Do not create a new branch.
- Record `git branch --show-current` in both repos.
- Record `git status --short` in both repos.
- Confirm Components has no Economy references.
- Confirm Economy bridge/sandbox are the only allowed projects with Components/WebGL references.
- Run existing boundary scripts.

## Validation
- `scripts/audit-simulation-boundaries.ps1`
- `npm run audit:webgllib` or the repository equivalent
- `dotnet build`
