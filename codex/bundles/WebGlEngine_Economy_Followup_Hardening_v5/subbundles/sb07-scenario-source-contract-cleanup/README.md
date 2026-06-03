# SB07 — Scenario source contract cleanup

Priority: P1
Related findings: F07

## Objective

Add pathless scenario source/load/export APIs; keep legacy path APIs as compatibility only. Add host-neutral AddEconomySimulationSandbox registration extension and configurable catalog roots.

## Required implementation rules

- Keep Components generic and domain-neutral.
- Prefer small cohesive changes over broad rewrites.
- Add or update tests before claiming the gate is closed.
- Capture failing-first proof for every P0/P1 behavioral bug.
- Update proof manifest and semantic invariants.

## Acceptance gate

Runtime UI and tests no longer depend on ExperimentJsonPath except in legacy compatibility paths.

## Required proof artifacts

- `proof/SB07/manifest.md`
- `proof/SB07/semantic-invariants.md`
- `proof/SB07/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant.
- Run focused tests for changed area.
- Run boundary audits.
- Run browser proof for playback/UI changes.
- Ensure no blank transcripts.
