# SB05 Semantic Invariants

## Invariant SB05-BRIDGE-001

Raw notes:
- RN-005: "Harden Simulation.WebGlBridge so it produces traceable executable WebGL run documents and can attach visual state metadata to snapshots."
- RN-006: "Validate shared-resource and finite-resource probes without hardcoding example-specific concepts into generic models."
- RN-010: "Do not duplicate global input actions across every frame."

Expected behavior:
- Bridge projection maps visual nodes, links, symbols, layers, and node-object context into an initial scene.
- Non-trivial visual actions produce WebGL run frames with non-empty executable stages and motion/patch commands.
- Global actions are partitioned by frame step and not duplicated across every frame.
- Missing subject/target nodes, missing asset mappings, unsupported visual action fallback to `Wait`, and invalid plans are emitted as bridge diagnostics instead of being silent.
- Shared-resource and finite-resource probes use the same generic bridge path.

Shallow-pass trap:
- A bridge can appear successful by creating metadata-only documents or fallback objects while silently dropping unresolved actions and missing visual mappings.

Adversarial negative proof:
- `ProjectorRecordsDiagnosticsForUnresolvedMappingsMissingAssetsAndWaitFallbacks` builds an input with missing assets, unresolved subject/target nodes, an unsupported action kind, and an invalid move action. It asserts diagnostics are present in the projected document metadata.
- The prior fallback-masked implementation would not report all of these diagnostics.
- Transcript: `bundle://proof/SB05/transcripts/economy-webglbridge-tests.txt`.

Semantic positive proof:
- `ProjectorCreatesExecutableStagesForSharedAndFiniteResourceProbes` proves both `shared-resource` and `finite-resource` probe inputs produce initial scene links plus a WebGL run frame with an executable motion stage and source input hash.
- Existing bridge tests prove initial scene object/link projection, traceable command stages, and non-duplicated global action partitioning.

Anti-stub audit:
- `bundle://proof/SB05/source-assertions/anti-stub-scan.txt` shows no production TODO, NotImplemented, fixture-specific branch, hardcode marker, SimpleAccounts reference, or Simulation.Ledger reference in bridge files.

Changed source files:
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlInitialSceneProjector.cs`.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs`.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs`.
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeTests.cs`.

Downstream dependency check:
- SB06, SB11, SB12, SB13, and SB14 can rely on bridge projection diagnostics and executable stage proof.
