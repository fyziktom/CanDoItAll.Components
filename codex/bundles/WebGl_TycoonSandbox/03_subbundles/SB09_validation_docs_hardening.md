# SB09 - Validation, docs, and hardening

## Build validation

Run:

```powershell
npm install
npm run webgllib:build-assets
npm run webgllib:verify-assets
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
dotnet build CanDoItAll.Components.slnx
```

## Browser validation

Run:

```powershell
dotnet run --project src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
```

Manual checks:

- `/` loads.
- `/tycoon-village` loads.
- No browser console errors.
- Canvas exists.
- Village visible.
- GLB assets or fallback primitives visible.
- Status symbols visible.
- Selection works.
- Inspector updates.
- Fit/reset/focus commands work.
- Proof snapshot works.

## Optional Playwright smoke test

Add only if the repo already has a test pattern. Otherwise document the test steps.

Suggested assertions:

- Page title contains WebGL sandbox.
- `[data-testid="webgl-scene-host"]` exists.
- `[data-testid="webgl-proof-object-count"]` has value > 0.
- `[data-testid="webgl-proof-symbol-count"]` has value > 0.
- Selection event updates `[data-testid="webgl-selected-object-id"]`.

## Documentation

Update:

```text
README.md
src/CanDoItAll.Components.WebGlLib/README.md
src/CanDoItAll.Components.WebGlSandbox/README.md
```

Include:

- What WebGlLib owns.
- What WebGlSandbox owns.
- How to add a new demo scene.
- How to add GLB assets.
- How future domain repos should consume WebGlLib.

## Implementation report

Create:

```text
artifacts/webgl-symbolic-tycoon-sandbox/IMPLEMENTATION_REPORT.md
```

Include:

- Summary.
- Files changed.
- Build results.
- Browser proof results.
- Known limitations.
- Follow-up work.
