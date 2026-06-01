# Normalized Requirements

| Requirement | Summary | Owning subbundles | Proof expectation |
|---|---|---|---|
| R01 | Preserve the current branch state and verify cross-repo project boundaries before implementation. | SB01 | Branch, commit, project inventory, and dependency transcript. |
| R02 | Prove generic WebGL stage barrier policies without Economy-specific behavior. | SB02 | JS audit plus focused runtime/barrier tests. |
| R03 | Prove deterministic per-object motion queue behavior. | SB03 | JS audit plus focused motion queue tests. |
| R04 | Make generic `WebGlRunDocument` execution observable through a headless controller/runtime contract. | SB04 | WebGlRunLib tests with current frame/stage/action diagnostics. |
| R05 | Keep the Economy WebGL bridge maintainable by preserving projector splits and behavior parity. | SB05 | Economy tests and source assertions for projector responsibilities. |
| R06 | Prevent renderer-specific terms from leaking into `Simulation.Abstractions` and `Simulation.Visualization`. | SB06, SB12 | Boundary audit plus targeted source search. |
| R07 | Add or prove a headless Economy simulation sandbox session model with load/project/step/seek/pause/resume/snapshot/analyze operations. | SB07 | Shared-resource and finite-resource session tests. |
| R08 | Promote reusable snapshot build, diff, store, and analysis services with separate data, visual, and full hashes. | SB08 | Snapshot roundtrip, diff, and generic analysis tests. |
| R09 | Enforce strict bridge diagnostics so unresolved mappings and fallback behavior are visible. | SB09 | Positive strict mapping and adversarial negative mapping tests. |
| R10 | Prove the full headless chain from input pack to WebGL run execution, snapshot, and analysis for both probes. | SB10 | End-to-end headless tests. |
| R11 | Capture performance/scalability proof for large probe sizes and bounded runtime logs/queues. | SB11 | JSON performance artifact and focused tests/audits. |
| R12 | Produce a concrete readiness report for the first future demo without building the final UI in this bundle. | SB13 | Report with exact files/tests/gaps. |
| R13 | Run final required Components and Economy validation commands and close proof artifact gaps. | SB14 | Build, test, audit transcripts plus final bundle validator output. |
