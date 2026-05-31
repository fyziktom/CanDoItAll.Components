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

## Status
- Completed.

## Prerequisites
- Prepared-stage bundle validator must pass after repair.
- Both repositories must be available locally.

## Exact Source References
- `repo://CanDoItAll.Components.slnx`
- `repo://tools/webgllib/audit-scene-runtime.cjs`
- `C:\repositories\CanDoItAll.Economy\CanDoItAll.Economy.slnx`
- `C:\repositories\CanDoItAll.Economy\scripts\audit-simulation-boundaries.ps1`

## Dependency Impact
- Establishes the branch, dirty-state, and boundary baseline for every later subbundle.

## Validation Depth
- Command transcript proof is sufficient; no production code change is expected.

## Acceptance Checklist
- Components branch and status are recorded.
- Economy branch and status are recorded.
- Boundary scans identify whether Components references Economy and where Economy references Components/WebGL.
- Existing build/audit commands are attempted and transcripted.

## Proof Required
- `bundle://proof/SB01/manifest.md`
- Branch/status transcripts for both repositories.
- Boundary and audit transcripts.

## Browser Validation Logging
- Browser validation is not required for this inventory subbundle.

## Progression Gate
- SB02-SB16 may proceed only after branch policy and boundary baseline are recorded.

## Suggested Agent Prompt
- Verify both repositories and capture branch, status, boundary, audit, and build baseline evidence without creating a branch.
