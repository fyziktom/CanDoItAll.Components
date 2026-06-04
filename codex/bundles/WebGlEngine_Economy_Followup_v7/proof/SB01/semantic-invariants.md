# Semantic invariants - SB01

Status: completed

| Invariant | Expected behavior | Disallowed shallow implementation | Failing-first / negative proof | Passing proof | Closure |
|---|---|---|---|---|---|
| SB01-PROOF-HYGIENE | Current proof claims are reconciled with real artifacts before downstream work starts. | Treating prepared manifests, old reports, or zero-byte logs as trusted proof. | `bundle://proof/SB01/transcripts/proof-hygiene-scan.txt` records old Economy zero-byte logs as invalid historical proof. | Components scan has zero zero-length text proof/report files after SB01 capture. | Passed |
| SB01-BROWSER-BASELINE | Current pause behavior is captured through browser assertions and diagnostics. | Screenshots only, or source-only reasoning. | `bundle://proof/SB01/browser/run-playback-pause-before.json` fails immediate UI `Playing=False` assertion. | Same artifact shows browser runtime idle with zero active motions, queued motions, and queued command stages after the wait. | Passed with downstream SB02 fix required |
| SB01-RESEARCH-CLASSIFICATION | Current experiment readiness is classified honestly. | Calling the stack research-ready from v6 proof alone. | Report lists missing strict/oracle/reproducibility/performance gates. | Report classifies current state as exploratory only. | Passed |

## Production Behavior Artifact Matrix

No new production signal, state, record, or event was introduced by SB01.

