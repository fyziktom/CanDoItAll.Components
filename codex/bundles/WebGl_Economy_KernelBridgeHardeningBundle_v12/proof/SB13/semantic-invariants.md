# SB13 Semantic Invariants

## INV-SB13-001 Visual Mapping Is Renderer-Neutral

- Expected behavior: Economy mappings use neutral categories, poses, and action keys, not direct renderer asset IDs.
- Negative proof: `bundle://proof/SB19/transcripts/economy-tests.txt`.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Neutral visual mapping | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/EconomyVisualMappingDefinition.cs` | `EconomyWebGlRunProjector` | JSON mapping to bridge context | `bundle://proof/SB19/transcripts/economy-tests.txt` |
