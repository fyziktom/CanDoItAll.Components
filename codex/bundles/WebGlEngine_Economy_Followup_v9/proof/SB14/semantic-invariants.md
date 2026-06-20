# SB14 Semantic Invariants

Status: completed

## Invariants

- Closed-resource transfers must conserve total quantity for each resource when no external source, sink, production, or destruction event exists.
- Increasing transfer magnitude must not reduce the target store quantity when source quantity and target capacity can satisfy the transfer.
- Independent events over disjoint resource/store pairs must converge to the same final store signature regardless of their order within the same step.
- Replaying the same scenario definition must produce the same deterministic frame hash chain and final store signature.
- Conservation checks must fail on a deliberately mutated final frame whose resource total differs from the initial closed-resource total.

## Boundary

These invariants are Economy headless truth checks. They do not introduce Components domain concepts, browser observer assumptions, or visual/runtime fallback behavior.

## Proof

- `bundle://proof/SB14/property-test-report.json`
- `bundle://proof/SB14/negative-mutation-proof.json`
- `bundle://proof/SB14/transcripts/metamorphic-property-tests.txt`
