# SB12 semantic invariants

Status: Completed 2026-06-02.

| Invariant ID | Expected behavior | Shallow-pass trap | Negative proof | Positive proof | Source files | Downstream dependency |
| --- | --- | --- | --- | --- | --- | --- |
| SB12-INV-001 | Both repositories build after all subbundles complete. | Validating only the changed project while cross-repo dependencies are broken. | Build transcripts fail on non-zero exit; residual warnings are captured in red-team notes. | `components-solution-build-release.txt` and `economy-solution-build-release.txt` pass with 0 errors. | Components and Economy solution graphs. | Final release readiness. |
| SB12-INV-002 | Focused WebGlLib, WebGlRunLib, and Economy sandbox/WebGlBridge tests pass in Release after the final build. | Relying on old test output from individual subbundles. | Test transcripts fail on semantic regressions. | Components tests pass 48/48 and 42/42; Economy focused tests pass 45/45. | Components test projects; Economy test project. | R03-R09 and R12 closure. |
| SB12-INV-003 | Package-mode proof uses a fresh SB12 feed and isolated caches for WebGlLib-only, WebGlBridge, and Economy Components. | Restoring from stale global NuGet cache or only testing project-reference mode. | Package graph assertions fail if expected SB12 packages are absent or if WebGlRunLib leaks into the WebGlLib-only sample. | SB12 package restore/build transcripts and package graph assertion transcripts pass. | Components package projects; WebGlLib-only sample; Economy WebGlBridge and Components projects. | R10 package readiness. |
| SB12-INV-004 | Browser proof for generic and Economy routes remains complete and passing after final validation. | Treating screenshots as proof without assertions, diagnostics, or console review. | Browser artifact audit fails on missing files, failed assertions, or non-empty console error logs. | `browser-proof-artifact-audit-sb12.txt` passes for SB11 large+narrow route proof. | Components `/run-playback`; Economy `/economy/simulation-sandbox`. | R12 final browser closure. |
| SB12-INV-005 | Final closure has no missing/empty critical proof manifests and no unresolved placeholders in completed subbundles. | Marking SB12 complete while proof placeholders remain. | Final proof manifest audit and placeholder scan fail on pending status, unresolved template tokens, unchecked checklist items, or missing semantic invariant files. | `final-proof-manifest-audit-sb12.txt`, `proof-placeholder-scan-after-sb12.txt`, and final bundle validators pass. | Bundle proof, report, and subbundle files. | R13 proof hygiene closure. |
| SB12-INV-006 | Genericity remains preserved across Components packages. | Economy semantics creep into WebGlLib/WebGlRunLib during final fixes. | Boundary audits fail on forbidden project references or domain terms. | SB12 WebGlLib and WebGlRunLib boundary audits pass; SB11/SB12 changed production code is Economy-owned CSS only. | Components WebGlLib/WebGlRunLib source; Economy CSS. | R14 future simulator reuse. |

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative test |
| --- | --- | --- | --- | --- |
| Final build/test matrix | SB12 `dotnet build` and focused `dotnet test` commands | Release readiness and execution report | Transcripts stored under `proof/SB12/transcripts` | Non-zero command exit blocks closure. |
| Fresh SB12 package feed | Components `dotnet pack` | Package-mode consumers | Versioned packages restored from `proof/SB12/package-proof.NuGet.config` with isolated caches | Package graph assertions reject stale/missing packages. |
| Browser proof audit | SB12 audit over SB11 route proof | UI/browser closure | Checks route, viewport, screenshots, diagnostics, assertions, and console errors | Audit fails if any proof artifact is missing or assertion is false. |
| Final raw closure audit | SB12 requirement audit and execution report | Handoff and future follow-up planning | R01-R14 mapped to closure result and evidence | Any partial/blocked/deferred item must be explicit. |
| Residual warning backlog | Red-team notes | Maintainers | Existing Economy/IPFS warnings listed separately from bundle blockers | Keeps warning noise visible without hiding it as prose-only residual risk. |

## Raw Requirement Closure

| Requirement | Status | Closure proof |
| --- | --- | --- |
| R01-R13 | Solved | `proof/SB12/transcripts/raw-requirement-closure-audit-sb12.md` maps each requirement to implementation and proof. |
| R14 | Solved / preserved | Components boundary audits pass and Economy-specific behavior remains outside Components packages. |
