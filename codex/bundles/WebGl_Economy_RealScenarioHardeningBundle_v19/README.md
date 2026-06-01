# CanDoItAll WebGL + Economy Real Scenario Hardening Bundle v19

This follow-up bundle reviews the current pushed state after the previous executable bridge readiness work and prepares the next hardening phase.

Primary goal:

- keep `CanDoItAll.Components` generic and Economy-free,
- keep joined simulation + visualization in `CanDoItAll.Economy`,
- harden the current headless real-scenario path,
- prepare a first safe large-screen integration test without turning Components into an Economy runtime,
- keep scenario examples generic: shared finite resource, finite spatial/resource ownership, small producer/community trade.

This bundle assumes both repositories are already cloned locally and that Codex must work in the currently checked-out branches.

Hard rules:

1. Do not create a new branch.
2. Do not move Economy-specific code into Components.
3. Do not optimize WebGL for small/medium/mobile/tablet.
4. Do not implement a final UI demo yet unless explicitly requested later.
5. All source-code comments must be in English.
6. Every proof transcript must contain real output; empty transcript files are not acceptable.

Main spreadsheet:

`05_spreadsheets/implementation_matrix.xlsx`
