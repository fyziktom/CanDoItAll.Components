# Semantic invariants SB01

Status: completed

## Invariants

| ID | Requirement | Evidence | Closure |
| --- | --- | --- | --- |
| `SB01-BOUNDARY` | Components proof must stay domain-neutral while reproducing the pause bug. | `bundle://proof/SB01/transcripts/source-assertion-baseline-scan.txt`; no production code changes in SB01. | Passed for baseline scope. |
| `SB01-RUNTIME-STATE` | Current-state proof must compare C# playback state with browser runtime diagnostics. | `bundle://proof/SB01/browser/failing-first-pause-assertions.json`; `bundle://proof/SB01/transcripts/failing-first-pause-playwright.txt`. | Passed as failing-first evidence. |
| `SB01-PROOF-HYGIENE` | Proof closure must include non-empty transcripts and machine-readable browser assertions. | `bundle://proof/SB01/transcripts/proof-hygiene-inventory.txt`; `bundle://proof/SB01/browser/failing-first-pause-assertions.json`. | Passed. |

## Semantic Adequacy Gate

| Gate item | SB01 result |
| --- | --- |
| Shallow-pass trap | A C#-only pause assertion or screenshot would miss JS command stages continuing after Pause. |
| Adversarial negative proof | `runtimeStillBusyImmediatelyAfterPause = true` and `runtimeStopSignalMissing = true` in `bundle://proof/SB01/browser/failing-first-pause-assertions.json`. |
| Semantic positive proof | The route builds and the browser proof observes real `/run-playback` runtime diagnostics at 1920x1080. |
| Anti-stub audit | `bundle://proof/SB01/transcripts/anti-stub-audit.txt`. |
| Raw-note literal closure | The note "pressing Pause ... did not stop the scene" is reproduced and remains unsolved for SB02/SB03 implementation. |

## Production Behavior Artifact Matrix

Not applicable. SB01 creates proof artifacts only and introduces no new production signal, state, record, or event.
