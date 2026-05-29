# SB02 — JS Module Cleanliness and Audit Tool

## Goal

Keep pure JavaScript maintainable without converting to TypeScript.

## Current risk

The scene runtime has been split into smaller files, but lifecycle/core modules can still grow into catch-all files. Add an audit tool so future commits are warned before JS becomes hard to maintain.

## Implementation tasks

Create:

```text
tools/webgllib/audit-scene-runtime.cjs
```

The audit must check:

- scene runtime JS file line counts;
- warning threshold: 220 lines;
- failure threshold: 320 lines, except documented temporary allowlist;
- no `innerHTML` in runtime modules unless the file and line are explicitly allowlisted with a comment explaining why it is static and safe;
- no `eval`, `new Function`, dynamic remote script loading, or raw HTML injection;
- public runtime entry file `01-webgl-scene.js` remains thin;
- `window.CanDoItAll.webglScene` and `window.CanDoItAll.webglWorkbench` references are not accidentally removed from asset includes/tests;
- all runtime scene JS files parse as modules.

Add package script:

```json
"webgllib:audit-scene-runtime": "node tools/webgllib/audit-scene-runtime.cjs"
```

## Refactor guidance

If `10-webgl-scene-lifecycle.js` remains too broad, extract:

```text
17-webgl-scene-shell.js
18-webgl-scene-state.js
19-webgl-scene-notifications.js
20-webgl-scene-disposal.js
```

Keep `10-webgl-scene-lifecycle.js` as orchestration only.

## Acceptance criteria

- `npm run webgllib:audit-scene-runtime` passes.
- The audit output is saved to `artifacts/webgl-runtime-hardening-v2/runtime-audit.txt`.
- No scene runtime JS file exceeds the hard threshold without documented rationale.
- No new TypeScript files are introduced.

