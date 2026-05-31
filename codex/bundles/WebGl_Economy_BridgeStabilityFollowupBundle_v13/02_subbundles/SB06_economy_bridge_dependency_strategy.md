# SB06 — Economy bridge dependency strategy for Components

## Problem
`CanDoItAll.Economy.Simulation.WebGlBridge.csproj` uses a relative sibling ProjectReference to `CanDoItAll.Components.WebGlRunLib`. This is convenient locally but fragile for CI/release.

## Tasks
- Keep bridge in Economy repo.
- Add a documented dual-mode strategy:
  - local development: project reference via configurable `ComponentsRepoRoot`
  - CI/release: package reference to `CanDoItAll.Components.WebGlRunLib`
- Add `Directory.Build.props` or MSBuild property support if appropriate.
- Add build failure message when local project reference path is unavailable and package mode is not configured.

## Tests
- local sibling checkout build works.
- package-mode build can be selected without editing project file.
- boundary audit remains green.
