# Proof manifest SB08

Status: completed

- Objective: Scenario manifest file-hash and pack-hash hardening.
- Gate: Changing any manifest-required scenario file causes catalog validation failure unless the manifest is regenerated intentionally.
- Result: Passed. File-system scenario manifests now declare strict `packHash` and per-required-file `fileHashes`; catalog validation compares both against the actual scenario directory before a descriptor can be used.

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `EconomySimulationScenarioManifest.PackHash` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/*/scenario.manifest.json` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/FileSystemEconomySimulationScenarioCatalog.cs` | Manifest declares expected scenario-directory hash; catalog computes the actual hash excluding `scenario.manifest.json`; descriptor is invalid when they differ. | `bundle://proof/SB08/transcripts/failing-first-required-companion-tamper-test.txt`; `bundle://proof/SB08/transcripts/economy-scenario-manifest-focused-tests.txt` |
| `EconomySimulationScenarioManifest.FileHashes` | Scenario manifest author/regenerator | `FileSystemEconomySimulationScenarioCatalog.ValidateManifestFileHashes` | Each required file must have a safe relative path key and strict SHA-256 value; catalog recomputes each existing required file hash. | `bundle://proof/SB08/transcripts/economy-scenario-manifest-focused-tests.txt` |
| Scenario descriptor validity | `FileSystemEconomySimulationScenarioCatalog.CreateDescriptor` | `ListScenarios`, `GetScenario`, `LoadScenario`, browser sandbox page | Invalid manifest hash state remains visible as descriptor validation errors and `GetScenario` throws. | `bundle://proof/SB08/transcripts/economy-scenario-manifest-focused-tests.txt`; `bundle://proof/SB08/browser/simulation-sandbox-manifest-hash-assertions.json` |

## Source Changes

- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomySimulationSandboxContracts.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/FileSystemEconomySimulationScenarioCatalog.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/shared-well/scenario.manifest.json`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/farmer-land/scenario.manifest.json`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationSandboxScenarioCatalogTests.cs`

## Proof Artifacts

- Failing-first source audit: `bundle://proof/SB08/transcripts/failing-first-scenario-manifest-hash-gap.txt`
- Failing-first red test: `bundle://proof/SB08/transcripts/failing-first-required-companion-tamper-test.txt`
- Focused catalog tests: `bundle://proof/SB08/transcripts/economy-scenario-manifest-focused-tests.txt`
- Economy build: `bundle://proof/SB08/transcripts/economy-build-after-scenario-manifest-hardening.txt`
- Browser proof transcript: `bundle://proof/SB08/transcripts/simulation-sandbox-manifest-hash-playwright.txt`
- Browser assertions: `bundle://proof/SB08/browser/simulation-sandbox-manifest-hash-assertions.json`
- Browser screenshot: `bundle://proof/SB08/browser/simulation-sandbox-manifest-hash-after.png`
- Source assertion scan: `bundle://proof/SB08/transcripts/source-assertion-scenario-manifest-hash-scan.txt`
- Anti-stub scan: `bundle://proof/SB08/transcripts/anti-stub-scenario-manifest-hash-scan.txt`
- Boundary scan: `bundle://proof/SB08/transcripts/components-domain-boundary-scan.txt`
- Dependency scan: `bundle://proof/SB08/transcripts/scenario-manifest-hash-dependency-scan.txt`
- Changed-file hashes: `bundle://proof/SB08/transcripts/changed-file-hashes.txt`
- Proof hygiene inventory: `bundle://proof/SB08/transcripts/proof-hygiene-inventory.txt`
- Prepared-stage bundle validator: `bundle://proof/SB08/transcripts/bundle-validator-after-sb08.txt`

## Validation Commands

- `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --no-restore --filter "FullyQualifiedName~SimulationSandboxScenarioCatalogTests" --logger "console;verbosity=normal"`
- `dotnet build .\CanDoItAll.Economy.slnx --no-restore`
- `npm exec --yes --package=@playwright/cli -- playwright-cli run-code --filename ...\proof\SB08\browser\simulation-sandbox-manifest-hash-proof.js`
- `python scripts\validate_bundle.py --stage prepared --profile initiative`
