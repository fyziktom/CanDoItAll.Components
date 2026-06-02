# SB06 Semantic Invariants

Subbundle: `SB06-scene-document-diagnostics-consistency`  
Status: `Completed`

## Invariants

| Invariant | Claim | Evidence | Status |
| --- | --- | --- | --- |
| SB06-LAYER-001 | Scene objects are the canonical source for layer membership. Layer `ObjectIds` are grouping references and duplicate or stale entries are warnings, not hidden inconsistencies. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocumentValidator.cs`, `bundle://proof/SB06/transcripts/failing-first-layer-validator.txt`, `bundle://proof/SB06/transcripts/passing-document-diagnostics-layer-validator-tests.txt` | Passed |
| SB06-LAYER-002 | Object removal cleans stale layer references through the patch reducer, so validation and mutation agree on layer membership semantics. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Patching/WebGlScenePatchReducer.cs`, `bundle://proof/SB06/transcripts/sb06-source-assertions.txt`, `bundle://proof/SB03/transcripts/passing-dotnet-patch-document-revision.txt` | Passed |
| SB06-LIVE-001 | Live `WebGlSceneModel` instances can be validated without wrapping them in a persisted `WebGlSceneDocument`, and forbidden run-layer metadata is still rejected. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneModelValidator.cs`, `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlLib.Tests/WebGlSceneDocumentSerializerTests.cs`, `bundle://proof/SB06/transcripts/passing-document-diagnostics-layer-validator-tests.txt` | Passed |
| SB06-DIAG-001 | All JS `buildDiagnosticsSnapshot` keys have C# runtime diagnostics representation or established C# extras; no critical JS diagnostics counters are lost. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs`, `bundle://proof/SB06/transcripts/passing-diagnostics-parity-scan.txt` | Passed |
| SB06-DIAG-002 | Proof snapshots preserve link-sync counters needed by downstream browser/performance proof. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneProofSnapshot.cs`, `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlLib.Tests/WebGlRuntimeDiagnosticsTests.cs`, `bundle://proof/SB06/transcripts/passing-document-diagnostics-layer-validator-tests.txt` | Passed |
| SB06-DOCS-001 | Public docs name the WebGlRunLib package and explain validation/layer membership without importing Economy semantics into WebGlLib. | `repo://CanDoItAll.Components/README.md`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md`, `bundle://proof/SB06/transcripts/sb06-anti-stub-and-boundary-scan.txt` | Passed |

## Shallow-Pass Trap

A weak implementation could add only a public `WebGlSceneModelValidator` shell or only a diagnostics DTO field, while leaving layer duplicate/stale membership unreported and live scene metadata unchecked. SB06 rejects that trap with a failing-first layer validator test, shared validation code, typed diagnostics parity scan, and browser-shaped deserialization coverage.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Layer membership warnings | `WebGlSceneDocumentValidator.ValidateLayers` | `WebGlSceneDocumentSerializer.Validate`, `WebGlSceneModelValidator.Validate` | Produced each validation pass from current scene objects and layer references. | `bundle://proof/SB06/transcripts/failing-first-layer-validator.txt` |
| Live scene validation result | `WebGlSceneModelValidator.Validate` | Direct WebGlLib scene consumers | Constructed per call and populated through shared scene validation. | `bundle://proof/SB06/transcripts/passing-document-diagnostics-layer-validator-tests.txt` |
| Typed link-sync counters | JS diagnostics snapshot plus C# runtime/proof DTOs | Interop diagnostics readers and proof snapshot checks | JS emits diagnostics; C# deserializes web JSON into additive properties. | `bundle://proof/SB06/transcripts/passing-diagnostics-parity-scan.txt` |

## Closure

SB06 closes REQ-008 for scene validation consistency and REQ-009 for typed diagnostics parity. It also advances REQ-001 by documenting the package map and preserving WebGlLib as a generic render substrate.
