# SB10 semantic invariants

Status: Completed 2026-06-02.

| Invariant ID | Expected behavior | Shallow-pass trap | Negative proof | Positive proof | Source files | Downstream dependency |
| --- | --- | --- | --- | --- | --- | --- |
| SB10-INV-001 | The Economy README describes the repository as ledger, account, and simulation infrastructure rather than ledger-only. | Adding a WebGL paragraph while leaving the opening description stale. | `proof/SB10/transcripts/failing-first-docs-public-surface-scan.txt` records the ledger-only wording before the patch. | `proof/SB10/transcripts/passing-docs-public-surface-scan.txt` and `docs-review-checklist.txt`. | `../CanDoItAll.Economy/README.md`. | SB11 browser proof and SB12 final closure should not rely on stale repo-level docs. |
| SB10-INV-002 | Simulation docs expose a package map that matches the source project graph. | A diagram looks plausible but disagrees with current `.csproj` references. | The boundary scanner fails if a Simulation project has unexpected refs or missing expected refs. | `proof/SB10/transcripts/simulation-project-boundary-scan.txt` passes for Abstractions, SimpleAccounts, Visualization, Ledger, WebGlBridge, and SimulationSandbox. | `../CanDoItAll.Economy/docs/simulation/architecture-boundaries.md`, Simulation project files. | SB12 red-team closure can compare docs against source graph. |
| SB10-INV-003 | Public surfaces and extension points are documented for backends, snapshots, input packs, visual mapping, WebGL projection, and sandbox hosting. | Listing project names without telling extension authors where to plug in. | Failing-first scan required public-surface and extension-point terms that were missing before the patch. | Passing docs scan and checklist confirm public surface coverage. | Architecture boundaries doc. | Future Economy and non-economy simulator work has a documented extension surface. |
| SB10-INV-004 | Reuse guidance keeps Components generic and leaves domain semantics in the consuming bridge. | Saying future simulators can reuse WebGL while quietly moving domain vocabulary into Components. | Components domain-leakage scan fails on unchecked Economy/domain terms outside allowed vendor and guardrail files. | `proof/SB10/transcripts/components-domain-leakage-scan.txt` passes and documents the allowlist. | Architecture boundaries doc; Components WebGlLib/WebGlRunLib source. | R14 genericity remains preserved for SB11/SB12. |
| SB10-INV-005 | Package-mode documentation keeps the SB09 static-graph restore command shape visible. | Docs mention package mode but omit the explicit bridge package flag, allowing stale transitive restores. | SB09 negative restore transcripts remain linked through SB09 manifest; SB10 source assertions verify the command text remains present. | `proof/SB10/transcripts/source-policy-assertions.txt` finds `UseComponentsWebGlPackages` and `UseComponentsWebGlRunLibPackage` docs. | Economy README and architecture boundaries doc. | SB12 package readiness review can reuse the documented command shape. |

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative test |
| --- | --- | --- | --- | --- |
| Economy README simulation overview | SB10 documentation patch | Developers and package consumers | Hash recorded in SB10 manifest | Failing-first scan rejects ledger-only opening text. |
| Simulation architecture package map | SB10 documentation patch | Maintainers and future simulator authors | Hash recorded in SB10 manifest | Structured boundary scan rejects source/doc drift. |
| Generic reuse guidance | SB10 documentation patch | Components maintainers and non-economy bridge authors | Retained in architecture doc | Components leakage scan rejects unchecked domain terms. |

## Raw Requirement Closure

| Requirement | Status | Closure proof |
| --- | --- | --- |
| R11 | Solved | Economy README and `docs/simulation/architecture-boundaries.md` now include simulation docs, package map, dependency diagram, public surface/extension points, package-readiness notes, and future simulator reuse guidance; proof in `proof/SB10/manifest.md`. |
| R14 | Preserved for SB10 | Documentation states domain-specific mapping belongs in Economy or consuming bridges, and Components WebGlLib/WebGlRunLib leakage scans pass. |
