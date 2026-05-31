# SB03 Semantic Invariants

## Invariant SB03-MOTION-001

Raw note:
- RN-008: "Motion queue behavior needs tests for replace, append, cancel queued item, cancel active item, dispose, and object removal."

Expected behavior:
- Append-mode motion queues behind an active same-object motion.
- When the active motion completes, the next motion starts from the object's current completed transform.
- Cancelling a queued motion removes only that queued item.
- Cancelling an active motion promotes the next queued motion from the current transform instead of stranding it.
- Object removal and clear paths remove active and queued motion state.

Shallow-pass trap:
- A test that only enqueues two motions and checks final position can miss queued cancellation and active-cancel stranding.

Adversarial negative proof:
- `tools/webgllib/audit-motion-queue.cjs` cancels an active motion midway while another motion is queued. The old implementation would leave the queued item stranded because `advanceMotions` returns when no active motions exist.
- Transcript: `bundle://proof/SB03/transcripts/motion-queue-audit-after-split.txt`.

Semantic positive proof:
- `repo://CanDoItAll.Components/artifacts/webgl-runtime-motion-queue-hardening-v14/motion-queue/motion-queue-proof.json` lists assertions for append, updated start transform, queued cancel, active cancel promotion, and object-removal clearing.
- `bundle://proof/SB03/transcripts/stage-runner-audit.txt` proves first stage immediate application, wait barrier blocking, later stage advancement, and cancellation diagnostics.

Anti-stub audit:
- `bundle://proof/SB03/source-assertions/anti-stub-scan.txt` shows no TODO, NotImplemented, fixture-specific branch, hardcode marker, or stale `pendingCommandStages` use in changed files.

Changed source files:
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/01-webgl-scene.js`.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/14-webgl-scene-motion.js`.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/29-webgl-scene-motion-queues.js`.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/31-webgl-scene-motion-cancellation.js`.
- `repo://CanDoItAll.Components/tools/webgllib/audit-motion-queue.cjs`.

Downstream dependency check:
- SB04/SB05 can rely on ordered stages and same-object motion queues without motion races or stranded queued motions.
