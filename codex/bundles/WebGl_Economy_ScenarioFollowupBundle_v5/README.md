# CanDoItAll WebGL + Economy Scenario Follow-up Bundle v5

This bundle is for two repositories that must stay prepared independently:

- `CanDoItAll.Components`
- `CanDoItAll.Economy`

The implementation must not directly connect Economy to WebGL yet. The goal of this bundle is to harden what was implemented, define the missing generic run/action/scenario layers, and prepare both repositories for a later integration wave.

## Non-negotiable execution rules

1. Do not create a new branch. Work in the branch that is already checked out in each repository.
2. Do not add references from `CanDoItAll.Components` to `CanDoItAll.Economy`.
3. Do not add references from `CanDoItAll.Economy.Simulation.*` projects to `CanDoItAll.Components.*` or WebGL packages.
4. Keep `WebGlLib` generic. It may render scenes and execute generic scene commands, but it must not know economy concepts.
5. Keep `WebGlRunLib` generic. It may define run documents, frame sources, event/action mapping, timeline playback, and command scheduling, but it must not know economy concepts.
6. Economy may define simulation scenarios, simulation events, visual frames, and visual actions, but it must not emit WebGL DTOs yet.
7. Every subbundle must end with a local refactoring checkpoint and validation evidence.

## Key outputs expected from Codex

- Components repo:
  - Harden `WebGlLib` after current `webgl-engine` branch.
  - Mature `WebGlRunLib` beyond DTO skeletons.
  - Move sandbox-only playback logic into generic reusable services.
  - Add generic action/event mapping without domain semantics.
  - Fix resource lifetime, scene indexes, patching, command result, and scene document issues.

- Economy repo:
  - Harden new `Simulation.*` projects.
  - Split large files.
  - Add generic scenario definition/load/validate contracts.
  - Add simulation event/action contracts suitable for later visualization.
  - Prepare `Shared Well Community` and `Small Entrepreneur Community` as scenario definitions, not only hardcoded frames.
  - Keep simple-account and ledger-backed simulation isolated behind shared abstractions.

See `05_spreadsheets/implementation_matrix.xlsx` for the authoritative implementation matrix.
