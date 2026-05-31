# SB08 — Strict experiment pack hashes for committed fixtures

## Problem
Strict hash validation exists, but committed fixture `experiment.json` may still contain placeholder hashes. Temporary strict-pack tests are useful but do not prove real fixtures are production-ready.

## Tasks
- Add a tool to recompute real SHA-256 hashes for fixture input packs.
- Update committed `experiment.json` files with real content hashes and pack hash.
- Add strict tests that validate committed shared-well and farmer-land fixtures with:
  - VerifyDocumentReferences=true
  - VerifyDocumentContentHashes=true
  - VerifyPackHash=true
  - StrictHashValidation=true

## Tests
- tampering any fixture file fails.
- changing experiment metadata after pack hash fails.
- placeholder hashes are rejected.
