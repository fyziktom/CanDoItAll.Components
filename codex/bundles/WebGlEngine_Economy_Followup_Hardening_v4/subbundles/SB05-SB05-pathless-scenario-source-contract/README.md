# SB05 Pathless scenario source contract

## Objective

Implement the next hardening step for the WebGL/Economy engine foundation.

## Scope

- Add pathless scenario source/descriptor contract.
- Add `Load(EconomySimulationScenarioSource source, ...)` and `TryLoad(...)` APIs.
- Keep path-based APIs as legacy convenience and route them through the source API.
- Update runtime UI to load from the catalog source, not `ExperimentJsonPath`.
- Add DI extension methods in `SimulationSandbox` for registering session service and file-system catalog.

## Out of scope

- Do not add domain semantics into Components packages.
- Do not rewrite unrelated systems.
- Do not close the subbundle with screenshots only.
- Do not accept empty required proof artifacts.

## Implementation guidance

- Start with a failing-first test or audit where applicable.
- Make the smallest cohesive refactor that fixes the root cause.
- Add source assertions that prove the intended path is used.
- Keep API compatibility where safe; otherwise document the migration.
- Ensure all source-code comments are in English.

## Required proof

- Pathless source unit tests.
- Runtime UI source-based load test.
- No runtime source code depends on `tests/` fixture paths.
- Source hash proof.

## Refactor gate

Before moving to the next subbundle, record:
- changed files;
- test/build/audit commands;
- proof artifact paths;
- open risks;
- whether public API changed and how users migrate.

## Execution record

Status: completed.

Changed files are recorded in `../../proof/SB05/changed-file-hashes.md`.

Proof artifacts:
- `../../proof/SB05/transcripts/failing-first.txt`
- `../../proof/SB05/transcripts/passing-tests.txt`
- `../../proof/SB05/transcripts/expanded-tests.txt`
- `../../proof/SB05/transcripts/source-assertions.txt`
- `../../proof/SB05/transcripts/boundary-audit.txt`
- `../../proof/SB05/source-hash-proof.md`

Public API change: `IEconomySimulationScenarioCatalog`, `IEconomySimulationSandboxWorkflow`, and `IEconomySimulationSandboxSessionService` now expose source-based APIs using `EconomySimulationScenarioSource`. Existing path-based session APIs remain available and route through `EconomySimulationScenarioSource.FromPath`.

Open risks: session export/import portability is intentionally deferred to SB07. Scenario manifest hardening is intentionally deferred to SB06.
