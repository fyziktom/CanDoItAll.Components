# One-shot Codex prompt

You are working in two already-cloned repositories:

- CanDoItAll.Components
- CanDoItAll.Economy

Do not create a new branch. Work only in the currently checked-out branches.

Implement the follow-up bundle `CanDoItAll_WebGl_Economy_RealScenarioHardeningBundle_v19`.

Focus on hardening the current headless executable simulation + visualization pipeline. Do not build a final UI demo. Do not optimize WebGL for small/medium/mobile/tablet screens.

Primary goals:

1. Keep Components generic and Economy-free.
2. Keep joined simulation + visualization in Economy.
3. Harden WebGL stage barriers, command journal and motion queue proof.
4. Make `WebGlRunDocument` executable and inspectable without browser UI.
5. Add an Economy real-scenario runner that exports artifacts for shared-resource and finite-resource probes.
6. Harden snapshot runtime attachment and snapshot analysis services.
7. Ensure bridge strict mode fails unresolved mapping unless explicitly allowed.
8. Produce a readiness report that says whether a real browser large-screen smoke test is now safe.

All source-code comments must be in English.

Required validation:
- Components build/tests/audit.
- Economy build/tests/boundary audit.
- Real scenario runner creates non-empty artifacts.
- No empty proof transcript files.
