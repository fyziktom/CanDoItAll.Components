# SB12 — Final cross-repo red-team closure

Priority: P0
Related findings: ALL

## Objective

Run final builds/tests/package/browser/performance/proof audits across both repos. Perform senior QA red-team review and produce closure matrix.

## Required implementation rules

- Keep Components generic and domain-neutral.
- Prefer small cohesive changes over broad rewrites.
- Add or update tests before claiming the gate is closed.
- Capture failing-first proof for every P0/P1 behavioral bug.
- Update proof manifest and semantic invariants.

## Acceptance gate

No open P0/P1 issues, pause proof passes, performance budget captured, proof validator passes.

## Required proof artifacts

- `proof/SB12/manifest.md`
- `proof/SB12/semantic-invariants.md`
- `proof/SB12/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant.
- Run focused tests for changed area.
- Run boundary audits.
- Run browser proof for playback/UI changes.
- Ensure no blank transcripts.
