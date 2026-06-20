# Proof manifest - SB14

Status: completed

## Scope

SB14 adds Economy-only metamorphic/property tests that detect simulator noise without depending on one golden scenario output. The tests cover closed-resource conservation, transfer monotonicity under available capacity, permutation invariance for independent same-step events, deterministic replay without drift, and an explicit negative mutation detector.

## Changed Files

Changed-file hashes:

- `bundle://proof/SB14/transcripts/changed-file-hashes.txt`

Economy tests:

- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationMetamorphicPropertyTests.cs`

## Property Matrix

| Property | Test | Negative proof |
| --- | --- | --- |
| Closed-resource conservation | `ClosedResourceTransfers_ConserveTotalsAcrossMagnitudes` checks five transfer magnitudes preserve total grain across initial/final frames. | `ConservationProperty_FailsOnNegativeMutation` increments `store.buyer.grain` by 1 and proves conservation fails. |
| Transfer monotonicity | `TransferMagnitude_IsMonotonicWhenTargetCapacityAllows` proves target store quantity is nondecreasing and equals magnitude while capacity allows. | Covered by focused test assertions and source assertion report. |
| Independent event permutation invariance | `IndependentEvents_ArePermutationInvariantForFinalState` compares final store signatures after reversing independent grain/tools events. | Covered by focused test assertions and source assertion report. |
| Replay no hidden drift | `ReplayingSameScenario_HasNoHiddenDrift` compares frame deterministic hash chains and final store signatures across two materializations. | Covered by focused test assertions and source assertion report. |

## Proof Artifacts

- Property test report: `bundle://proof/SB14/property-test-report.json`
- Negative mutation proof: `bundle://proof/SB14/negative-mutation-proof.json`
- Focused tests: `bundle://proof/SB14/transcripts/metamorphic-property-tests.txt`
- Source assertions: `bundle://proof/SB14/transcripts/source-assertions.txt`
- Anti-stub audit: `bundle://proof/SB14/transcripts/anti-stub-audit.txt`
- Changed-file hashes: `bundle://proof/SB14/transcripts/changed-file-hashes.txt`
- Bundle validator transcript: `bundle://proof/SB14/transcripts/bundle-validator.txt`
- Semantic invariants: `bundle://proof/SB14/semantic-invariants.md`

## Closure

SB14 passes. The focused test suite passed 5/5, the property report marks all four metamorphic families as passing, and the negative mutation proof shows a corrupted final grain total is detected instead of accepted by fallback behavior. No browser proof is required because this subbundle changes only Economy headless tests.
