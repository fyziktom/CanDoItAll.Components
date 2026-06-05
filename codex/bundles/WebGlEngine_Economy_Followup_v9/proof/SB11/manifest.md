# Proof manifest - SB11

Status: completed

## Scope

SB11 adds generic event/action primitives for contribution, obligation creation, claim issuance, and return payment. These are Economy simulation primitives only; no Components code or UI-specific economics changed.

## Changed Files

Changed-file hashes:

- `bundle://proof/SB11/transcripts/changed-file-hashes.txt`

Economy production files:

- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Events/SimulationEvent.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Events/SimulationEventKindRegistry.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Events/SimulationEventNormalizer.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleSimulationEventHandlers.cs`
- `repo://C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleSimulationStateTransitionEngine.Mutations.cs`

Economy tests:

- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationEconomicTrustHardeningTests.cs`

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `actor.resource.contribution` | `SimulationEventKind` and `SimulationEventKindRegistry` | Strict scenario validation, event compiler, simple-account handler registry | Normalizes aliases such as `resource-contribution`; handler emits a credit flow with reason `contribution` | Failing-first transcript shows the event was previously `unknown-event-handler` with zero flows |
| `actor.claim.issue` | `SimulationEventKind` and `SimulationEventKindRegistry` | Simple-account handler registry and semantic oracle | Transfers a claim resource with flow reason `claim`; does not create extra claim quantity | Semantic oracle preserves total `equity-claim` at `10` |
| `actor.return.pay` | `SimulationEventKind` and `SimulationEventKindRegistry` | Simple-account handler registry and semantic oracle | Transfers a return resource with flow reason `return`; preserves total credit | Semantic oracle preserves total `credit` at `110` after contribution and return |
| `actor.obligation.create` | `SimulationEventKind` and `SimulationEventKindRegistry` | Simple-account relationship updater | Creates a relationship with kind `actor.obligation.create` and strength equal to event magnitude; no resource flow is minted | Semantic oracle shows obligation strength `40` with unchanged resource totals |
| Event participant location normalization | `SimulationEventNormalizer` | Event normalization and compile/build paths | Source/target location metadata is normalized with explicit empty-string fallback | Abstractions and SimpleAccounts builds pass with 0 warnings and 0 errors |

## Proof Artifacts

- Failing-first contribution probe: `bundle://proof/SB11/failing-first-generic-contribution.txt`
- Event registry and semantic-flow tests: `bundle://proof/SB11/event-registry-and-investment-flow-tests.txt`
- Semantic oracle JSON: `bundle://proof/SB11/investment-flow-oracle.json`
- Semantic oracle transcript: `bundle://proof/SB11/investment-flow-oracle.txt`
- Abstractions build: `bundle://proof/SB11/abstractions-build.txt`
- SimpleAccounts build: `bundle://proof/SB11/simpleaccounts-build.txt`
- Source assertions: `bundle://proof/SB11/transcripts/source-assertions.txt`
- Changed-file hashes: `bundle://proof/SB11/transcripts/changed-file-hashes.txt`
- Anti-stub audit: `bundle://proof/SB11/transcripts/anti-stub-audit.txt`
- Bundle validator transcript: `bundle://proof/SB11/transcripts/bundle-validator.txt`
- Semantic invariants: `bundle://proof/SB11/semantic-invariants.md`

## Closure

SB11 passes. The failing-first probe recorded `actor.resource.contribution` as unknown before implementation. The final tests pass 2/2, the semantic oracle passes with conserved totals (`credit=110`, `equityClaim=10`), and both touched production projects build with 0 warnings and 0 errors.
