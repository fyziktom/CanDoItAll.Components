# SB11 Proof Manifest

Status: Completed

## Scope

Visual mapping schema/version validation, strict loader, missing asset diagnostics, fallback-heavy diagnostics, and fixture proof for both experiment visual mapping JSON files.

## Changed Files

| File | SHA-256 proof |
|---|---|
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/EconomyVisualMappingDefinition.cs` | `bundle://proof/SB11/hashes/visual-mapping-file-hashes.txt` |
| `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationExperimentLoaderTests.cs` | `bundle://proof/SB11/hashes/visual-mapping-file-hashes.txt` |
| `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/shared-well/visual.mapping.json` | `bundle://proof/SB11/hashes/visual-mapping-file-hashes.txt` |
| `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/farmer-land/visual.mapping.json` | `bundle://proof/SB11/hashes/visual-mapping-file-hashes.txt` |

## Command Transcripts

| Command | Result | Path |
|---|---|---|
| `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter FullyQualifiedName~SimulationExperimentLoaderTests` | Pass, 7 tests | `bundle://proof/SB11/transcripts/simulation-experiment-loader-tests.txt` |
| `powershell -ExecutionPolicy Bypass -File .\scripts\audit-simulation-boundaries.ps1` | Pass | `bundle://proof/SB11/transcripts/simulation-boundary-audit.txt` |

## Source Assertions

| Assertion | Path |
|---|---|
| Source scan proves supported schema version, strict loader, category/node/pose/symbol/anchor/action mapping coverage, missing asset diagnostics, fallback-heavy diagnostics, and fixture mappings. | `bundle://proof/SB11/source-assertions/visual-mapping-source-assertions.txt` |
| Anti-stub scan covers validator/loader and visual mapping tests. | `bundle://proof/SB11/source-assertions/anti-stub-scan.txt` |

## Behavior Artifacts

| Artifact | Path |
|---|---|
| Loader/validator tests proving strict schema rejection, missing asset/fallback diagnostics, WebGL-neutral key rejection, and strict fixture mapping load for both visual mapping JSON files | `bundle://proof/SB11/transcripts/simulation-experiment-loader-tests.txt` |

## Downstream Gate

SB15 may proceed because bridge visual choices are backed by strict configurable schema data rather than hardcoded example assets.
