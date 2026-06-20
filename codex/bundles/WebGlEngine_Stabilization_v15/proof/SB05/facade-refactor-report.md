# SB05 Facade Refactor Report

`WebGlSceneView` public interop methods were moved from:

- `src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor`

to:

- `src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.Interop.cs`

The Razor file now keeps markup, parameters, lifecycle callbacks, and disposal. Public method names and signatures remain frozen through WebGlLib approval tests.

Validation:

- WebGlLib tests passed: 66/66.
- WebGlLib package-content approval was regenerated for the new partial file.
