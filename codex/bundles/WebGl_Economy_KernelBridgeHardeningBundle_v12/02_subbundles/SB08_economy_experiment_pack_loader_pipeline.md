# SB08 - Economy Experiment Pack Loader Pipeline

## Goal

Create a single high-level loader that transforms an input pack into a validated run input.

## Proposed service

```csharp
public interface ISimulationExperimentInputPackLoader
{
    SimulationExperimentLoadResult Load(string experimentJsonPath, SimulationExperimentLoadOptions options);
}
```

## Load steps

1. Load `experiment.json`.
2. Validate input pack.
3. Load referenced documents.
4. Verify hashes.
5. Deserialize:
   - scenario definition
   - placement
   - parameters
   - institution rules
   - run plan
   - visual mapping
   - invariants
6. Apply placement and parameters.
7. Normalize scenario.
8. Validate scenario.
9. Compile event stream.
10. Return traceable result with all hashes.

## Output

```text
SimulationExperimentLoadResult
  ExperimentInputPack
  ScenarioDefinition
  Placement
  Parameters
  RunPlan
  VisualMapping
  Invariants
  EventStream
  Hashes
  Diagnostics
```

## Tests

Use both shared-well and farmer-land fixtures.
