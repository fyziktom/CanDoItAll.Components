# Source reference matrix

| Source | Concern | Subbundle |
| --- | --- | --- |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Components/Components/SimulationSandbox/EconomySimulationSandboxPage.razor` | Runtime test fixture path, direct service construction, browser UI proof. | SB02, SB11 |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunFrameApplyResult.cs` | Direct frame commands dropped when stages exist. | SB03 |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneRevisionPolicy.cs` | Normalize does not fully mirror revision. | SB04 |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` | Runtime options ignored during scene reset. | SB04 |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js` | Strict vs warning partial patch mode. | SB05 |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs` | Domain provenance vs generic leakage. | SB06 |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunValidator.cs` | Static initial object id assumption. | SB07 |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/16-webgl-scene-models.js` | Instance ownership and material clone handling. | SB08 |
| `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/17-webgl-scene-resources.js` | Texture ownership and disposal. | SB08 |
| `repo://CanDoItAll.Components/Directory.Build.props` | Shared package version and stale package risk. | SB09 |
| `repo://CanDoItAll.Economy/README.md` | Stale repo overview after simulation additions. | SB10 |
