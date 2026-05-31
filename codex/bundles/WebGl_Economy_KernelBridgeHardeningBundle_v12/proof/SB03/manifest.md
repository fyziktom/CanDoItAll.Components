# SB03 Proof Manifest

Status: Completed

## Scope

Components motion append mode has sequential per-object queue proof and exposes queue/cancellation diagnostics.

## Changed File Hashes

| Reference | SHA-256 |
|---|---|
| `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/29-webgl-scene-motion-queues.js` | `CB9891E89AE2015EDE61136FF026C83688B5A6AE533505CC517704B6BCFEAB0E` |
| `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/14-webgl-scene-motion.js` | `509415451599521CEFA91EC7A398E637D3C38F682C687403A592B233998DA93A` |
| `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/25-webgl-scene-diagnostics.js` | `4A949BECB2E73A10025ED8874A8D98DBDD1D6BD40829F8A3E06A5E07A4B35A4E` |
| `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js` | `735909980BB20F330F2A1D4CAC920F7F26AEE49463179D50B34143AAB59F6487` |
| `repo://tools/webgllib/audit-motion-queue.cjs` | `5518EDAC2A7DE594F3255809257BE0F46C04259BE76EE1B046C918934DF42944` |

## Command Transcripts

| Artifact | SHA-256 |
|---|---|
| `bundle://proof/SB03/transcripts/motion-queue-audit.txt` | `4929238F19ECF96A067CCDAE74A49CE03FA0AC6ECB32E91F3F31A3609120C0C8` |
| `bundle://proof/SB03/transcripts/source-assertions.txt` | `32D2F74A7917823644CF1B299A6B05099F6D96547DFC73126D2F8134B2A5D141` |

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| `queuedMotionCount`, `maxMotionQueueLength`, `cancelledMotionCount` | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/29-webgl-scene-motion-queues.js` and `14-webgl-scene-motion.js` | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js` diagnostics snapshot | Enqueue append, promote on completion, cancel/clear | `bundle://proof/SB03/transcripts/motion-queue-audit.txt` proves only one active same-object motion and one queued motion after append. |

