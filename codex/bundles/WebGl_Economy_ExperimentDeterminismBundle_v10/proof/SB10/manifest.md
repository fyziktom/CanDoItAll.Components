# SB10 proof manifest

## Scope

Economy typed references and event taxonomy.

## Changed files

- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Events/SimulationRef.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Events/SimulationEvent.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Events/SimulationEventKindRegistry.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Events/SimulationEventNormalizer.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Hashing/SimulationDeterministicHash.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationExperimentInputTests.cs`

## Proof

- Transcript: `bundle://proof/SB08/transcripts/economy-validation.txt`
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- Semantic invariants: `bundle://proof/SB10/semantic-invariants.md`

## Failing-first / semantic proof

`TypedEventRefs_DistinguishActorWaterFromResourceWater` proves typed refs preserve actor/resource identity even when legacy string ids collide.

## Production Behavior Artifact Matrix

| Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- |
| `SimulationEventNormalizer` typed refs and canonical taxonomy | event hashes, event stream, visual action mapping | legacy strings -> typed refs/canonical kind -> deterministic downstream behavior | Actor `water` and resource `water` remain separate references. |
