# SB11 — Docs, migration, and troubleshooting

Priority: P2
Related findings: F12
Status: Completed

## Objective

Update developer docs for playback lifecycle, pause semantics, scenario packs, package mode, deterministic replay, and proof rules.

## Required implementation rules

- Keep Components generic and domain-neutral.
- Prefer small cohesive changes over broad rewrites.
- Add or update tests before claiming the gate is closed.
- Capture failing-first proof for every P0/P1 behavioral bug.
- Update proof manifest and semantic invariants.

## Acceptance gate

Docs include a concrete pause bug troubleshooting checklist and host integration recipe.

## Required proof artifacts

- `proof/SB11/manifest.md`
- `proof/SB11/semantic-invariants.md`
- `proof/SB11/transcripts/*.txt` with non-empty content
- browser screenshots plus JSON assertions when UI/runtime behavior is changed
- source assertion scan proving changed contracts are present

## QA checklist

- Build Components and/or Economy as relevant.
- Run focused tests for changed area.
- Run boundary audits.
- Run browser proof for playback/UI changes.
- Ensure no blank transcripts.

## Completion summary

Gate result: Passed. Components docs now include a host integration recipe, Pause bug troubleshooting checklist, replay-mode guidance, package-mode proof rules, and proof-hygiene requirements. Economy docs now own scenario-pack manifest hashing, pathless catalog/source loading, deterministic replay behavior, and package-mode boundary notes.

- `bundle://proof/SB11/manifest.md`
- `bundle://proof/SB11/semantic-invariants.md`
- `bundle://proof/SB11/transcripts/source-assertion-docs-coverage-scan.txt`
- `bundle://proof/SB11/transcripts/docs-link-check.txt`
- `bundle://proof/SB11/transcripts/anti-stub-docs-scan.txt`
- `bundle://proof/SB11/transcripts/components-domain-neutral-host-doc-scan.txt`
