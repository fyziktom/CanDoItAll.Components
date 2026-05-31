# Semantic invariants SB07

Status: Completed

| Invariant | Raw note | Expected behavior | Disallowed shallow implementation | Passing proof | Negative proof |
| --- | --- | --- | --- | --- | --- |
| INV-WEBGL-QUEUE | N01 | Same-object append motions run sequentially and staged waits schedule later commands. | Only preserving duplicate motion metadata while running motions concurrently. | bundle://proof/SB20/transcripts/components-npm-audit-motion-queue.log | bundle://proof/SB20/red-team-proof-audit.md |
| INV-INPUT-PACK | N02 | Input packs reject duplicate, missing, unsafe, malformed, and stale-hash inputs. | Only checking that some inputs list exists. | bundle://proof/SB20/transcripts/economy-dotnet-test.log | bundle://proof/SB20/transcripts/economy-dotnet-test.log |
| INV-GENERIC-PARAMS | N03 | Generic parameters are resource-scoped, not water/well-specific fields. | Renaming tests while leaving DailyWaterNeed or MaxDailyDraw in production contracts. | bundle://proof/SB20/changed-file-hashes.tsv | bundle://proof/SB20/red-team-proof-audit.md |
| INV-TRANSITION | N04 | Event dispatch, store lookup, effects, metrics, and invariants are generic and deterministic. | A switch/count-only proof or fixture-specific branch. | bundle://proof/SB20/transcripts/economy-dotnet-test.log | bundle://proof/SB20/transcripts/economy-boundary-audit.log |
| INV-PROBES | N05/N06 | Shared-well and farmer-land probes run from JSON input-pack referenced files. | Hardcoded runtime state that bypasses pack documents. | bundle://proof/SB20/transcripts/economy-dotnet-test.log | bundle://proof/SB20/red-team-proof-audit.md |

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| MotionQueue | repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/14-webgl-scene-motion.js | repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/15-webgl-scene-render-loop.js | bundle://proof/SB20/transcripts/components-npm-audit-motion-queue.log | bundle://proof/SB20/red-team-proof-audit.md |
| InputPackValidation | repo://src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationExperimentInputPack.cs | repo://tests/CanDoItAll.Economy.Tests/SimulationExperimentInputTests.cs | bundle://proof/SB20/transcripts/economy-dotnet-test.log | bundle://proof/SB20/transcripts/economy-dotnet-test.log |
| SimulationInvariantEvaluation | repo://src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationMetricAndInvariantEvaluation.cs | repo://tests/CanDoItAll.Economy.Tests/SimulationExperimentInputTests.cs | bundle://proof/SB20/transcripts/economy-dotnet-test.log | bundle://proof/SB20/red-team-proof-audit.md |
