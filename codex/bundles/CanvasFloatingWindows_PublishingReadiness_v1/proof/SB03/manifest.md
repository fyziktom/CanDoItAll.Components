# SB03 Proof Manifest

Subbundle: `SB03 Canvas Contract And State Model Hardening`

Status: `Completed`

Owned raw notes and requirements:

- RAW03, RAW05.
- R04, R06, R08, R09.

Semantic invariant contract:

- `bundle://proof/SB03/semantic-invariants.md`

Changed-file hashes:

- `repo://tests/CanDoItAll.Components.BaseLib.Tests/CanvasContractBehaviorTests.cs` current SHA-256 `093c413691847e236acbdc09ee597c089e77b702321066eb8e8116bfa99b3717`
- `repo://tests/CanDoItAll.Components.BaseLib.Tests/CanDoItAll.Components.BaseLib.Tests.csproj` current SHA-256 `d408b002b5e7e49fbc32ae150824abac7d1d2e2f156736a4ab9ff82773c8d76d`
- Full current changed-file hash transcript: `bundle://proof/SB03/transcripts/changed-file-hashes.txt`

Command transcripts:

- Focused Canvas contract test transcript: `bundle://proof/SB03/transcripts/dotnet-test-canvas-contracts.txt`
- Combined Canvas/Overlay contract test transcript: `bundle://proof/SB03/transcripts/dotnet-test-canvas-overlay-contracts.txt`
- Source assertion transcript: `bundle://proof/SB03/transcripts/source-assertions-canvas-contracts.txt`
- Semantic adequacy transcript: `bundle://proof/SB03/transcripts/semantic-adequacy.txt`

Falling-first / adversarial negative proof:

- Failing-first: N/A process/no production behavior change. This phase added regression tests and a CanvasLib test project reference, but production Canvas code did not require edits.
- Negative proof coverage: `bundle://proof/SB03/transcripts/dotnet-test-canvas-contracts.txt` covers malformed JSON fallback, duplicate/blank selection ids, non-positive window geometry, Canvas-to-Overlay roundtrip, pinned layout collision, and calendar request defaults.

Passing / semantic positive proof:

- Passing transcript: `bundle://proof/SB03/transcripts/dotnet-test-canvas-contracts.txt`
- Passing downstream contract transcript: `bundle://proof/SB03/transcripts/dotnet-test-canvas-overlay-contracts.txt`
- Semantic positive proof transcript: `bundle://proof/SB03/transcripts/semantic-adequacy.txt`

Source-level assertions:

- `bundle://proof/SB03/transcripts/source-assertions-canvas-contracts.txt` verifies production paths for tolerant parsing, selection normalization, window state conversion, layout collision resolution, and calendar request construction.

Anti-stub audit:

- `bundle://proof/SB03/transcripts/anti-stub-audit.txt` states no stubs or blockers in SB03 production/test scope.

Downstream smoke proof:

- Browser proof is deferred to SB05-SB08 by bundle contract. SB03 downstream smoke is contract-level: `bundle://proof/SB03/transcripts/dotnet-test-canvas-overlay-contracts.txt` confirms Canvas and Overlay contract tests pass together before runtime/UI phases.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `CanvasWorkbenchUiState` | `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Workbench/CanvasWorkbenchUiState.cs` parses and serializes UI state | `repo://src/CanDoItAll.Components.CanvasLib/Components/Workbench/CanvasWorkbench.razor` consumes `Surface.UiState` | `bundle://proof/SB03/transcripts/source-assertions-canvas-contracts.txt` verifies parse/normalization/conversion paths | `bundle://proof/SB03/transcripts/dotnet-test-canvas-contracts.txt` covers malformed JSON, blank/duplicate ids, and non-positive geometry |
| `SelectionModel` | `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Graph/Interaction/SelectionModel.cs` produces normalized selection models | `repo://src/CanDoItAll.Components.CanvasLib/Components/Workbench/CanvasWorkbench.razor` and `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Core/AccessibilityMirrorLayer.cs` consume normalized selections | `bundle://proof/SB03/transcripts/source-assertions-canvas-contracts.txt` verifies `From` and `RemoveMissing` paths | `bundle://proof/SB03/transcripts/dotnet-test-canvas-contracts.txt` rejects blank/duplicate ids and stale missing selections |
| `CanvasCalendarSaveRequest` and `CanvasCalendarExportRequest` | `repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CanvasCalendar.razor` creates callback request records | Calendar consumers receive save/export callbacks through `repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CanvasCalendar.razor` parameters | `bundle://proof/SB03/transcripts/source-assertions-canvas-contracts.txt` verifies request construction and record definitions | `bundle://proof/SB03/transcripts/dotnet-test-canvas-contracts.txt` covers default values, context, format, and event payloads |

