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

## Closure

Status: completed

- Components now emits a generic `WebGlRunObserverProofReport` that compares expected and browser-loaded run document hashes and fails observer proof for missing runtime/UI exercise, runtime/UI errors, or document mismatch.
- Economy documentation and focused coverage keep browser observer proof separate from headless/oracle economic readiness; browser observer failure blocks visual demo claims without mutating headless run artifacts.
- Browser proof at `proof/SB13/browser/observer-boundary-proof.json` exercises RunPlayback, captures runtime/UI diagnostics, confirms matching document hashes, and confirms visual stepping does not change the expected/browser-loaded document hash.
