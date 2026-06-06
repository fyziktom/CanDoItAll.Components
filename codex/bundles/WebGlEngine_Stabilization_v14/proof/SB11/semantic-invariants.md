# SB11 Semantic Invariants: Package-mode samples and static assets

## SB11-RC

Raw note owned: stabilize and freeze the generic Components WebGL engine without adding consuming-app semantics.

Expected behavior: Package-mode sample proof builds WebGlLib-only and generic WebGlRunLib samples from generated packages and records static asset inventory.

Disallowed shallow implementation: a change that only updates incomplete proof markers, snapshots, or diagnostics text without enforcing the runtime/source contract.

Failing-first or closed-gap proof: bundle://proof/SB11/transcripts/failing-first-or-closed-gap.txt

Passing proof: bundle://proof/SB11/transcripts/dotnet-pack.txt; bundle://proof/SB11/transcripts/webglrunlib-sample-package-build.txt; bundle://proof/SB16/transcripts/dotnet-pack-final.txt

Changed source files and hashes: bundle://proof/SB16/changed-file-hashes.txt

Production assertions: repo://samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj; repo://samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj; repo://src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj; repo://src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj

Adversarial negative case: ValidateComponentsWebGlRunLibReference target rejects missing local project reference unless package mode is explicit.

Semantic positive case: Project-mode and package-mode sample restore/build transcripts pass using generated nupkg output.

Downstream dependency check: final SB16 build, tests, package proof, browser proof, and domain-boundary audits passed.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| SB11-RC behavior | repo://samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj; repo://samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj; repo://src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj; repo://src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj | bundle://proof/SB11/transcripts/dotnet-pack.txt; bundle://proof/SB11/transcripts/webglrunlib-sample-package-build.txt; bundle://proof/SB16/transcripts/dotnet-pack-final.txt | bundle://proof/SB11/transcripts/implementation-validation.txt | bundle://proof/SB11/transcripts/failing-first-or-closed-gap.txt |

## Closure

Status: Solved. This invariant is backed by existing transcripts and the final bundle closure report.


