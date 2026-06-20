# Risk register

| ID | Severity | Area | Risk | Required mitigation |
| --- | --- | --- | --- | --- |
| R01 | Critical | Runtime | Pause/stop proof can be false-positive if only C# loop stops | Browser proof must assert no active/queued motions/stages after pause |
| R02 | Critical | Simulation | Warnings can hide semantic model errors | Strict experiment mode must fail on semantic warnings |
| R03 | Critical | Simulation | Ambiguous store resolution biases results | Explicit store resolution policy and ambiguity errors |
| R04 | Critical | Metrics | Unknown metrics/invariants can produce zero-like pass | Typed registry and strict preflight |
| R05 | High | Replay | Full replay per step can become O(n²) | Add incremental and snapshot-anchor replay modes |
| R06 | High | Behavior | Hard-coded behavior expansion changes economic meaning | Versioned behavior expansion profiles |
| R07 | High | Proof | Browser proof may assert accepted commands, not settled state | Runtime idle await and settled diagnostics |
| R08 | High | Scenario packs | Pack hash is computed but manifest trust model still incomplete | Per-file hash manifest and signed/verified pack descriptor |
| R09 | High | UI | Visualization failures may contaminate economic validity | Separate validity bands and classify failures |
| R10 | Medium | Performance | Warning-only budgets are not enough | Hard budgets for headless deterministic operations |
| R11 | Medium | Persistence | Sync APIs coexist with async persistence semantics | Mark sync export with persistence as unsupported; prefer async |
| R12 | Medium | Documentation | Users may overtrust current simulator | Add confidence-level docs and troubleshooting |
