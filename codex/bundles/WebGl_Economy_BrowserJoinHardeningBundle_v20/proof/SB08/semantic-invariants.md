# SB08 Semantic Invariants

## Invariant ID

SB08-session-persistence

## Shallow-pass trap

Export/import can appear to roundtrip in memory while ignoring missing experiment files, stale input packs, invalid steps, or tampered snapshots.

## Adversarial negative proof

`SessionPersistence_ExportsImportsThroughFreshServiceAndRejectsInvalidState` rejects missing relative experiment paths, mismatched input pack hashes, unavailable current steps, and modified snapshot payloads.

## Semantic positive proof

The same test exports a paused session at a non-initial step, writes `session.json`, persists all projected snapshots, lists them by run id, and imports with a fresh service while preserving current step and snapshot hash.

## Anti-stub audit

`bundle://proof/SB08/transcripts/anti-stub-audit.txt` confirms no placeholder/stub markers in the SB08 production or focused test sources.
