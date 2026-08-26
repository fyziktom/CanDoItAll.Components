# Repository Agent Instructions

## Shared Standards

Follow the reviewed standards in a resolved `CanDoItAll.SharedInfo` clone. This
repository owns its local implementation and documented exceptions.

Use `$apply-candoitall-shared-standards` when available. It checks an explicit
`CANDOITALL_SHAREDINFO_ROOT` and nearby sibling locations without assuming a fixed
developer profile or repositories directory.

## Repository Scope

- This repository owns reusable Blazor component libraries, their static assets,
  package metadata, tests, and maintained visual samples.
- It does not own application-specific workflows or the file-provider and editor
  packages maintained by `CanDoItAll.FileTools`.
- Keep shared family policy in `CanDoItAll.SharedInfo`; keep Components implementation
  and documented exceptions here.

## Commands

- Restore: `dotnet restore CanDoItAll.Components.slnx --configfile NuGet.config`
- Build: `dotnet build CanDoItAll.Components.slnx --configuration Release`
- Test: `dotnet test CanDoItAll.Components.slnx --configuration Release`
- Validate assets: `npm run build:tailwind; npm run assets:verify`
- Package: `.\tools\deployment\nugets\Build-NuGets.ps1`

## Component Proof

- Reusable `BaseLib` changes require small, medium, and large viewport coverage in the
  Sandbox when the behavior is viewport-sensitive.
- Other libraries target large-screen application use by default; preserve existing
  responsive behavior unless cross-viewport work is explicitly in scope.
- Prefer shared component parameters and Sandbox proof routes over application-specific
  structural CSS.

## Tooling Layout

- Put new automation under a lower-case `tools/<area>` directory.
- `tools/pack-packages.ps1`, `scripts/pack-release.ps1`, and the
  `scripts/webgl-engine` entry points are compatibility paths; do not add new tooling
  beside them.
- Keep generated packages and proof under ignored `artifacts` or `output` directories.

## Safety

- Keep sibling repositories read-only unless the user explicitly requests a multi-repo
  change.
- Do not commit generated output, local settings, credentials, runtime state, browser
  artifacts, or task proof.
- Preserve repository-specific changes unrelated to the active task.
