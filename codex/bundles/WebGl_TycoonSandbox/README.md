# CanDoItAll.Components WebGL Symbolic Tycoon Sandbox Workflow Bundle

## Purpose

This bundle is for Codex execution in the standalone `CanDoItAll.Components` repository.

The goal is to evolve the existing generic `CanDoItAll.Components.WebGlLib` into a reusable WebGL layer that can later support economy/process/project/agent visualizations without carrying any domain-specific logic. The first visible proof is a separate generic WebGL sandbox rendering a small tycoon-like village scene from GLB assets already present in the repository.

## Scope

Implement the generic pieces needed for a tycoon-style visualization:

- Scene models and services.
- Asset catalog models and resolution services.
- Status symbols above objects.
- Interaction state and typed events.
- Interop/runtime contracts.
- A new standalone generic WebGL sandbox.
- A small village demo scene using available GLB models.

## Hard boundaries

Do not add economy-specific terms such as debt, trust, conflict, rule complexity, attention price, collapse risk, or loans into `WebGlLib`.

Do not reference `CanDoItAll.Modules.Processes`, `CanDoItAll.Economy`, or the main `CanDoItAll` app.

Do not replace the existing `WebGlWorkbench` with a breaking rewrite. Add the new generic scene layer beside it first, then optionally create an adapter later.

## Recommended execution order

1. Inventory current `WebGlLib`, runtime JS, assets, and sandbox structure.
2. Add generic C# contracts in `WebGlLib`.
3. Add asset catalog/service contracts and default in-memory services.
4. Add generic symbol model and visual policy contracts.
5. Add selection/hover/interaction contracts.
6. Add `WebGlSceneView` component and JS runtime façade.
7. Add standalone `CanDoItAll.Components.WebGlSandbox`.
8. Add tycoon village sample data factory and page.
9. Validate builds, asset loading, deterministic snapshots, and browser proof.

## Execution Status

Completed on 2026-05-29.

- Prepared validation: `bundle://proof/SB01/transcripts/prepared-validation.txt`
- Final validation: `bundle://proof/SB09/transcripts/completed-validation.txt`
- Execution report: `bundle://reviews/01-execution-report.md`
- Implementation report: `repo://artifacts/webgl-symbolic-tycoon-sandbox/IMPLEMENTATION_REPORT.md`
- Browser proof: `bundle://proof/SB08/browser/webgl-tycoon-village-final-desktop.png`, `bundle://proof/SB08/browser/webgl-tycoon-village-final-mobile.png`, and `bundle://proof/SB08/browser/webgl-tycoon-village-final-proof.json`

## Important coding conventions

All source code comments must be in English.

Keep model classes nullable-safe, with reasonable defaults and no domain leakage.

Prefer additive changes and compatibility wrappers over rewrites.
