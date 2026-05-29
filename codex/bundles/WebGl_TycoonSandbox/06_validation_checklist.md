# 06 - Validation checklist

## Build commands

```powershell
npm install
npm run webgllib:build-assets
npm run webgllib:verify-assets
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
dotnet build CanDoItAll.Components.slnx
```

## Runtime command

```powershell
dotnet run --project src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
```

## Browser proof

Open:

```text
/tycoon-village
```

Verify:

- WebGL canvas renders.
- Village scene appears.
- At least one model loaded from GLB or visible fallback primitive.
- At least one building visible.
- At least one prop visible.
- At least one agent visible.
- At least three status symbols visible above objects.
- Symbol color/intensity are visibly different.
- Hover updates UI.
- Click selection updates inspector.
- Fit view works.
- Reset camera works.
- Proof snapshot returns non-zero object count.
- Proof snapshot returns non-zero symbol count.
- Browser console has no runtime errors.

## Dependency proof

Confirm no forbidden project references:

```powershell
Select-String -Path src/CanDoItAll.Components.WebGlSandbox/*.csproj -Pattern "Processes|Economy|CanDoItAll.Modules|CanDoItAll.Web"
Select-String -Path src/CanDoItAll.Components.WebGlLib/*.csproj -Pattern "Processes|Economy|CanDoItAll.Modules|CanDoItAll.Web"
```

Expected: no matches.

## Compatibility proof

Existing build must still pass:

```powershell
dotnet build src/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
```

Existing `WebGlWorkbench` runtime must remain exposed as:

```js
window.CanDoItAll.webglWorkbench
```

New scene runtime must be exposed as:

```js
window.CanDoItAll.webglScene
```
