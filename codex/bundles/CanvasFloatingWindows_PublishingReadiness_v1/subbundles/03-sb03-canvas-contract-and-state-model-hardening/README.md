# SB03 Canvas Contract And State Model Hardening

## Status

- `Ready`

## Objective

Create a trustworthy contract-test foundation for CanvasLib state, selection, serialization, layout, calendar requests, and window-state roundtrips before runtime or UI refactors.

## Covered Inputs

- RAW03: Canvas preparation, refactor, hardening, and true validation.
- RAW05: Preserve all functionality.
- R04, R06, R08, R09.

## Prerequisites

- SB01 inventory gate passed.
- SB02 passed or documented that generic window-state tests are still pending but not blocking non-window Canvas tests.
- Test project location is chosen and documented.

## Exact Source References

- repo://src/CanDoItAll.Components.CanvasLib/Canvas/Workbench/CanvasWorkbenchUiState.cs
- repo://src/CanDoItAll.Components.CanvasLib/Canvas/Workbench/CanvasWorkbenchSurface.cs
- repo://src/CanDoItAll.Components.CanvasLib/Canvas/Workbench/CanvasWorkbenchNode.cs
- repo://src/CanDoItAll.Components.CanvasLib/Canvas/Graph/Interaction/SelectionModel.cs
- repo://src/CanDoItAll.Components.CanvasLib/Canvas/Graph/Composition/CanvasNodeLayout.cs
- repo://src/CanDoItAll.Components.CanvasLib/Canvas/Core/SerializationPersistencePack.cs
- repo://src/CanDoItAll.Components.CanvasLib/Canvas/Calendar/CanvasCalendarContracts.cs
- repo://tests/CanDoItAll.Components.BaseLib.Tests

## Deliverables

- Dedicated CanvasLib tests or an explicitly documented temporary test home.
- Tests for workbench state parsing/serialization, selection normalization, window-state conversion, layout/collision behavior, and calendar contract requests.
- Source assertions documenting public state ownership.
- SB03 proof manifest and semantic invariants.

## Dependency Impact

- SB04 runtime changes depend on stable serialized state and contract expectations.
- SB05-SB08 browser proof depends on tests that catch state regressions hidden by happy-path rendering.
- SB09 API approval depends on intentional public contract shape.

## Validation Depth

- Critical foundation.
- Unit/contract tests with adversarial negative cases.
- Semantic Adequacy Gate and artifact-backed proof manifest required.

## Implementation Steps

1. Decide whether to add `tests/CanDoItAll.Components.CanvasLib.Tests` or extend an existing test project with clear rationale.
2. Add failing-first tests for malformed JSON fallback, duplicate/blank selection ids, non-positive window geometry, Canvas-to-Overlay state roundtrip, layout collision edge cases, and calendar request defaults.
3. Implement only the smallest source changes needed to satisfy tests.
4. Run targeted tests and relevant existing standard tests.
5. Capture source assertions proving behavior lives in production code, not fixtures.
6. Update execution report and create SB03 proof artifacts.

## Scope Exceptions

- Browser interaction proof is deferred to SB05-SB08.
- Package/API snapshot updates are deferred to SB09.

## Do Not Do

- Do not change runtime JavaScript behavior in this subbundle except when tests expose an unavoidable contract bug.
- Do not make state parsing throw for malformed persisted input unless explicitly approved.
- Do not edit WebGL files.

## Acceptance Checklist

- Canvas state tests prove positive and negative cases.
- Window-state conversion between CanvasLib and OverlayLib is covered.
- Calendar request/response contracts are covered.
- Existing Overlay/standard tests still pass.

## Proof Required

- Failing-first transcript for new contract tests before production fixes when behavior changes.
- Passing test transcript after fixes.
- Source assertion transcript.
- `bundle://proof/SB03/manifest.md`
- `bundle://proof/SB03/semantic-invariants.md`
- Anti-stub audit transcript.

## Browser Validation Logging

- N/A unless a source change needs smoke proof.
- If browser smoke is used, target `/groups/canvas` at 1366x900 and record screenshot/console result.

## Progression Gate

- SB04-SB08 may proceed only after Canvas state/contract tests pass or an explicit blocker is recorded.
- Reopen SB03 if later runtime or browser proof exposes state/serialization/contract ambiguity.

## Suggested Agent Prompt

```text
Execute SB03 only. Add CanvasLib contract tests for state, selection, serialization, layout, calendar, and window roundtrip behavior. Preserve functionality, capture failing-first/passing proof where behavior changes, and stop before runtime UI refactors.
```
