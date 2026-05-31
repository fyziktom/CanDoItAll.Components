# SB04 - Components command journal and replay proof

## Goal
Make delayed stage execution inspectable after the initial batch command returns.

## Required actions

1. Add a generic command/stage journal to WebGL runtime state.
2. Record each stage start, apply result, completion, warning and failure.
3. Expose a compact proof snapshot that includes journal counters and recent stage result ids.
4. Add replay-oriented tests for staged batches.
5. Avoid large serialized result payloads on every frame.

## Acceptance criteria

- A user can inspect what happened after delayed stages executed.
- Initial command result is not the only place where stage results exist.
- Journal is bounded and safe for long-running sessions.
