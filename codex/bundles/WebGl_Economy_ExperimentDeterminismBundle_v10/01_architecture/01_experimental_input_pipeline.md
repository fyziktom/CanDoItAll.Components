# Experimental input pipeline

## Target pipeline

```text
ExperimentInputPack
  -> Load + validate
  -> Normalize to canonical scenario definition
  -> Hash canonical inputs
  -> Compile scheduled + generated events
  -> Expand behavior/rule policies
  -> Run deterministic simulation kernel
  -> Emit frames/deltas/events/metrics
  -> Map to visual intentions
  -> Later bridge to WebGlRunLib actions
```

## Required input files

| File | Purpose | Must be deterministic? |
| --- | --- | --- |
| `experiment.json` | identity, hypothesis, treatment, institution, source hashes | yes |
| `scenario.definition.json` | actors, resources, locations, stores, rules, behaviors | yes |
| `placement.json` | actual actor/object/store coordinates and topology | yes |
| `parameters.json` | capacities, travel costs, taxes, admin cost, demand settings | yes |
| `institution.rules.json` | allocation, resale, tax, anti-monopoly, enforcement rules | yes |
| `run.plan.json` | backend id, start/end step, seed reference, output settings | yes |
| `visual.mapping.json` | domain visual intentions to generic action categories | yes |
| `expected.invariants.json` | sanity checks and interpretation metrics | yes |

## Randomization policy

Randomization may be used only before simulation:

```text
RandomPlacementRequest + GeneratorVersion + Seed
  -> Generated placement.json
  -> Review / commit / hash
  -> Simulation consumes placement.json only
```

The simulator must not call random placement directly during the run unless the scenario explicitly models random shocks as scheduled events whose realized values are persisted before result interpretation.

## Output provenance

Every `SimulationRunIdentity` or result document should include:

- `experimentId`
- `inputPackId`
- `scenarioDefinitionHash`
- `placementHash`
- `parameterHash`
- `rulesHash`
- `runPlanHash`
- `compilerVersion`
- `backendId`
- `backendVersion`
