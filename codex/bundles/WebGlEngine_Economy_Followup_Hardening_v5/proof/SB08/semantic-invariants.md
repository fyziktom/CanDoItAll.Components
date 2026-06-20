# Semantic invariants SB08

Status: completed

## Invariants

- SB08-I1: Every scenario manifest accepted by `FileSystemEconomySimulationScenarioCatalog` must declare `packHash`, and the declared value must exactly match the deterministic hash of all scenario pack files except `scenario.manifest.json`.
- SB08-I2: Every `requiredFiles` entry must have a corresponding safe relative `fileHashes` entry with a strict lowercase SHA-256 hash, and the declared hash must match the current file content.
- SB08-I3: Tampering with a manifest-required companion file that is not referenced by `experiment.json` must still invalidate the descriptor and make `GetScenario` throw.
- SB08-I4: Runtime descriptors remain pathless and publish the computed descriptor `PackHash`; manifest hash verification does not reintroduce legacy experiment paths.
- SB08-I5: Components remains domain-neutral for SB08; Economy owns scenario manifest semantics.

## Production Behavior Artifact Matrix

| Invariant | Producer | Consumer | Proof |
| --- | --- | --- | --- |
| SB08-I1 | Scenario manifests | `FileSystemEconomySimulationScenarioCatalog.ValidateScenarioPackManifest` | `bundle://proof/SB08/transcripts/source-assertion-scenario-manifest-hash-scan.txt`; `bundle://proof/SB08/transcripts/economy-scenario-manifest-focused-tests.txt` |
| SB08-I2 | Scenario manifests | `FileSystemEconomySimulationScenarioCatalog.ValidateManifestFileHashes` | `bundle://proof/SB08/transcripts/source-assertion-scenario-manifest-hash-scan.txt`; `bundle://proof/SB08/transcripts/economy-scenario-manifest-focused-tests.txt` |
| SB08-I3 | Test tamper fixture | Catalog descriptor and `GetScenario` | `bundle://proof/SB08/transcripts/failing-first-required-companion-tamper-test.txt`; `bundle://proof/SB08/transcripts/economy-scenario-manifest-focused-tests.txt` |
| SB08-I4 | Catalog descriptor/session service | Sandbox UI and browser diagnostics | `bundle://proof/SB08/browser/simulation-sandbox-manifest-hash-assertions.json`; `bundle://proof/SB08/transcripts/simulation-sandbox-manifest-hash-playwright.txt` |
| SB08-I5 | SB08 scoped edits | Components packages | `bundle://proof/SB08/transcripts/components-domain-boundary-scan.txt` |

## Evidence Summary

- Failing-first test proved the pre-fix catalog accepted a tampered manifest-required companion file: `Assert.False` failed because the descriptor remained valid.
- Passing focused tests proved 14 catalog tests, including the new tamper test and shipped runtime scenario packs.
- Browser proof loaded `/economy/simulation-sandbox`, rendered `shared-well`, showed the new pack hash prefix/suffix, and kept validity `valid`.
