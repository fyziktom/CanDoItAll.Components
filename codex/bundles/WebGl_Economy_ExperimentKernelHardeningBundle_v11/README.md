# CanDoItAll WebGL + Economy Experiment Kernel Hardening Bundle v11

This bundle is a follow-up after Codex implemented the experiment-determinism wave in `CanDoItAll.Components` and `CanDoItAll.Economy`.

The goal is not to build the shared-well demo yet. The shared-well and farmer-land examples are used only as probes to detect missing generic simulation and visualization primitives.

## Non-negotiable rules

- Do not create a new branch. Work in the currently checked-out branch in each repository.
- Do not connect `CanDoItAll.Economy` to `CanDoItAll.Components` yet.
- Keep WebGL runtime domain-neutral.
- Keep WebGL large-screen / desktop only. Do not add small, medium, mobile, tablet, or phone optimization work.
- Keep all code comments in English.
- Treat JSON input packs as the source of truth for economic experiments. Runtime randomization inside simulation is forbidden.

## Main finding

The implementation is directionally strong, but there are still foundational risks:

1. WebGL ordered actions can still be broken by motion replacement or by executing staged batches without real stage timing.
2. Economy experiment input packs exist, but validation is too shallow for Vernon L. Smith-style controlled experiments.
3. Some supposedly generic Economy abstractions contain shared-well-specific fields such as `DailyWaterNeed` and `MaxDailyDraw`.
4. The simple transition engine exists, but it is still event-switch based, store lookups are scan-heavy, event effects are not interpreted generically, and placement/parameters/rules do not yet drive all transitions.
5. Shared-well readiness is better, but a repeatable demo would still require generic inventory, travel-cost, trade/resale, rule/tax/admin, and invariant evaluation support.
6. Farmer-land probe must be used to prevent overfitting the engine to a water/well example.

## Execution profile

Follow the CanDoItAll Bundle Workflow discipline:

- preserve raw inputs and source references;
- execute subbundles in order;
- keep proof under `proof/SBxx/`;
- update `reviews/01-execution-report.md` after each subbundle;
- record changed file hashes and semantic invariants;
- close every raw finding as Solved, Partially solved, or Not solved.

Start with `06_prompts/one_shot_codex_prompt.md`.

## Validation summary

Final status: Completed.

- Prepared-stage bundle validation passed after repairing missing validator, phase plan, traceability, assumptions/risks, and self-review artifacts.
- Components validation passed: runtime audit, C#/JS batch parity audit, motion queue audit, asset verification, WebGlLib tests, WebGlRunLib tests, and solution build.
- Economy validation passed: simulation boundary/no-runtime-randomness audit, full test suite, and solution build.
- Browser validation is N/A for this wave because the bundle explicitly forbids building the shared-well UI demo and the changes are library/runtime contracts.
- Raw notes N01-N07 are closed as Solved in `traceability/01-raw-note-closure.md` and `reviews/01-execution-report.md`.
