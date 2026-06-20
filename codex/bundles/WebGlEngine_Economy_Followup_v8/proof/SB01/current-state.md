# SB01 Current State

Captured on 2026-06-04.

## Baseline

- Components baseline proof exists in `proof/SB01/components-webgllib-tests-baseline.txt` and `proof/SB01/components-webglrunlib-tests-baseline.txt`.
- Economy baseline proof exists in `proof/SB01/economy-focused-tests-baseline.txt`.
- Proof inventory scan exists in `proof/SB01/proof-integrity-scan.txt`.

## Components Changed-File Hashes

- `src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor` sha256:4d5899f6665a02fc9d05ef871ccad4ae6fd777db941fe14d7b3db476889870ec
- `src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneProofSnapshot.cs` sha256:2a94a2bf2e818f12807c052da2dfa6c6296a1e788af07ff3222e35d600773e01
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js` sha256:c40873bb54889221ca0a8a3a8ca0ef0d48672b2c6c731a83ce2657ae69867412
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/26-webgl-scene-command-batch.js` sha256:fc587e3a03c0f656170e213a5c5bab51129a63b5315137dc05c9f86456b17567
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/39-webgl-scene-runtime-stop.js` sha256:a0085fc52c4c343d45db35d7cea849a508b0bb8fe96c8c44756a0f6fafa46504
- `src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunObserverProof.cs` sha256:e35b3dd17ce73adc1bffa98db71fcfd1ee49a8f830c7d83350e8c5cff32a4107
- `src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` sha256:e2994f5d72b137944654024f9791dac35e13eba6bd4c0b3f26aa76d5792ffbff
- `src/CanDoItAll.Components.WebGlSandbox/wwwroot/sandbox-webgl-proof.js` sha256:7ccf66facc3ab5399270c98af26b987d004772ceacda948bc5cfab1cdf40e20b
- `tests/CanDoItAll.Components.WebGlLib.Tests/WebGlSceneViewExternalImportLifecycleTests.cs` sha256:0b13e6f6fc5947a3d081b91a1326922cf637bc13e4aa557cac0d26b45fb62512
- `tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunValidatorTests.cs` sha256:c886e4b41f0e52e669702972ad4e249265dda4ce255bb5b9fa15c176d440ed3c

## Economy Changed-File Hashes

- `src/CanDoItAll.Economy.Cli/Program.cs` sha256:159792e8458dd210c176781953bc18e2a252ff7094d4cf2b41185fa7a6b96d59
- `src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationExperimentPolicies.cs` sha256:b03b062b05cecc6e6efc176b8c13618f888f1a5a8528646dfc5a19f92ffabe3e
- `src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationMetricAndInvariantEvaluation.cs` sha256:9aeccbc72a644f2c3a4f5072e3b2dba22f71455fe1a3620886d66816a74d73bb
- `src/CanDoItAll.Economy.SimulationSandbox/EconomyExperimentDesignHarness.cs` sha256:b4f976f8795c8cc611e295719fad0fbd68c250d679e590766886b34d8fc6f0e0
- `src/CanDoItAll.Economy.SimulationSandbox/EconomyExperimentReadiness.cs` sha256:74b43a1d0f007f663e93dd6044d795020d15bb2645f8184968940785029a743d
- `src/CanDoItAll.Economy.SimulationSandbox/EconomyHeadlessRunManifest.cs` sha256:ad3c25f8ab86852182f17960330f7e940bdcc34e0bce6f7c7952f3a15bd7f06a
- `tests/CanDoItAll.Economy.Tests/SimulationEconomicTrustHardeningTests.cs` sha256:96fd0d26e884df164f73243ba06f216a776ae4964f0893335fab8b2de14eac8d
- `tests/CanDoItAll.Economy.Tests/Fixtures/GoldenOracles/economic-oracles.json` sha256:09b3345c862760dc33fe5ad6797175664cb969f49f449f24d29926a42a3e5f73

## Boundary Decision

Components changes remain generic WebGL/runtime/observer proof behavior. Economy changes own readiness, diagnostic classification, oracle, manifest, metric, and design-comparison semantics. Browser evidence gates the browser-observer band only and is not economic ground truth.
