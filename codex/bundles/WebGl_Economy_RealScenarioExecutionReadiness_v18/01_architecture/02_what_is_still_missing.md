# What Is Still Missing Before Full Simulation + Visualization Join

## 1. Executable WebGL run proof

We need proof that `WebGlRunDocument` is not just a DTO artifact. It must be executable by a stable generic runner that can:

- load initial scene
- apply frame stages
- respect stage barriers
- wait for motions
- keep command/stage journal
- expose diagnostics
- pause and resume
- report errors without silent fallback

## 2. Runtime snapshot integration

Current snapshots include visual state, but actual browser runtime capture still needs a stable contract:

- active stage IDs
- pending stage IDs
- active motion IDs
- pending motion IDs
- command journal excerpt
- runtime diagnostics
- current scene revision
- selected object / camera state if relevant

The data snapshot remains primary; runtime visual snapshot is an attachment.

## 3. SimulationSandbox session lifecycle

The sandbox needs a stable session lifecycle beyond precomputed projection:

```text
Load -> Validate -> Materialize -> Project -> Seek -> Play -> Pause -> Snapshot -> Analyze -> Export
```

It should support backend-neutral operation and should not assume simple accounts forever.

## 4. Stronger fallback policy

The bridge should not silently route unresolved mappings to diagnostic fallback objects unless explicitly configured.

A strict mode should fail on:

- unresolved subject object
- unresolved target object
- missing category mapping
- missing asset mapping when visual mapping requires asset-backed objects
- unsupported action kind
- empty stage without explicit wait marker

## 5. Real probe artifacts

Create generated artifacts for at least two generic probes:

- shared-resource community probe
- finite-resource concentration probe

The artifacts must show input hashes, frames, visual frames, WebGL run document, snapshots, analysis and readiness report.
