# SB07 proof manifest

Status: Completed 2026-06-02.

## Changed file hashes

| SHA-256 | Path |
| --- | --- |
| `7fda37c0b97e4fa9588b1d39f8c0fd3a291473a2bf4426636d812ef3c722fcf3` | `docs/webgl/run-layer-boundary.md` |
| `3507b39f7de469235bbfbed38a5eae69212e4c77f006b335e2c6776684ac4494` | `src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunFrameExecutionValidator.cs` |
| `af4502721a90f6167e10d55bfa16fc6cbc10bcfd9172523216b4729f1c5d5c50` | `tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunDocumentRunnerTests.cs` |
| `182ba8f2747c899f98cd49aaf07e9ae74ab19457e4b61ffcc7a05ac6736efc20` | `codex/bundles/WebGlEngine_Economy_Followup_Hardening_v2/proof/SB07/scenario-inventory.md` |
| `979041eeeda3d0a402f069f506f9a442a45630a8c3f7834cfc741ffe3ffd8ef7` | `../CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunValidator.cs` |
| `40f23a6059742d3db6ed5d92e9c914130bf53078b3708011c43d74503e8a7208` | `../CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.cs` |
| `981103342cce118d5204c15ca9d2f82e56dd0afd316941e830b86288982bd7f0` | `../CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.Helpers.cs` |

## Command transcripts

| Command | Transcript | Exit | Result |
| --- | --- | --- | --- |
| Focused Economy dynamic-object validator tests before implementation | `bundle://proof/SB07/transcripts/failing-first-economy-dynamic-object-validator-tests.txt` | 1 for `dotnet test` | Failing-first: add-object-then-motion was rejected against the initial scene object set. |
| Focused Economy dynamic-object validator tests after implementation | `bundle://proof/SB07/transcripts/passing-economy-dynamic-object-validator-tests.txt` | 0 | 2 tests passed: earlier-stage add then later motion accepted; same-stage motion to new object rejected. |
| Focused Components direct frame motion test before implementation | `bundle://proof/SB07/transcripts/failing-first-components-direct-frame-motion-validation.txt` | 1 for `dotnet test` | Failing-first: direct frame-level motion to an unknown object applied successfully. |
| Focused Components direct frame motion test after implementation | `bundle://proof/SB07/transcripts/passing-components-direct-frame-motion-validation.txt` | 0 | Generic runtime rejects unresolved direct frame-level motions before applying the frame. |
| Economy scenario dynamic-object scan | `bundle://proof/SB07/transcripts/economy-scenario-dynamic-object-scan.txt` | 0 | No add/remove object WebGlRun patch payloads found in shipped scenario inputs or fixtures. |
| `dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj` | `bundle://proof/SB07/transcripts/components-webglrunlib-tests.txt` | 0 | 42 WebGlRunLib tests passed. |
| Economy bridge-family tests | `bundle://proof/SB07/transcripts/economy-webglbridge-tests.txt` | 0 | 26 Economy WebGL bridge tests passed with local Components project references. |
| `dotnet build CanDoItAll.Components.slnx` | `bundle://proof/SB07/transcripts/components-build.txt` | 0 | Components solution build passed with 0 warnings and 0 errors. |
| `npm run webglrunlib:audit-boundary` | `bundle://proof/SB07/transcripts/components-webglrunlib-boundary-audit.txt` | 0 | WebGlRunLib boundary audit passed. |
| `npm run webgllib:audit-boundary` | `bundle://proof/SB07/transcripts/components-webgllib-boundary-audit.txt` | 0 | WebGlLib boundary audit passed. |
| Components `git diff --check` | `bundle://proof/SB07/transcripts/components-git-diff-check.txt` | 0 | No whitespace errors; only existing LF-to-CRLF warnings. |
| Economy `git diff --check` | `bundle://proof/SB07/transcripts/economy-git-diff-check.txt` | 0 | No whitespace errors; only existing LF-to-CRLF warnings. |
| `python scripts/validate_bundle.py --stage prepared --profile initiative` | `bundle://proof/SB07/transcripts/bundle-validator-after-sb07.txt` | 0 | Bundle validator passed after SB07 proof/doc updates. |
| SB07 proof placeholder scan | `bundle://proof/SB07/transcripts/sb07-proof-placeholder-scan.txt` | 0 | No stale SB07 proof placeholders found. |

## Browser artifacts

N/A. SB07 changed validators and tests only; no browser-visible runtime or UI behavior changed.

## Source assertions

`bundle://proof/SB07/transcripts/source-policy-assertions.txt` records:

- Economy validator tracks an evolving known-object id set.
- Economy validator applies add/remove object patch effects and validates stages in playback order.
- Positive earlier-stage dynamic object and negative same-stage motion tests exist.
- Generic runtime validates direct frame-level motions.
- Boundary documentation names the dynamic object policy.
- Scenario inventory records current shipped scenarios are static.

## Anti-stub audit

`bundle://proof/SB07/transcripts/changed-file-placeholder-scan.txt` passed with no `TODO`, `stub`, `placeholder`, or `NotImplementedException` markers in SB07 changed production/test/doc files. `bundle://proof/SB07/transcripts/sb07-proof-placeholder-scan.txt` passed with no stale SB07 proof placeholders.

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Dynamic known-object set | `EconomyWebGlRunValidator` | Economy bridge validation callers | Starts with initial scene ids, then applies object additions/removals from patches as frames/stages are validated | Failing-first Economy test rejected an object added by an earlier stage. |
| Same-stage motion rejection | Economy dynamic-object tests and validator motion-before-patch order | Bridge validators and future scenario authors | Motions in a stage must target objects known before that stage; new objects can be moved in later stages/frames | Negative test rejects same-stage motion to the newly added object. |
| Direct frame motion validation | `WebGlRunFrameExecutionValidator` | `WebGlRunDocumentRunner` | Direct frame-level motions are validated before frame-level patches are applied | Failing-first Components test applied an unresolved direct frame motion. |
| Scenario dynamic-object inventory | `proof/SB07/scenario-inventory.md` | SB10 docs and SB11 route proof | Current shipped Economy scenarios remain static; future scenarios have a documented extension path | Scenario scan found no add/remove object patch payloads in current scenario inputs/fixtures. |

## Gate decision

Pass. SB07 commits to dynamic-supported validation with ordered object-id evolution, keeps same-stage motion dependencies invalid, records current scenario inventory, and preserves Components generic boundaries.
