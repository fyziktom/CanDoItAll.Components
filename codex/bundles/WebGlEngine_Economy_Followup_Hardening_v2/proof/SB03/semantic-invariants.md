# SB03 semantic invariants

Status: Completed.

Chosen policy: Option A from `bundle://architecture/03-run-frame-command-semantics.md`. Mixed direct frame-level commands and staged commands are invalid.

| Invariant ID | Source raw note | Expected behavior | Disallowed shallow implementation | Failing-first / negative proof | Passing / positive proof | Source files | Downstream dependency |
| --- | --- | --- | --- | --- | --- | --- | --- |
| SB03-INV-001 | R03/F02 | A valid `WebGlRunFrame` cannot contain both direct frame-level `ScenePatches`/`Motions` and `Stages`. | Leave `FromFrame` silently dropping direct commands while only adding a warning in docs. | `bundle://proof/SB03/transcripts/failing-first-components-mixed-frame-policy-tests.txt` shows validator accepted mixed frames and apply result returned no error. | `bundle://proof/SB03/transcripts/passing-components-webglrunlib-tests-after-compiler-staged-only.txt` proves mixed frames are rejected/reported and direct-only/staged-only frames remain valid. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunFrameCommandPolicy.cs`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunFrameApplyResult.cs` | SB05 patch transaction proof and SB11 browser proof rely on unambiguous command batches. |
| SB03-INV-002 | R03/F02 | Generic action compilation produces staged-only frame commands when stages exist. | Reject mixed frames but keep compiler-generated samples invalid, making `/run-playback` fail before user action. | `bundle://proof/SB03/transcripts/failing-first-source-command-mirror-scan.txt` shows git HEAD mirrored staged commands onto frame-level lists in the generic compiler and Economy bridge. | `bundle://proof/SB03/transcripts/passing-source-command-mirror-removed-scan.txt`, `bundle://proof/SB03/browser/run-playback-assertions-after-batch.json`, and `bundle://proof/SB03/transcripts/passing-components-webglrunlib-tests-after-compiler-staged-only.txt` prove staged-only compiler output and successful batch playback. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/WebGlRunActionCompiler.cs`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackClock.cs` | SB11 `/run-playback` browser proof depends on this route staying executable. |
| SB03-INV-003 | R03/R14 | Economy bridge projections comply with the generic staged-only policy without duplicating commands to frame-level lists. | Fix only Components tests while leaving Economy bridge frames invalid under the new generic validator. | `bundle://proof/SB03/transcripts/failing-first-economy-staged-only-projection-test.txt` shows old bridge output had frame-level duplicate motions. | `bundle://proof/SB03/transcripts/passing-economy-webglbridge-focused-tests-after-compiler-staged-only.txt` and `bundle://proof/SB03/transcripts/passing-economy-tests-full-after-frame-policy.txt` prove staged-only bridge output and full-suite compatibility. | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs`, `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeTests.cs` | SB06/SB07 bridge validators and SB11 Economy browser proof rely on this generic contract. |

## Production assertions

- Mixed direct+staged frames are rejected by `WebGlRunDocumentValidator`.
- Direct callers of `WebGlRunFrameApplyResult.FromFrame` receive an error for mixed frames.
- Direct-only and staged-only frames remain valid.
- `WebGlRunActionCompiler` no longer mirrors staged commands into `frame.ScenePatches` or `frame.Motions`.
- `WebGlRunPlaybackClock` hashes staged command payloads for deterministic timeline identity.
- Economy WebGL bridge generated frames with stages keep frame-level command lists empty.
- `/run-playback` applies the batch proof frame with 24 commands and 24 stages, and no mixed-frame policy error.

## Anti-stub audit

See `bundle://proof/SB03/transcripts/passing-anti-stub-scan.txt`.

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative test |
| --- | --- | --- | --- | --- |
| Mixed-frame validation error | `WebGlRunFrameCommandPolicy` | Document validator and direct apply-result generation | Created whenever a mixed direct+staged frame is validated or converted to an apply result. | `bundle://proof/SB03/transcripts/failing-first-components-mixed-frame-policy-tests.txt` then `bundle://proof/SB03/transcripts/passing-components-webglrunlib-tests-after-compiler-staged-only.txt`. |
| Staged-only compiled frames | `WebGlRunActionCompiler` | WebGlRun playback and sandbox route | Produced from action plans when commands compile to stages. | `bundle://proof/SB03/transcripts/passing-source-command-mirror-removed-scan.txt`. |
| Staged-only bridge frames | `EconomyWebGlActionStageProjector` | Economy sandbox, bridge validators, browser route | Produced for each visual frame projection. | `bundle://proof/SB03/transcripts/failing-first-economy-staged-only-projection-test.txt` then `bundle://proof/SB03/transcripts/passing-economy-webglbridge-focused-tests-after-compiler-staged-only.txt`. |
| Batch playback browser diagnostics | `/run-playback` sandbox page | Browser proof and SB11 follow-up | Updated after the `Batch frame` action. | `bundle://proof/SB03/browser/run-playback-assertions-after-batch.json` shows `batchProofApplied=true`, command/stage counts 24, and `bodyHasMixedPolicyError=false`. |
