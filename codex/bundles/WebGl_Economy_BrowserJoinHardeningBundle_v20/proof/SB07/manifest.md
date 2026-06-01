# SB07 Proof Manifest

Status: Completed

## Scope

Strict visual mapping completion for the real browser-smoke probes.

## Changed File Hashes

- `bundle://proof/SB07/transcripts/changed-file-hashes.txt`

## Command Transcripts

- `bundle://proof/SB07/transcripts/strict-mapping-tests.txt`
- `bundle://proof/SB07/transcripts/fixture-mapping-assertions.txt`
- `bundle://proof/SB07/transcripts/source-assertions.txt`
- `bundle://proof/SB07/transcripts/anti-stub-audit.txt`

## Source Assertions

- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/shared-well/visual.mapping.json` and `farmer-land/visual.mapping.json` now cover admin/write pose, risk/warning symbol, rule/tax/fee symbol, resource transfer visual, and relationship/conflict pulse.
- The two fixture mappings set `allowNoOpPoseFallback` and `allowNoOpSymbolFallback` to false.
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/*/experiment.json` raw visual hashes and canonical pack hashes were updated and revalidated.
- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.cs` proves positive strict projection and an adversarial missing-symbol failure.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative-test citation |
|---|---|---|---|---|
| Strict fixture mappings | Committed fixture `visual.mapping.json` files | SB11 browser-smoke input | Loaded by `SimulationExperimentInputPackLoader` and projected by `EconomySimulationSandboxSessionService` | `FixtureProbeFailsStrictlyWhenRequiredSmokeMappingIsRemoved` proves a required risk symbol cannot disappear silently. |
| Strict projection options | `EconomyWebGlProjectionOptions` with fallback flags false | Strict mapping tests and browser smoke | Used during session load/projection | `FixtureProbeProjectsWithStrictMappingWithoutFallbacks` proves no fallback object/no-op pose/no-op symbol diagnostics are present. |
| Manifest hashes | Fixture `experiment.json` files | Strict input-pack validation | Checked before session projection | `bundle://proof/SB07/transcripts/fixture-mapping-assertions.txt` proves raw visual hashes and pack hashes match. |

## Semantic Adequacy Evidence

- Semantic positive proof: both shared-well and farmer-land project under strict fallback-disabled options with no fallback object, no no-op pose fallback, and no no-op symbol fallback.
- Adversarial negative proof: removing the required risk symbol from the shared-well mapping produces strict `missing-symbol-mapping` failure.
- Anti-stub audit: `bundle://proof/SB07/transcripts/anti-stub-audit.txt`.

## Closure

SB07 passed. The selected browser-smoke inputs no longer rely on hidden fallback/no-op visual mappings.
