# SB06 Proof Manifest

Subbundle: `SB06-scene-document-diagnostics-consistency`
Status: `Completed`
Prepared by: Codex
Completed at UTC: `2026-06-02T02:50:00Z`

## Changed File Hashes

| Repo | Path | Before SHA-256 | After SHA-256 | Why changed |
| --- | --- | --- | --- | --- |
| CanDoItAll.Components | `repo://CanDoItAll.Components/README.md` | `24c604c574666eb536044e4dfb4d61ba3eb0be20fcb9451220789cef37b56b17` | `a5d75f6367db0a90e2740d2bf4b0d1fcac3cbf6561d754d51eaae0a5219a5d38` | Added `CanDoItAll.Components.WebGlRunLib` to the root package map. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md` | `50b5ec0c58f5f84b66921e8f34068e84895fcb200ab7d258cd89b9d5489e0bcf` | `4a577b0f7de0aa0089123b90c1f8161b9df7ac6305fcaad5fd1e1424a20f1f07` | Documented live scene validation, canonical layer membership, and the generic WebGlLib boundary. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocumentMetadataPolicy.cs` | `e20b174f13ebcdcf36b01b38830137e8584a48a01b6ee9ebab7ddb681c3107dc` | `a391376cc551459ab9691c04f1d3da6c9bc2fcb7408135194e8032aebe34f126` | Exposed scene-scoped metadata enumeration so live scene validation checks the same forbidden keys as document validation. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocumentValidator.cs` | `0004d402d7e93c10350b894a165a6db27ca63533890777dc28b6a6db5fbf6aef` | `dd5b590bbd60e5e519bac622ecb29f3115f79ede61390fc219482a6cb6784375` | Added reusable scene validation and layer duplicate/stale membership diagnostics. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneModelValidator.cs` | `NEW` | `b9728f80391d2dcbc75681d513eaf4a85746ce0c8052b1c64bb0182f45d7c85d` | Added a live scene validator for callers that do not wrap scenes in persisted documents. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs` | `5f01b310a2077598e5b5acd2ee07bc41d85967d6591cea8958c068faa9d3e12a` | `da952983833e39c4e86714dd5fa7e3e418db26a5e852c42921ea96751ad3bac5` | Added typed link-sync counters emitted by JS diagnostics. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneProofSnapshot.cs` | `510a7790de953e3235864985c08cddfe1c579825347ab3d2189dc25216cb3790` | `92b495104122d1949784dc9af9af1d6773b37fa5c72766d02445a9d44b3a2e2a` | Added proof snapshot fields for link-sync counters. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlLib.Tests/WebGlSceneDocumentSerializerTests.cs` | `5989dde5874aec71fdd9e885677e6bf67f378d6c932669788f48291ac127baa0` | `8e813b902e434af65707ecd4fb7f7feb23919f7d3be525253b5c0814b393cf72` | Added failing-first and positive coverage for duplicate/stale layer membership and live scene validation. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlLib.Tests/WebGlRuntimeDiagnosticsTests.cs` | `3d4f476b2f80b1e1072c134bb53196fc4041c9201c532543670b4b0ebd4f7cc4` | `b53dbde6467509c4d7d207a49835278df9387f4c175c3199e836172bda483f74` | Added typed diagnostics and proof snapshot round-trip tests, including a browser diagnostics capture shape. |

## Commands

| Command | Working directory | Transcript path | Result |
| --- | --- | --- | --- |
| `dotnet test tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj --filter "Document|Layer|Validator"` | `repo://CanDoItAll.Components` | `bundle://proof/SB06/transcripts/failing-first-layer-validator.txt` | Failed before the validator change because duplicate/stale layer warnings were absent. |
| JS/C# diagnostics parity scan for `buildDiagnosticsSnapshot` keys | `repo://CanDoItAll.Components` | `bundle://proof/SB06/transcripts/passing-diagnostics-parity-scan.txt` | Passed with `missing=[]`. |
| `dotnet test tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj --filter "Document|Diagnostics|Layer|Validator"` | `repo://CanDoItAll.Components` | `bundle://proof/SB06/transcripts/passing-document-diagnostics-layer-validator-tests.txt` | Passed: 20 tests. |
| `dotnet build CanDoItAll.Components.slnx` | `repo://CanDoItAll.Components` | `bundle://proof/SB06/transcripts/components-solution-build.txt` | Passed with 0 warnings and 0 errors. |
| `dotnet test CanDoItAll.Components.slnx --no-build` | `repo://CanDoItAll.Components` | `bundle://proof/SB06/transcripts/components-solution-test-no-build.txt` | Passed: WebGlLib 44 tests, WebGlRunLib 28 tests. |
| Source assertion scan | `repo://CanDoItAll.Components` | `bundle://proof/SB06/transcripts/sb06-source-assertions.txt` | Passed; validator, layer cleanup, docs, DTOs, and tests located. |
| Anti-stub and boundary scan | `repo://CanDoItAll.Components` | `bundle://proof/SB06/transcripts/sb06-anti-stub-and-boundary-scan.txt` | Passed; no stub shortcuts or economy/domain leakage in SB06 production code. |
| `python scripts\validate_bundle.py --stage execution --profile initiative` | `bundle://` | `bundle://proof/SB06/transcripts/bundle-validate-execution.txt` | Passed for 14 subbundles. |
| Filtered `git diff --check` | `repo://CanDoItAll.Components` | `bundle://proof/SB06/transcripts/git-diff-check.txt` | Passed with no non-line-ending findings. |
| SB06 placeholder scan | `repo://CanDoItAll.Components` | `bundle://proof/SB06/transcripts/sb06-placeholder-scan.txt` | Passed with no template leftovers. |

## Source Assertions

| Assertion | Source path | Line/range or search term | Verified by |
| --- | --- | --- | --- |
| Persisted document validation now delegates to reusable scene validation. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocumentValidator.cs` | `ValidateScene(document.Scene, result)` | `bundle://proof/SB06/transcripts/sb06-source-assertions.txt` |
| Live scene validation is available without persisted document wrapping. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneModelValidator.cs` | `WebGlSceneModelValidator` | `bundle://proof/SB06/transcripts/sb06-source-assertions.txt` |
| Layer `ObjectIds` are checked for duplicate and stale entries, with scene objects as canonical membership source. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocumentValidator.cs` | `ValidateLayers`, `canonical membership source` | `bundle://proof/SB06/transcripts/sb06-source-assertions.txt` |
| Object removal cleans layer membership references. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Patching/WebGlScenePatchReducer.cs` | `layer.ObjectIds.RemoveAll` | `bundle://proof/SB06/transcripts/sb06-source-assertions.txt` |
| JS diagnostics link-sync counters have typed C# DTO fields. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs` | `LinksUpdatedLastFrame`, `LinkSyncScanCount`, `LinkSyncIndexedHitCount` | `bundle://proof/SB06/transcripts/passing-diagnostics-parity-scan.txt` |
| Proof snapshots also retain link-sync counters. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneProofSnapshot.cs` | `LinksUpdatedLastFrame`, `LinkSyncScanCount`, `LinkSyncIndexedHitCount` | `bundle://proof/SB06/transcripts/sb06-source-assertions.txt` |
| Root package map includes WebGlRunLib. | `repo://CanDoItAll.Components/README.md` | `CanDoItAll.Components.WebGlRunLib` | `bundle://proof/SB06/transcripts/sb06-source-assertions.txt` |

## Semantic Adequacy Gate

| Item | Required evidence | Status | Artifact |
| --- | --- | --- | --- |
| Shallow-pass trap named | A validator that only checks object/link IDs would miss duplicate layer membership, stale layer object IDs, and live scene forbidden metadata. | Passed | `bundle://proof/SB06/semantic-invariants.md` |
| Adversarial negative proof | Duplicate/stale layer membership test failed before the validator change. | Passed | `bundle://proof/SB06/transcripts/failing-first-layer-validator.txt` |
| Semantic positive proof | Focused tests cover duplicate layer membership, stale layer object IDs, invalid vectors, missing assets, diagnostics deserialization, live scene validation, and browser-shaped diagnostics. | Passed | `bundle://proof/SB06/transcripts/passing-document-diagnostics-layer-validator-tests.txt` |
| Anti-stub audit | Stub/TODO scan and production boundary scan clean for SB06 touched files. | Passed | `bundle://proof/SB06/transcripts/sb06-anti-stub-and-boundary-scan.txt` |
| Raw-note closure | REQ-001, REQ-008, and REQ-009 SB06 slices mapped in traceability and execution report. | Passed | `bundle://traceability/01-requirement-traceability.md`, `bundle://reviews/01-execution-report.md` |
| Downstream smoke | Solution build and no-build WebGlLib/WebGlRunLib tests pass. | Passed | `bundle://proof/SB06/transcripts/components-solution-build.txt`, `bundle://proof/SB06/transcripts/components-solution-test-no-build.txt` |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Layer membership warnings | `WebGlSceneDocumentValidator.ValidateLayers` | `WebGlSceneDocumentSerializer.Validate`, `WebGlSceneModelValidator.Validate`, callers inspecting `WebGlSceneDocumentValidationResult.Warnings` | Computed during validation from scene objects and layer `ObjectIds`; no persistence mutation is required. | `bundle://proof/SB06/transcripts/failing-first-layer-validator.txt` |
| Live scene validation result | `WebGlSceneModelValidator.Validate` | Runtime or host code validating `WebGlSceneModel` directly | Creates a fresh validation result and applies the shared scene validator. | `bundle://proof/SB06/transcripts/passing-document-diagnostics-layer-validator-tests.txt` |
| Typed link-sync diagnostics counters | JS `buildDiagnosticsSnapshot`, C# `WebGlRuntimeDiagnostics`, C# `WebGlSceneProofSnapshot` | Interop diagnostics consumers and proof snapshot deserializers | JS emits camelCase diagnostics; C# web defaults deserialize them into PascalCase DTO properties. | `bundle://proof/SB06/transcripts/passing-diagnostics-parity-scan.txt` |

## Browser Validation Analytics

| Route | Viewport | Actions | Screenshot/log artifacts | Result |
| --- | --- | --- | --- | --- |
| N/A | N/A | No browser-visible JS/runtime behavior changed in SB06. Browser diagnostics capture shape is covered by DTO deserialization tests and the JS/C# parity scan. | `bundle://proof/SB06/transcripts/passing-document-diagnostics-layer-validator-tests.txt`, `bundle://proof/SB06/transcripts/passing-diagnostics-parity-scan.txt`; prior real capture source: `bundle://proof/SB05/transcripts/passing-browser-resource-cache-proof.json` | Passed / no new browser run required. |

## Refactor Gate Result

- Touched files reviewed: yes; source assertion scan cites validators, DTOs, docs, and tests.
- Duplicates removed: document and live scene validation share `ValidateScene`.
- Layering checked: no Economy/domain leakage in SB06 production code.
- Fixture-specific code removed: no fixture-only branches found.
- Docs/tests updated: yes; root package map, WebGlLib README, unit tests, and diagnostics parity proof updated.
- Remaining refactor risk: low; public DTO additions are backward compatible additive fields.
