# Codex Master Prompt

You are executing the `CanDoItAll WebGL Engine + Economy Hardening Workflow Bundle`.

Work one subbundle at a time. Treat the bundle as the contract.

Before editing code:

1. Read `README.md`.
2. Read `plan/01-phase-plan.md`.
3. Read `traceability/01-requirement-traceability.md`.
4. Read the selected subbundle README.
5. Run the entry validation described in that subbundle.
6. Reopen source references listed under `## Exact Source References`.
7. Confirm current repo state because some items may already have changed.

During implementation:

- Keep `CanDoItAll.Components.WebGlLib` generic and lightweight.
- Keep `CanDoItAll.Components.WebGlRunLib` generic and domain-neutral.
- Keep Economy-specific mapping in `CanDoItAll.Economy.Simulation.WebGlBridge`.
- Do not move domain concepts downward into Components.
- Record proof while working; do not reconstruct proof from memory later.
- Run the mandatory refactor gate before closing each subbundle.
- For critical subbundles, create `proof/SBxx/manifest.md` and `proof/SBxx/semantic-invariants.md`.

Stop conditions:

- If a prerequisite gate is missing or weak, stop and repair/reopen the prerequisite.
- If current source contradicts this bundle, update the bundle current-state notes before implementation.
- If a test passes only because of fixture-specific data, add adversarial negative proof before closure.
