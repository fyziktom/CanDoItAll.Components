# SB01 — Current Branch Inventory Guard

## Goal

Start safely, document the real current state, and prevent Codex from creating another branch.

## Hard branch rule

Run:

```powershell
git branch --show-current
git status --short
```

Do not create a branch. Do not run `git checkout -b`, `git switch -c`, or any equivalent. Work in the currently checked-out branch only.

If the branch is unexpected, stop and report. Do not self-correct by creating/switching branches.

## Inventory tasks

Collect current file metrics:

```powershell
Get-ChildItem src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/*.js | Sort-Object Name | ForEach-Object { "{0} {1}" -f ((Get-Content $_.FullName).Count), $_.FullName }
Get-ChildItem src/CanDoItAll.Components.WebGlSandbox -Recurse -Include *.cs,*.razor,*.css | Sort-Object FullName | ForEach-Object { "{0} {1}" -f ((Get-Content $_.FullName).Count), $_.FullName }
```

Write the results to:

```text
artifacts/webgl-runtime-hardening-v2/01_INVENTORY.md
```

## Acceptance criteria

- The implementation report states the branch name used.
- No new branch was created.
- Inventory includes JS runtime file line counts.
- Inventory includes sandbox C#/Razor/CSS line counts.
- Inventory identifies files that exceed target thresholds.

