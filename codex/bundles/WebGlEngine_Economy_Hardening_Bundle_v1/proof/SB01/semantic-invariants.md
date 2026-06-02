# SB01 Semantic Invariants

Subbundle: `SB01`

## Invariants

| Invariant ID | Requirement | Expected behavior | Disallowed shallow implementation | Negative proof | Positive proof |
| --- | --- | --- | --- | --- | --- |
| SB01-CURRENTNESS | REQ-001, REQ-015 | Downstream phases start from freshly observed repo state, not stale bundle-prep assumptions. | Reuse a cached snapshot or old source line notes without branch refs, source hashes, and current command output. | `bundle://proof/SB01/codeanalytics-snapshot-summary.md` records that the cached Components snapshot had 138 scoped docs and was stale for SB01. | `bundle://proof/SB01/current-state-inventory.md`, `bundle://proof/SB01/changed-file-baseline.md`, and `bundle://proof/SB01/transcripts/sb01-source-assertion-scan.txt` record fresh refs, hashes, and source assertions. |
| SB01-BASELINE | REQ-015 | Components and Economy baseline builds/tests either pass or are recorded as blockers before feature work starts. | Listing projects or saying "builds should pass" without command transcripts and exit codes. | Economy baseline warnings are preserved in `bundle://proof/SB01/transcripts/economy-build-slnx.txt`; they would be hidden by a shallow pass summary. | Components build, WebGlLib tests, WebGlRunLib tests, Economy build, and Economy WebGl/Simulation tests all exit 0 in the SB01 transcripts. |
| SB01-BOUNDARY | REQ-001 | Current evidence preserves the intended layering: WebGlLib is independent, WebGlRunLib sits above it, and Economy consumes the run layer. | Only inspect package names, or only inspect one repo. | `bundle://proof/SB01/transcripts/sb01-anti-stub-and-boundary-scan.txt` would surface production TODO/NotImplemented or forbidden domain terms in scoped Components WebGl source. | `bundle://proof/SB01/current-state-inventory.md` and CodeAnalytics snapshot `snap-20260601231917-d9c63db7` show WebGlLib has no project references, WebGlRunLib references WebGlLib, and WebGlSandbox references both. |

## Production Behavior Artifact Matrix

No new production signal, state, record, or event was introduced by SB01. This subbundle records audit and proof artifacts only.

## Reopen Triggers

- A later phase discovers a source file, project, or repo ref missing from `bundle://proof/SB01/changed-file-baseline.md`.
- Components or Economy baseline build/test commands fail after SB01 without an owning downstream subbundle recording the new failure.
- Browser proof in SB02, SB03, SB04, SB05, SB09, or SB13 contradicts the source assertions captured in SB01.
- A package/dependency scan reveals WebGlLib referencing WebGlRunLib or Economy.
- A fallback path is used without explicit diagnostic mode.
