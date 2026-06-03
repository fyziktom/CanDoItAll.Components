# SB07 — Scenario source contract cleanup

Priority: P1
Related findings: F07
Status: Completed
Completed: 2026-06-03

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

Gate result: Passed. File-system catalog descriptors are now pathless for runtime consumers, catalog/source sessions export empty legacy path fields, `AddEconomySimulationSandbox` with a configured catalog root is covered by tests, and legacy path APIs remain only for explicit compatibility flows.

## Required proof artifacts

- `proof/SB07/manifest.md`
- `proof/SB07/semantic-invariants.md`
- `proof/SB07/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant. Completed: `proof/SB07/transcripts/economy-build-after-scenario-source-cleanup.txt`.
- Run focused tests for changed area. Completed: `proof/SB07/transcripts/economy-scenario-source-focused-tests.txt`.
- Run boundary audits. Completed: `proof/SB07/transcripts/source-assertion-scenario-source-cleanup-scan.txt`, `proof/SB07/transcripts/experiment-json-path-dependency-scan.txt`, `proof/SB07/transcripts/components-domain-boundary-scan.txt`, and `proof/SB07/transcripts/anti-stub-scenario-source-scan.txt`.
- Run browser proof for playback/UI changes. Completed: `proof/SB07/browser/simulation-sandbox-pathless-catalog-assertions.json`, `proof/SB07/browser/simulation-sandbox-pathless-catalog-after.png`, and `proof/SB07/transcripts/simulation-sandbox-pathless-catalog-playwright.txt`.
- Ensure no blank transcripts. Completed: `proof/SB07/transcripts/proof-hygiene-inventory.txt`.
