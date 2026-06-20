# SB08 — Scenario manifest file-hash and pack-hash hardening

Priority: P1
Related findings: F08

## Objective

Add manifest file hashes and packHash verification for every required file; define tamper behavior and negative tests for changed companion docs.

## Required implementation rules

- Keep Components generic and domain-neutral.
- Prefer small cohesive changes over broad rewrites.
- Add or update tests before claiming the gate is closed.
- Capture failing-first proof for every P0/P1 behavioral bug.
- Update proof manifest and semantic invariants.

## Acceptance gate

Changing any scenario file causes catalog validation failure unless manifest is regenerated intentionally.

## Required proof artifacts

- `proof/SB08/manifest.md`
- `proof/SB08/semantic-invariants.md`
- `proof/SB08/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant.
- Run focused tests for changed area.
- Run boundary audits.
- Run browser proof for playback/UI changes.
- Ensure no blank transcripts.

## Execution result

Status: Completed

- Added `packHash` and `fileHashes` to `EconomySimulationScenarioManifest`.
- Hardened `FileSystemEconomySimulationScenarioCatalog` to validate manifest pack hashes, safe file-hash keys, strict SHA-256 values, required-file coverage, and content equality.
- Updated shipped `shared-well` and `farmer-land` scenario manifests with deterministic pack hashes and per-required-file hashes.
- Added a failing-first then passing test for a tampered manifest-required companion file outside the experiment-input hash set.
- Captured browser proof that the runtime sandbox still loads `shared-well` as valid with the hardened manifest.

Primary proof:

- `bundle://proof/SB08/manifest.md`
- `bundle://proof/SB08/semantic-invariants.md`
- `bundle://proof/SB08/transcripts/economy-scenario-manifest-focused-tests.txt`
- `bundle://proof/SB08/browser/simulation-sandbox-manifest-hash-assertions.json`
