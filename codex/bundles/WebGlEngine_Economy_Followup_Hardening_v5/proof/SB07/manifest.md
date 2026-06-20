# Proof manifest SB07

Status: completed
Completed: 2026-06-03

- Objective: Scenario source contract cleanup.
- Gate: Passed. Runtime UI and focused tests no longer load from `ExperimentJsonPath`; catalog/source sessions and exports keep legacy path fields empty while path APIs remain for compatibility.
- Owned findings: F07.

## Changed-file hashes

| Path | SHA-256 | Purpose |
| --- | --- | --- |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/FileSystemEconomySimulationScenarioCatalog.cs` | `04532c011112a0d907aa2a6a64c7621f3163356d2127109b60efcb789ece2c21` | Keeps file paths internal to the catalog while publishing pathless descriptors and source streams. |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomySimulationSandboxSessionService.cs` | `b1383ee4e0327b34f95c81ce30676f8f5964b14a7b4a68acfc31505eed6a116a` | Keeps catalog/source-loaded sessions and exports pathless; legacy path fields are populated only for path loads or configured persistence. |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyRealScenarioRunner.cs` | `f67d28a5f04d63f4f3182eb5d043d2817f894377434900c24da86d0f9591ed7f` | Resolves exported scenario ids from the session before path-derived legacy ids. |
| `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationSandboxScenarioCatalogTests.cs` | `33428e68d83b42c6f27be624ea5d2e27044b7c913dce1f5be8dc0068fad99106` | Adds pathless catalog/source and registration-root coverage. |
| `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationSandboxSessionTests.cs` | `0ba7a0d3984df8853a43bafd77c914e84a5a0653a2e38a5ec9c397e466c0b839` | Proves catalog session exports are portable with empty legacy path fields. |

## Proof artifact hashes

| Path | SHA-256 | Purpose |
| --- | --- | --- |
| `bundle://proof/SB07/transcripts/failing-first-scenario-source-path-gap.txt` | See `bundle://proof/SB07/transcripts/changed-file-hashes.txt`. | Baseline proof that file-system descriptors and tests previously depended on `ExperimentJsonPath`. |
| `bundle://proof/SB07/transcripts/economy-scenario-source-focused-tests.txt` | `45331f9bde3ff574de4187e76028694775d09bcc1f65c4e0bfede76a50705408` | Focused Economy tests passed, 28 tests. |
| `bundle://proof/SB07/transcripts/economy-build-after-scenario-source-cleanup.txt` | `e763e1370d607cc2b1cf3bfb7ac01bcf0f8ae0016ebbc1d5130d69efb6208f8b` | Economy solution build passed after scenario source cleanup. |
| `bundle://proof/SB07/browser/simulation-sandbox-pathless-catalog-proof.js` | `34cc50489337a7e7143cd9d7376707acc0540b10093ac3df5011583e353376b0` | Browser proof harness for pathless catalog UI load. |
| `bundle://proof/SB07/browser/simulation-sandbox-pathless-catalog-assertions.json` | `b12b40340f528067d958334c37e80824ad61d7ce423bea768a5036326af0d039` | Machine-readable browser assertions; all assertions passed. |
| `bundle://proof/SB07/browser/simulation-sandbox-pathless-catalog-after.png` | `5d6d8f692bf180ad66b0fe32c799ae662d5be5b312aed164b705bb6f37df7957` | Screenshot of the runtime sandbox loaded from catalog source with scenario/hash visible. |
| `bundle://proof/SB07/transcripts/simulation-sandbox-pathless-catalog-playwright.txt` | See `bundle://proof/SB07/transcripts/changed-file-hashes.txt`. | Playwright transcript for pathless catalog browser proof. |
| `bundle://proof/SB07/transcripts/source-assertion-scenario-source-cleanup-scan.txt` | See `bundle://proof/SB07/transcripts/changed-file-hashes.txt`. | Source scan proving pathless contracts and tests exist. |
| `bundle://proof/SB07/transcripts/experiment-json-path-dependency-scan.txt` | See `bundle://proof/SB07/transcripts/changed-file-hashes.txt`. | Dependency scan proving runtime UI and tests no longer rely on `scenario.ExperimentJsonPath`. |
| `bundle://proof/SB07/transcripts/components-domain-boundary-scan.txt` | See `bundle://proof/SB07/transcripts/changed-file-hashes.txt`. | Boundary scan proving Components packages remained domain-neutral. |
| `bundle://proof/SB07/transcripts/anti-stub-scenario-source-scan.txt` | See `bundle://proof/SB07/transcripts/changed-file-hashes.txt`. | Anti-stub audit for changed source, tests, and proof harness. |

## Command transcripts

- `bundle://proof/SB07/transcripts/failing-first-scenario-source-path-gap.txt`: `HEAD` baseline scan showed file-system descriptors returned `experimentPath`, source openers used `descriptor.ExperimentJsonPath`, and tests loaded via `scenario.ExperimentJsonPath`.
- `bundle://proof/SB07/transcripts/economy-scenario-source-focused-tests.txt`: focused tests for scenario catalog, session service, and sandbox component passed 28 tests.
- `bundle://proof/SB07/transcripts/economy-build-after-scenario-source-cleanup.txt`: Economy solution build passed with existing warnings and zero errors.
- `bundle://proof/SB07/transcripts/simulation-sandbox-pathless-catalog-playwright.txt`: browser proof passed on `http://127.0.0.1:5312/economy/simulation-sandbox` using a local-dev `--no-launch-profile` node.

## Browser proof summary

`bundle://proof/SB07/browser/simulation-sandbox-pathless-catalog-assertions.json` proves the runtime sandbox loaded `shared-well`, rendered a valid scenario pack hash, and did not render `ExperimentJsonPath`, test fixture paths, or absolute `experiment.json` paths. The screenshot confirms the loaded sandbox and visible catalog scenario metadata.

## Semantic adequacy gate

- Shallow-pass trap: merely hiding the path in the UI would not prove source/session/export contracts were pathless.
- Adversarial negative proof: focused tests fail if runtime tests load via `scenario.ExperimentJsonPath`, if catalog exports retain legacy path fields, or if configured catalog-root registration does not load by scenario id.
- Semantic positive proof: browser proof, source assertions, and 28 focused tests cover pathless catalog descriptors, source load, export/import, and host-neutral registration.
- Boundary proof: `bundle://proof/SB07/transcripts/components-domain-boundary-scan.txt`.
- Anti-stub audit: `bundle://proof/SB07/transcripts/anti-stub-scenario-source-scan.txt`.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Pathless `EconomySimulationScenarioDescriptor.ExperimentJsonPath` for file-system catalog entries | `FileSystemEconomySimulationScenarioCatalog.CreateDescriptor`. | Runtime UI, session service, tests. | Empty for catalog descriptors; path resolution remains internal through `ResolveExperimentJsonPath`. | Focused test asserts descriptor path is empty and loads via `LoadScenario`. |
| `EconomySimulationScenarioSource` stream loading | File-system and in-memory catalogs. | `EconomySimulationSandboxSessionService` and workflow materialization. | Catalog returns streams and companion files without exposing absolute paths to sessions. | Focused tests and browser proof load `shared-well` through scenario id/source. |
| Empty legacy export path fields for catalog sessions | `CreateSessionExport`. | Importers and persisted session exports. | `ExperimentJsonPath`, `BaseDirectory`, and `RelativeExperimentPath` stay empty for catalog/pathless sessions unless a legacy path load or explicit persistence root is used. | Session tests assert exported path fields are empty and import still succeeds after moving the catalog. |
| Configurable catalog-root registration | `AddEconomySimulationSandbox` and `UseFileSystemScenarioCatalog`. | Node host and test hosts. | Registers sandbox services, workflow, pipelines, and the selected catalog root in DI. | Registration-root test resolves the catalog/session service and loads pathlessly by scenario id. |
| Legacy path compatibility | `ResolveExperimentJsonPath`, `Load(string experimentJsonPath)`, legacy import fallback. | Old path-based hosts and session exports. | Retained only for explicit legacy path flows. | Legacy import test still passes; dependency scan verifies runtime UI/tests do not load from `scenario.ExperimentJsonPath`. |
