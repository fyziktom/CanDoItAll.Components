# SB03 Semantic Invariants

## Invariant UI State Serialization

- Invariant ID: `SB03-INV-UISTATE-SERIALIZATION`
- Source raw note: RAW03 requires Canvas hardening and true validation; RAW05 requires preserving existing behavior.
- Expected behavior: Canvas UI state parsing is tolerant of malformed JSON, normalizes selected/highlighted/collapsed lists, normalizes window keys and geometry, and round-trips Canvas window state through OverlayLib state semantics.
- Disallowed shallow implementation: Tests that only deserialize a happy-path payload without malformed input, duplicate ids, or non-positive geometry.
- Failing-first test: N/A process/no production behavior change; this phase adds regression tests and no production source edits were needed.
- Passing test: `bundle://proof/SB03/transcripts/dotnet-test-canvas-contracts.txt` and `bundle://proof/SB03/transcripts/semantic-adequacy.txt` include `SB03-INV-UISTATE-SERIALIZATION`.
- Changed source files: `repo://tests/CanDoItAll.Components.BaseLib.Tests/CanvasContractBehaviorTests.cs` and `repo://tests/CanDoItAll.Components.BaseLib.Tests/CanDoItAll.Components.BaseLib.Tests.csproj`.
- Production assertions: `bundle://proof/SB03/transcripts/source-assertions-canvas-contracts.txt` cites `CanvasWorkbenchUiState.Parse`, `NormalizeWindowStates`, `ToOverlayWindowState`, and `FromOverlayWindowState`.
- Red-team negative case: Tests cover malformed JSON, blank/duplicate ids, non-positive geometry, and Canvas-to-Overlay roundtrip.
- Downstream dependency check: SB04 runtime refactor and SB07 CanvasFloatingWindow proof can rely on stable UI state normalization.

## Invariant Selection And Layout

- Invariant ID: `SB03-INV-SELECTION-LAYOUT`
- Source raw note: RAW03 requires Canvas maintainability/hardening and R06 requires selection and layout contract tests.
- Expected behavior: `SelectionModel` trims ids, removes blanks/duplicates, preserves or promotes primary id intentionally, removes missing nodes, and layout collision resolution separates overlapping unpinned nodes from pinned nodes.
- Disallowed shallow implementation: Selection proof that only counts ids, or layout proof that never creates overlapping nodes.
- Failing-first test: N/A process/no production behavior change; this phase adds regression tests over existing production behavior.
- Passing test: `bundle://proof/SB03/transcripts/dotnet-test-canvas-overlay-contracts.txt` and `bundle://proof/SB03/transcripts/semantic-adequacy.txt` include `SB03-INV-SELECTION-LAYOUT`.
- Changed source files: `repo://tests/CanDoItAll.Components.BaseLib.Tests/CanvasContractBehaviorTests.cs`.
- Production assertions: `bundle://proof/SB03/transcripts/source-assertions-canvas-contracts.txt` cites `SelectionModel.From`, `RemoveMissing`, and `CanvasLayoutCollisionResolver.Resolve`.
- Red-team negative case: Tests reject blank/duplicate selection ids, a stale primary id after missing-node removal, and unresolved overlap against a pinned node.
- Downstream dependency check: SB05 workbench interactions and SB08 matrix can depend on tested selection/layout contracts.

## Invariant Calendar Contracts

- Invariant ID: `SB03-INV-CALENDAR-CONTRACTS`
- Source raw note: RAW03 and R09 require calendar to be treated as a first-class Canvas publishing surface.
- Expected behavior: Calendar surface defaults and save/export request records expose stable view, timezone, locale, slot, create/export, event, and context semantics.
- Disallowed shallow implementation: Calendar proof that only checks class existence without exercising request records or default values.
- Failing-first test: N/A process/no production behavior change; this phase adds regression tests over existing public contracts.
- Passing test: `bundle://proof/SB03/transcripts/dotnet-test-canvas-contracts.txt` and `bundle://proof/SB03/transcripts/semantic-adequacy.txt` include `SB03-INV-CALENDAR-CONTRACTS`.
- Changed source files: `repo://tests/CanDoItAll.Components.BaseLib.Tests/CanvasContractBehaviorTests.cs`.
- Production assertions: `bundle://proof/SB03/transcripts/source-assertions-canvas-contracts.txt` cites `CanvasCalendarSaveRequest`, `CanvasCalendarExportRequest`, and their usage in `CanvasCalendar.razor`.
- Red-team negative case: Tests reject existence-only proof by asserting defaults and object identity through request payloads.
- Downstream dependency check: SB06 browser proof can rely on stable calendar request/default contracts.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `CanvasWorkbenchUiState` | `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Workbench/CanvasWorkbenchUiState.cs` parses and serializes UI state | `repo://src/CanDoItAll.Components.CanvasLib/Components/Workbench/CanvasWorkbench.razor` consumes surface UI state | `bundle://proof/SB03/transcripts/source-assertions-canvas-contracts.txt` verifies parse, normalization, and Overlay state conversion paths | `bundle://proof/SB03/transcripts/dotnet-test-canvas-contracts.txt` covers malformed JSON, duplicate ids, blank ids, and non-positive geometry |
| `SelectionModel` | `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Graph/Interaction/SelectionModel.cs` creates normalized selection models | `repo://src/CanDoItAll.Components.CanvasLib/Components/Workbench/CanvasWorkbench.razor` and `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Core/AccessibilityMirrorLayer.cs` consume normalized selections | `bundle://proof/SB03/transcripts/source-assertions-canvas-contracts.txt` verifies `From` and `RemoveMissing` production paths | `bundle://proof/SB03/transcripts/dotnet-test-canvas-contracts.txt` covers blank/duplicate ids and missing-node removal |
| `CanvasCalendarSaveRequest` and `CanvasCalendarExportRequest` | `repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CanvasCalendar.razor` creates save/export requests | Consumers receive records through `SaveEventAsync` and `ExportAsync` callbacks in `repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CanvasCalendar.razor` | `bundle://proof/SB03/transcripts/source-assertions-canvas-contracts.txt` verifies request records and callback construction | `bundle://proof/SB03/transcripts/dotnet-test-canvas-contracts.txt` covers defaults, context, format, and visible event payloads |
