# Semantic invariants SB02

Status: Completed

- Invariant ID: `SB02-RUNTIME-GENERIC-AUDIT`
- Source raw note: RN-003
- Expected behavior: generic runtime JS remains auditable, acyclic, under hard file-size thresholds, and free of Economy/probe-specific vocabulary.
- Disallowed shallow implementation: relying on a prose claim without running the runtime audit and forbidden-term scan.
- Failing-first test: N/A; SB02 is an audit gate with no production behavior change.
- Passing test: `bundle://proof/SB02/transcripts/runtime-audit.txt`
- Changed source files: none.
- Production assertions: `bundle://proof/SB02/source-assertions/runtime-js-audit-summary.txt`
- Red-team negative case: forbidden-domain scan explicitly searches generic runtime JS for Economy/probe terms and records no matches.
- Downstream dependency check: SB03 may proceed with warning-threshold split candidates documented.
