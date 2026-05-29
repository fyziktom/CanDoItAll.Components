# SB02 — JS Runtime Module Cleanliness

## Goal

Keep JavaScript clean, modular, safe, and maintainable without TypeScript.

## Findings to address

The current module split is good, but several modules are still large enough to drift:
- lifecycle owns state creation, handlers, diagnostics, lights, dispose, notifications.
- patching owns normalization, application, result building, failure handling.
- model diagnostics owns import options, template diagnostics, material fixes, debug bounds.
- asset catalog and model loading are separate, but model visibility/fallback behavior needs clearer ownership.

## Tasks

1. Extend `tools/webgllib/audit-scene-runtime.cjs`:
   - report import graph,
   - detect circular imports,
   - detect modules importing from public façade `01-webgl-scene.js`,
   - detect duplicate local implementations of command-result builders,
   - detect duplicate vector normalization helpers,
   - keep façade under 180 lines,
   - keep ordinary modules under 260 warning / 340 fail unless allowlisted with reason.

2. Add `docs/webgl/js-runtime-module-map.md`:
   - table of each module,
   - public responsibilities,
   - forbidden responsibilities,
   - allowed imports,
   - expected tests/proofs.

3. Split only if needed:
   - `10-webgl-scene-lifecycle.js` may split `state`, `notifications`, `handlers`, `lights`.
   - `13-webgl-scene-patching.js` should move command result helpers to a shared module.
   - `18-webgl-scene-model-diagnostics.js` may split import options vs diagnostics vs debug helpers.

4. Prohibit unsafe DOM/API usage:
   - no dynamic script creation,
   - no unsafe `innerHTML` except static audited empty-state markup,
   - no `eval`,
   - no unbounded global state.

## Validation

```powershell
npm run webgllib:audit-scene-runtime
node --check src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/01-webgl-scene.js
```

## Done criteria

- Runtime module map updated.
- Audit catches import cycles and duplicate helpers.
- No module becomes a hidden monolith.
