# Normalized Requirements

| ID | Requirement | Acceptance signal |
| --- | --- | --- |
| R01 | Stabilize Components before further Economy work | Final RC freeze manifest says no Economy code changes were made |
| R02 | Freeze public C# APIs | API baselines exist and CI fails on unapproved public API drift |
| R03 | Freeze JS runtime API | webglScene API manifest exists and CI fails on unapproved method drift |
| R04 | Preserve ultra-light WebGlLib consumption | WebGlLib-only sample passes project and package mode without WebGlRunLib |
| R05 | Preserve generic WebGlRunLib consumption | Generic RunLib sample passes project and package mode without domain packages |
| R06 | Keep generic engine domain-free | Boundary audit scans source/tests/docs/tools/workflows with controlled allowlists |
| R07 | Make domain driver model explicit | Driver manifest/hash validation and scrubber tests pass |
| R08 | Harden idle/stop proof | Strict browser proof asserts semantic and visual idle after stop/apply-and-wait |
| R09 | Reduce WebGlSceneView risk | Public API preserved while implementation is split/refactored |
| R10 | Stabilize package contents | Package inventory snapshot and static asset proof pass |
| R11 | Document freeze policy | Docs define what future work belongs in Components vs domain drivers |
| R12 | Produce final red-team signoff | Final report lists all pass/fail/deferred items |
