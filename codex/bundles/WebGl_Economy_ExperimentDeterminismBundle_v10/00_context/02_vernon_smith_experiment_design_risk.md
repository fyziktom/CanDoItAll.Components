# Experiment design risk: why explicit inputs matter

The simulator is intended for economic experiments. It must therefore behave like a controlled laboratory instrument, not like an opaque toy world.

Practical implications inspired by Vernon L. Smith-style experimental discipline:

1. Define the institution/rules explicitly.
2. Define starting endowments explicitly.
3. Define private values, capacities, distances, rights, costs, and constraints explicitly.
4. Define actor topology and location explicitly.
5. Define randomization as a documented pre-simulation input generation step.
6. Save generated random placements/parameters as versioned input files.
7. The actual run must be deterministic from the persisted inputs.
8. Every run result must reference the exact input hashes that produced it.
9. Output interpretation must be tied to stated hypotheses and measured metrics, not "this looked interesting".

This bundle therefore adds an experiment input pack concept:

```text
ExperimentInputPack
  experiment.json
  scenario.definition.json
  placement.json
  parameters.json
  institution.rules.json
  run.plan.json
  visual.mapping.json
  expected.invariants.json
```

The seed is not enough. The seed plus generator version may reproduce generated data in theory, but for interpretability and auditability the generated data itself must also be persisted.
