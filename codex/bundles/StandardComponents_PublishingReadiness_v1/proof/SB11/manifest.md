# SB11 Proof Manifest

Status: Completed

## Scope

SB11 ran the full standard-component visual validation matrix for publishing readiness. Canvas/WebGL internals remained out of scope.

## Semantic Contract

- `bundle://proof/SB11/semantic-invariants.md`

## Implementation Repairs From Visual Review

- Hardened `SummaryTile` text wrapping and removed negative tracking from summary values.
- Changed sandbox hero summary grids to two columns in constrained hero side rails.
- Added `ListDetailShell` container-query layout so list/detail panes split only when the component itself has enough width.
- Replaced TreeView truncation with multi-line wrapping for long navigation labels.
- Replaced PrefixedField adornment truncation with readable wrapping and full-width mobile stacked adornments.
- Hardened tab text rules so wrapping tabs wrap and scroll tabs can grow to label width.
- Hardened the SB11 matrix verifier to understand local scroll/clipping regions and to support alternate screenshot output roots.

## Proof Artifacts

- Matrix report: `bundle://proof/SB11/data/sb11-visual-matrix.json`
  - SHA-256 `22b28e2fe0955226c06a1d138bce2e1e83f50fd572149f6ad134242d8bfe659f`.
- Visual review notes: `bundle://proof/SB11/data/sb11-visual-review-notes.json`
- File hashes: `bundle://proof/SB11/data/sb11-file-hashes.json`
- Final matrix screenshots: `bundle://proof/SB11/screenshots/matrix-final`
- Playwright MCP screenshots: `bundle://proof/SB11/screenshots/mcp`
- Build transcript: `bundle://proof/SB11/transcripts/sb11-dotnet-build.txt`
- Tailwind transcript: `bundle://proof/SB11/transcripts/sb11-tailwind-build.txt`
- Locked BaseLib tests: `bundle://proof/SB11/transcripts/sb11-baselib-tests.txt`
- Interaction verifier reruns: `bundle://proof/SB11/transcripts/sb11-sb06-inputs-verifier.txt`, `bundle://proof/SB11/transcripts/sb11-sb07-actions-feedback-verifier.txt`, `bundle://proof/SB11/transcripts/sb11-sb08-layout-navigation-overlays-verifier.txt`, `bundle://proof/SB11/transcripts/sb11-sb09-data-display-charts-mermaid-verifier.txt`
- Source assertions: `bundle://proof/SB11/transcripts/sb11-source-assertions.txt`
- Anti-stub audit: `bundle://proof/SB11/transcripts/sb11-anti-stub-audit.txt`
- Prepared validator: `bundle://proof/SB11/transcripts/sb11-prepared-validator.txt`
- Closure gate: `bundle://proof/SB11/transcripts/sb11-closure-gate.txt`
- Passing transcript: `bundle://proof/SB11/transcripts/sb11-visual-matrix.txt`.
- Failing-first: N/A process/non-production completed-stage proof normalization; real failing-first visual observations are captured in `bundle://proof/SB11/data/sb11-visual-review-notes.json` and structured in `bundle://proof/SB11/semantic-invariants.md`.

## Final Results

- SB11 matrix: 51 routes, 4 viewports, 102 screenshots, 817 checks, 0 failures, 0 console errors.
- SB06 rerun: 22 checks, 0 console errors.
- SB07 rerun: 37 checks, 0 failures, 0 console errors.
- SB08 rerun: 67 checks, 0 failures, 0 console errors.
- SB09 rerun: 57 checks, 0 failures, 0 console errors.
- Build: 0 warnings, 0 errors.
- BaseLib tests: 31 passed, 0 failed.
