# Senior QA Inspector Preflight

This bundle is acceptable for Codex execution only if the following assumptions remain true:

- The next implementation wave is Components-only.
- The goal is stabilization and freeze, not new Economy capability.
- Any domain-specific need must be solved by domain-driver extensibility, not generic engine vocabulary.
- Proof artifacts must be non-empty and directly tied to code paths.
- Public API changes must be explicit and approved.

## QA concerns before execution

1. The package baseline must be taken before refactoring, otherwise it will be unclear what changed.
2. The JS API manifest must include method names and expected return result shapes, not only function names.
3. WebGlSceneView refactoring must preserve binary/source compatibility for host code.
4. Runtime idle strict mode must be tested in browser, not only via source scan.
5. Domain leakage must scan source and package output separately; allowlists for docs should not weaken shipping-source gates.
6. Samples must prove package mode from generated `.nupkg`, not only project references.
