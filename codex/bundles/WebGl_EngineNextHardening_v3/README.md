# CanDoItAll.Components WebGL Engine Next Hardening Bundle v3

Target repository: `fyziktom/CanDoItAll.Components`

Target branch rule: **work in the currently checked-out branch only**. For the current task this is expected to be `webgl-engine`, but Codex must verify it locally and must not create, switch to, or push a new branch unless the user explicitly asks.

This bundle continues after the `webgl-engine` branch hardening pass. The current branch already contains:
- A thin `window.CanDoItAll.webglScene` façade in `01-webgl-scene.js`.
- Split runtime modules for lifecycle, graph, drag, patching, motion, render loop, resources, model diagnostics, and shell.
- A `WebGlSceneDocument` serializer.
- Model diagnostics and Model Lab.
- External GLB model registration attempts.
- Runtime audit tooling.

The next step is not to add economy semantics into WebGlLib. It is to harden the generic WebGL engine, prepare a generic run/playback layer above it, and define a safe future boundary for economy simulations.

Recommended execution:
1. Read `01_codex_master_prompt.md`.
2. Execute subbundles in order.
3. Stop at every refactoring gate and produce evidence before continuing.
4. Keep WebGlLib generic and domain-neutral.
5. Put economy-specific implementation in a future Economy repo bundle only.

## Execution status

Completed on `webgl-engine` with evidence under `artifacts/webgl-engine-next-hardening/`.

- Refactoring gates 1-3 passed with line-count warnings below the hard audit threshold.
- Final command validation passed: npm install, audit, GLB inventory, asset build/verify, WebGlLib build, WebGlSandbox build, WebGlRunLib build, WebGlLib tests, and full solution build.
- Browser validation passed for `/tycoon-village`, `/model-lab`, and `/run-playback`; screenshots and proof notes are recorded in `artifacts/webgl-engine-next-hardening/browser/`.
- The v3 bundle originally lacked a validator script, so `scripts/validate_bundle.py` was added as a lightweight structural gate for prepared/completed checks.
