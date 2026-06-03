# Critical findings

## F01 — RunPlayback pause is not a real runtime stop (P0)

Repository: Components

RunPlayback PlayAsync starts a long loop and PauseAsync only flips isPlaying/cancels CTS; it does not cancel already queued WebGL command stages or active/queued JS motions.

Target subbundle(s): SB01,SB02,SB03

## F02 — Playback UI event handler can monopolize component command flow (P0)

Repository: Components

PlayAsync remains an awaited long-running component event handler. In Blazor Server this can make Pause/Cancel feel ignored because another UI event cannot reliably interleave with the active play handler.

Target subbundle(s): SB03

## F03 — No public stop-all WebGL runtime operation (P0)

Repository: Components

JS has cancelCommandStageRunner and clearMotions, but the public window.CanDoItAll.webglScene API exposes clearMotions only. There is no one-shot stopRuntimeActivity/cancelCommandStages method for playback hosts.

Target subbundle(s): SB02

## F04 — MotionCompleted callback can overwrite paused status (P1)

Repository: Components

RunPlayback HandleMotionCompleted updates status after a pause/cancel, so a stale JS completion callback can make state look active or successful after user cancellation.

Target subbundle(s): SB03

## F05 — ApplyPlaybackAsync lacks playback transaction/cancellation summary (P1)

Repository: Components

Multi-frame ApplyPlaybackAsync now exists, but it returns mostly frame results and does not provide a strong transaction model, first-failed frame snapshot, cancellation reason, or compensating stop behavior.

Target subbundle(s): SB05

## F06 — Economy UI deterministic replay is O(n) per step and O(n²) across long playback (P1)

Repository: Economy

ApplyCurrentFrameCoreAsync builds replay from frame 0 through current frame for every apply. This is correct for absolute safety but expensive for long simulations and unnecessary for forward-only play.

Target subbundle(s): SB06

## F07 — Scenario API is improved but still path-biased (P1)

Repository: Economy

ScenarioSource exists, but descriptor/session export still carry ExperimentJsonPath and legacy path fields. Consumers can still accidentally couple to local filesystem paths.

Target subbundle(s): SB07

## F08 — Scenario manifest locks only part of pack semantics (P1)

Repository: Economy

Catalog computes pack hash, but manifest currently validates experiment content hash and required files; it does not strongly bind every required file hash and manifest-declared pack hash.

Target subbundle(s): SB08

## F09 — Proof hygiene still needs machine enforcement (P1)

Repository: Both

Prior bundle reports mention browser/build/package proof, but several committed proof transcripts are empty files. This permits false-positive closure.

Target subbundle(s): SB10,SB12

## F10 — Large simulation performance budgets are implicit (P2)

Repository: Both

There is no enforced budget for objects, commands, pending motions, command-stage queue size, memory growth, or long-run dispose/recreate cycles.

Target subbundle(s): SB09

## F11 — WebGlRun runner state lacks first-class playback lifecycle (P2)

Repository: Components

Runner supports Load/Seek/Step/Apply but no explicit Pause/Cancel/Stop semantics mapped to browser runtime stop operations.

Target subbundle(s): SB04

## F12 — Documentation needs a user-facing playback troubleshooting section (P2)

Repository: Both

The pause bug shows that developer docs must describe lifecycle, cancellation, deterministic replay, and runtime stop rules.

Target subbundle(s): SB11

