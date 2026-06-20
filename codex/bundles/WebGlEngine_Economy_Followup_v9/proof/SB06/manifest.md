# Proof manifest - SB06

Status: completed

## Scope

SB06 removes economy-specific forbidden terms from generic production policy and makes domain-boundary checking opt-in through `WebGlRunGenericBoundaryOptions`. Source provenance remains structural and traceability-only, while economy-term negative coverage is supplied by a copied test fixture. A Windows GitHub Actions scan now audits only generic `src` C# projects for configured domain terms.

## Changed files

Changed-file hashes:

- `bundle://proof/SB06/transcripts/changed-file-hashes.txt`

Production files:

- `repo://src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs`
- `repo://src/CanDoItAll.Components.WebGlRunLib/Actions/WebGlRunActionPlanValidator.cs`

Test/config files:

- `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunValidatorTests.cs`
- `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj`
- `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/fixtures/domain-leakage-terms.json`
- `repo://.github/workflows/domain-leakage.yml`

## Proof artifacts

- Domain leakage report: `bundle://proof/SB06/domain-leakage-report.json`
- Domain leakage scan transcript: `bundle://proof/SB06/transcripts/domain-leakage-scan.txt`
- WebGlRunLib test transcript: `bundle://proof/SB06/transcripts/webglrunlib-tests.txt`
- Sandbox build transcript: `bundle://proof/SB06/transcripts/webglsandbox-build.txt`
- Source assertions: `bundle://proof/SB06/transcripts/source-assertions.txt`
- Anti-stub scan: `bundle://proof/SB06/transcripts/anti-stub-scan.txt`
- Bundle validator transcript: `bundle://proof/SB06/transcripts/bundle-validator.txt`
- Semantic invariants: `bundle://proof/SB06/semantic-invariants.md`

## Semantic adequacy gate

- Generic default behavior: `new WebGlRunDocumentValidator()` and `new WebGlRunActionPlanValidator()` use `WebGlRunGenericBoundaryOptions.None`, so generic Components production code no longer embeds economy vocabulary.
- Opt-in boundary behavior: tests load `fixtures/domain-leakage-terms.json` and pass those terms to validators to prove economy terms are rejected only when supplied by the calling boundary.
- Source provenance: source metadata keeps an allowlist and executable policy-term rejection, but opaque source values are not scanned as domain terms.
- CI scope: `.github/workflows/domain-leakage.yml` scans only `src/CanDoItAll.Components.WebGlLib` and `src/CanDoItAll.Components.WebGlRunLib` C# files and excludes `bin`/`obj`.

## Closure

SB06 passes. `WebGlRunLib.Tests` passed 68/68, `WebGlSandbox` built with 0 warnings and 0 errors, the configured domain scan covered 142 generic C# files with zero matches, and the negative economy-domain validation now depends on a test fixture instead of production hardcoding.
