# SB14 Semantic Invariants

Subbundle: `SB14`

## Invariants

| Invariant ID | Requirement | Expected behavior | Disallowed shallow implementation | Negative proof | Positive proof |
| --- | --- | --- | --- | --- | --- |
| SB14-INV-001 | Final closure must map every normalized requirement to proof. | REQ-001 through REQ-015 each have status, evidence and residual-risk notes. | Marking the bundle completed while a requirement is unmapped, partial without a follow-up, or only prose-backed. | `sb14-requirement-closure-audit.txt` checks every REQ id. | `reviews/03-requirement-closure-table.md` closes all requirements. |
| SB14-INV-002 | Final QA reports must agree with execution evidence. | Senior QA, C# Blazor, JS runtime and manager reports cite the executed proof and do not claim unrun validation. | New final reports that contradict manifests or invent missing browser proof. | `sb14-source-assertions.txt` verifies review report presence and pass/close decisions. | `reviews/04-senior-qa-execution-final-check.md`, `05-csharp-blazor-architecture-final-review.md`, `06-vanilla-js-runtime-final-review.md`, `07-manager-summary.md`. |
| SB14-INV-003 | Closure cannot hide unresolved markers. | Final execution docs/proof do not contain unresolved status blockers outside historical/preparation context. | Leaving SB14 or REQ-015 marked pending in final docs. | `sb14-open-marker-scan.txt` checks final closure docs. | `README.md`, `reviews/01-execution-report.md`, `traceability/01-requirement-traceability.md`, `proof/SB14/manifest.md`. |
| SB14-INV-004 | Completed-stage validation must pass after final docs are written. | Bundle structural validator passes with `--stage completed --profile initiative`. | Running only prepared/execution validation before final docs, or skipping validator. | Validator would fail on missing required files/sections. | `proof/SB14/transcripts/bundle-validate-completed.txt`. |

## Production Behavior Artifact Matrix

SB14 creates no production runtime artifact. It creates final governance artifacts:

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Requirement closure table | SB14 | QA/maintainers | Final review artifact, updated when bundle scope changes. | Requirement closure audit. |
| Final review reports | SB14 | QA/architects/manager | Final review artifacts, updated when proof changes. | Source assertion and open-marker scans. |

## Reopen Triggers

- A later audit finds a requirement without proof or with stale/contradictory evidence.
- A final review claims a browser/test/build proof not present in manifests.
- A critical subbundle manifest or semantic invariant is removed.
- Completed-stage bundle validation fails.
