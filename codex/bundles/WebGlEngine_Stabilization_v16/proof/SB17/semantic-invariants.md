# Semantic invariants - SB17

- Browser proof must compare browser-loaded state to expected state, not only compare expected values to themselves.
- Observer validity requires document hash, scene content hash, driver hash, runtime idle, UI exercise, completed stage IDs, final positions, and empty console errors.
- Cancellation proof must advance runtime stop generation and settle idle.
- Proof remains generic; `/run-playback` uses domain-neutral object and stage names.

## Production Behavior Artifact Matrix

| Behavior | Producer | Consumer | Lifecycle | Proof |
|---|---|---|---|---|
| Observer proof report | `run-browser-observer-proof.cjs` | reviewers and bundle closure | browser validation | `bundle://proof/SB17/browser/browser-observer-proof.json` |
| Screenshot proof | Playwright page screenshot | reviewers | after observer assertions pass | `bundle://proof/SB17/browser/run-playback.png` |
