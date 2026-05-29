# SB15 Semantic Invariants

- Invariant ID: `SB15-closure-proof`
- Source raw note: Close the preparation wave only after implementation, validation, tests, WebGL screenshots, and dependency checks are complete.
- Expected behavior: Final reports, proof manifests, semantic evidence, and browser analytics cite concrete validation outputs.
- Disallowed shallow implementation: Marking statuses complete while any build, test, screenshot, or boundary scan proof is missing.
- Failing-first test: N/A process-only no production behavior; completed-stage validation checks proof depth.
- Passing test: Completed-stage bundle validation and final validation commands recorded in `bundle://proof/SB15/transcripts/closure-validation.md`.
- Changed source files: `bundle://reviews/01-execution-report.md`
- Production assertions: Closure cites passing builds, tests, audits, dependency scans, and WebGL pixel proof.
- Red-team negative case: The validator rejects pending statuses, weak raw-note proof, and missing critical proof manifests.
- Downstream dependency check: Final answer cites validation results and no running sandbox process remains.
