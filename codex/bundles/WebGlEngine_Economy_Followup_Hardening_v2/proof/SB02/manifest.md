# SB02 proof manifest

Status: Completed.

Owned requirements: R01, R02, R14 baseline support. SB02 also provides the runtime scenario-provider foundation for R12/SB11.

Raw notes: `bundle://inputs/raw-user-request.md`, `bundle://analysis/02-critical-findings.md`, `bundle://requirements/01-normalized-requirements.md`.

Semantic invariant contract: `bundle://proof/SB02/semantic-invariants.md`.

## Changed file hashes

| File | Before SHA-256 | After SHA-256 | Evidence |
| --- | --- | --- | --- |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Components/Components/SimulationSandbox/EconomySimulationSandboxPage.razor` | `6b016ec5984aa5654ee0ca1aa01a5bdfcccbcef521f20a970ffc330ef379b334` | `3acc1537f50496d6836568d03a94a69916598f52ee9e864ae5d976aa27f70d97` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/CanDoItAll.Economy.Node.csproj` | `656bfa45dd8db88ddfac88a5a3efe1237fbcd8b4acc7bee240ce7dc48fa1884f` | `da2d67dd82e74eb593937f5a0f4a1e3b69ae0f62f93eaabbbfa38f711cf56420` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/EconomyNodeServiceRegistration.cs` | `922758daaebda9ff1799a20d401012317530546d46a81c73cf149fce6e4fee40` | `679d325e0e229d34086175012273745fc1b0432a662fb23c12ac25ff09b09b4e` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomySimulationSandboxContracts.cs` | `c7732dad3798eb61fa57eed53a1f270d6ed3d84ff930a45ebeb4a50ce865cadb` | `08e81079e16053a0d99b6a8a1cba222bb3e8bb22e5528c319706812ff1757ddb` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/FileSystemEconomySimulationScenarioCatalog.cs` | `absent` | `744693dbf519ff8d13dd3fc50e506fb9e96fbc7d224c980af3e2acad28650232` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationSandboxScenarioCatalogTests.cs` | `absent` | `7c7fc0b7126b8aa08ba6e6b0813c985e42a0a2dfe550d193b9e4c3fe0df7e6f2` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomySimulationSandboxComponentTests.cs` | `242b51871770cdd7c0f6657a2279e6e4c78692bffd12e785ffcdabf0586115ba` | `93aa8dc5824d8f300258fdd43f3d581649afbe01c283cae35aa8fe5fcbc574b5` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/farmer-land/expected.invariants.json` | `absent` | `b4833b3b12350aa0c95e0aa42ec3cb5004f6e09b1e4a57d2480f7d1553acb446` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/farmer-land/experiment.json` | `absent` | `52a347d53961c80a82f0c24de9ee0d33f85ee2ddeb2b6ce9ed23d719e8bcb723` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/farmer-land/institution.rules.json` | `absent` | `520ef8561c209013e20607f76b01c3896e43da84d874643aca14e4ab0a567692` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/farmer-land/parameters.json` | `absent` | `1fd309a66019a9e88659a37aba0bf16d996e5462b4051f9a266fd0c6b730e024` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/farmer-land/placement.json` | `absent` | `26577ad4db2a99bf2e819acdcb4ed784159554bcaca9d343339d30150c9e5794` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/farmer-land/run.plan.json` | `absent` | `55bfe3a60fd0da28b841208b2e7362a0df4d9e665dc7412dac4365191857b577` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/farmer-land/scenario.definition.json` | `absent` | `dc43367d6cce4f6ed368b167908016c1a9588149a6f1ad6562e2f725a6721df1` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/farmer-land/visual.mapping.json` | `absent` | `3fca5acf55ac0a44ea39a9bd27d3f90ac28674133df790d212bf8a8133cbb6b7` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/shared-well/expected.invariants.json` | `absent` | `51485cc9517cd6f4f9e39afd6857c88512211d728168bb31a94737a8084bd632` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/shared-well/experiment.json` | `absent` | `990264814d461b7f0f19f2a2e1a24b5119d6711adae6b2543dffcd9d7008d4a4` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/shared-well/institution.rules.json` | `absent` | `b0534a5a847c99d664b329498f0d3cc3251b85bb0218e2d9958a2d9184f9f019` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/shared-well/parameters.json` | `absent` | `5b746fff621d70264f546af7f3370b24deeeb9d938f91ce729129e5ed7fdd4a6` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/shared-well/placement.json` | `absent` | `f07f3c73bbff00a4bc0872a4a02d55492fa901b548177e447a7998f32d987f4f` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/shared-well/run.plan.json` | `absent` | `50a5d9318d55e6e5fa16a4b8e3a8f047053b06d8ee33678dd363c20212b43215` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/shared-well/scenario.definition.json` | `absent` | `26b062d52bb322db68a1d0df6f2522b8aa9c39a921061cf92faf7786a6ca8170` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/shared-well/visual.mapping.json` | `absent` | `e228aac08c9bda5894b3e5c945cde07ed6832c348b21509bbc6370b3b7f88b84` | `bundle://proof/SB02/transcripts/changed-file-before-after-hashes.txt` |

## Command transcripts

| Command / action | Result | Transcript |
| --- | --- | --- |
| Failing-first source search for runtime `tests`/`Fixtures` path search and direct session-service construction | Fail before implementation, proving old runtime coupling | `bundle://proof/SB02/transcripts/failing-first-runtime-fixture-search.txt` |
| Focused catalog tests first run | Fail during implementation due missing test using; repaired before closure | `bundle://proof/SB02/transcripts/passing-scenario-catalog-tests-first-run.txt` |
| `dotnet test ... --filter FullyQualifiedName~SimulationSandboxScenarioCatalogTests` | Pass, 3 tests | `bundle://proof/SB02/transcripts/passing-scenario-catalog-tests.txt` |
| Runtime source search for `tests`, `Fixtures`, `ExperimentInputs`, direct session construction | Pass, no production runtime hits | `bundle://proof/SB02/transcripts/passing-runtime-fixture-search-removed.txt` |
| Existing simulation sandbox session tests | Pass, 6 tests | `bundle://proof/SB02/transcripts/passing-existing-simulation-sandbox-session-tests.txt` |
| `dotnet build src/CanDoItAll.Economy.Node/CanDoItAll.Economy.Node.csproj` plus output-content assertion | Pass; runtime sample content copied under `SimulationScenarios/EconomySimulationSandbox` | `bundle://proof/SB02/transcripts/passing-node-build-runtime-sample-content.txt` |
| Economy simulation boundary audit | Pass after component harness fix | `bundle://proof/SB02/transcripts/passing-economy-simulation-boundary-audit-after-component-harness-fix.txt` |
| Production anti-stub/fixture scan | Pass for touched production paths | `bundle://proof/SB02/transcripts/passing-anti-stub-fixture-scan-v2.txt` |
| Full Economy solution test suite | Pass, 549 tests | `bundle://proof/SB02/transcripts/passing-economy-tests-full-after-component-harness-fix.txt` |
| Node server start/stop for browser proof | Pass; local route served on `http://127.0.0.1:5097` and process stopped | `bundle://proof/SB02/transcripts/node-server-start.txt`, `bundle://proof/SB02/transcripts/node-server-stop.txt` |

## Browser artifacts

| Artifact | Result |
| --- | --- |
| `bundle://proof/SB02/browser/assertions-large.json` | URL `/economy/simulation-sandbox`, viewport 1600x1000, loaded `Shared Well`, 3 frames, 13 objects, canvas present, `containsTestFixturePath=false`. |
| `bundle://proof/SB02/browser/assertions-after-apply.json` | Clicked `Apply frame`; browser apply state `applied`, initial scene reset true, runtime error/warning counts 0, `containsTestFixturePath=false`. |
| `bundle://proof/SB02/browser/console-large.txt` | 0 browser errors, 0 browser warnings. |
| `bundle://proof/SB02/browser/console-after-apply.txt` | 0 browser errors, 0 browser warnings after frame apply. |
| `bundle://proof/SB02/browser/economy-simulation-sandbox-large.png` | Large desktop screenshot after runtime scenario load. |
| `bundle://proof/SB02/browser/economy-simulation-sandbox-after-apply.png` | Large desktop screenshot after applying frame 0. |
| `bundle://proof/SB02/browser/snapshot-large.md` | DOM/accessibility snapshot for loaded route. |

## Source assertions

| Assertion | Evidence |
| --- | --- |
| `EconomySimulationSandboxPage` uses DI for `IEconomySimulationSandboxSessionService` and `IEconomySimulationScenarioCatalog`; it no longer calls `new EconomySimulationSandboxSessionService()` or walks to `tests/Fixtures/ExperimentInputs`. | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Components/Components/SimulationSandbox/EconomySimulationSandboxPage.razor`; `bundle://proof/SB02/transcripts/passing-runtime-fixture-search-removed.txt` |
| Runtime scenario catalog abstraction and descriptor exist in the sandbox contract layer. | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomySimulationSandboxContracts.cs` |
| Filesystem catalog lists app-owned sample scenario directories, opens experiments/companions, and rejects traversal. | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/FileSystemEconomySimulationScenarioCatalog.cs`; `bundle://proof/SB02/transcripts/passing-scenario-catalog-tests.txt` |
| Node registers sandbox session service and scenario catalog from `AppContext.BaseDirectory/SimulationScenarios/EconomySimulationSandbox`. | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/EconomyNodeServiceRegistration.cs` |
| Node project copies runtime sample content to build/publish output. | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/CanDoItAll.Economy.Node.csproj`; `bundle://proof/SB02/transcripts/passing-node-build-runtime-sample-content.txt` |
| Browser route loads the runtime catalog scenario, not a test fixture path. | `bundle://proof/SB02/browser/assertions-large.json`; `bundle://proof/SB02/browser/assertions-after-apply.json` |

## Anti-stub audit

Production anti-stub/fixture scan passed for touched production paths and checks `TODO`, `NotImplemented`, `Fixtures`, `ExperimentInputs`, `proof-only`, and `placeholder success`: `bundle://proof/SB02/transcripts/passing-anti-stub-fixture-scan-v2.txt`.

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Runtime scenario descriptors | `FileSystemEconomySimulationScenarioCatalog.ListScenarios()` | `EconomySimulationSandboxPage` and tests | Discovered from app-owned `SimulationScenarios/EconomySimulationSandbox/{scenarioId}/experiment.json` directories at runtime | Failing-first runtime source search shows previous `tests/Fixtures/ExperimentInputs` search; passing source search and browser assertions show no fixture path. |
| Runtime scenario content files | `src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/**` copied by Node csproj | Node route and sandbox session service | Included in build/publish output via `CopyToOutputDirectory` and `CopyToPublishDirectory` | Node build transcript asserts copied content exists in output without relying on test project folders. |
| Scenario catalog stream access | `FileSystemEconomySimulationScenarioCatalog.OpenExperiment/OpenCompanionFile` | `EconomySimulationSandboxSessionService.Load` and future UI providers | Reads only validated scenario ids and normalized relative companion paths | `FileSystemScenarioCatalogRejectsTraversal` rejects scenario and companion traversal attempts. |
| Browser runtime loaded-state diagnostics | `EconomySimulationSandboxPage` and WebGL browser runtime | Browser validation and SB11 follow-up | Rendered as route state and diagnostics after load/apply actions | Browser assertions prove `containsTestFixturePath=false`, canvas present, apply state `applied`, runtime error/warning counts 0. |

## Gate decision

Pass. SB02 removes the runtime test-fixture directory dependency from the Economy sandbox page and Node route, introduces an app-owned scenario catalog and runtime sample packaging path, proves traversal rejection and full-suite compatibility, and records Node browser proof with assertions, screenshots, console state, and runtime diagnostics. SB11 remains responsible for wider large+narrow UI proof across all target routes, but the scenario-provider prerequisite is closed.
