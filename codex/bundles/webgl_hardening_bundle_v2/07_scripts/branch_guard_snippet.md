# Branch Guard Snippet for Codex

Paste near the top of future execution prompts:

```text
Do not create a new branch. Work in the branch that is already checked out.
At start, run `git branch --show-current` and `git status --short`.
Do not run `git checkout -b`, `git switch -c`, or any equivalent branch creation command.
If the current branch is unexpected or unsafe, stop and report instead of creating a branch.
```

Suggested PowerShell check:

```powershell
$currentBranch = git branch --show-current
$status = git status --short
Write-Host "Current branch: $currentBranch"
Write-Host $status
```

