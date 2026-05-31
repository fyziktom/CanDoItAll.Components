# SB04 Semantic Invariants

## INV-SB04-001 Sequence Actions Produce Stages

- Expected behavior: sequence steps become deterministic ordered stages in `WebGlSceneCommandBatch`.
- Shallow-pass trap: preserving only flat patches/motions and relying on metadata to imply order.
- Positive proof: `bundle://proof/SB04/transcripts/webglrunlib-tests.txt`.
- Source proof: `bundle://proof/SB04/transcripts/source-assertions.txt`.

## INV-SB04-002 Bridge Traceability Metadata Survives

- Expected behavior: action/stage/visual/source identifiers are present on emitted commands.
- Negative proof: tests assert the specific metadata fields on the built batch motion.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Staged command metadata | `repo://src/CanDoItAll.Components.WebGlRunLib/WebGlRunActionCompiler.cs` | `repo://src/CanDoItAll.Components.WebGlRunLib/Planning/WebGlRunActionPlanBatchBuilder.cs` | Compile, normalize, execute | `bundle://proof/SB04/transcripts/webglrunlib-tests.txt` |

