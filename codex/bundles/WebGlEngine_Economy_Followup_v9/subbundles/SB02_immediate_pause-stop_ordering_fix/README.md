# SB02 — Immediate Pause/Stop ordering fix

## Execution status

Status: **Completed** on 2026-06-04.

Proof:

- `proof/SB02/manifest.md`
- `proof/SB02/semantic-invariants.md`
- `proof/SB02/browser/runplayback-pause-assertions.json`
- `proof/SB02/browser/runplayback-pause-after.png`
- `proof/SB02/transcripts/webglrunlib-tests.txt`

## Goal

Make Pause stop browser runtime before waiting for C# playback drain.

## Scope

Repository scope: **Components**  
Priority: **P0**

## Required implementation work

- Reorder StopPlaybackAsync: first best-effort StopRuntimeActivityAsync(waitForIdle:false), then cancel task, then final waitForIdle true.
- Add stale callback generation guard for MotionCompleted and CommandCompleted.
- Expose UI diagnostics for runtimeStopGeneration and idle blockers.


## Required proof

- browser proof: Play -> Pause -> no motion/stage blockers within 500ms
- unit/bUnit proof for stop ordering


## Hard gates

- No placeholder proof files.
- No empty transcript may be referenced as passing proof.
- Every changed production behavior must have failing-first or negative proof where feasible.
- Browser proof must include screenshot, console logs, diagnostics JSON and explicit assertions when the subbundle touches UI/runtime behavior.
- If any gate cannot be completed, stop and write a `REOPEN.md` with exact remaining work.

## QA review prompts

- Does the change reduce simulator noise or merely document it?
- Does the change keep Components generic?
- Does the change separate headless economic truth from browser observer evidence?
- Could a scenario pass because of fallback/default behavior instead of intended economics?
