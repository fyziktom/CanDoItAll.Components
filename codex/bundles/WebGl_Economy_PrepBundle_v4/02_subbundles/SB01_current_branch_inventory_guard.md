# SB01 — Current branch and inventory guard

## Repositories

- `CanDoItAll.Components`
- `CanDoItAll.Economy`

## Rule

Work in currently checked-out branches. Do not create new branches.

## Tasks

1. Record current branch in both repos:
   - `git branch --show-current`
2. Record `git status --short`.
3. Run project inventory:
   - Components: list `.csproj`, current solution entries, JS runtime files, package scripts.
   - Economy: list `.csproj`, current solution entries, project references.
4. Write:
   - `artifacts/webgl-engine-prep-v4/01_INVENTORY.md`
   - `artifacts/economy-simulation-prep-v4/01_INVENTORY.md`

## Validation

- fail if any command output indicates a newly created branch;
- fail if either repo has untracked accidental temporary files outside `artifacts`, `codex`, or intended source paths.
