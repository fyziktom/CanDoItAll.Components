# Proof manifest SB02

Status: completed
Completed: 2026-06-03

- Objective: WebGlLib runtime stop API.
- Gate: Passed. Calling stop twice is idempotent; queued stages and active/queued motions drop to zero.
- Owned findings: F01, F03.

## Changed-file hashes

| Path | SHA-256 | Purpose |
| --- | --- | --- |
| `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/39-webgl-scene-runtime-stop.js` | `11A862A480B5172F96635420670E225C5E923D467EED56FB16A6149C17674674` | New runtime stop and stage-cancel implementation. |
| `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/01-webgl-scene.js` | `CF72E14944C75C23FC1FCF10EF0EEB8C3E8579AD0CB887867BBE0FF8D40B4EC2` | Public JS facade exposes `stopRuntimeActivity` and `cancelCommandStages`. |
| `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/25-webgl-scene-diagnostics.js` | `83377553EF2D4FA81DFCC7827AC2DA8582999D792079E4263A08398F1FF95A7E` | Runtime stop diagnostic defaults. |
| `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js` | `B095B0DE2E796163D92CA07A106DFD0CD0BD5B59EB3D4BFC6DDD1560E807AEB5` | Diagnostics snapshot exposes runtime stop fields. |
| `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js` | `117C5D727A46C71C57AF5FEC21B87384C190393E667A99E3EA3509D7E4735212` | Proof snapshot exposes runtime stop fields. |
| `repo://src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor` | `DB0760738846F28EACE789A12EF160DA1A2EA9D9120D37A6C1CD2FB57D9BF6CF` | Blazor component exposes `StopRuntimeActivityAsync` and `CancelCommandStagesAsync`. |
| `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs` | `1FCAF267B67EDF02B5D20E2017D944DF4D3609F580AB3533F1C11E13002D43E5` | C# diagnostics DTO includes runtime stop fields. |
| `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneProofSnapshot.cs` | `4202B4FFAA21C9E0E704B15BB2F4A79E5BFE533F49CC6AE96020815C8EB9D8D0` | C# proof snapshot DTO includes runtime stop fields. |
| `repo://tests/CanDoItAll.Components.WebGlLib.Tests/WebGlRuntimeDiagnosticsTests.cs` | `FA9DDB3A90E9FD9BB4FDEFC7733983C34C97257FA00C61533A3530ACB26B155F` | Runtime stop diagnostics round-trip test. |
| `repo://tests/CanDoItAll.Components.WebGlLib.Tests/WebGlSceneViewExternalImportLifecycleTests.cs` | `716E698074E8AFEFE9F6472AFB69D4876D167B5DA2110930B380D8FFF95B2A4E` | `WebGlSceneView` facade invocation test. |
| `repo://tools/webgllib/audit-runtime-stop.cjs` | `20B8CB9F38FEDFE60769FFEF241CDF34766618E4AA567AD3674F1B1E7396818D` | Runtime-level semantic audit for stop/idempotency/stage-only cancel. |

## Proof artifact hashes

| Path | SHA-256 | Purpose |
| --- | --- | --- |
| `bundle://proof/SB02/browser/runtime-stop-browser-proof.js` | `4812EF39D07E751233CD887A0B970A2A1FE394AB68BEAE79FB3130A11D16FAB7` | Browser proof script. |
| `bundle://proof/SB02/browser/runtime-stop-assertions.json` | `E5F1180DCD8F74E232B0FA23756A859AB86BF73FE0819C005D2CD3989D4BB7B5` | Machine-readable browser assertions. |
| `bundle://proof/SB02/browser/runtime-stop-after.png` | `7B0537E198175C58056C34C4A3F621B4B3CE5C4A1B9520075C872DF9177A9171` | Browser screenshot after runtime stop proof. |
| `bundle://proof/SB02/transcripts/runtime-stop-audit.txt` | `0AE9FDDD42E035A48724E625E290086DD2E695238E3F7A6E3734250900537CE8` | JS runtime audit passed. |
| `bundle://proof/SB02/transcripts/webgllib-focused-tests.txt` | `48A018D5E77F05CAF20CBEB05B12A19A601ED1D8A6413396749FAFECF65F3EF7` | WebGlLib focused test project passed, 55 tests. |
| `bundle://proof/SB02/transcripts/webgllib-runtime-boundary-audit.txt` | `8496743FA4CFF4A6992F01FC31AC1545C544C4EBC82349BAA0A10A62CDAAA391` | Runtime boundary audit passed with existing warning-only line-count debt. |
| `bundle://proof/SB02/transcripts/components-build-after-runtime-stop.txt` | `6D4969C6AF1FFFC144A422F8174A0EDAC48C0A8B01CF5C5F3E4C5E63BDE01D1A` | Solution build passed with zero warnings/errors. |
| `bundle://proof/SB02/transcripts/runtime-stop-playwright.txt` | `DC83AF8443D2A4BD65608BA2D41439405D6CDD8594F379F31A88194D32A3FBA3` | Playwright runtime stop proof transcript. |
| `bundle://proof/SB02/transcripts/source-assertion-runtime-stop-scan.txt` | `39F3002642FAA1B82C6237CA9D4A55854E3620CDFD2809E0363E0AED8ECD6A68` | Source scan proving public contracts and diagnostics exist. |
| `bundle://proof/SB02/transcripts/anti-stub-audit.txt` | `E93800B92DD7E1619EE31DD6BE4D0383B9A330A99E581DA32B77DCD9EB6C1687` | Anti-stub audit for changed/new code files. |
| `bundle://proof/SB02/transcripts/proof-hygiene-inventory.txt` | `3DCCA6309F481A90D8FAC192525832475F4C4029CD92A952821C79F853AC42DB` | SB02 inventory: nine transcript-like files, zero blanks. |

## Command transcripts

- `bundle://proof/SB02/transcripts/runtime-stop-audit.txt`: `node tools\webgllib\audit-runtime-stop.cjs` passed for `stopRuntimeActivity`, `cancelCommandStages`, idempotent idle stop, motion clearing, and stage cancellation.
- `bundle://proof/SB02/transcripts/webgllib-focused-tests.txt`: `dotnet test tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore` passed 55 tests.
- `bundle://proof/SB02/transcripts/webgllib-runtime-boundary-audit.txt`: runtime audit passed; warnings are existing line-count warnings in older runtime modules.
- `bundle://proof/SB02/transcripts/components-build-after-runtime-stop.txt`: solution build passed with zero warnings and zero errors.
- `bundle://proof/SB02/transcripts/runtime-stop-playwright.txt`: browser proof called the public `window.CanDoItAll.webglScene.stopRuntimeActivity` facade twice.

## Browser proof summary

`bundle://proof/SB02/browser/runtime-stop-assertions.json` records:

- runtime work observed before stop: `queuedCommandStageCount = 1`, active wait barrier `wait-seconds`;
- first stop result succeeded and cleared active/queued runtime work;
- diagnostics after first stop: `activeMotionCount = 0`, `queuedMotionCount = 0`, `queuedCommandStageCount = 0`, `commandStageBarrierPolicy = ""`;
- first stop recorded `lastRuntimeStopReason = "sb02-proof-stop"` and `lastRuntimeStopCancelledCommandStageCount = 2`;
- second stop succeeded and stayed idle with `lastRuntimeStopReason = "sb02-proof-stop-again"`.

## Semantic adequacy gate

- Shallow-pass trap: a facade-only implementation could return success while leaving JS command stages or motions alive.
- Failing-first baseline: `bundle://proof/SB01/browser/failing-first-pause-assertions.json` and `bundle://proof/SB01/transcripts/failing-first-pause-playwright.txt` reproduced the active queued-stage gap before the runtime stop API existed.
- Adversarial negative proof: `repo://tools/webgllib/audit-runtime-stop.cjs` checks active motion, queued motion, active barrier, queued command stage, and second-stop idle semantics; `cancelCommandStages` is also verified to leave motions untouched.
- Semantic positive proof: browser proof uses `/run-playback` and the real public JS facade to clear queued stage work and record diagnostics.
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`.
- Raw-note closure: F03 is solved; F01 remains partially solved until SB03 wires RunPlayback pause/cancel into the new API.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `runtimeStopCount` | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/39-webgl-scene-runtime-stop.js` increments on every runtime stop. | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js`, `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js`, `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs`, `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneProofSnapshot.cs`. | Created when `stopRuntimeActivity` runs; visible in diagnostics/proof snapshots until scene reset/import. | `bundle://proof/SB02/transcripts/runtime-stop-audit.txt` proves second stop increments the counter while state stays idle. |
| `lastRuntimeStopReason` | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/39-webgl-scene-runtime-stop.js` writes normalized reason. | Same diagnostics/proof consumers plus browser assertions. | Updated on each stop call. | `bundle://proof/SB02/browser/runtime-stop-assertions.json` proves first and second stop reasons are distinct and recorded. |
| `clearedMotionCount` and `lastRuntimeStopClearedMotionCount` | Runtime stop module counts active plus queued motions before clearing. | Diagnostics/proof snapshots and C# DTOs. | Cumulative count and last-stop count update on stop; zero on idle second stop. | `bundle://proof/SB02/transcripts/runtime-stop-audit.txt` starts with one active and two queued motions, then proves second stop reports zero new clears. |
| `lastRuntimeStopCancelledCommandStageCount` | Runtime stop and stage cancel module count active barrier plus queued stages before cancel. | Diagnostics/proof snapshots and C# DTOs. | Updated on stop or stage-only cancel. | `bundle://proof/SB02/transcripts/runtime-stop-audit.txt` proves stage-only cancel clears stages while preserving motion activity. |
