# SB18 Semantic Invariants

## INV-SB18-001 Performance Evidence Is Proof-Only

- Expected behavior: bottlenecks are measured and recorded for desktop/large-screen scope, without mobile optimization or broad rewrites.
- Positive proof: `bundle://proof/SB18/transcripts/components-performance-audit.txt` and `repo://CanDoItAll.Economy/artifacts/economy/performance/simulation-performance-proof.json`.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Bottleneck measurements | Components audit and Economy performance test | Final closure | Run, measure, write artifact | `bundle://proof/SB19/transcripts/economy-tests.txt` |
