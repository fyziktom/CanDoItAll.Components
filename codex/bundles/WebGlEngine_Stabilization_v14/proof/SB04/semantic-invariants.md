# SB04 Semantic Invariants: JavaScript runtime API freeze baseline

## SB04-RC

Raw note owned: stabilize and freeze the generic Components WebGL engine without adding consuming-app semantics.

Expected behavior: The window.CanDoItAll.webglScene API has a JSON approval manifest with method names and result-shape expectations.

Disallowed shallow implementation: a change that only updates incomplete proof markers, snapshots, or diagnostics text without enforcing the runtime/source contract.

Failing-first or closed-gap proof: bundle://proof/SB04/transcripts/failing-first-or-closed-gap.txt

Passing proof: bundle://proof/SB16/transcripts/dotnet-test-webgllib-final.txt

Changed source files and hashes: bundle://proof/SB16/changed-file-hashes.txt

Production assertions: repo://tests/CanDoItAll.Components.WebGlLib.Tests/fixtures/approvals/webgllib-webglscene-js-api.approved.json; repo://tests/CanDoItAll.Components.WebGlLib.Tests/WebGlLibFreezeApprovalTests.cs

Adversarial negative case: JS manifest test fails if runtime method names drift or approved result shapes are missing.

Semantic positive case: JS API manifest test parses runtime methods and validates approved result shapes.

Downstream dependency check: final SB16 build, tests, package proof, browser proof, and domain-boundary audits passed.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| SB04-RC behavior | repo://tests/CanDoItAll.Components.WebGlLib.Tests/fixtures/approvals/webgllib-webglscene-js-api.approved.json; repo://tests/CanDoItAll.Components.WebGlLib.Tests/WebGlLibFreezeApprovalTests.cs | bundle://proof/SB16/transcripts/dotnet-test-webgllib-final.txt | bundle://proof/SB04/transcripts/implementation-validation.txt | bundle://proof/SB04/transcripts/failing-first-or-closed-gap.txt |

## Closure

Status: Solved. This invariant is backed by existing transcripts and the final bundle closure report.


