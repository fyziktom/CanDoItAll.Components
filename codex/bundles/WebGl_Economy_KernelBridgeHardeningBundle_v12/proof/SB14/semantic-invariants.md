# SB14 Semantic Invariants

## INV-SB14-001 Bridge Is Compile-Only And Boundary-Safe

- Expected behavior: bridge references only Economy Abstractions, Economy Visualization, and WebGlRunLib.
- Positive proof: `bundle://proof/SB19/transcripts/economy-tests.txt` and `bundle://proof/SB19/transcripts/economy-boundary-audit.txt`.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| WebGlRun bridge mapper | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs` | Future bridge host | Map generic visual actions to run actions | `bundle://proof/SB19/transcripts/economy-tests.txt` |
