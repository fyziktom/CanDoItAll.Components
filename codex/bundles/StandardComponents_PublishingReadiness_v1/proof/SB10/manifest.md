# SB10 Proof Manifest - Compatibility Cleanup Packaging And Public API Hardening

Status: `Passed`  
Completed local date: `2026-06-29`

## Owned Requirements

- RAW01: Preparation of repository for publishing.
- RAW06: Audit duplicate AppComponents basic components.
- SB10 acceptance: standard package build/test/pack succeeds, compatibility shims have an owner and migration path, and no Canvas/WebGL changes are required.

## Semantic Contract

- `bundle://proof/SB10/semantic-invariants.md`

## Production Changes

- Added explicit `Charts` and `Common` references to the non-WebGL `CanDoItAll.Components.BaseLib.Tests` project so the standard test suite can lock all standard package surfaces.
- Added `StandardPublishingApprovalTests` covering standard public API metadata, project packability metadata, source package inputs, compatibility shim inventory, and compatibility policy coverage.
- Added approval snapshots for standard public API metadata, packability metadata, source package inputs, and the 21 retained BaseLib compatibility shims.
- Added `docs/standard-components-compatibility-policy.md` documenting every retained compatibility shim, preferred replacement, and SB12 removal gate.
- Added `verify-sb10-packages.py` to inspect generated NuGet archives for required DLL/readme/nuspec/static assets, package hashes, and forbidden source/build leakage.
- Generated five proof NuGet packages under `bundle://proof/SB10/packages` using proof version `0.1.0-sb10`.

## Changed-File Hashes

- Hash manifest JSON: `bundle://proof/SB10/data/sb10-file-hashes.json`
- Hash transcript: `bundle://proof/SB10/transcripts/sb10-file-hashes.txt`
- Package hashes: `bundle://proof/SB10/data/sb10-package-verification.json`
- Representative changed-file SHA-256: `repo://tests/CanDoItAll.Components.BaseLib.Tests/StandardPublishingApprovalTests.cs` `34cb36e1e6d6fc8f9962fc2d9737e23d999d183483bc830f76e9200ccec3590b`.

## Command Transcripts

- Standard build: `bundle://proof/SB10/transcripts/sb10-standard-build.txt`
- Standard tests: `bundle://proof/SB10/transcripts/sb10-standard-tests.txt`
- Standard pack: `bundle://proof/SB10/transcripts/sb10-standard-pack.txt`
- Package verifier: `bundle://proof/SB10/transcripts/sb10-package-verifier.txt`
- Source assertions: `bundle://proof/SB10/transcripts/sb10-source-assertions.txt`
- Anti-stub audit: `bundle://proof/SB10/transcripts/sb10-anti-stub-audit.txt`
- Git whitespace check: `bundle://proof/SB10/transcripts/sb10-git-diff-check.txt`
- Passing transcript: `bundle://proof/SB10/transcripts/sb10-package-verifier.txt`.
- Failing-first: N/A process/non-production completed-stage proof normalization; SB10 verifier negative cases are documented in `bundle://proof/SB10/semantic-invariants.md`.

## Package And API Proof

- Package verification JSON: `bundle://proof/SB10/data/sb10-package-verification.json`
- Common package: `bundle://proof/SB10/packages/CanDoItAll.Components.Common.0.1.0-sb10.nupkg`
- BaseLib package: `bundle://proof/SB10/packages/CanDoItAll.Components.BaseLib.0.1.0-sb10.nupkg`
- Charts package: `bundle://proof/SB10/packages/CanDoItAll.Components.Charts.0.1.0-sb10.nupkg`
- OverlayLib package: `bundle://proof/SB10/packages/CanDoItAll.Components.OverlayLib.0.1.0-sb10.nupkg`
- Mermaid package: `bundle://proof/SB10/packages/CanDoItAll.Components.Mermaid.0.1.0-sb10.nupkg`
- Public API approval: `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/standard-public-api.metadata.approved.json`
- Packability approval: `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/standard-project-packability.approved.json`
- Source package input approval: `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/standard-source-package-inputs.approved.txt`
- Compatibility shim approval: `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/standard-compatibility-shims.approved.txt`
- Compatibility policy: `repo://docs/standard-components-compatibility-policy.md`

## Validation Summary

- Standard builds for Common, BaseLib, Charts, OverlayLib, and Mermaid: passed, each with 0 warnings and 0 errors.
- `dotnet test tests/CanDoItAll.Components.Common.Tests/CanDoItAll.Components.Common.Tests.csproj --no-restore --nologo`: passed, 5 tests.
- `dotnet test tests/CanDoItAll.Components.BaseLib.Tests/CanDoItAll.Components.BaseLib.Tests.csproj --no-restore --nologo`: passed, 31 tests including standard publishing approval tests.
- Standard packs for Common, BaseLib, Charts, OverlayLib, and Mermaid: passed; five `.nupkg` files generated under `bundle://proof/SB10/packages`.
- `python codex/bundles/StandardComponents_PublishingReadiness_v1/scripts/verify-sb10-packages.py --package-dir codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB10/packages --version 0.1.0-sb10 --output codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB10/data/sb10-package-verification.json`: passed, 5 packages, 0 failed.
- `git diff --check`: passed; LF-to-CRLF warnings were captured in the transcript.

## Compatibility Decision

- The 21 BaseLib compatibility shims remain published until SB12.
- New code should use the replacement primitives in `docs/standard-components-compatibility-policy.md`.
- Any removal or rename must update consumer migration proof, public API approvals, package-input approvals, and SB12 transfer audit evidence in the same change.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| Standard package outputs | `bundle://proof/SB10/transcripts/sb10-standard-pack.txt` creates five proof packages. | `bundle://proof/SB10/packages/CanDoItAll.Components.Common.0.1.0-sb10.nupkg`, `bundle://proof/SB10/packages/CanDoItAll.Components.BaseLib.0.1.0-sb10.nupkg`, `bundle://proof/SB10/packages/CanDoItAll.Components.Charts.0.1.0-sb10.nupkg`, `bundle://proof/SB10/packages/CanDoItAll.Components.OverlayLib.0.1.0-sb10.nupkg`, and `bundle://proof/SB10/packages/CanDoItAll.Components.Mermaid.0.1.0-sb10.nupkg` are the concrete package outputs. | `bundle://proof/SB10/data/sb10-package-verification.json` proves required entries, static assets, and SHA-256 hashes. | The package verifier rejects missing DLL/readme/nuspec/static assets and source/build leakage. |
| Public API freeze | `repo://tests/CanDoItAll.Components.BaseLib.Tests/StandardPublishingApprovalTests.cs` builds the metadata snapshot. | `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/standard-public-api.metadata.approved.json` is the approved API contract. | `bundle://proof/SB10/transcripts/sb10-standard-tests.txt` proves the locked approval passes without update mode. | Any public API drift fails the test until reviewed. |
| Compatibility shim policy | `repo://docs/standard-components-compatibility-policy.md` documents shim replacements and removal gates. | Existing consumers keep the shims while new consumers use standard replacements. | `bundle://proof/SB10/transcripts/sb10-standard-tests.txt` proves every shim is covered by policy. | Undocumented shim additions/removals fail the compatibility approval/policy tests. |

## Closure Decision

SB10 is closed. Downstream SB11 may rely on standard package build/test/pack proof, public API/package-input approvals, package verification, and a documented compatibility removal gate.
