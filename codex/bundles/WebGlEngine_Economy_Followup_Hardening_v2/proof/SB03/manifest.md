# SB03 proof manifest

Status: Completed.

Owned requirements: R03, R14 support. SB03 also contributes `/run-playback` evidence for R12/SB11.

Raw notes: `bundle://analysis/02-critical-findings.md`, `bundle://architecture/03-run-frame-command-semantics.md`, `bundle://requirements/01-normalized-requirements.md`.

Semantic invariant contract: `bundle://proof/SB03/semantic-invariants.md`.

## Changed file hashes

| File | Before SHA-256 | After SHA-256 | Evidence |
| --- | --- | --- | --- |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunFrameCommandPolicy.cs` | `absent` | `10f10cba48268f640c4cdf59054f5967100aae02b848c96db14ae213042a4d05` | `bundle://proof/SB03/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs` | `0ed2546578b5363e8b130861ffb3441e332ff2de3d7bbbb6d016c941f7a9eb16` | `7ef04edbde24759721089df598986be53a30ec95919db2dda0cf541600076dcb` | `bundle://proof/SB03/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunFrameApplyResult.cs` | `b58f6052fa0befa08fb36a89991e93995a2d496710617da996f5dcfa8313572c` | `2e7d89195df573503bcf1bf9cf7672efb05208cb639f02a9cd1cbb8b21bc3769` | `bundle://proof/SB03/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/WebGlRunActionCompiler.cs` | `51bc838b3d0ddcd36f7fc98f4591facafaab403b80740586e1f637fb9f4e68a0` | `9e5b08bc7be5ef2d4623f51976c796cc809e02b950d56dde91433cf169f6e780` | `bundle://proof/SB03/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackClock.cs` | `050c56e6f3bcac1e2b91fbcc3b580e3f197141d6ccd78d9bf96813331b2f6167` | `e49978107a33618b2d39f441d4a6417c1ee4b199a3461605395d8c7eb7b117be` | `bundle://proof/SB03/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunActionCompilerTests.cs` | `77380821bbf007bb64fb1e1f1ba3b20c52fca720f7555c2d5828301bade2e0ef` | `af36926598ca26da4b33f4eaa69e9cef7ea732f30435c215b1c0d17248961a4e` | `bundle://proof/SB03/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunPlaybackControllerTests.cs` | `7e15ae5e9b04266ca76e1dff14dae572ceed63248ad3d126f608fbd915fb57a1` | `c875cc8fb03bbcf73e2adc3719267b49f0b15a990949324d584edf28dfd468e3` | `bundle://proof/SB03/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunValidatorTests.cs` | `dd82e8995bf236da8e03623f429238dd0ad0090e62ffe2d14772182d490f5236` | `fc4d43c37795412f0985f1dc971d6722378b3922e96f760069f25fe24710219c` | `bundle://proof/SB03/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs` | `60315336b5fc9d9d7305bf01d433b349810424023b7cfc2e5ff5c797ed3b3f1d` | `3036b3db622e4781cc87c0c89fb3176eded08a188338c39bb8f7ce5394530330` | `bundle://proof/SB03/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeTests.cs` | `f624aae7223b80e0eef6565b02fd55e0051fcb1cf97599b510d9256a7efc4c2c` | `c5d3f44f6b7f66748b36532a8b9191580dadd771308126fd2388b234fb2acfbe` | `bundle://proof/SB03/transcripts/changed-file-before-after-hashes.txt` |

## Command transcripts

| Command / action | Result | Transcript |
| --- | --- | --- |
| Failing-first Components validator/apply-result tests for mixed direct+staged frames | Fail before implementation: validator accepted mixed frame and apply result had no error | `bundle://proof/SB03/transcripts/failing-first-components-mixed-frame-policy-tests.txt` |
| Failing-first Economy projection compliance test | Fail before implementation: Economy projector duplicated staged motion into frame-level motions | `bundle://proof/SB03/transcripts/failing-first-economy-staged-only-projection-test.txt` |
| Failing-first source scan for old staged-command mirror assignments in git HEAD | Pass as negative proof: found four old mirror assignments across Components compiler and Economy bridge | `bundle://proof/SB03/transcripts/failing-first-source-command-mirror-scan.txt` |
| Focused Components policy tests | Pass, 5 tests | `bundle://proof/SB03/transcripts/passing-components-mixed-frame-policy-tests.txt` |
| Focused Economy staged-only projection test | Pass, 1 test | `bundle://proof/SB03/transcripts/passing-economy-staged-only-projection-test.txt` |
| Full WebGlRunLib test project after staged-only compiler fix | Pass, 36 tests | `bundle://proof/SB03/transcripts/passing-components-webglrunlib-tests-after-compiler-staged-only.txt` |
| Economy bridge/session focused tests after staged-only compiler fix | Pass, 30 tests | `bundle://proof/SB03/transcripts/passing-economy-webglbridge-focused-tests-after-compiler-staged-only.txt` |
| Components solution build | Pass, 0 warnings, 0 errors | `bundle://proof/SB03/transcripts/passing-components-dotnet-build.txt` |
| Components WebGlRunLib boundary audit | Pass | `bundle://proof/SB03/transcripts/passing-components-webglrunlib-boundary-audit.txt` |
| Components WebGlSandbox build after staged-only compiler fix | Pass, 0 warnings, 0 errors | `bundle://proof/SB03/transcripts/passing-components-webglsandbox-build-after-compiler-staged-only.txt` |
| Full Economy solution tests | Pass, 550 tests | `bundle://proof/SB03/transcripts/passing-economy-tests-full-after-frame-policy.txt` |
| Economy simulation boundary audit | Pass | `bundle://proof/SB03/transcripts/passing-economy-simulation-boundary-audit-after-frame-policy.txt` |
| Anti-stub scan on touched production paths | Pass | `bundle://proof/SB03/transcripts/passing-anti-stub-scan.txt` |
| Source scan for old staged-command mirror assignments | Pass, no hits | `bundle://proof/SB03/transcripts/passing-source-command-mirror-removed-scan.txt` |
| Components sandbox server start/stop for browser proof | Pass, server started on `http://127.0.0.1:5098` and stopped after proof | `bundle://proof/SB03/transcripts/components-sandbox-server-start-after-restart.txt`, `bundle://proof/SB03/transcripts/components-sandbox-server-stop-after-browser-proof.txt` |

## Browser artifacts

| Artifact | Result |
| --- | --- |
| `bundle://proof/SB03/browser/run-playback-assertions-before-batch-v2.json` | `/run-playback` ready, canvas present, no mixed-frame policy error before action. |
| `bundle://proof/SB03/browser/run-playback-assertions-after-batch.json` | Clicked `Batch frame`; frame 4 applied, 24 commands, 24 stages, 23 queued stages, canvas present, `batchProofApplied=true`, no mixed-frame policy error. |
| `bundle://proof/SB03/browser/run-playback-console-after-batch.txt` | 0 browser errors, 0 browser warnings. |
| `bundle://proof/SB03/browser/run-playback-after-batch.png` | Full-page screenshot after batch frame proof. |
| `bundle://proof/SB03/browser/run-playback-snapshot-after-batch.md` | Accessibility/DOM snapshot after batch frame proof. |

## Source assertions

| Assertion | Evidence |
| --- | --- |
| Mixed direct frame-level commands plus staged commands are invalid before playback. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunFrameCommandPolicy.cs`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs`, `bundle://proof/SB03/transcripts/passing-components-webglrunlib-tests-after-compiler-staged-only.txt` |
| `WebGlRunFrameApplyResult.FromFrame` reports the mixed-frame policy error if called directly, so command loss is no longer silent. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunFrameApplyResult.cs`, `bundle://proof/SB03/transcripts/passing-components-webglrunlib-tests-after-compiler-staged-only.txt` |
| Generic action compiler output is staged-only for staged commands and no longer mirrors stage commands onto frame-level lists. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/WebGlRunActionCompiler.cs`, `bundle://proof/SB03/transcripts/passing-source-command-mirror-removed-scan.txt` |
| Timeline identity includes staged command payloads, not only direct frame-level lists. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackClock.cs`, `bundle://proof/SB03/transcripts/passing-components-webglrunlib-tests-after-compiler-staged-only.txt` |
| Economy bridge frames with stages no longer duplicate commands onto frame-level lists. | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs`, `bundle://proof/SB03/transcripts/passing-economy-webglbridge-focused-tests-after-compiler-staged-only.txt` |

## Anti-stub audit

Touched production paths have no `TODO`, `NotImplemented`, `proof-only`, `placeholder success`, `Fixtures`, or `ExperimentInputs` hits: `bundle://proof/SB03/transcripts/passing-anti-stub-scan.txt`.

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Mixed-frame command policy | `WebGlRunFrameCommandPolicy` | `WebGlRunDocumentValidator`, `WebGlRunFrameApplyResult.FromFrame` | Checked during document validation and direct frame apply-result generation | Failing-first Components test accepted mixed frame and emitted no apply error; passing tests reject/report it. |
| Staged-only action compiler output | `WebGlRunActionCompiler` | WebGlRun playback, sandbox `/run-playback`, downstream bridge consumers | Generated whenever action plans compile to stages | Browser proof initially exposed old mixed sample behavior; final browser assertion shows no mixed policy error and batch frame applies. |
| Staged command timeline identity | `WebGlRunPlaybackClock.BuildDeterministicTimelineIdentity` | Playback diagnostics/caches that compare timeline identity | Recomputed from run document frames and staged command payloads | New test mutates a staged motion target and proves identity changes. |
| Economy staged-only projection | `EconomyWebGlActionStageProjector` | Economy sandbox/session/WebGL bridge validators | Produced for each projected visual frame | Failing-first Economy test showed duplicated frame-level motion; passing test and full Economy suite prove compliance. |

## Gate decision

Pass. SB03 chose the reject-mixed policy, enforced it in validation and direct apply-result generation, removed staged-command mirroring from both the generic compiler and Economy bridge, updated identity hashing to include staged command payloads, and proved `/run-playback` still applies the batch proof frame with 24 commands/24 stages and zero browser console errors or warnings.
