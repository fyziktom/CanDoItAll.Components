# Normalized Requirements

| ID | Title | Requirement | Priority | Repo | Owning subbundles |
| --- | --- | --- | --- | --- | --- |
| REQ-001 | Preserve ultra-light WebGlLib core | WebGlLib must remain a domain-neutral render substrate usable for simple model viewing and scene visualization without requiring run/simulation packages. | Critical | Components | SB01,SB07,SB14 |
| REQ-002 | Fix JS module/runtime correctness | Detect and fix unresolved ES module symbols such as resolveObjectPosition before browser proof; add a repeatable static audit. | Critical | Components | SB02 |
| REQ-003 | Transactional patch semantics | JS patching must preflight validate or apply on a clone before committing so failed patches cannot leave partial state; align with C# reducer behavior. | Critical | Components | SB03 |
| REQ-004 | Canonical revision policy | Unify Scene.Revision vs UiState.Revision semantics across C#, JS, export/import, patching, document hashing and tests. | Critical | Components | SB03 |
| REQ-005 | Incremental update performance | Transform-only, symbol-only and link-only updates must avoid full scene rebuild; add diagnostics proving full rebuild was avoided. | Critical | Components | SB04 |
| REQ-006 | Texture-safe resource ownership | Separate material ownership from texture ownership so disposing a cloned material does not dispose shared template textures. | Critical | Components | SB05 |
| REQ-007 | Asset fallback/cache hardening | Make GLB/GLTF loading, primitive fallback, cache disposal and model diagnostics deterministic, observable and safe under repeated scene rebuilds/imports. | High | Components | SB05 |
| REQ-008 | Scene consistency validation | Add reusable scene validation for object IDs, link endpoints, layers, asset references, vectors, revision policy and metadata boundary. | High | Components | SB06 |
| REQ-009 | Typed diagnostics parity | Align C# WebGlRuntimeDiagnostics with JS diagnostics snapshot and mark unsupported/extra diagnostics intentionally. | High | Components | SB06 |
| REQ-010 | WebGlRunLib boundary hardening | Keep run/playback contracts generic, above WebGlLib, and free of Economy or production-line concepts; move or fence heavy semantics out of WebGlLib. | Critical | Components | SB08 |
| REQ-011 | Run action/stage/barrier semantics | Clarify and test action planning, command batch compilation, stage barriers, manual/event waits, motion queues and coalescing semantics. | Critical | Components | SB08,SB09 |
| REQ-012 | Economy bridge strict mapping | Harden Economy WebGlBridge so all actions carry source provenance, unresolved mappings are errors unless explicitly allowed, and fallback modes are auditable. | Critical | Economy | SB10 |
| REQ-013 | Economy generic simulator proof | Ensure the two current economy examples remain only examples over generic simulation contracts; add generic scenario/probe tests including larger scenarios. | High | Economy | SB11 |
| REQ-014 | Cross-repo package/project integration | Prove Components project-reference mode and package mode are both documented and buildable; prevent circular dependencies. | High | Both | SB12 |
| REQ-015 | Browser/performance/red-team proof | Require browser validation, stress metrics, memory-dispose proof, anti-stub scans, source hashes and final QA closure before completion. | Critical | Both | SB13,SB14 |
