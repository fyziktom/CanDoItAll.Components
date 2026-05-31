# Proof manifest SB04

Status: Completed

## Scope

Generic WebGL motion queue semantics: ordered append motion, explicit queue policy behavior, edge cases, and queue diagnostics.

## Changed Files

- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/14-webgl-scene-motion.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/29-webgl-scene-motion-queues.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js`
- `repo://tools/webgllib/audit-motion-queue.cjs`

SHA-256 hashes:

- `bundle://proof/SB04/hashes/changed-file-hashes.txt`

## Command Transcripts

- Failing-first queue-policy contract scan: `bundle://proof/SB04/transcripts/failing-first-motion-queue-policy-contract.txt`
- Semantic positive motion queue audit: `bundle://proof/SB04/transcripts/motion-queue-audit.txt`
- WebGlLib tests: `bundle://proof/SB04/transcripts/webgllib-tests.txt`
- Runtime audit: `bundle://proof/SB04/transcripts/runtime-audit.txt`

## Source Assertions

- Queue policy source assertions and anti-stub scan: `bundle://proof/SB04/source-assertions/motion-queue-source-assertions.txt`
- Runtime audit artifact: `repo://artifacts/webgl-runtime-motion-queue-hardening-v15/motion-queue/motion-queue-proof.json`
- `queuePolicy` accepts `append`, `replace`, `cancel-and-replace`, and `reject-if-active`.
- Diagnostics expose active motion IDs, queued motion IDs, and per-object queue snapshots.
- Proof snapshots and diagnostics snapshots expose queue details.

## Semantic Adequacy Gate

- Shallow-pass trap: an implementation that only appends queued motions could pass a simple two-motion test while failing replacement, rejection, and edge cases.
- Adversarial negative proof: `bundle://proof/SB04/transcripts/failing-first-motion-queue-policy-contract.txt` shows explicit queue policy and queued-ID diagnostics were absent before implementation.
- Semantic positive proof: `bundle://proof/SB04/transcripts/motion-queue-audit.txt` proves A -> B -> C -> home ordering, queue policies, zero-duration completion, missing-object rejection, and queue diagnostics.
- Anti-stub audit: `bundle://proof/SB04/source-assertions/motion-queue-source-assertions.txt` records no `TODO`, `NotImplemented`, `not implemented`, or fixture-specific markers in changed motion queue files.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| `queuePolicy` command field | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/14-webgl-scene-motion.js` | Motion enqueue runtime and command result metadata | Normalized per motion command, controls active/queued state before scheduling render. | Failing-first scan and `reject-if-active` audit assertion. |
| `queuedMotionIds` / `motionQueueSnapshot` | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/29-webgl-scene-motion-queues.js` | Core diagnostics snapshot and proof snapshot | Recomputed on enqueue, promotion, cancel, clear, and completion. | A-B-C-home audit asserts queued IDs and recalculated start positions. |

## Failures / Blockers

- No SB04 blocker.
- Runtime audit remains green with warning-threshold files only.
