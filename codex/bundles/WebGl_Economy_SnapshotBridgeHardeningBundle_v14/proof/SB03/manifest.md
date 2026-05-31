# SB03 Proof Manifest

Status: Completed

## Scope

Components stage runner and per-object motion queue behavior proof.

## Changed Files

| File | SHA-256 proof |
|---|---|
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/01-webgl-scene.js` | `bundle://proof/SB03/hashes/changed-file-sha256.txt` |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/14-webgl-scene-motion.js` | `bundle://proof/SB03/hashes/changed-file-sha256.txt` |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/29-webgl-scene-motion-queues.js` | `bundle://proof/SB03/hashes/changed-file-sha256.txt` |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/31-webgl-scene-motion-cancellation.js` | `bundle://proof/SB03/hashes/changed-file-sha256.txt` |
| `repo://CanDoItAll.Components/tools/webgllib/audit-motion-queue.cjs` | `bundle://proof/SB03/hashes/changed-file-sha256.txt` |

## Command Transcripts

| Command | Result | Path |
|---|---|---|
| `npm run webgllib:audit-motion-queue` | Pass | `bundle://proof/SB03/transcripts/motion-queue-audit-after-split.txt` |
| `npm run webgllib:audit-stage-runner` | Pass | `bundle://proof/SB03/transcripts/stage-runner-audit.txt` |
| `npm run webgllib:audit-scene-runtime` | Pass, 9 warnings and no hard failures | `bundle://proof/SB03/transcripts/runtime-audit-after-split.txt` |
| `dotnet test .\tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj --no-build` | Pass, 35 tests | `bundle://proof/SB03/transcripts/webgllib-tests-after-split.txt` |

## Source Assertions

| Assertion | Path |
|---|---|
| Motion cancellation, queued removal, promotion, and object-removal clearing paths are present. | `bundle://proof/SB03/source-assertions/motion-source-assertions.txt` |
| Changed files have no TODO, NotImplemented, fixture-specific, hardcode marker, or stale `pendingCommandStages` use. | `bundle://proof/SB03/source-assertions/anti-stub-scan.txt` |

## Behavior Artifacts

| Artifact | Path |
|---|---|
| Motion queue edge-case proof JSON | `repo://CanDoItAll.Components/artifacts/webgl-runtime-motion-queue-hardening-v14/motion-queue/motion-queue-proof.json` |
| Stage runner proof JSON | `repo://CanDoItAll.Components/artifacts/webgl-runtime-stage-runner-hardening-v14/stage-runner/stage-runner-proof.json` |

## Semantic Gate

See `bundle://proof/SB03/semantic-invariants.md`.
