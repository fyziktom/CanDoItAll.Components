# Target architecture: research-grade simulation stack

## Layering

```text
Economy scenario pack
  -> scenario source/catalog
  -> scenario validation + strict policy
  -> behavior expansion profile
  -> headless simulation backend
  -> metric/invariant registry
  -> golden oracle/readiness gates
  -> reproducibility manifest + artifact lake
  -> optional WebGL projection
  -> optional browser observer proof
```

## Source of truth

Economic state is produced by headless simulation and validated by deterministic artifacts. WebGL never becomes the source of truth.

## Research mode

Add a named policy, for example:

```csharp
EconomyExperimentPolicy.ResearchStrict
```

It should make ambiguous or unknown behavior fail early. Demo modes can remain permissive, but reports must clearly label them as non-research-ready.

## Readiness levels

Recommended machine-readable status:

- `engineering-demo`
- `exploratory`
- `headless-valid`
- `oracle-valid`
- `browser-observer-valid`
- `research-ready`
- `not-comparable`
- `failed`

Only `research-ready` should be used for cross-scenario economic claims.

## Determinism

Every run should have:

- scenario pack hash
- behavior profile hash
- simulator version
- code commit hashes for both repos
- package versions
- deterministic seed
- frame hash chain
- metric/invariant output hashes
- readiness report hash
- artifact manifest hash

## Browser runtime

Browser runtime proof must distinguish:

- accepted
- scheduled
- active
- settled
- cancelled
- failed

A frame is not proven until the runtime is idle or an explicit expected non-idle state is recorded.
