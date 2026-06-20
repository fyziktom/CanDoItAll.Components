# SB08 Semantic Invariants: Domain driver contract freeze

## SB08-RC

Raw note owned: stabilize and freeze the generic Components WebGL engine without adding consuming-app semantics.

Expected behavior: The pass-through domain driver only passes approved generic action kinds and maps unknown kinds to Wait; manifest schema approval was updated.

Disallowed shallow implementation: a change that only updates incomplete proof markers, snapshots, or diagnostics text without enforcing the runtime/source contract.

Failing-first or closed-gap proof: bundle://proof/SB08/transcripts/failing-first-or-closed-gap.txt

Passing proof: bundle://proof/SB16/transcripts/dotnet-test-webglrunlib-final.txt

Changed source files and hashes: bundle://proof/SB16/changed-file-hashes.txt

Production assertions: repo://src/CanDoItAll.Components.WebGlRunLib/DomainDrivers/WebGlRunDomainMappingDriver.cs; repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunValidatorTests.cs

Adversarial negative case: Unknown driver action kind maps to Wait rather than leaking unsupported vocabulary.

Semantic positive case: Validator tests prove unknown pass-through driver action kinds map to Wait.

Downstream dependency check: final SB16 build, tests, package proof, browser proof, and domain-boundary audits passed.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| SB08-RC behavior | repo://src/CanDoItAll.Components.WebGlRunLib/DomainDrivers/WebGlRunDomainMappingDriver.cs; repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunValidatorTests.cs | bundle://proof/SB16/transcripts/dotnet-test-webglrunlib-final.txt | bundle://proof/SB08/transcripts/implementation-validation.txt | bundle://proof/SB08/transcripts/failing-first-or-closed-gap.txt |

## Closure

Status: Solved. This invariant is backed by existing transcripts and the final bundle closure report.


