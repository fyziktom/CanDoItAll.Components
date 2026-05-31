# SB10 — Snapshot store hardening

## Goal
Make snapshots useful beyond in-memory tests.

## Required
- async snapshot store interface
- file-backed JSON snapshot store
- descriptor indexes by run/scenario/step
- content hash verification on load
- optional compression extension point
- delete/list/query tests

## Validation
- Save 100 snapshots.
- List by run id.
- Load by snapshot id.
- Detect tampering.
