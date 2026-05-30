# SB08 - Components: refactoring gate

Run after Components changes.

Audit and refactor if needed:

- `18-webgl-scene-model-diagnostics.js` should be split if over threshold;
- `11-webgl-scene-graph.js` should be split into object graph and link graph if over threshold;
- `13-webgl-scene-patching.js` should move command-result logic fully to `20-webgl-scene-command-results.js`;
- `WebGlSceneDocumentSerializer.cs` should be split into serializer, validator, hash calculator, normalizer.

Do not add new features during this gate except to preserve behavior after refactor.

Validation:

- `npm run webgllib:audit-scene-runtime`
- `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj`
