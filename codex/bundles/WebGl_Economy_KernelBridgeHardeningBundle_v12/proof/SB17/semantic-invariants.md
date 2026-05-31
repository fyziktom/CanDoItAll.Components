# SB17 Semantic Invariants

## INV-SB17-001 Same Pipeline For Both Examples

- Expected behavior: examples differ by input data, not core scenario branching.
- Positive proof: `repo://CanDoItAll.Economy/artifacts/economy/readiness/shared-well-and-farmer-land-readiness.json`.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Generic readiness flow | Loader, transition engine, visual mapper, metric evaluator | Readiness report | Shared pipeline across examples | `bundle://proof/SB19/transcripts/economy-tests.txt` |
