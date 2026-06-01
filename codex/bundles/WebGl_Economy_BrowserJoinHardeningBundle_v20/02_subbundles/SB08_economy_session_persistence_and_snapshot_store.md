# SB08 - Economy session persistence and snapshot store wiring

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
