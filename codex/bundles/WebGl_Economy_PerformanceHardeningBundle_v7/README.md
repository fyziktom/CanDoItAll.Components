# CanDoItAll WebGL + Economy Performance Hardening Bundle v7

## Purpose

This bundle is a follow-up hardening and refactoring package after the latest Codex implementation in both repositories:

- `fyziktom/CanDoItAll.Components`, branch currently expected to be `webgl-engine`
- `fyziktom/CanDoItAll.Economy`, currently inspected on the default branch visible through GitHub

The goal is not to couple the two repositories yet. The goal is to prepare both sides so the future bridge can map economic simulation events to generic WebGL run actions without leaking WebGL types into Economy and without leaking Economy semantics into the generic renderer.

## Hard non-negotiable rules

1. Do not create a new branch. Work in the branch that is already checked out in each repository.
2. Do not optimize the WebGL renderer, sandbox, run playback, or WebGL CSS for small or medium screens.
3. Treat WebGL as a large-screen desktop surface only.
4. Keep all source-code comments in English.
5. Do not introduce TypeScript. Keep JavaScript modular, small, audited, and well tested.
6. Do not add a reference from `CanDoItAll.Economy.*` projects to `CanDoItAll.Components.*` packages in this wave.
7. Do not add Economy-specific concepts to `CanDoItAll.Components.WebGlLib` or `CanDoItAll.Components.WebGlRunLib`.

## Main deliverables

- Harden `WebGlLib` and `WebGlRunLib` performance and runtime safety.
- Move remaining demo-orchestration logic out of sandbox pages into reusable run services.
- Complete generic action planning semantics: event -> action -> command batch -> scene patch/motion.
- Harden scene command batching so it does not incorrectly coalesce ordered semantics.
- Add large-screen-only validation gates.
- Prepare Economy for loadable scenario definitions, simulation event streams, and visual intentions without WebGL coupling.
- Add targeted tests and audit scripts.
- Produce evidence reports after every refactoring gate.

## Most important workbook

See:

```text
05_spreadsheets/implementation_matrix.xlsx
```

The workbook contains subbundle ownership, priority, dependency constraints, performance risks, large-screen policy, validation gates, and source references.
