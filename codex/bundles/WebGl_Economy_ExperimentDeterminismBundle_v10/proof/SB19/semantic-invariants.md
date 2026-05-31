# SB19 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB19-INV-001 | Every subbundle has a proof manifest and semantic invariant file. | Closing based only on a verbal summary. | `reviews/01-execution-report.md`; `proof/SB*/manifest.md` |
| SB19-INV-002 | Raw feedback is closed note-by-note. | Hiding unresolved scope under generic success language. | `reviews/01-execution-report.md` |
| SB19-INV-003 | Final validation records bundle, Components, Economy, guard-scan, and broad-suite results. | Closing without rerunning the broad tests and boundary audits. | `proof/SB19/transcripts/closure-validation.txt` |

## Production Behavior Artifact Matrix

No production signal, state, record, or event is introduced by SB19.
