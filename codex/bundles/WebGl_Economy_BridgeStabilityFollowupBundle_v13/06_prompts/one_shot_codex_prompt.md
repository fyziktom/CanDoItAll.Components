# One-shot prompt for Codex

You are working in two already checked-out repositories:

1. `CanDoItAll.Components`
2. `CanDoItAll.Economy`

Do not create a new branch. Work in the currently checked-out branch in each repo.

Implement this bundle phase by phase. Keep Components generic and UI/WebGL-only. Keep the joined simulation + visualization implementation in Economy, under `CanDoItAll.Economy.Simulation.WebGlBridge` and future Economy-side sandbox code.

Do not optimize WebGL for small, medium, mobile, tablet, or phone screens. WebGL proof is desktop/large-screen only.

Start with SB01. Do not start dependent subbundles before prerequisite proof is recorded. Every critical subbundle needs proof under `proof/SBxx/` with changed-file hashes, commands, source assertions, and semantic invariants.

Key goals:
- turn bridge from metadata-only skeleton into a generic run projection pipeline;
- build initial WebGL scene from economy visual frames;
- compile visual actions into actual WebGL run action plans and staged command batches;
- preserve ordered actions;
- keep strict deterministic experiment input pack validation;
- prevent example-specific terms from leaking into generic code.
