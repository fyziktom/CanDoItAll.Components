# SB12 - Economy SimulationSandbox preparation

Goal:
- Prepare, but do not yet overbuild, the combined sandbox.

Tasks:
1. Add design or skeleton for `CanDoItAll.Economy.SimulationSandbox`.
2. Sandbox may reference:
   - Simulation.Abstractions,
   - SimpleAccounts/Ledger as backends,
   - Simulation.Visualization,
   - Simulation.WebGlBridge,
   - Components WebGl UI packages if needed.
3. Sandbox should load input pack -> run backend -> visual frame -> bridge -> WebGl view.
4. Add placeholder routes only if low-risk.
5. Do not add final shared-well demo yet unless explicitly requested.

Acceptance:
- Clear wiring plan exists in Economy repo.
