# SB01 - Cross-repo inventory, branch guard, and large-screen guard

## Goal

Prevent Codex from working on the wrong branch or spending time on unsupported viewport sizes.

## Required actions

1. In both repositories, record:
   - current branch
   - current HEAD
   - changed files
   - solution projects
2. Assert that Codex did not create or switch to a new branch.
3. Add or update a short `docs/*` note stating:
   - WebGL surfaces are large-screen desktop only.
   - No small/medium screen optimization is allowed for WebGL.
4. Add an audit script/check that fails if new WebGL docs/prompts contain small/medium/mobile optimization tasks, except for an explicit "do not optimize" warning.

## Validation

- Branch inventory report exists in both repos.
- No new branch was created.
- WebGL validation screenshots/proofs are large-screen only.
