# SB11 Proof Manifest: Package-mode samples and static assets

Status: Completed
Invariant contract: bundle://proof/SB11/semantic-invariants.md
Changed-file hashes: bundle://proof/SB16/changed-file-hashes.txt

## Owned Requirements

- Components-only implementation scope.
- Package-mode samples and static assets.
- Preserve generic WebGlLib/WebGlRunLib boundaries and public API approval discipline.

## Proof Artifacts

| Evidence | Path |
|---|---|
| Failing-first or closed-gap | bundle://proof/SB11/transcripts/failing-first-or-closed-gap.txt |
| Implementation validation | bundle://proof/SB11/transcripts/implementation-validation.txt |
| Domain-boundary audit | bundle://proof/SB11/transcripts/domain-boundary-audit.txt |
| Changed files | bundle://proof/SB11/transcripts/changed-files.txt |
| Source assertions | bundle://proof/SB11/transcripts/source-assertions.txt |
| Anti-stub audit | bundle://proof/SB11/transcripts/anti-stub-audit.txt |
| Primary passing proof | bundle://proof/SB11/transcripts/dotnet-pack.txt; bundle://proof/SB11/transcripts/webglrunlib-sample-package-build.txt; bundle://proof/SB16/transcripts/dotnet-pack-final.txt |

## Source-Level Assertions

repo://samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj; repo://samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj; repo://src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj; repo://src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj

## Semantic Adequacy Gate

- Invariant id: SB11-RC
- Shallow-pass trap: accepting file existence, approval snapshot churn, or panel counters without proving the runtime/source behavior.
- Adversarial negative proof: ValidateComponentsWebGlRunLibReference target rejects missing local project reference unless package mode is explicit.
- Semantic positive proof: Project-mode and package-mode sample restore/build transcripts pass using generated nupkg output.
- Anti-stub audit: bundle://proof/SB16/transcripts/anti-stub-audit-owned-final.txt
- Raw-note closure: Solved for SB11; no scope words were narrowed and no consuming-app semantics were added to Components.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| SB11-RC behavior | repo://samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj; repo://samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj; repo://src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj; repo://src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj | bundle://proof/SB11/transcripts/dotnet-pack.txt; bundle://proof/SB11/transcripts/webglrunlib-sample-package-build.txt; bundle://proof/SB16/transcripts/dotnet-pack-final.txt | bundle://proof/SB11/transcripts/implementation-validation.txt | bundle://proof/SB11/transcripts/failing-first-or-closed-gap.txt |

## Closure Decision

Pass. Required proof artifacts are present, source references are portable, domain-boundary gates pass, and final validation is cited from the transcripts above.


