# SB01 — Cross-repo inventory, branch, and large-screen guard

## Goal
Verify current branch and changed file inventory in both repositories before touching code.

## Required rules
- Do not create a new branch.
- Work only in the currently checked-out branch for each repo.
- Record `git branch --show-current`, `git status --short`, and changed file inventory.
- Confirm WebGL work is desktop/large-screen only.

## Validation
- Components: run existing build/tests/audit.
- Economy: run existing build/tests/boundary audit.
- Save inventory output under the appropriate `artifacts/` folder in each repo.
