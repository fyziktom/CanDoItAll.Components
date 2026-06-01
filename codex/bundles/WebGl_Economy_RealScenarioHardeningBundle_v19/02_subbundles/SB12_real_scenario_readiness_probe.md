# SB12 - Real scenario readiness probe

Codex must produce a readiness report answering:

- Can shared-resource probe be run headlessly?
- Can finite-resource/ownership probe be run headlessly?
- Can a small producer/community trade probe be expressed without new core types?
- Which exact fields/actions are missing for browser playback?
- Which strict mode failures remain?

This is a report artifact, not a final UI.

## Status

Completed.

## Goal

Produce artifact-backed readiness reports for required scenarios and explicitly evaluate the optional small producer/community probe.

## Prerequisites

- SB08 runner artifacts must exist.
- SB09, SB10, and SB11 gates must pass or list explicit gaps in the report.

## Owned Requirements

- R12 Readiness Report.

## Dependency Impact

Provides the go/no-go basis for SB13 large-screen smoke planning.

## Validation Depth

Critical report proof: generated JSON must answer every required question with source artifact citations and remaining strict failures.

## Proof Required

- Generated readiness report paths for required scenarios.
- Command transcript or test output proving reports were generated.
- Proof manifest and semantic invariant contract.

## Progression Gate

Pass only when reports answer all required questions and do not claim a final UI demo exists.

Gate result: Passed. `repo://CanDoItAll.Economy/artifacts/economy/readiness/real-scenario-readiness-report.json` answers all five required questions, cites generated `shared-well` and `farmer-land` runner artifacts, reports zero remaining strict failures, marks the small-producer/community trade probe expressible without new core types, lists SB13 browser playback actions as deferred, and keeps `finalUiDemoClaimed` false.
