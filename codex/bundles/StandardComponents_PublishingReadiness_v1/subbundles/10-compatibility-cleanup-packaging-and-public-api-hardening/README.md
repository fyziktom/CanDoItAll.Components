# SB10 Compatibility Cleanup Packaging And Public API Hardening

## Status

- Status: `Completed`

## Objective

Prepare standard packages for transfer by hardening packability, public API boundaries, compatibility policy, and non-WebGL test coverage.

## Covered Inputs

- RAW01: Preparation of repository for publishing.
- RAW06: Audit duplicate AppComponents basic components.

## Prerequisites

- Checkpoint C passed.
- SB04 duplicate decisions complete.

## Exact Source References

- repo://Directory.Build.props
- repo://CanDoItAll.Components.slnx
- repo://src/CanDoItAll.Components.BaseLib/CanDoItAll.Components.BaseLib.csproj
- repo://src/CanDoItAll.Components.Common/CanDoItAll.Components.Common.csproj
- repo://src/CanDoItAll.Components.Charts/CanDoItAll.Components.Charts.csproj
- repo://src/CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj
- repo://src/CanDoItAll.Components.Mermaid/CanDoItAll.Components.Mermaid.csproj

## Deliverables

- Standard component test project(s).
- Pack/build validation.
- Compatibility/deprecation documentation.
- Public API or package content approvals where appropriate.

## Dependency Impact

- Blocks final visual matrix and transfer readiness.
- Can reopen SB03/SB04 if public API exposes wrong primitives.

## Validation Depth

- Critical Semantic Adequacy Gate.
- Clean build/test/pack transcripts.
- Package content and public API assertions.
- Critical foundation: before closure, create `proof/SB10/manifest.md` and `proof/SB10/semantic-invariants.md` with Semantic Adequacy Gate evidence, changed-file hashes, transcripts, source assertions, anti-stub audit, and raw-note literal closure.


## Implementation Steps

- Create/extend non-WebGL standard component tests.
- Audit IsPackable and project references.
- Lock public API/package content for standard libraries.
- Document compatibility shims and removal path.

## Scope Exceptions

- WebGL and Canvas implementation changes are excluded unless this subbundle explicitly updates sandbox separation metadata without touching their component internals.

## Do Not Do

- Do not refactor unrelated WebGL or Canvas source.
- Do not delete compatibility or AppComponents code without migration proof and consumer checks.
- Do not close UI work from source inspection only.

## Acceptance Checklist

- Standard package build/test/pack succeeds.
- Compatibility shims have owner and migration path.
- No Canvas/WebGL changes required.

## Proof Required

- dotnet build/test/pack transcripts.
- Changed-file hashes.
- API/package assertion artifacts.
- Anti-stub audit.

## Completion Evidence

- Proof manifest: `bundle://proof/SB10/manifest.md`
- Semantic invariants: `bundle://proof/SB10/semantic-invariants.md`
- Package verification JSON: `bundle://proof/SB10/data/sb10-package-verification.json`
- Standard build transcript: `bundle://proof/SB10/transcripts/sb10-standard-build.txt`
- Standard test transcript: `bundle://proof/SB10/transcripts/sb10-standard-tests.txt`
- Standard pack transcript: `bundle://proof/SB10/transcripts/sb10-standard-pack.txt`
- Public API/package approvals: `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals`
- Compatibility policy: `repo://docs/standard-components-compatibility-policy.md`

## Browser Validation Logging

- N/A unless docs/sandbox UI changes; if UI docs routes change, capture affected route screenshots.

## Progression Gate

- The subbundle validator must pass closure review before downstream dependent subbundles start.
- If proof is weak or a screenshot shows wrapping, clipping, layout, available-space, or interaction defects, keep this subbundle `In progress` and reopen prerequisites as needed.

## Suggested Agent Prompt

Execute SB10 after visual groups are stable. Treat package/API proof as a release gate, not a paperwork step.
