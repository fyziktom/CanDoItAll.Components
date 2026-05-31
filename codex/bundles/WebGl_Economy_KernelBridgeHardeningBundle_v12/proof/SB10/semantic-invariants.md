# SB10 Semantic Invariants

## INV-SB10-001 Handler Order Is Stable

- Expected behavior: registry resolution is deterministic even when handlers are registered out of order.
- Positive proof: `bundle://proof/SB19/transcripts/economy-tests.txt`.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Ordered event handlers | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleSimulationEventHandlers.cs` | Transition engine | Event kind normalization to ordered application | `bundle://proof/SB19/transcripts/economy-tests.txt` |
