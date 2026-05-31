# SB09 — Economy snapshot analysis services

## Goal
Make snapshot analysis a reusable system, not test-only logic.

## Required analyzers
- admin burden analyzer
- active issue analyzer
- resource concentration analyzer
- relationship stress analyzer
- pending event analyzer
- visual stage pressure analyzer
- invariant summary analyzer

## Output
- `SimulationSnapshotAnalysisReport`
- machine-readable findings
- human-readable short explanation
- severity and source paths

## Validation
The shared-resource probe must answer:
"Why does it look like many actors are doing paperwork?"

without scenario-specific code.

## Status
- Completed.

## Prerequisites
- SB08 snapshot builder service proof is complete.

## Exact Source References
- `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.Abstractions\Snapshot`
- `C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\SimulationSnapshotAnalysisProbeTests.cs`

## Dependency Impact
- Critical explanation layer for SB13-SB14 and final sandbox readiness.

## Validation Depth
- Requires production analyzers, machine-readable findings, human-readable explanation, source paths, and a semantic proof that rejects scenario-specific string matching.

## Acceptance Checklist
- Analyzer services exist in production code.
- Findings carry severity and source paths.
- Shared-resource paperwork explanation is produced without well/water-specific generic code.

## Proof Required
- `bundle://proof/SB09/manifest.md`
- `bundle://proof/SB09/semantic-invariants.md`
- Economy test transcript and genericity scan.

## Browser Validation Logging
- Browser validation is not required for analysis services.

## Progression Gate
- SB13-SB14 may proceed when analysis can explain pressure from generic snapshot data.

## Suggested Agent Prompt
- Implement reusable snapshot analyzers that answer admin-pressure questions without scenario-specific generic code.
