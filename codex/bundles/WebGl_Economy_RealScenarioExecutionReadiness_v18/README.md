# CanDoItAll WebGL + Economy Real Scenario Execution Readiness Bundle v18

This follow-up bundle hardens the current cross-repo WebGL/Economy simulation foundation after the v17 implementation.

## Repositories

- `fyziktom/CanDoItAll.Components`, branch currently used by Codex: `webgl-engine`
- `fyziktom/CanDoItAll.Economy`, current branch used by Codex: current checked-out branch, currently observed as `main`

## Core decision

The joined simulation + visualization layer must remain in `CanDoItAll.Economy`, not in `CanDoItAll.Components`.

`Components` provides generic WebGL scene/run/runtime primitives only.
`Economy` owns experiment input packs, simulation backends, visualization frames, WebGL bridge mapping, sandbox workflow, sessions, snapshots and scenario probes.

## Main answer

The implementation is close enough to run headless real scenario tests for the current fixture scenarios, especially `shared-well` and `farmer-land`.

It is not yet ready for a polished interactive browser demo without another hardening pass, because we still need stronger executable proof that the run document can be applied frame-by-frame to the WebGL runtime, stronger runtime-stage proof, stronger snapshot analysis services, and stricter fallback diagnostics.

## Bundle goals

1. Verify the current implementation did not regress layer boundaries.
2. Harden the generic runtime/action/stage execution path.
3. Harden Economy-side simulation sandbox sessions and snapshot workflows.
4. Prove the current shared-resource and finite-resource probes end-to-end in headless mode.
5. Prepare the first large-screen browser proof without moving Economy logic into Components.
6. Keep all generic layers free of example-specific vocabulary.

## Must-not rules

- Do not create a new branch. Work in the currently checked-out branch in each repository.
- Do not add any Economy reference to `CanDoItAll.Components`.
- Do not add SimpleAccounts or Ledger references to `CanDoItAll.Economy.Simulation.WebGlBridge`.
- Do not optimize WebGL for small/medium/mobile/tablet screens. Desktop/large-screen only.
- Do not treat fixtures or probe-specific words as acceptable in generic source code.
- Do not implement only the well/farmer examples; use them as readiness probes for generic primitives.

## Primary matrix

See `05_spreadsheets/implementation_matrix.xlsx`.

## Primary prompt

See `06_prompts/one_shot_codex_prompt.md`.
