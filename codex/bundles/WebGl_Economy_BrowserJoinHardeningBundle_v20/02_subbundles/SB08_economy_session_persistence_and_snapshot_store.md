# SB08 - Economy session persistence and snapshot store wiring

## Status

Completed. Closure gate passed.

## Goal

Make pause/snapshot/analyze useful beyond tests.

## Tasks

- Wire `FileSimulationSnapshotStore` into `EconomySimulationSandboxSessionService` or a session store.
- Add session persistence options:
  - base directory,
  - relative experiment path,
  - snapshot directory,
  - exported session JSON.
- Import should validate:
  - experiment path exists,
  - input pack hash matches if available,
  - current step exists,
  - snapshot hash validates.

## Acceptance

- Export/import session roundtrip survives process restart in tests.
- Snapshot store can list snapshots per run.

## Prerequisites

- SB06 artifact shape and SB07 strict input state are known.

## Owned Requirements

- R08 Session persistence.

## Dependency Impact

Durable session/snapshot state supports SB09 analysis and SB11 smoke artifacts.

## Validation Depth

Tests must export, import through fresh service/store instances, validate experiment path/hash/step/snapshot hash, and list snapshots per run.

## Proof Required

- Session persistence test transcript.
- Snapshot store listing test transcript.
- `bundle://proof/SB08/manifest.md`

## Browser Validation Logging

N/A for service-level persistence. SB11 uses the persisted snapshot output.

## Progression Gate

Pass only when import rejects invalid state and accepts a restart-style valid roundtrip.
