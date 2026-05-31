# Experiment Determinism Contract

The simulator must support Vernon L. Smith-style experimental discipline:

- Input values must be explicit, interpretable, and reproducible.
- Randomization is allowed only as a pre-run generation step.
- Randomization output must be persisted into versionable JSON before simulation starts.
- A run must be replayable using only persisted input files and the engine version.
- Every frame/delta hash must trace back to the experiment pack hash.
- Metrics and invariants must be computed from frames, not hand-written expectations.

## Required input pack documents

A production-ready experiment input pack should contain or reference:

```text
experiment.json
scenario.definition.json
placement.json
parameters.json
institution.rules.json
run.plan.json
visual.mapping.json
expected.invariants.json
```

All documents must have:

- schema version
- stable IDs
- canonical sorting / normalization
- real SHA-256 hash
- relative safe path
- strict validation result

## Strict hash rule

The strict validator must reject placeholder hashes:

```text
sha256:scenario
sha256:placement
sha256:parameters
```

Use:

```text
sha256:<64 lowercase hexadecimal characters>
```

## Runtime rule

The runtime simulator must not call random generators. It may read generated JSON from input packs.
