# SB05 proof manifest

Status: Completed 2026-06-02.

## Changed file hashes

| SHA-256 | Path |
| --- | --- |
| `1c87080e199c83cea28cf645dfdad71ddb78cdb8b7aa5fd85122ab0b99db304a` | `src/CanDoItAll.Components.WebGlLib/WebGl/Patching/WebGlScenePatch.cs` |
| `6cb3bb349c36a8e5c6a72da1464d5401a0399c5a31df0efcad6c1dc6d9f4f47a` | `src/CanDoItAll.Components.WebGlLib/WebGl/Patching/WebGlScenePatchReducer.cs` |
| `dab8cc1c989a60a1f749b7743f4a1eca60c430048288d4664470e3059d8cf1ca` | `src/CanDoItAll.Components.WebGlLib/WebGl/Patching/WebGlScenePatchPolicy.cs` |
| `1d8a2fb15eb11f225bae507c0d47e9c8eb73ce45e68f6b85a0e0f4ccad931161` | `src/CanDoItAll.Components.WebGlLib/WebGl/Patching/WebGlScenePatchResult.cs` |
| `2d4ac8608fecb98f02f426b04be06702c0c4a602c005a107bc98964958a88aa0` | `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js` |
| `9059ec1398068437d5182b1a973687e0cebd2bf27a98402ba10f512e78c8b60d` | `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/37-webgl-scene-patch-policy.js` |
| `eb079fc2192aff421a28139386905293036c13e43d9917697022dc848abf2309` | `tests/CanDoItAll.Components.WebGlLib.Tests/WebGlScenePatchReducerTests.cs` |
| `356d0d8aea53ef09fc944615d494db91770bff78b40676fbd93db2793d77b5bd` | `src/CanDoItAll.Components.WebGlSandbox/Components/Pages/TycoonVillage.razor` |
| `75e5c236cbd284abd4d7cf5724c96dec7903045572412bc4849b94189d5ad894` | `src/CanDoItAll.Components.WebGlSandbox/Components/Pages/TycoonVillage.razor.cs` |
| `dc1f5b12bab81c5aef431f3e330fa09f9414ecf3c3de1f9f09b281cecca845f8` | `src/CanDoItAll.Components.WebGlSandbox/Components/Pages/TycoonVillageInspectorPanel.razor` |
| `075b5e5790c995a933b9c67715bb3552a73abab271ee98bbd32b3042723573f7` | `docs/webgl/run-layer-boundary.md` |

## Command transcripts

| Command | Transcript | Exit | Result |
| --- | --- | --- | --- |
| Focused reducer tests before implementation | `bundle://proof/SB05/transcripts/failing-first-patch-transaction-tests.txt` | 1 | Failing-first: explicit permissive mode was not recognized and strict results had no metadata. |
| Focused reducer tests after implementation | `bundle://proof/SB05/transcripts/passing-focused-csharp-patch-transaction-tests.txt` | 0 | Strict all-or-none and permissive invalid-link metadata tests passed. |
| `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj` | `bundle://proof/SB05/transcripts/components-webgllib-tests.txt` | 0 | 48 WebGlLib tests passed. |
| `dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj` | `bundle://proof/SB05/transcripts/components-webglrunlib-tests.txt` | 0 | 37 WebGlRunLib tests passed. |
| `dotnet build CanDoItAll.Components.slnx` | `bundle://proof/SB05/transcripts/components-build.txt` | 0 | Components solution build passed with 0 warnings and 0 errors. |
| `npm run webgllib:audit-scene-runtime` | `bundle://proof/SB05/transcripts/components-webgllib-scene-runtime-audit.txt` | 0 | Scene runtime audit passed with existing warning-level line-count notes; hard thresholds pass after helper split. |
| `npm run webgllib:audit-boundary` | `bundle://proof/SB05/transcripts/components-webgllib-boundary-audit.txt` | 0 | WebGlLib boundary audit passed. |
| `python scripts/validate_bundle.py --stage prepared --profile initiative` | `bundle://proof/SB05/transcripts/bundle-validator-after-sb05.txt` | 0 | Bundle validator passed after SB05 proof/doc updates. |
| SB05 proof placeholder scan | `bundle://proof/SB05/transcripts/sb05-proof-placeholder-scan.txt` | 0 | No stale SB05 proof placeholders found. |

## Browser artifacts

| Artifact | Path | Result |
| --- | --- | --- |
| Browser assertion summary | `bundle://proof/SB05/browser/assertions.md` | Strict and warning patch mode assertions recorded. |
| Initial runtime state JSON | `bundle://proof/SB05/transcripts/browser-initial-runtime-state.json` | `agent.helper` initial position and proof-link absence recorded. |
| Strict runtime assertions JSON | `bundle://proof/SB05/transcripts/browser-strict-runtime-assertions.json` | Strict bad-link patch failed, did not move object, and added no proof links. |
| Warning runtime assertions JSON | `bundle://proof/SB05/transcripts/browser-warning-runtime-assertions.json` | Warning bad-link patch applied valid object/link changes and skipped invalid link. |
| Screenshot | `bundle://proof/SB05/browser/tycoon-village-patch-transaction-proof.png` | Full-page browser screenshot after warning-mode proof. |
| Console log | `bundle://proof/SB05/browser/tycoon-village-console.txt` | 0 errors, 0 warnings. |
| Server stdout/stderr | `bundle://proof/SB05/transcripts/webglsandbox-server-stdout.txt`, `bundle://proof/SB05/transcripts/webglsandbox-server-stderr.txt` | Sandbox served `http://localhost:5099/tycoon-village`; server stopped after proof. |

## Source assertions

`bundle://proof/SB05/transcripts/source-policy-assertions.txt` records:

- `WebGlScenePatchTransactionModes.Strict` and `PermissiveInvalidLinks`.
- C# result metadata keys `patchTransactionMode`, `missingLinkEndpointMode`, `patchClassification`, and `skippedLinkIds`.
- JS policy module `37-webgl-scene-patch-policy.js` resolving transaction mode and recording skipped links.
- Browser route buttons and data-testid fields for strict/warning proof.
- Documentation of strict and permissive-invalid-link modes.

## Anti-stub audit

`bundle://proof/SB05/transcripts/changed-file-placeholder-scan.txt` passed with no `TODO`, `stub`, `placeholder`, or `NotImplementedException` markers in SB05 production/doc changed files. `bundle://proof/SB05/transcripts/sb05-proof-placeholder-scan.txt` passed with no stale SB05 proof placeholders.

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Strict patch transaction mode | `WebGlScenePatchPolicy`, JS patch policy module | C# reducer, JS patch runtime, browser route | Default for all patches unless explicit permissive mode is selected | Failing-first strict test showed result metadata was absent; browser strict proof confirms no mutation after invalid link. |
| Permissive invalid-link mode | Patch metadata `patchTransactionMode=permissive-invalid-links` or `missingLinkEndpointMode=warn` | C# reducer and JS runtime | Applies valid object/link changes; skips invalid added links with warning | Failing-first permissive test failed because explicit mode was not recognized. |
| Patch result metadata | C# `WebGlScenePatchResult.Metadata`, JS command result metadata | Tests, browser UI, downstream command consumers | Includes transaction mode, missing-link endpoint mode, patch classification, and skipped link ids when applicable | Focused tests and browser assertions fail if keys are absent or stale. |
| Browser proof controls | `/tycoon-village` sandbox route | Playwright proof and future manual verification | Strict/warn buttons issue production JS patch calls through `WebGlSceneView.ApplyPatchDetailedAsync` | Console and runtime assertion artifacts prove production path behavior, not a fixture-only shortcut. |

## Gate decision

Pass. SB05 names and enforces strict and permissive invalid-link patch modes across C# and JS, records failing-first and passing semantic proof, includes browser proof with assertions and console status, and preserves WebGlLib boundaries.
