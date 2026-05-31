# SB18 proof manifest

## Scope

Performance and bottleneck proofs.

## Changed files

- `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlLib.Tests/WebGlSceneCommandBatchTests.cs`
- `repo://CanDoItAll.Components/tools/webgllib/audit-sharedwell-performance.cjs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationPreparationTests.cs`
- `bundle://proof/SB18/economy-performance-proof.json`

## Proof

- Transcript: `bundle://proof/SB18/transcripts/performance-validation.txt`
- Economy proof artifact: `bundle://proof/SB18/economy-performance-proof.json`
- Components proof artifact: `repo://CanDoItAll.Components/artifacts/webgl-economy-sharedwell-hardening-v9/performance/components-performance-proof.json`
- Semantic invariants: `bundle://proof/SB18/semantic-invariants.md`

## Failing-first / semantic proof

The proof covers 1000 WebGL command batch items and an Economy synthetic scenario of 100 actors x 50 scheduled steps with warnings/metrics. Do not treat this as small-screen optimization work.

## Production Behavior Artifact Matrix

| Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- |
| WebGL command batch audit and Economy performance test | bundle proof and future bottleneck tracking | synthetic input -> measured counts/durations -> proof artifacts | No small/medium/mobile WebGL optimization was added. |
