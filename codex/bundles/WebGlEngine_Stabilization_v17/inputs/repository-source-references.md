# Repository source references inspected

## Components files

- `CanDoItAll.Components.slnx`: solution composition includes WebGlLib, WebGlRunLib, sandbox, samples and tests.
- `Directory.Build.props`: default `IsPackable=false`.
- `src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj`: WebGlLib opt-in packable.
- `src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj`: WebGlRunLib opt-in packable.
- `package.json`: includes `webgl:validate-rc`.
- `scripts/validate-webgl-rc.ps1` and `scripts/webgl-engine/validate-release-candidate.ps1`: single release-candidate validation command.
- `src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor`: public boundary component.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/01-webgl-scene.js`: JS public surface.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/26-webgl-scene-command-batch.js`: command batch and settled-state behavior.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js`: wait-for-idle behavior.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/41-webgl-scene-runtime-idle-state.js`: semantic/visual idle model.
- `src/CanDoItAll.Components.WebGlRunLib/Actions/WebGlRunActionKinds.cs`: generic action vocabulary.
- `src/CanDoItAll.Components.WebGlRunLib/DomainDrivers/WebGlRunDomainMappingDriver.cs`: generic domain-driver contract.
- `samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj`: package-mode gap.
- `samples/CanDoItAll.Components.WebGlRunLibGenericSample/Program.cs`: current minimal generic route sample.
- `.github/workflows/domain-leakage.yml`: domain leakage CI.
- `tools/webgllib/domain-boundary-audit.config.json`: hard/soft domain audit profiles.
- `tests/CanDoItAll.Components.WebGlLib.Tests/WebGlLibFreezeApprovalTests.cs`: API/JS/package approval tests.
- `tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunFreezeApprovalTests.cs`: RunLib approval tests.

## External sources

- Three.js official docs: https://threejs.org/docs/
- PlayCanvas Engine docs: https://developer.playcanvas.com/user-manual/engine/
- PlayCanvas optimization docs: https://developer.playcanvas.com/user-manual/optimization/
- Babylon.js official website/features: https://www.babylonjs.com/
- regl GitHub README: https://github.com/regl-project/regl
