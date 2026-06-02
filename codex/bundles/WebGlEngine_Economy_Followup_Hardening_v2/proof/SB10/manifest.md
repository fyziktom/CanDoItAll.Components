# SB10 proof manifest

Status: Completed 2026-06-02.

## Changed file hashes

Recorded in `proof/SB10/transcripts/changed-file-hashes.txt`.

| Repo | File | SHA256 |
| --- | --- | --- |
| Economy | `README.md` | `6b168137144b6734e2024046e3f1f5c75708d86f5a5d15901da4e14c87a19c13` |
| Economy | `docs/simulation/architecture-boundaries.md` | `46ba753c77216123435cc96c3e5c79794c9fe21ca44a04938e325fb31e827bc8` |

## Command transcripts

- Failing-first docs/public-surface scan: `proof/SB10/transcripts/failing-first-docs-public-surface-scan.txt`.
- Passing docs/public-surface scan: `proof/SB10/transcripts/passing-docs-public-surface-scan.txt`.
- Documentation review checklist: `proof/SB10/transcripts/docs-review-checklist.txt`.
- Simulation project boundary scan: `proof/SB10/transcripts/simulation-project-boundary-scan.txt`.
- Source assertions: `proof/SB10/transcripts/source-policy-assertions.txt`.
- Components domain-leakage scan: `proof/SB10/transcripts/components-domain-leakage-scan.txt`.
- Anti-stub/placeholder scan: `proof/SB10/transcripts/anti-stub-placeholder-scan.txt`.
- Changed file hash transcript: `proof/SB10/transcripts/changed-file-hashes.txt`.

## Browser artifacts

Not applicable. SB10 changed Markdown documentation only and no hosted UI/doc route was changed.

## Source assertions

- Economy `README.md` now presents the repository as a ledger, account, and simulation module, lists the simulation projects, and links `docs/simulation/architecture-boundaries.md`.
- `docs/simulation/architecture-boundaries.md` includes a Mermaid dependency diagram, package map, public surface/extension point inventory, host/package readiness notes, reuse guidance for non-economy simulators, and guardrails.
- The structured boundary scan confirms current `Simulation.*` project references match the package map, including WebGlBridge's conditional Components WebGL project/package references.
- Components WebGlLib/WebGlRunLib source was scanned for unchecked Economy/domain leakage, with vendor files and intentional boundary guardrails excluded.

## Anti-stub audit

`proof/SB10/transcripts/anti-stub-placeholder-scan.txt` passes for changed SB10 documentation paths.

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Root README simulation map | Economy `README.md` | Developers and package consumers | Updated doc committed with hash proof | Failing-first scan showed ledger-only wording and missing simulation/public-surface coverage |
| Simulation architecture package map | `docs/simulation/architecture-boundaries.md` | Economy maintainers, SB11/SB12 validators, future simulator authors | Updated Markdown with Mermaid and package map | Structured boundary scan fails if source project refs drift from documented map |
| Public extension point inventory | `docs/simulation/architecture-boundaries.md` | Backend, snapshot, visualization, WebGL bridge, and sandbox extension authors | Documentation-only artifact | Passing docs scan requires public surface and extension point coverage |
| Genericity guardrail | Reuse guidance plus Components leakage scan | Components package maintainers and non-economy simulator authors | Documentation and scan transcript | Components leakage scan fails on unchecked Economy/domain terms outside allowed guardrail files |

## Gate decision

Pass. SB10 refreshes the Economy simulation documentation and public-surface map without touching browser-visible runtime or code behavior. Source package boundary, doc checklist, anti-stub, and Components leakage scans pass.
