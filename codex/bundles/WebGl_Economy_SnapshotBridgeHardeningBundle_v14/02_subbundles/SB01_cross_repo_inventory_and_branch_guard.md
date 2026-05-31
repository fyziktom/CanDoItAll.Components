# SB01 - Cross-repo inventory and branch guard

Goal:
- Confirm current branches in both repositories.
- Do not create new branches.
- Record source file inventory before edits.

Tasks:
1. Record current branch names.
2. Record changed source files relevant to WebGL, WebGlRunLib, Economy simulation, visualization, WebGlBridge.
3. Run existing build/tests/audits before edits.
4. Confirm Components has no Economy references.
5. Confirm Economy bridge is the only layer referencing Components WebGlRunLib.

Validation:
- `git branch --show-current` in both repos.
- `dotnet build`
- existing boundary audits.

## Status

Completed.

## Prerequisites

None. This subbundle must run first and establishes the branch/source baseline for all later work.

## Validation Depth

Record branch names, git status, relevant source inventory, baseline build/test/audit transcripts, and boundary scan results before production edits.

## Progression Gate

SB02-SB15 may proceed only after branch guard evidence and baseline command outcomes are recorded in the execution report.
