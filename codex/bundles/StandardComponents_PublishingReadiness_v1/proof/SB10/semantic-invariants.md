# SB10 Semantic Invariants

## SB10-INV-001 Packable Standard Libraries

- Invariant ID: `SB10-INV-001`
- Source raw note: RAW01 requires preparation of the standard component repositories for publishing.
- Expected behavior: `CanDoItAll.Components.Common`, `BaseLib`, `Charts`, `OverlayLib`, and `Mermaid` are explicitly packable, use shared package metadata from `Directory.Build.props`, include package readmes, and produce NuGet archives with expected `net10.0` DLLs and static assets.
- Disallowed shallow implementation: rely on project metadata only, pack the solution including excluded Canvas/WebGL packages, skip actual `.nupkg` inspection, or leave source/build leakage inside archives.
- Failing-first test: before SB10, the standard package set did not have a locked proof archive verification gate for all five standard libraries.
- Passing test: `bundle://proof/SB10/transcripts/sb10-standard-pack.txt` creates all five packages, `bundle://proof/SB10/transcripts/sb10-package-verifier.txt` verifies them, and `bundle://proof/SB10/transcripts/sb10-source-assertions.txt` prints `SB10-INV-001`.
- Changed source files: `repo://Directory.Build.props` SHA-256 `cf86702e504b6faf3ea0f4dcded56cdea46c8db248c4af5d8e84e169967b9bb9`; `bundle://scripts/verify-sb10-packages.py` SHA-256 `b641e7354e0d0b868fa22585c2d33ef4325d023f26e57e2351fcd165eee10923`.
- Production assertions: `bundle://proof/SB10/data/sb10-package-verification.json` proves required DLL/readme/nuspec/static-asset expectations, SHA-256 package hashes, and no source/build leakage.
- Red-team negative case: the package verifier rejects missing DLL/readme/nuspec/static assets and forbidden source/build entries.
- Downstream dependency check: SB11 and SB12 can validate visual and transfer readiness against the exact standard package set instead of broad solution output.

## SB10-INV-002 Public API And Package Inputs Are Locked

- Invariant ID: `SB10-INV-002`
- Source raw note: RAW01 requires publishing readiness, and RAW06 requires compatibility care before old duplicate/basic surfaces are removed.
- Expected behavior: standard package public API metadata is frozen in approval tests; project packability metadata, source package inputs, and compatibility shim inventory are also frozen.
- Disallowed shallow implementation: add proof docs without test-enforced approvals, leave approvals outside the standard test suite, or freeze only WebGL/WebGlRun APIs while standard packages remain unguarded.
- Failing-first test: before SB10, no locked approval suite covered standard public API metadata, packability metadata, source package inputs, and shim inventory together.
- Passing test: `bundle://proof/SB10/transcripts/sb10-standard-tests.txt` proves 31 BaseLib tests including `StandardPublishingApprovalTests`, and `bundle://proof/SB10/transcripts/sb10-source-assertions.txt` prints `SB10-INV-002`.
- Changed source files: `repo://tests/CanDoItAll.Components.BaseLib.Tests/StandardPublishingApprovalTests.cs` SHA-256 `34cb36e1e6d6fc8f9962fc2d9737e23d999d183483bc830f76e9200ccec3590b`; `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/standard-public-api.metadata.approved.json` SHA-256 `4f516a53e7a4352c481a471f2220352529f9995c02c32f710221f4b29c6e1b71`; `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/standard-source-package-inputs.approved.txt` SHA-256 `cfe824817d24e69bb7f5f94a819847bb62820762bc20bfb752079d222c329b70`.
- Production assertions: approval tests fail on unreviewed public API drift, packability drift, source package input drift, or compatibility shim inventory drift.
- Red-team negative case: changing public types or adding/removing shim source without approval refresh fails locked-mode tests.
- Downstream dependency check: SB12 can transfer the standard surface with an explicit API/package-input baseline.

## SB10-INV-003 Compatibility Shim Removal Gate

- Invariant ID: `SB10-INV-003`
- Source raw note: RAW06 says old AppComponents/basic duplicates must not be removed blindly because some old behavior may still be consumed.
- Expected behavior: the 21 BaseLib compatibility shims remain published until SB12 proves consumer migration, and every compatibility shim has a documented replacement path and removal gate.
- Disallowed shallow implementation: delete or rename compatibility shims during packaging cleanup, document only a subset, or allow new shims without updating the inventory, policy doc, and approval snapshot.
- Failing-first test: before SB10, compatibility shim retention and removal readiness were not enforced by both docs and approval tests.
- Passing test: `bundle://proof/SB10/transcripts/sb10-standard-tests.txt` proves `CompatibilityPolicyDocumentsEveryShimAndRemovalGate`, and `bundle://proof/SB10/transcripts/sb10-source-assertions.txt` prints `SB10-INV-003`.
- Changed source files: `repo://docs/standard-components-compatibility-policy.md` SHA-256 `53c1dbc694273c6d1788abd811df3a13fb032d1fadd62eb1585e150f9db47568`; `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/standard-compatibility-shims.approved.txt` SHA-256 `ba277488ab5124b1f2b30ef08cdf470810e7f9966cbc1d2995a485bf766b964b`.
- Production assertions: compatibility policy documents every retained shim, preferred replacement, and SB12 removal gate; tests reject policy omissions.
- Red-team negative case: undocumented shim additions/removals or missing removal-gate text fail the compatibility approval/policy tests.
- Downstream dependency check: transfer repositories can keep compatibility shims while separately planning consumer migration/removal.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| Standard NuGet packages | Five standard `.csproj` files set `IsPackable=true` and inherit shared metadata from `repo://Directory.Build.props`. | Concrete packages are `bundle://proof/SB10/packages/CanDoItAll.Components.Common.0.1.0-sb10.nupkg`, `bundle://proof/SB10/packages/CanDoItAll.Components.BaseLib.0.1.0-sb10.nupkg`, `bundle://proof/SB10/packages/CanDoItAll.Components.Charts.0.1.0-sb10.nupkg`, `bundle://proof/SB10/packages/CanDoItAll.Components.OverlayLib.0.1.0-sb10.nupkg`, and `bundle://proof/SB10/packages/CanDoItAll.Components.Mermaid.0.1.0-sb10.nupkg`. | `bundle://proof/SB10/data/sb10-package-verification.json` proves required entries, static assets, hashes, and no source/build leakage. | `bundle://proof/SB10/transcripts/sb10-source-assertions.txt` and `bundle://proof/SB10/transcripts/sb10-package-verifier.txt` reject missing packability, missing verifier expectations, missing packages, and package leakage. |
| Standard public API approvals | `repo://tests/CanDoItAll.Components.BaseLib.Tests/StandardPublishingApprovalTests.cs` generates and compares public API, packability, source-input, and compatibility snapshots. | `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/standard-public-api.metadata.approved.json` is the locked consumer-facing API surface. | `bundle://proof/SB10/transcripts/sb10-standard-tests.txt` proves the approval tests pass without update mode. | Any public type/member drift changes the approval snapshot and fails the test suite until deliberately reviewed. |
| Compatibility policy | `repo://docs/standard-components-compatibility-policy.md` documents the 21 retained shims and replacements. | Existing consumers can keep compiling against shims while new code uses standard replacements. | `bundle://proof/SB10/transcripts/sb10-standard-tests.txt` proves every shim in the filesystem approval is documented with an SB12 removal gate. | The test rejects undocumented shim removal/addition and policy omissions. |

## Semantic Gate Decision

Pass. SB10 includes pack/build/test transcripts, concrete NuGet package outputs, package verification JSON, public API and package-input approval tests, compatibility policy proof, anti-stub audit output, and changed-file hashes.
