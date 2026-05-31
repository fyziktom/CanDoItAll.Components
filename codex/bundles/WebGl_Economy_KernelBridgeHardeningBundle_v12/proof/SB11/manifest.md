# SB11 Proof Manifest

Status: Completed

## Scope

Transition engine diagnostics now report unknown event kinds, missing actors/stores, capacity rejection, insufficient stock, and negative stock prevention/allowance.

## Evidence

| Artifact | Purpose |
|---|---|
| `bundle://proof/SB19/transcripts/economy-tests.txt` | Diagnostic tests for unknown handlers, capacity overflow, insufficient stock, and non-negative behavior. |
| `bundle://proof/SB19/transcripts/source-assertions.txt` | Source assertions for diagnostic codes. |

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| `SimpleSimulationStepResult.Messages` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleSimulationStateTransitionEngine.cs` and `.Mutations.cs` | Tests, readiness diagnostics, future UI | Normalize/apply event and collect warnings/errors | `bundle://proof/SB19/transcripts/economy-tests.txt` |
