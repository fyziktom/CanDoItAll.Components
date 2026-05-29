# SB07 - Standalone generic WebGL sandbox

## Goal

Create a separate sandbox project dedicated to WebGL proof pages.

## Project path

```text
src/CanDoItAll.Components.WebGlSandbox
```

## csproj

Use:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <AssemblyName>CanDoItAll.Components.WebGlSandbox</AssemblyName>
    <RootNamespace>CanDoItAll.Components.WebGlSandbox</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\CanDoItAll.Components.BaseLib\CanDoItAll.Components.BaseLib.csproj" />
    <ProjectReference Include="..\CanDoItAll.Components.OverlayLib\CanDoItAll.Components.OverlayLib.csproj" />
    <ProjectReference Include="..\CanDoItAll.Components.WebGlLib\CanDoItAll.Components.WebGlLib.csproj" />
  </ItemGroup>
</Project>
```

## Pages

Add:

```text
/
  Home page with links and runtime diagnostics.

 /tycoon-village
  Main village proof.

 /asset-catalog
  Lists detected/resolved assets and fallback status.
```

## Assets

Use `WebGlLibHeadAssets` and `WebGlLibBodyAssets` from WebGlLib.

Make sure the scene runtime script is included.

## Validation

Update:

```text
CanDoItAll.Components.slnx
README.md
```

Run:

```powershell
dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
dotnet build CanDoItAll.Components.slnx
```
