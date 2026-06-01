# SB04 - Components runtime snapshot for browser state

## Status

Completed. Entry and closure gates passed.

## Goal

Expose a browser/runtime snapshot that Economy can attach to simulation snapshots.

## Tasks

- Add or harden a generic runtime snapshot DTO:
  - current frame index,
  - active stages,
  - queued stages,
  - active motions,
  - queued motions,
  - command journal tail,
  - stage barrier state,
  - last runtime errors/warnings.
- Keep it domain-neutral.
- Ensure snapshot size is bounded.

## Acceptance

- A test proves runtime snapshot can be exported from a fake/applied run.
- No Economy terms.

## Prerequisites

- SB02 and SB03 proof completed or explicitly blocked without affecting snapshot fields.

## Owned Requirements

- R04 Runtime snapshot.

## Dependency Impact

Economy snapshot and browser smoke proof need this generic runtime state. If fields are unbounded or domain-specific, SB08/SB11 must reopen this phase.

## Validation Depth

Unit tests must prove bounded snapshot export from an applied run/fake runtime and source scans must prove no Economy terms in Components.

## Proof Required

- `dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj`
- `bundle://proof/SB04/transcripts/runtime-snapshot-tests.txt`
- `bundle://proof/SB04/transcripts/source-assertions.txt`
- `bundle://proof/SB04/manifest.md`
- `bundle://proof/SB04/semantic-invariants.md`

## Browser Validation Logging

N/A here. Browser snapshot capture is logged in SB11.

## Semantic Adequacy Gate

- Shallow-pass trap: snapshot object exists but omits live queues, journal, barrier, warning, or error state.
- Adversarial negative proof: oversized diagnostics are bounded and do not export unbounded queues/journal.
- Semantic positive proof: applied runtime state exports current frame, active/queued stages, motions, journal tail, barrier state, and diagnostics.
- Anti-stub audit: snapshot DTO and exporter contain no Economy-specific fields or placeholder code.

## Progression Gate

Pass only when snapshot fields are behavior-backed and bounded. SB08/SB11 must cite this proof when attaching browser state to Economy snapshots.
