# SB09 — Large simulation performance budgets and resource stress

Priority: P2
Related findings: F10

## Objective

Create performance budgets and stress harnesses for objects, motions, command stages, GLB cache, dispose/recreate, and long-run playback.

## Required implementation rules

- Keep Components generic and domain-neutral.
- Prefer small cohesive changes over broad rewrites.
- Add or update tests before claiming the gate is closed.
- Capture failing-first proof for every P0/P1 behavioral bug.
- Update proof manifest and semantic invariants.

## Acceptance gate

Budget test outputs machine-readable metrics and fails on regression thresholds.

## Required proof artifacts

- `proof/SB09/manifest.md`
- `proof/SB09/semantic-invariants.md`
- `proof/SB09/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant.
- Run focused tests for changed area.
- Run boundary audits.
- Run browser proof for playback/UI changes.
- Ensure no blank transcripts.
