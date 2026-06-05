# SB01 semantic invariants

## Invariants

| Invariant | Raw note / requirement | Disallowed shallow pass | Positive proof | Negative / adversarial proof | Status |
| --- | --- | --- | --- | --- | --- |
| SB01-I01 | "Probihly vsechny opravy?" / verify whether prior fixes really happened. | Merely cite v8 as completed. | `bundle://proof/SB01/closure-matrix.md` maps v8 R01-R13 to current source/proof state. | `bundle://proof/SB01/proof-hygiene-report.json` records open findings and v9 owners instead of closing them. | Passed |
| SB01-I02 | Identify simulator-noise risks before research use. | Treat proof existence as behavior correctness. | Closure matrix separates Done, Partially solved, and open v9 owners. | Source assertions flag pause, observer self-comparison, idle semantics, generic boundary, and missing third scenario. | Passed |
| SB01-I03 | Preserve Components genericity concerns. | Ignore domain leakage because tests pass. | Closure matrix cites the production generic boundary policy. | Proof hygiene report leaves `SB01-F04` open for SB06. | Passed |

## Production Behavior Artifact Matrix

SB01 is an audit-only subbundle. It introduces no new production signal, state, record, event, scheduler, or lifecycle path. The production behavior artifacts it audits are existing source files listed in `bundle://proof/SB01/closure-matrix.md`.

## Anti-stub audit

SB01 changed only bundle proof artifacts. The audit explicitly rejects placeholder closure by listing open findings with owning subbundles. No production `TODO`, `NotImplemented`, fixture-specific branch, or template-only output was introduced by this subbundle.
