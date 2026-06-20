# SB08 Readiness Evidence Contract Report

## Implementation Summary

- Added `EconomyExperimentEvidenceRef` with `kind`, `path`, `hash`, and `schemaVersion` fields.
- Added `EconomyExperimentEvidenceValidator` to validate evidence ids, expected bands, bytes, ref fields, strict lowercase SHA-256 hashes, and top-level hash/schema consistency.
- Updated runtime/UI/oracle readiness band construction so `exerciseRequested` records the requested boolean, while `exercised` becomes true only when evidence validators pass.
- Updated browser observer metadata to require validator-passed runtime and UI evidence.
- Updated `ResearchReady` to require validator-passed oracle and browser-observer evidence.

## Test Proof

- `dotnet test ... --filter "FullyQualifiedName~ReadinessReportV3"` passed 2/2.
- `ReadinessReportV3_HardGatesRequireEvidenceExposeMachineStatusResearchReadyAndWarningBudget` proves positive evidence refs unlock research-ready and boolean-only claims do not.
- `ReadinessReportV3_BrokenEvidenceRefsBlockExerciseAndResearchReady` proves a malformed runtime evidence ref blocks runtime exercise, browser observer pass, and research-ready.
- `dotnet test ... --no-build --filter "FullyQualifiedName~ReadinessReport|FullyQualifiedName~BrowserObserverFailure"` passed 6/6.
- `dotnet build CanDoItAll.Economy.SimulationSandbox.csproj --no-restore --nologo /clp:ErrorsOnly` passed with 0 warnings and 0 errors.

## Browser Proof

No browser proof was required for SB08 because the change is a readiness-report contract and test-only assertion update in Economy SimulationSandbox. No Blazor UI, JS runtime, or browser observer implementation path was changed.
