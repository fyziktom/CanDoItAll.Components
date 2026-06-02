# SB02 semantic invariants

Status: Completed.

| Invariant ID | Source raw note | Expected behavior | Disallowed shallow implementation | Failing-first / negative proof | Passing / positive proof | Source files | Downstream dependency |
| --- | --- | --- | --- | --- | --- | --- | --- |
| SB02-INV-001 | R01/F01 | Runtime UI and Node route load scenarios from app-owned runtime content, not `tests/CanDoItAll.Economy.Tests/Fixtures`. | Move the fixture path string to a helper or test-only name while the production page still walks to `tests/`. | `bundle://proof/SB02/transcripts/failing-first-runtime-fixture-search.txt` shows the original production page searched `tests/Fixtures/ExperimentInputs` and constructed the session service directly. | `bundle://proof/SB02/transcripts/passing-runtime-fixture-search-removed.txt`, `bundle://proof/SB02/browser/assertions-large.json`, and `bundle://proof/SB02/browser/assertions-after-apply.json` show no runtime test-fixture path and successful route load/apply. | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Components/Components/SimulationSandbox/EconomySimulationSandboxPage.razor`, `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/**` | SB11 browser proof depends on this route loading from runtime content. |
| SB02-INV-002 | R02/F01 | Scenario loading is behind a catalog abstraction with runtime and test responsibilities separated. | Keep page-local filesystem probing or only assert that sample JSON files exist. | Traversal attempts are modeled as invalid input in `FileSystemScenarioCatalogRejectsTraversal`. | `bundle://proof/SB02/transcripts/passing-scenario-catalog-tests.txt` proves catalog listing, experiment/companion access, sandbox load, WebGL validation, and traversal rejection. | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomySimulationSandboxContracts.cs`, `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/FileSystemEconomySimulationScenarioCatalog.cs`, `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationSandboxScenarioCatalogTests.cs` | SB10 docs should describe this public boundary; SB11 can use catalog diagnostics. |
| SB02-INV-003 | R02/R14 | Runtime sample content is packaged by the Node app and copied to output/publish paths without depending on stale test fixture folders. | Pass tests only from repository root where test fixtures remain present. | Changed-file hashes show the runtime samples were absent before SB02; failing-first proof shows old runtime source coupled to fixture folders. | `bundle://proof/SB02/transcripts/passing-node-build-runtime-sample-content.txt` proves Node build output contains `SimulationScenarios/EconomySimulationSandbox/shared-well/experiment.json` and `farmer-land/experiment.json`; full Economy tests pass in `bundle://proof/SB02/transcripts/passing-economy-tests-full-after-component-harness-fix.txt`. | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/CanDoItAll.Economy.Node.csproj`, `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/EconomyNodeServiceRegistration.cs` | SB09 package-mode proof and SB11 deployment-like browser proof can build on this but still own their broader gates. |

## Production assertions

- `EconomySimulationSandboxPage` receives `IEconomySimulationSandboxSessionService` and `IEconomySimulationScenarioCatalog` through DI and has a `DefaultScenarioId` parameter.
- `FileSystemEconomySimulationScenarioCatalog` validates scenario ids and companion relative paths before opening files.
- Node registers the catalog against `AppContext.BaseDirectory/SimulationScenarios/EconomySimulationSandbox`.
- Runtime sample content exists under `src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/**` and is configured for build and publish copy.
- Browser proof for `/economy/simulation-sandbox` shows `containsTestFixturePath=false`, 3 loaded frames, 13 objects, canvas present, and zero browser console errors/warnings.

## Anti-stub audit

See `bundle://proof/SB02/transcripts/passing-anti-stub-fixture-scan-v2.txt`.

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative test |
| --- | --- | --- | --- | --- |
| Runtime scenario descriptors | `FileSystemEconomySimulationScenarioCatalog.ListScenarios()` | `EconomySimulationSandboxPage` and tests | Recomputed from runtime sample directories when the catalog is used. | `bundle://proof/SB02/transcripts/failing-first-runtime-fixture-search.txt` and `bundle://proof/SB02/transcripts/passing-runtime-fixture-search-removed.txt`. |
| Runtime sample files | Node project content items | Node app output/publish folder and scenario catalog | Copied by MSBuild with `PreserveNewest`. | `bundle://proof/SB02/transcripts/passing-node-build-runtime-sample-content.txt`. |
| Catalog path safety | `FileSystemEconomySimulationScenarioCatalog` | Runtime scenario consumers | Rejects traversal and missing scenario roots before opening streams. | `FileSystemScenarioCatalogRejectsTraversal` in `bundle://proof/SB02/transcripts/passing-scenario-catalog-tests.txt`. |
| Browser route diagnostics | `EconomySimulationSandboxPage` and browser runtime | Browser proof and SB11 | Updated after scenario load and frame apply. | `bundle://proof/SB02/browser/assertions-after-apply.json` shows runtime error/warning counts 0 and `containsTestFixturePath=false`. |
