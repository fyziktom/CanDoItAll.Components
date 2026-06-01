# One-shot Codex prompt

You are working in two already-cloned repositories:

- `CanDoItAll.Components`
- `CanDoItAll.Economy`

Do not create a new branch. Work only in the currently checked-out branches.

Implement the follow-up hardening/refactoring bundle `CanDoItAll_WebGl_Economy_ExecutableBridgeReadinessBundle_v17`.

Primary goals:

1. Harden generic WebGL stage barriers, motion queues, stage diagnostics and executable run document behavior in `CanDoItAll.Components`.
2. Keep all WebGL runtime JavaScript modular, small, domain-neutral and safe. Do not migrate to TypeScript.
3. Keep `CanDoItAll.Components` completely free of Economy references.
4. Keep the connected simulation + visualization implementation in `CanDoItAll.Economy`.
5. Harden `Simulation.WebGlBridge`, `SimulationSandbox`, snapshot builder/analyzer/diff/store and backend-neutral sandbox orchestration.
6. Ensure shared-resource and finite-resource probes remain generic and do not leak example-specific concepts into generic libraries.
7. Produce artifact-backed proof for every critical subbundle.

Strict rules:

- No new branch.
- No small/medium/mobile/tablet WebGL optimization or proof work.
- All source-code comments must be in English.
- If a requested behavior is not implemented, mark it as blocked with exact reason and follow-up, not as a vague residual risk.

Execute subbundles in dependency order from `02_subbundles/`.

Validation commands are in `04_validation/validation_commands.md`.
