# WebGL Scene Runtime Module Map

The scene runtime stays dependency-inward: `01-webgl-scene.js` is the only public browser facade, and other modules must not import it.

| Module | Public responsibilities | Forbidden responsibilities | Allowed imports | Expected tests/proofs |
| --- | --- | --- | --- | --- |
| `01-webgl-scene.js` | `window.CanDoItAll.webglScene` facade and interop exception boundary. | Runtime state ownership, graph mutation, asset loading. | Runtime service modules only. | Facade line-count audit, browser route smoke. |
| `02-webgl-scene-core.js` | Three import, normalization, diagnostics snapshot, vector/material helpers. | Scene mutation and DOM shell changes. | Vendor Three only. | `node --check`, diagnostics JSON proof. |
| `03-webgl-scene-assets.js` | Asset lookup, profile/variant resolution, primitive fallback handoff. | GLTF loading internals and model-specific fixes. | Core, primitives, models, resources. | Asset profile browser proof and asset verifier. |
| `04-webgl-scene-symbols.js` | Status symbol groups, animation, billboard sync. | Simulation meaning for symbols. | Core, assets, resources, indexes. | Symbol toggle/animation browser proof. |
| `05-webgl-scene-interaction.js` | Pointer hit testing, selection, hover, keyboard basics. | Drag math and scene graph rebuilds. | Core, drag, graph. | Selection/hover browser proof. |
| `06-webgl-scene-camera.js` | Camera creation, controls, fit/focus/reset, viewport sync. | Render-loop ownership. | Core only. | Browser screenshots after fit/focus. |
| `07-webgl-scene-overlays.js` | HTML label overlay positioning. | Unsafe dynamic markup. | Core only. | Audit unsafe DOM checks. |
| `08-webgl-scene-proof.js` | Portable proof snapshot payload. | Hidden runtime mutation. | None. | Browser proof snapshots. |
| `09-webgl-scene-primitives.js` | Generated primitive meshes. | GLB template ownership. | Core, resources. | Primitive profile browser proof. |
| `10-webgl-scene-lifecycle.js` | State creation/update/dispose, handlers, renderer shell integration. | Command result factories and scheduler internals. | Core, assets, interaction, camera, graph, render loop, resources, shell, asset cache. | Build, route smoke, disposal diagnostics. |
| `11-webgl-scene-graph.js` | Object/link groups, visibility-aware rebuilds, transform sync. | Patch validation and asset cache ownership. | Core, assets, symbols, resources, indexes. | Layer visibility proof snapshot. |
| `12-webgl-scene-drag.js` | Ground-plane drag, constraints, snap, move callbacks. | Selection policy. | Core, interaction, graph. | Drag browser proof. |
| `13-webgl-scene-patching.js` | Patch normalization, validation, graph mutation, revision update. | Local command-result factories. | Core, interaction, graph, command results. | Malformed patch negative proof and detailed result. |
| `14-webgl-scene-motion.js` | Generic transform motion queue, cancellation, completion callback. | Pathfinding, physics, steering, simulation clocks. | Core, interaction, graph, command results. | Motion enqueue/cancel/completion proof. |
| `15-webgl-scene-render-loop.js` | Frame rendering and per-frame sync. | Scheduler policy and run clocks. | Symbols, camera, overlays, graph, motion, scheduler. | Idle scheduler proof. |
| `16-webgl-scene-models.js` | GLTF loading, model instance creation, import option application. | Asset-specific hacks and cache ownership policy. | Core, loader, diagnostics, resources, asset cache. | Model Lab diagnostics and screenshots. |
| `17-webgl-scene-resources.js` | Resource ownership marks and disposal counters. | Asset cache policy. | None. | Disposal diagnostics. |
| `18-webgl-scene-model-diagnostics.js` | Import options, bounds/material diagnostics, debug helpers. | Per-asset fixes. | Core only. | Model diagnostics JSON. |
| `19-webgl-scene-shell.js` | Host DOM shell creation. | Dynamic script/unsafe HTML. | None. | Unsafe DOM audit. |
| `20-webgl-scene-command-results.js` | Shared command result/failure history helpers. | Patch or motion semantics. | None. | Audit duplicate helper check. |
| `21-webgl-scene-asset-cache.js` | State-local GLB template cache lifecycle and counters. | Global cache/reference counting. | Resources. | Recreate/dispose diagnostics. |
| `22-webgl-scene-scheduler.js` | Auto/continuous/on-demand frame scheduling and idle state. | Rendering work or simulation time. | None. | Idle render-count browser proof. |
| `23-webgl-scene-indexes.js` | Object/link/layer/tag/asset indexes and visibility queries. | Scene mutation. | None. | Layer proof snapshot counts. |
| `24-webgl-scene-notifications.js` | Pointer normalization and bounded .NET callback notification helpers. | Selection policy and drag math. | Core only. | Import-cycle audit. |

Runtime modules may use browser APIs only through audited, bounded paths. Dynamic script creation, `eval`, `document.write`, unreviewed `innerHTML`, and unbounded global state are audit failures.
