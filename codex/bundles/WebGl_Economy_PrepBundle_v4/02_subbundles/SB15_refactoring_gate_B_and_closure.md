# SB15 — Refactoring gate B and closure

## Goal

Force cleanup before the next feature wave.

## Tasks

1. Review newly added projects:
   - no giant files;
   - no anemic monolith service;
   - no duplicated hash logic;
   - no duplicated command result logic;
   - no misplaced domain types.
2. Generate:
   - `artifacts/webgl-engine-prep-v4/REFACTORING_GATE_B.md`
   - `artifacts/economy-simulation-prep-v4/REFACTORING_GATE_B.md`
3. Update docs:
   - Components WebGL/Run boundary.
   - Economy simulation architecture boundary.
4. Update XLSX status if Codex edits workbook or add completion report mapping each row.

## Done criteria

Do not mark complete unless:
- both repos build;
- tests pass;
- dependency scans pass;
- no new branch was created;
- no cross-repo references were added.
