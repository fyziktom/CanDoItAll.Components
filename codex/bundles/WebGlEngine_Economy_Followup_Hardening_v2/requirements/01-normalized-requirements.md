# Normalized requirements

| ID | Requirement | Priority | Owning subbundle |
| --- | --- | --- | --- |
| R01 | Remove runtime dependency on `tests/CanDoItAll.Economy.Tests/Fixtures` from Economy browser/Node sandbox. | P0 | SB02 |
| R02 | Introduce app-owned scenario catalog/loading abstraction for Economy simulation sandbox. | P0 | SB02 |
| R03 | Make WebGlRun mixed direct+staged frame commands impossible to silently lose. | P0 | SB03 |
| R04 | Define and enforce canonical scene revision policy across JS/C# document normalization/export/import/patching. | P0 | SB04 |
| R05 | Define browser reset semantics for `WebGlSceneDocument.RuntimeOptions`. | P1 | SB04 |
| R06 | Name and test patch transaction modes, including strict all-or-none and permissive invalid-link warning behavior. | P1 | SB05 |
| R07 | Clarify generic WebGlRun validation policy for domain provenance and source metadata. | P1 | SB06 |
| R08 | Decide static-only vs dynamic object timeline behavior for Economy WebGL bridge validation. | P1 | SB07 |
| R09 | Stress resource ownership and async asset load/dispose race behavior. | P1 | SB08 |
| R10 | Prove WebGlLib-only and WebGlRunLib package consumption without stale packages or accidental heavy dependencies. | P1 | SB09 |
| R11 | Refresh Economy simulation documentation, package map, and public surface boundaries. | P2 | SB10 |
| R12 | Harden Economy Simulation Sandbox UI proof: large screen, narrow width, Node route, scenario provider, browser runtime diagnostics. | P1 | SB11 |
| R13 | Audit proof manifest quality and remove/mark noisy or empty proof artifacts. | P1 | SB01, SB12 |
| R14 | Preserve genericity for future production-line simulators and non-economy visualizations. | P0 | All |
