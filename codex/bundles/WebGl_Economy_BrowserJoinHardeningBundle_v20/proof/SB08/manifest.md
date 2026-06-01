# SB08 Proof Manifest

Status: Completed

## Scope

Economy session persistence and snapshot store wiring.

## Changed File Hashes

- `bundle://proof/SB08/transcripts/changed-file-hashes.txt`

## Command Transcripts

- `bundle://proof/SB08/transcripts/session-persistence-tests.txt`
- `bundle://proof/SB08/transcripts/snapshot-store-listing-tests.txt`
- `bundle://proof/SB08/transcripts/source-assertions.txt`
- `bundle://proof/SB08/transcripts/anti-stub-audit.txt`

## Source Assertions

- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomySimulationSandboxContracts.cs` defines `EconomySimulationSandboxSessionPersistenceOptions` plus export fields for base directory, relative experiment path, snapshot directory, session JSON, input pack hash, and current snapshot hash.
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomySimulationSandboxSessionService.cs` writes configured session JSON, saves snapshots through `FileSimulationSnapshotStore`, and validates path/hash/step/snapshot state on import.
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationSandboxSessionTests.cs` proves restart-style import with a fresh service and rejection of invalid path, pack hash, step, and tampered snapshot.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative-test citation |
|---|---|---|---|---|
| `session.json` | `EconomySimulationSandboxSessionService.ExportSession` | Fresh service import and SB11 proof tooling | Written when persistence base directory is configured | `SessionPersistence_ExportsImportsThroughFreshServiceAndRejectsInvalidState` proves missing experiment path and bad input hash are rejected. |
| File snapshots | `FileSimulationSnapshotStore` via session export | Import validation, listing, analysis, smoke proof | Saved for every projected session snapshot | Same test proves listing by run id and tampered snapshot rejection. |
| Snapshot descriptor index | `FileSimulationSnapshotStore` | Restart-style listing and lookup | Reopened by fresh store instance | `bundle://proof/SB08/transcripts/snapshot-store-listing-tests.txt` proves index/list/load/delete/tamper behavior. |

## Semantic Adequacy Evidence

- Semantic positive proof: a copied shared-well fixture exports session JSON plus snapshots, then imports through a fresh service with current step, pause state, and current snapshot hash preserved.
- Adversarial negative proof: missing experiment path, wrong input pack hash, invalid step, and tampered snapshot all fail import.
- Anti-stub audit: `bundle://proof/SB08/transcripts/anti-stub-audit.txt`.

## Closure

SB08 passed. Session and snapshot state can survive a process-restart style roundtrip and is backed by listable file snapshots.
