# Semantic invariants SB07

Status: completed

## Invariants

| ID | Requirement | Evidence | Closure |
| --- | --- | --- | --- |
| `SB07-PATHLESS-CATALOG-DESCRIPTOR` | Runtime catalog descriptors must not expose absolute `ExperimentJsonPath` values. | `NodeRuntimeScenarioCatalogLoadsSampleScenarioThroughPathlessSource`; `bundle://proof/SB07/transcripts/source-assertion-scenario-source-cleanup-scan.txt`. | Passed. |
| `SB07-SOURCE-LOAD` | Runtime sessions must load catalog scenarios by `EconomySimulationScenarioSource` or scenario id, not by `scenario.ExperimentJsonPath`. | `bundle://proof/SB07/transcripts/experiment-json-path-dependency-scan.txt`; `bundle://proof/SB07/transcripts/economy-scenario-source-focused-tests.txt`. | Passed. |
| `SB07-PORTABLE-EXPORT` | Catalog/session exports must keep legacy path fields empty while preserving scenario id and pack hash. | `SessionService_LoadsByScenarioIdAndExportsPortableScenarioReference`; `SessionService_AsyncLoadProjectSnapshotExportImportRoundTripsPortableCatalogSource`. | Passed. |
| `SB07-LEGACY-COMPATIBILITY` | Path APIs remain available only for explicit compatibility flows. | `SessionService_ImportsBackwardCompatibleLegacyPathExport`; `FileSystemScenarioCatalogRejectsTraversal`. | Passed. |
| `SB07-HOST-NEUTRAL-REGISTRATION` | Hosts can register sandbox services and catalog roots without constructing services inside UI components. | `AddEconomySimulationSandboxRegistersConfiguredCatalogRootAndPathlessSessionService`; `RuntimeSandboxComponentDoesNotSearchTestFixturesOrConstructSessionService`. | Passed. |
| `SB07-DOMAIN-BOUNDARY` | Components packages remain generic; scenario-source behavior stays in Economy. | `bundle://proof/SB07/transcripts/components-domain-boundary-scan.txt`; `bundle://proof/SB07/transcripts/anti-stub-scenario-source-scan.txt`. | Passed. |

## Semantic Adequacy Gate

| Gate item | SB07 result |
| --- | --- |
| Shallow-pass trap | Hiding paths from the page would not prove source/session/export contracts. Tests assert descriptor, session, and export legacy path fields are empty for catalog flows. |
| Adversarial negative proof | Dependency scan fails if the UI references `ExperimentJsonPath` or tests load from `scenario.ExperimentJsonPath`. |
| Semantic positive proof | Focused tests prove pathless source load, registration-root DI, portable export/import, and legacy import compatibility. Browser proof confirms the runtime route loads from catalog without rendering path text. |
| Anti-stub audit | `bundle://proof/SB07/transcripts/anti-stub-scenario-source-scan.txt`. |
| Raw-note literal closure | F07 is solved for the runtime sandbox and tests: `ExperimentJsonPath` is no longer required except explicit legacy compatibility flows. |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Pathless catalog descriptor | File-system catalog descriptor creation. | UI, session service, and tests. | Empty `ExperimentJsonPath`; scenario id, title, version, content hash, and pack hash remain populated. | Focused test asserts descriptor path is empty and still loads a valid scenario. |
| Scenario source streams | Catalog `GetScenarioSource`. | Workflow materialization and session service. | Opens experiment and companion files through safe stream callbacks. | Pathless source test and runtime catalog test load and validate WebGL run frames. |
| Portable catalog session export | Session export builder. | Importers and persisted session payloads. | Path fields empty for catalog/pathless sessions; scenario id and pack hash drive import. | Session import test moves catalog directory and imports by scenario id/pack hash. |
| Catalog-root DI registration | `AddEconomySimulationSandbox` options. | Node and test hosts. | Registers sandbox service graph plus configured file-system or injected catalog. | Registration test resolves services and loads pathlessly. |
| Legacy path API | `ResolveExperimentJsonPath` and path-load/import overloads. | Compatibility callers only. | Maintained without driving runtime UI/source tests. | Legacy import and traversal tests remain green; dependency scan prevents runtime/test recoupling. |
