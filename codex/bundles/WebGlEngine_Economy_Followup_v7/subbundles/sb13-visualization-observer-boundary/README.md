# SB13 — Visualization observer boundary

## Repository scope

Both

## Goal

Ensure WebGL visualization cannot change economic truth or hide simulation errors.

## Tasks

- Mark WebGL/browser proof as observer-only.
- Compare headless run document to browser-loaded document hashes.
- Ensure browser runtime errors/warnings cannot be ignored in observer proof.
- Prove visualization failures do not mutate headless artifacts.
- Add doc explaining how to interpret visual playback vs economic state.

## Acceptance criteria

- Economic readiness can pass without browser proof only as headless-valid/oracle-valid, not browser-observer-valid.
- Browser proof can fail without invalidating headless artifacts, but must block visual demo claims.

## Required proof artifacts

- `proof/SB13/browser/observer-boundary-proof.json`
- `proof/SB13/transcripts/headless-vs-browser-hash-tests.txt`

## Gate

Visualization must never be the economic source of truth.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.
