# One-shot Codex prompt

You are working in two already-cloned repositories:

- `CanDoItAll.Components`
- `CanDoItAll.Economy`

Do **not** create a new branch. Work in the currently checked-out branch in each repository.

Implement this follow-up bundle v15.

Primary goals:

1. Harden the generic WebGL stage runner and per-object motion queue in Components.
2. Keep all Components code generic and free of Economy references.
3. Harden the Economy-side `Simulation.WebGlBridge`.
4. Keep the connected simulation + visualization version in Economy, not Components.
5. Add/strengthen first-class simulation snapshot builder, analyzer, store, diff, and visual attachment workflows.
6. Keep WebGL desktop/large-screen only. Do not add mobile/tablet/small-screen work.
7. Remove or quarantine example-specific vocabulary from generic code.
8. Update tests, boundary audits, proof manifests, and execution report.

Before implementation:

- Read `05_spreadsheets/implementation_matrix.xlsx`.
- Execute SB01 inventory first.
- Validate current branches.
- Run existing boundary audits.
- Confirm which code already exists so you do not duplicate it.

Execution order:

1. SB01
2. SB02-SB05 Components hardening
3. SB06-SB12 Economy bridge/snapshot/sandbox hardening
4. SB13-SB15 readiness/performance probes
5. SB16 closure

Required proof:

- Components build/test/audit transcripts.
- Economy build/test/boundary-audit transcripts.
- Snapshot export/import hash proof.
- Bridge projection proof with real patches/motions/stages.
- Negative tests for unresolved mappings and domain leakage.
- No new branch.
