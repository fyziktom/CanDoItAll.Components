# One-shot Codex prompt

You are working across two already-cloned repositories:

- `CanDoItAll.Components`
- `CanDoItAll.Economy`

Do not create a new branch. Work in the currently checked-out branch in each repo.

Implement the v16 follow-up hardening bundle. Keep `CanDoItAll.Components` generic and UI/WebGL-only. Keep the joined simulation + visualization workflow in `CanDoItAll.Economy`. Do not add Economy references to Components.

Primary goals:

1. Harden WebGL JS runtime maintainability and proof.
2. Add explicit stage barrier semantics beyond `waitSeconds`.
3. Strengthen per-object motion queue proof.
4. Add command/stage journal proof for delayed stages.
5. Harden Economy WebGL bridge validation and traceability.
6. Decompose broad bridge/visual mapping files before they grow further.
7. Promote snapshot builder/analyzer/diff/store capabilities from test-local proof into reusable contracts/services.
8. Make Economy `SimulationSandbox` backend-neutral instead of hardwiring SimpleAccounts.
9. Keep shared-resource and constrained-spatial-resource examples as probes only, not as production-specific kernel logic.
10. Run boundary, test and build gates.

Hard constraints:

- Do not implement final demo UI in this wave.
- Do not optimize WebGL for small/medium/mobile/tablet screens.
- Do not introduce TypeScript.
- Do not hide unresolved bridge mappings behind fallback objects unless an explicit fallback policy allows it.
- Do not allow generic code to contain example terms such as water/well/farmer/land/parcel except in fixtures, probe tests, scenario factories or docs.

Execute subbundles in order from `02_subbundles/` and update proof manifests under `proof/SBxx/`.
