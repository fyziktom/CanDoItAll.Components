# SB11 Semantic Invariants

## INV-SB11-001 Unsafe Mutations Are Diagnosed

- Expected behavior: stock shortages and capacity rejections produce deterministic diagnostics and rejected flows.
- Positive proof: `bundle://proof/SB19/transcripts/economy-tests.txt`.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Rejected transfer diagnostics | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleSimulationStateTransitionEngine.Mutations.cs` | Step result and frame flows | Apply transfer, clamp/reject, report | `bundle://proof/SB19/transcripts/economy-tests.txt` |
