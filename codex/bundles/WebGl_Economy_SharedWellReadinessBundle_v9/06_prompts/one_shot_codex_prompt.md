# One-shot Codex prompt

You are working in two already-cloned repositories:
- CanDoItAll.Components
- CanDoItAll.Economy

Do not create or switch branches. Work in the branch that is currently checked out in each repository.

Execute the v9 follow-up bundle. Use the shared-well scenario only as a generic readiness probe. Do not implement the final demo.

Main priorities:
1. Harden ordered WebGlRun action stages so sequence semantics cannot be broken by batching.
2. Add parity and performance proofs for C# and JS command batching.
3. Add canonical scenario definition and event normalizers in Economy.
4. Add generic behavior/rule event expansion interfaces.
5. Add simple state transition materializer so frames are not only hardcoded scenario outputs.
6. Add generic distance/inventory/trade/tax/admin primitives needed by the shared-well readiness probe.
7. Harden Economy visual action ordering and binding resolution.
8. Preserve strict separation: no Economy references in Components; no Components/WebGL references in Economy.
9. Do not optimize WebGL for small/medium/mobile/tablet screens. WebGL is desktop/large-screen only.

All source comments must be in English.
