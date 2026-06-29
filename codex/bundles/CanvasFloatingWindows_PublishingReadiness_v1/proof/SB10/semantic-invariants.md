# SB10 Semantic Invariants

## SB10-INV-PROOF-COVERAGE

- Every subbundle must have a proof manifest and semantic invariants file.
- Execution report rows must be completed or explicitly unresolved with an owner.
- Evidence: `bundle://proof/SB10/final-proof-audit.md`.

## SB10-INV-RAW-NOTES

- Every raw user note must close as solved, partially solved, or not solved with evidence.
- No partially solved raw note may hide a required in-scope defect.
- Evidence: `bundle://proof/SB10/raw-note-closure.md`.

## SB10-INV-WEBGL-EXCLUSION

- WebGL implementation, package, sample, test, and tool files must remain outside the changed source scope.
- Any WebGL build/restore issue discovered during validation must be separated as future work.
- Evidence: `bundle://proof/SB10/webgl-exclusion-source-assertion.txt`.

## SB10-INV-RUNTIME-DEPENDENCY

- Canvas, floating-window, calendar, and preview runtime code must remain plain browser JavaScript plus C# and Razor.
- npm must remain tooling-only for this scope.
- Evidence: `bundle://proof/SB10/runtime-dependency-red-team.txt`.

## SB10-INV-TRANSFER

- Final transfer docs must list validation commands, package/docs status, known follow-ups, and WebGL separation.
- Evidence: `bundle://proof/SB10/open-source-transfer-checklist.md`.


## Validator Contract Summary

- Invariant ID: `SB10-INV-PROOF-COVERAGE`
- Source raw note: RAW01, RAW02, RAW03, RAW04, RAW05, RAW06, RAW07.
- Expected behavior: Final closure must prove every subbundle, raw note, WebGL exclusion, runtime dependency boundary, and transfer checklist before marking the bundle complete.
- Disallowed shallow implementation: Completed status with missing manifests, unresolved raw notes, hidden WebGL drift, npm runtime dependency creep, or missing validator output.
- Failing-first test: N/A process/no production behavior change; SB10 performs final proof and transfer closure only.
- Passing test: `bundle://proof/SB10/transcripts/dotnet-test-final-in-scope.txt`
- Changed source files: `bundle://proof/SB10/final-proof-audit.md`, `bundle://proof/SB10/raw-note-closure.md`, `bundle://proof/SB10/fake-proof-resistance.md`, and `bundle://proof/SB10/open-source-transfer-checklist.md`.
- Production assertions: `bundle://proof/SB10/webgl-exclusion-source-assertion.txt` and `bundle://proof/SB10/runtime-dependency-red-team.txt`
- Red-team negative case: `bundle://proof/SB10/transcripts/proof-inventory-audit.txt` rejects missing proof artifacts, and source assertions reject WebGL or npm runtime drift.
- Downstream dependency check: The bundle can transfer to open-source publication preparation after `bundle://reviews/completed-validation.txt` passes.


