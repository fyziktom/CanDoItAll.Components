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
