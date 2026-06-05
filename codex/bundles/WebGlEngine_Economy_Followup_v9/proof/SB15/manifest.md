# Proof manifest - SB15

Status: completed

## Scope

SB15 hardens the Economy headless CLI and manifest workflow. The runtime catalog now includes the v9 third scenario pack, `multi-goods-elite`, `scenario run --all` proves all catalog scenarios are executed, and `scenario manifest-diff` exposes categorized manifest comparison for scenario, model, policy, oracle, and runtime changes.

## Changed Files

Changed-file hashes:

- `bundle://proof/SB15/transcripts/changed-file-hashes.txt`

Economy production files:

- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Cli/Program.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyHeadlessRunManifest.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyExperimentReadiness.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios/EconomySimulationSandbox/multi-goods-elite/`

Economy tests:

- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/CliCommandTests.cs`
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationEconomicTrustHardeningTests.cs`

## Behavior Matrix

| Requirement | Implementation | Proof |
| --- | --- | --- |
| CLI runs all catalog scenarios including third scenario | Added `multi-goods-elite` to the runtime catalog with a strict `scenario.manifest.json`; `scenario run --all` enumerates all three descriptors | `cli-catalog-run-summary.json` records three manifests and `multi-goods-elite` as `headless-valid`; `cli-run-all-catalog.txt` records CLI exit/statuses |
| Manifest diff categorizes scenario/model/policy/oracle/runtime changes | Added `scenario manifest-diff`; normalized diff categories to the five required categories and added category counts | `manifest-diff-positive.txt` proves equivalent repeated runs; `manifest-diff-negative.txt` proves all five categories are emitted |
| Approved volatile artifacts explicitly listed | Headless run manifest schema advanced to `economy-headless-run/v4` and writes `approvedVolatileArtifacts` | Source assertions and CLI manifests verify `readiness-report.json` is listed and marked volatile |

## Proof Artifacts

- CLI catalog run summary: `bundle://proof/SB15/cli-catalog-run-summary.json`
- Manifest diff proof: `bundle://proof/SB15/manifest-diff-proof.json`
- CLI all-catalog transcript: `bundle://proof/SB15/transcripts/cli-run-all-catalog.txt`
- CLI multi-goods repeated-run transcript: `bundle://proof/SB15/transcripts/cli-run-multi-goods-repro.txt`
- Manifest diff positive transcript: `bundle://proof/SB15/transcripts/manifest-diff-positive.txt`
- Manifest diff negative transcript: `bundle://proof/SB15/transcripts/manifest-diff-negative.txt`
- Focused tests: `bundle://proof/SB15/transcripts/headless-cli-focused-tests.txt`
- Source assertions: `bundle://proof/SB15/transcripts/source-assertions.txt`
- Anti-stub audit: `bundle://proof/SB15/transcripts/anti-stub-audit.txt`
- Changed-file hashes: `bundle://proof/SB15/transcripts/changed-file-hashes.txt`
- Bundle validator transcript: `bundle://proof/SB15/transcripts/bundle-validator.txt`
- Semantic invariants: `bundle://proof/SB15/semantic-invariants.md`

## Closure

SB15 passes. The CLI all-catalog proof shows all three runtime catalog scenarios were executed, with legacy strict failures surfaced as headless truth and the third scenario `multi-goods-elite` headless-valid. Manifest diff proof covers both equivalent repeated runs and a negative mutated manifest emitting scenario, model, policy, oracle, and runtime categories.
