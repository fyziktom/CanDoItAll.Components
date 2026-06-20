# SB01 — Current-state and pause proof audit

## Purpose

Reproduce and classify current pause/playback behavior and baseline experiment reliability.

## Scope

Repositories:

- `fyziktom/CanDoItAll.Components` / branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy` / branch `main`

## Implementation tasks

- Create a browser test that clicks Play, waits for active motion/stage work, clicks Pause, then asserts the runtime settles.
- Capture diagnostics before play, during play, immediately after pause, and after idle wait.
- Audit all proof transcripts from the previous bundle; fail empty or placeholder-only files.
- Record exact current git SHAs, changed-file hashes, and source references.

## Required proof

- Browser assertion: activeMotionCount=0, queuedMotionCount=0, queuedCommandStageCount=0 after Pause.
- No stale MotionCompleted callback changes status after Pause.
- Proof validator reports zero empty critical transcripts.

## Refactor gate

Before closing this subbundle, Codex must add a short self-review covering:

- API compatibility,
- generic/domain boundary,
- deterministic behavior,
- performance risk,
- proof adequacy,
- remaining open risks.

## Stop conditions

Do not continue to the next subbundle if a critical proof is browser-screenshot-only, placeholder-only, warning-only where a hard gate is required, or not tied to a source invariant.
