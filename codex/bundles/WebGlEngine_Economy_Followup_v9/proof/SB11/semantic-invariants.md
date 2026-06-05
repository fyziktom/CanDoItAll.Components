# SB11 Semantic Invariants

## Invariants

- Generic contribution, claim, return, and obligation events must be first-class registry entries, not unknown event strings hidden in metadata.
- Contribution, claim, and return events must emit resource flows with generic reasons and must not mint free resources.
- Obligation creation must create relationship state and must not move resources by itself.
- The semantic oracle must prove resource conservation across contribution, claim, obligation, and return.
- New primitives must not introduce Economy-specific UI behavior or Components code changes.
- Registry aliases must stay generic (`resource-contribution`, `obligation-create`, `claim-issue`, `return-pay`) and avoid monopoly/elite/investor domain names.

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `actor.resource.contribution` | `SimulationEventKind` and `SimulationEventKindRegistry` | Strict scenario validation, event compiler, simple-account handler registry | Normalizes aliases such as `resource-contribution`; handler emits a credit flow with reason `contribution` | Failing-first transcript shows the event was previously `unknown-event-handler` with zero flows |
| `actor.claim.issue` | `SimulationEventKind` and `SimulationEventKindRegistry` | Simple-account handler registry and semantic oracle | Transfers a claim resource with flow reason `claim`; does not create extra claim quantity | Semantic oracle preserves total `equity-claim` at `10` |
| `actor.return.pay` | `SimulationEventKind` and `SimulationEventKindRegistry` | Simple-account handler registry and semantic oracle | Transfers a return resource with flow reason `return`; preserves total credit | Semantic oracle preserves total `credit` at `110` after contribution and return |
| `actor.obligation.create` | `SimulationEventKind` and `SimulationEventKindRegistry` | Simple-account relationship updater | Creates a relationship with kind `actor.obligation.create` and strength equal to event magnitude; no resource flow is minted | Semantic oracle shows obligation strength `40` with unchanged resource totals |
| Event participant location normalization | `SimulationEventNormalizer` | Event normalization and compile/build paths | Source/target location metadata is normalized with explicit empty-string fallback | Abstractions and SimpleAccounts builds pass with 0 warnings and 0 errors |

## Proof Links

- `bundle://proof/SB11/failing-first-generic-contribution.txt`
- `bundle://proof/SB11/event-registry-and-investment-flow-tests.txt`
- `bundle://proof/SB11/investment-flow-oracle.json`
- `bundle://proof/SB11/abstractions-build.txt`
- `bundle://proof/SB11/simpleaccounts-build.txt`
- `bundle://proof/SB11/transcripts/source-assertions.txt`
