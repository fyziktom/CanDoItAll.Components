# Validation Checklist

## Branch and inventory

- [ ] `git branch --show-current` captured.
- [ ] `git status --short` captured.
- [ ] No new branch created.
- [ ] JS file line inventory captured.
- [ ] Sandbox file line inventory captured.

## Build/test commands

```powershell
npm install
npm run webgllib:build-assets
npm run webgllib:verify-assets
npm run webgllib:audit-scene-runtime
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
dotnet build CanDoItAll.Components.slnx
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
```

## Browser proof

- [ ] `/tycoon-village` renders.
- [ ] Optional `/model-lab` renders if implemented.
- [ ] Primitive profile renders visible objects.
- [ ] Model-low profile renders or reports diagnostics.
- [ ] Model-high profile renders or reports diagnostics.
- [ ] Bad/invisible model produces diagnostics.
- [ ] Fallback remains visible.
- [ ] Patch command changes object transform.
- [ ] Motion command smoothly moves object.
- [ ] Motion completion diagnostics are visible.
- [ ] Export/import round trip works.
- [ ] Idle render scheduler sleeps in static scene.
- [ ] Continuous render mode continues rendering.
- [ ] Create/dispose repeat proof leaves one active canvas.
- [ ] `window.CanDoItAll.webglScene` exists.
- [ ] `window.CanDoItAll.webglWorkbench` still exists.
- [ ] Console has no unexpected errors.

## Evidence files

Write to:

```text
artifacts/webgl-runtime-hardening-v2/
```

Minimum:

```text
01_INVENTORY.md
IMPLEMENTATION_REPORT.md
VALIDATION.md
runtime-audit.txt
browser-summary.json
browser-console.log
browser-proof.json
```

