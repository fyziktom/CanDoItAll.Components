# Proof manifest - SB13

Status: completed

## Scope

SB13 extends Economy design-matrix factor materialization so factor cells can safely change effective scenario documents beyond the previous narrow bindings. It records the original/effective scenario hash and applied effects per run, rejects no-effect cells, and keeps the work Economy-only.

## Changed Files

Changed-file hashes:

- `bundle://proof/SB13/transcripts/changed-file-hashes.txt`

Economy production files:

- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomyExperimentDesignHarness.cs`

Economy tests:

- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationEconomicTrustHardeningTests.cs`

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `EffectiveScenarioHash` and `AppliedEffects` | `EconomyExperimentDesignHarness` | `design-matrix-summary.json`, downstream comparison/proof tooling | Every run record carries original scenario hash, effective scenario hash, and the concrete factor effects applied before execution | Focused tests assert every materialized run has a non-empty effective hash different from the original |
| Safe `json-pointer` binding | Design matrix factor metadata | Scenario materializer | Pointers are restricted to whitelisted scenario document roots and existing JSON targets; values are coerced by explicit type or existing node kind | `factor-effect-report.json` records `design-factor-no-effect:unchangedPointer` for an unchanged pointer cell |
| Typed capacity bindings | `initial-store-capacity`, `resource-capacity` | Scenario materializer | Store/resource capacity cells update effective scenario documents and hashes | Source assertions verify both effects are present once in each materialized run |
| Typed schedule and investment bindings | `scheduled-event-step`, `scheduled-event-offset`, `investment-rate` | Scenario materializer | Event schedule and event metadata factors update scheduled events before hashing/running | Focused tests and proof report show schedule step and investment rate effects in each cell |
| Policy fee/threshold binding | `rule-parameter`, `policy-fee`, `policy-threshold`, `fee` | Scenario materializer | Rule parameter cells update policy/fee/threshold values while still recording effective scenario hash even when model output hash is unchanged | `factor-effect-report.json` shows two policy-fee levels with distinct effective scenario/configuration hashes |

## Proof Artifacts

- Factor effect report: `bundle://proof/SB13/factor-effect-report.json`
- Design matrix summary: `bundle://proof/SB13/design-matrix-summary.json`
- Full design-matrix run output: `bundle://proof/SB13/design-matrix-run/`
- Generated source pack: `bundle://proof/SB13/factor-source/`
- No-effect negative materialization evidence: `bundle://proof/SB13/no-effect-negative/`
- Proof generator transcript: `bundle://proof/SB13/transcripts/factor-effect-proof-run.txt`
- Focused tests: `bundle://proof/SB13/transcripts/design-harness-focused-tests.txt`
- SimulationSandbox build: `bundle://proof/SB13/transcripts/simulationsandbox-build.txt`
- Source assertions: `bundle://proof/SB13/transcripts/source-assertions.txt`
- Anti-stub audit: `bundle://proof/SB13/transcripts/anti-stub-audit.txt`
- Changed-file hashes: `bundle://proof/SB13/transcripts/changed-file-hashes.txt`
- Bundle validator transcript: `bundle://proof/SB13/transcripts/bundle-validator.txt`
- Semantic invariants: `bundle://proof/SB13/semantic-invariants.md`

## Closure

SB13 passes. The proof run produced two factor cells, two distinct effective scenario hashes, two distinct configuration hashes, and a rejected no-effect pointer cell. Focused design-harness tests pass 2/2, and the source assertions confirm JSON pointer, capacity, schedule, investment-rate, and policy-parameter effects are recorded in the design summary.
