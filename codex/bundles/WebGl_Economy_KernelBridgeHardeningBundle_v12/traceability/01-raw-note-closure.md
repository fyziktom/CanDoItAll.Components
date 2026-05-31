# Raw Note Closure

| Raw note ID | Exact wording / source | Normalized requirements | Owning subbundle | Planned proof | Closure |
|---|---|---|---|---|---|
| RN-001 | "Do not implement a finished demo. Do implement the missing foundation that makes the demo possible without shortcuts." | RQ-C-001 through RQ-E-008 | SB02-SB18 | Source assertions, tests, readiness report, bridge skeleton only | Solved: `bundle://proof/SB02/manifest.md`, `bundle://proof/SB08/manifest.md`, `bundle://proof/SB14/manifest.md`, `bundle://proof/SB17/manifest.md`, `bundle://proof/SB18/manifest.md` |
| RN-002 | "`waitSeconds` is not a true asynchronous stage barrier." | RQ-C-001, RQ-C-002, RQ-C-003 | SB02 | JS audit/browser-order transcript | Solved: `bundle://proof/SB02/manifest.md` |
| RN-003 | "`queueMode=append` prevents replacement but does not yet mean a real sequential per-object queue." | RQ-C-004, RQ-C-005 | SB03 | Motion queue audit transcript | Solved: `bundle://proof/SB03/manifest.md` |
| RN-004 | "Sequence actions set metadata but do not clearly produce staged command batches." | RQ-C-006 | SB04 | C# staged-batch tests and source assertions | Solved: `bundle://proof/SB04/manifest.md` |
| RN-005 | "C# and JS normalizers are duplicated. They must produce equivalent stage, coalescing, and duplicate-motion decisions." | RQ-C-007 | SB05 | Shared JSON fixtures, C# tests, JS audit | Solved: `bundle://proof/SB05/manifest.md` |
| RN-006 | "`WebGlRunPlaybackController` ready for bridge-driven runs." | RQ-C-008 | SB06 | Run playback tests | Solved: `bundle://proof/SB06/manifest.md` |
| RN-007 | "Input pack fixture hashes are placeholders." | RQ-E-001 | SB07 | Strict validator tests with real hashes and stale hashes | Solved: `bundle://proof/SB07/manifest.md` |
| RN-008 | "Need a full experiment pack loader." | RQ-E-002 | SB08 | Loader tests for shared-well and farmer-land fixtures | Solved: `bundle://proof/SB08/manifest.md` |
| RN-009 | "Example-specific concepts can still leak into generic abstractions." | RQ-005, RQ-E-003 | SB09 | Forbidden-term scan and refactor source assertions | Solved: `bundle://proof/SB09/manifest.md` |
| RN-010 | "Handlers are still internal lambdas/static methods inside one large class." | RQ-E-004 | SB10 | Registry tests and source assertions | Solved: `bundle://proof/SB10/manifest.md` |
| RN-011 | "Transition engine state and diagnostics must be safer and easier to debug." | RQ-E-005 | SB11 | Diagnostic tests | Solved: `bundle://proof/SB11/manifest.md` |
| RN-012 | "Experiment interpretation explicit." | RQ-E-006 | SB12 | Metric/invariant tests | Solved: `bundle://proof/SB12/manifest.md` |
| RN-013 | "The bridge needs `visual.mapping.json`, but current Economy visual mapping is mostly code policy." | RQ-E-007 | SB13 | Serializable visual mapping tests/source assertions | Solved: `bundle://proof/SB13/manifest.md` |
| RN-014 | "Prepare the first bridge project without wiring a final demo." | RQ-E-008 | SB14 | Compile-only bridge contracts and reference scan | Solved: `bundle://proof/SB14/manifest.md` |
| RN-015 | "WebGL remains desktop / large-screen only." | RQ-004 | SB15 | Large-screen drift audit | Solved: `bundle://proof/SB15/manifest.md` |
| RN-016 | "Prevent the new foundation from becoming unmaintainable." | RQ-P-003 | SB16 | File-size audit | Solved: `bundle://proof/SB16/manifest.md` |
| RN-017 | "Build a readiness probe that proves both examples can pass through the same generic pipeline." | RQ-005, RQ-E-002, RQ-E-006, RQ-E-007 | SB17 | Readiness JSON artifact and genericity assertions | Solved: `bundle://proof/SB17/manifest.md` |
| RN-018 | "Performance bottleneck proofs." | RQ-P-003 | SB18 | Benchmark/smoke transcripts and risk notes | Solved: `bundle://proof/SB18/manifest.md` |
| RN-019 | "stable, maintainable, generic, ready for simulation-to-visualization bridge, no mobile drift." | RQ-P-003 | SB19 | Full command transcripts, final validator, closure report | Solved: `bundle://proof/SB19/manifest.md` |
