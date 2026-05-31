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
