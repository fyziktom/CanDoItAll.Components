# One-shot Codex prompt

You are working in two repositories already cloned locally:

- CanDoItAll.Components
- CanDoItAll.Economy

Do not create or switch branches. Work in the currently checked out branch in each repository.

Implement the follow-up hardening bundle v7. Keep WebGL large-screen desktop only and do not spend time on small/medium screen optimization. Do not introduce TypeScript. Keep JavaScript modular, audited, and safe.

In Components:
- Harden WebGlRunLib contracts and split large files.
- Add or finish reusable playback controller/action planner services.
- Harden command batching so ordered/sequential actions are not coalesced incorrectly.
- Add target/anchor resolution diagnostics.
- Ensure asset cache is disposed from lifecycle.
- Add performance proofs for large-screen desktop only.

In Economy:
- Split large Simulation.* files.
- Add loadable scenario definitions and validators.
- Add simulation event streams and materializers.
- Add WebGL-independent visual action DTOs/mappers.
- Keep SimpleAccounts, Ledger, Visualization, and Abstractions isolated.
- Do not connect Economy to Components yet.

Run all validation commands and write implementation/evidence reports.
