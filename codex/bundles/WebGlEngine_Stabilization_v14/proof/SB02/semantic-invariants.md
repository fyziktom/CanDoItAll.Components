# SB02 Semantic Invariants: Package scope and packable boundary

## SB02-RC

Raw note owned: stabilize and freeze the generic Components WebGL engine without adding consuming-app semantics.

Expected behavior: Directory.Build.props now defaults IsPackable=false; approved package libraries opt in; sandbox/sample projects opt out; package scope tests cover project and package reference modes.

Disallowed shallow implementation: a change that only updates incomplete proof markers, snapshots, or diagnostics text without enforcing the runtime/source contract.

Failing-first or closed-gap proof: bundle://proof/SB02/transcripts/failing-first-or-closed-gap.txt

Passing proof: bundle://proof/SB02/transcripts/failing-first-or-closed-gap.txt

Changed source files and hashes: bundle://proof/SB16/changed-file-hashes.txt

Production assertions: repo://Directory.Build.props; repo://samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj; repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/ComponentsPackageScopeTests.cs

Adversarial negative case: Package scope test rejects ambiguous WebGlRunLib sample reference mode and accidental packable projects.

Semantic positive case: ComponentsPackageScopeTests validate packable opt-in and RunLib sample package/project modes.

Downstream dependency check: final SB16 build, tests, package proof, browser proof, and domain-boundary audits passed.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| SB02-RC behavior | repo://Directory.Build.props; repo://samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj; repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/ComponentsPackageScopeTests.cs | bundle://proof/SB02/transcripts/failing-first-or-closed-gap.txt | bundle://proof/SB02/transcripts/implementation-validation.txt | bundle://proof/SB02/transcripts/failing-first-or-closed-gap.txt |

## Closure

Status: Solved. This invariant is backed by existing transcripts and the final bundle closure report.


