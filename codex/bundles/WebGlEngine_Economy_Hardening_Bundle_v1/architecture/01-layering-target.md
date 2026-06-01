# Target Layering Architecture

## Layer 1: `CanDoItAll.Components.WebGlLib`

Owns:

- generic `WebGlSceneModel`;
- scene objects, links, layers, anchors, symbols;
- asset catalogs, variants, import recipes, primitive fallbacks;
- render runtime options and diagnostics;
- `WebGlSceneView` Razor component;
- vanilla JS/Three.js scene rendering;
- browser interaction primitives: click, hover, select, drag;
- scene patches and command batches only as render-command transport;
- scene document serialization and hashing.

Must not own:

- simulation clock;
- replay lifecycle;
- economy rules;
- production-line rules;
- pathfinding as domain behavior;
- persistence providers;
- scenario lifecycle;
- ledger/market/agent semantics.

## Layer 2: `CanDoItAll.Components.WebGlRunLib`

Owns:

- generic run document;
- frames and action stages;
- action planning and batch compilation;
- generic visual-state and object-binding contracts;
- generic playback/run validation;
- adapters that transform run frames into WebGlLib scene patches and command batches.

Must not own:

- economy-specific action names;
- production-line-specific stations/machines/products;
- domain resource accounting;
- ledger or market semantics.

## Layer 3: `CanDoItAll.Economy.Simulation.WebGlBridge`

Owns:

- mapping `EconomyVisualFrame` and `EconomyVisualAction` into `WebGlRunDocument`;
- strict provenance from source experiment/frame/action/event/input pack;
- economy-specific visual mapping definitions;
- diagnostic fallback policies;
- validation that bad mappings do not silently render fake success.

Must not modify Components to support economy-only shortcuts.

## Future Layer 4: domain-specific simulators

Examples:

- economic experiment simulator;
- production-line simulator;
- process/workflow simulator.

These simulators consume WebGlRunLib and WebGlLib; they must not become dependencies of Components.
