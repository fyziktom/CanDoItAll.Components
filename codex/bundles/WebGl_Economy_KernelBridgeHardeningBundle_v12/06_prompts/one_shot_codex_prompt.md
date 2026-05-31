# One-shot Codex Prompt

You are working in two already-cloned repositories:
- CanDoItAll.Components
- CanDoItAll.Economy

Do not create a new branch. Work in the currently checked-out branch in each repository.

Implement the follow-up bundle `CanDoItAll_WebGl_Economy_KernelBridgeHardeningBundle_v12`.

Primary goals:
1. Harden Components stage execution and per-object motion queue semantics.
2. Harden WebGlRun action-plan-to-staged-batch conversion.
3. Harden Economy experiment input pack strict validation and loader pipeline.
4. Refactor remaining domain/example leakage out of generic models.
5. Prepare bridge-ready contracts/design without building the final demo.
6. Keep WebGL desktop/large-screen only. Do not optimize WebGL for small/medium/mobile/tablet screens.
7. Preserve genericity: shared-well and farmer-land must be probes, not hardcoded engine behavior.

Follow subbundle order from `02_subbundles/`.
Record proof under `proof/SBxx/`.
Run validation commands in `04_validation/validation_commands.md`.
Do not close until all critical subbundles have artifact-backed proof.
