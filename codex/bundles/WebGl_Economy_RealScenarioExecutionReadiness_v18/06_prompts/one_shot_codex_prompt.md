# One-Shot Codex Prompt

You are working in two already-cloned repositories:

- `CanDoItAll.Components`
- `CanDoItAll.Economy`

Do not create or switch branches. Work only in the currently checked-out branch in each repository.

Implement the follow-up bundle `CanDoItAll_WebGl_Economy_RealScenarioExecutionReadinessBundle_v18`.

Primary goal: make the simulation + visualization join ready for real headless scenario testing while preserving generic, maintainable architecture.

Key rules:

1. Components must remain generic and Economy-free.
2. Joined simulation + visualization belongs in Economy, especially `CanDoItAll.Economy.SimulationSandbox` and `CanDoItAll.Economy.Simulation.WebGlBridge`.
3. WebGL is desktop/large-screen only. Do not add mobile/small/medium screen work.
4. Do not specialize generic source code for well/water/farmer/land examples.
5. Produce proof transcripts and generated real-probe artifacts.
6. If a proof command cannot run, record the exact blocker and do not fake transcript contents.

Execute subbundles in order:

- SB01 cross-repo inventory and branch guard
- SB02 Components runtime executable stage proof
- SB03 Components WebGlRunDocument runner contract
- SB04 Economy real headless scenario artifacts
- SB05 Economy SimulationSandbox session hardening
- SB06 bridge strict mapping and fallback policy
- SB07 snapshot analysis and diff completion
- SB08 domain leakage refactoring gate
- SB09 visual mapping and assets readiness
- SB10 performance and scalability probe
- SB11 first large-screen browser probe design
- SB12 validation and closure

Final output in each repo:

- updated code/tests
- updated audits if needed
- proof transcripts
- generated real-probe artifacts
- execution report

All source code comments must be in English.
