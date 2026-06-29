# SB12 Semantic Invariants

## SB12-INV-001 Completed Proof Graph

- Invariant ID: `SB12-INV-001`
- Source raw note: RAW01 and RAW09 require publishing preparation to close with phase checkpoints and proof.
- Expected behavior: all 12 subbundles are completed, every critical subbundle has a manifest and semantic invariant contract, root README marks final closure passed, and the execution report cites each critical semantic proof.
- Disallowed shallow implementation: mark the bundle done while any subbundle is ready/pending or while critical proof contracts are uncited.
- Failing-first test: completed-stage validator initially rejected pending SB12 status, missing semantic evidence sections, and older proof-format gaps.
- Passing test: `bundle://proof/SB12/transcripts/sb12-source-assertions.txt` prints `SB12-INV-001`, and `bundle://proof/SB12/transcripts/sb12-completed-validator.txt` passes.
- Changed source files: `bundle://reviews/01-execution-report.md`; `bundle://README.md`; `bundle://proof/SB12/manifest.md`.
- Production assertions: final root/report status is `Completed`, semantic evidence sections exist for SB01, SB02, SB03, SB04, SB05, SB10, SB11, and SB12, and all subbundle READMEs are `Completed`.
- Red-team negative case: any pending subbundle, missing manifest/invariant, missing report citation, or failed completed validator blocks closure.
- Downstream dependency check: pure repository transfer can consume one closed proof graph instead of scattered intermediate notes.

## SB12-INV-002 Final Standard Visual Gate Is Clean

- Invariant ID: `SB12-INV-002`
- Source raw note: RAW10 requires real Playwright screenshots and action-state proof.
- Expected behavior: SB11 final matrix remains the standard visual gate: 51 routes, 4 viewports, 102 screenshots, 817 checks, 0 failures, and 0 console errors.
- Disallowed shallow implementation: close transfer readiness by package tests only while skipping visual proof.
- Failing-first test: SB11 first matrix found visual failures and false-positive detector gaps before repairs.
- Passing test: `bundle://proof/SB12/transcripts/sb12-source-assertions.txt` prints `SB12-INV-002` and validates `bundle://proof/SB11/data/sb11-visual-matrix.json`.
- Changed source files: `bundle://proof/SB11/data/sb11-visual-matrix.json` SHA-256 `22B28E2FE0955226C06A1D138BCE2E1E83F50FD572149F6AD134242D8BFE659F`.
- Production assertions: execution report browser analytics includes SB11 final matrix proof.
- Red-team negative case: any failed check, console error, missing screenshot count, or missing SB11 analytics row fails the SB12 closure verifier.
- Downstream dependency check: future standard-component publish changes should rerun the matrix before release.

## SB12-INV-003 Package Transfer Gate Is Clean

- Invariant ID: `SB12-INV-003`
- Source raw note: RAW01 requires repository preparation for publishing.
- Expected behavior: five standard packages build, test, pack, and pass package archive verification for `0.1.0-sb12`.
- Disallowed shallow implementation: run build only, inspect packages manually, include Canvas/WebGL packages, or omit static asset checks.
- Failing-first test: package verification rejects missing DLL/readme/nuspec/static assets and forbidden source/build leakage.
- Passing test: `bundle://proof/SB12/transcripts/sb12-standard-build.txt`, `bundle://proof/SB12/transcripts/sb12-standard-tests.txt`, `bundle://proof/SB12/transcripts/sb12-standard-pack.txt`, `bundle://proof/SB12/transcripts/sb12-package-verifier.txt`, and `bundle://proof/SB12/transcripts/sb12-source-assertions.txt` print `SB12-INV-003`.
- Changed source files: `bundle://proof/SB12/data/sb12-package-verification.json` SHA-256 `70A3987EDA07AD300636C1241787A765098B2E84AF7A510B0FA425EE138015D0`.
- Production assertions: Common, BaseLib, Charts, OverlayLib, and Mermaid packages are verified; package verifier reports five packages and zero failures.
- Red-team negative case: missing package id, missing required package entry, source/build leakage, or non-passing verification JSON fails closure.
- Downstream dependency check: pure repository transfer should rerun the same gate after path/package metadata changes.

## SB12-INV-004 Raw Notes And Follow-Up Scope Are Literal

- Invariant ID: `SB12-INV-004`
- Source raw note: RAW01 through RAW10.
- Expected behavior: every raw note is closed with proof, the xlsx map is cited, and WebGL/Canvas implementation remains explicitly outside this standard bundle with a named follow-up.
- Disallowed shallow implementation: mark notes solved with vague prose, hide WebGL/Canvas gaps, or omit transfer checklist details.
- Failing-first test: completed-stage raw-note proof-depth validation rejects weak raw note proof values.
- Passing test: `bundle://proof/SB12/transcripts/sb12-source-assertions.txt` prints `SB12-INV-004`, proving RAW01-RAW10 closure, mandatory xlsx citation, and WebGL/Canvas follow-up separation.
- Changed source files: `bundle://proof/SB12/raw-note-closure.md`; `bundle://proof/SB12/transfer-checklist.md`; `bundle://proof/SB12/residual-risk-and-followup.md`.
- Production assertions: `bundle://inventories/standard-components-publishing-map.xlsx` SHA-256 `4E9B70AFB1C93EB5D37615885381125C4070F5F7F464D001741E3573A3DDA2EA` remains the transfer map.
- Red-team negative case: any raw note not solved, missing xlsx map, or WebGL/Canvas implementation claim inside standard transfer proof fails the closure verifier.
- Downstream dependency check: WebGL/Canvas follow-up work has a separate named bundle boundary.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| Completed proof graph | `bundle://reviews/01-execution-report.md` and all subbundle READMEs publish final status. | Pure repository transfer consumes `bundle://proof/SB12/transfer-checklist.md`. | `bundle://proof/SB12/transcripts/sb12-completed-validator.txt` proves the completed-stage validator passes. | The validator rejects pending status, missing semantic proof, weak raw-note proof, bad transcripts, and broken artifact references. |
| Standard package set | `bundle://proof/SB12/transcripts/sb12-standard-pack.txt` creates the five standard packages. | `bundle://proof/SB12/data/sb12-package-verification.json` verifies the packages for transfer. | `bundle://proof/SB12/transcripts/sb12-package-verifier.txt` proves required entries and no leakage. | The package verifier rejects missing package entries and forbidden source/build artifacts. |
| Raw-note closure | `bundle://proof/SB12/raw-note-closure.md` closes RAW01-RAW10. | `bundle://reviews/01-execution-report.md` carries the same closure into the bundle-level report. | `bundle://proof/SB12/transcripts/sb12-source-assertions.txt` proves closure and xlsx citation. | Missing solved rows or weak proof values fail the source assertions and completed validator. |

## Semantic Gate Decision

Pass. SB12 closes standard publishing transfer readiness with final package proof, full visual proof, raw-note closure, residual risk separation, transfer checklist, anti-stub audit, completed-stage validator proof, and changed-file hashes.

