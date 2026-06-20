# SB05 Semantic Invariants

## Invariants

- The observer proof must compare the expected run document with a distinct browser-loaded document, never the same `runDocument` instance twice.
- A browser proof is valid only after the browser runtime has imported the initial scene, produced diagnostics, produced a proof snapshot hash, completed all expected stage ids, and reported runtime idle with no blockers.
- Final object positions are compared by object id and numeric vector tolerance, not by JSON property order.
- Browser proof artifacts must include assertions JSON, observer JSON, screenshot, console log, source assertions, and hash evidence.
- The browser-loaded document capture must avoid fragile full-scene export over Blazor Server; the real browser-state artifact is the proof snapshot hash plus runtime diagnostics captured after import/playback.

## Proof Links

- `bundle://proof/SB05/browser/observer-proof-assertions.json`
- `bundle://proof/SB05/browser/observer-proof.json`
- `bundle://proof/SB05/browser/observer-proof-after.png`
- `bundle://proof/SB05/browser/observer-proof-console.log`
- `bundle://proof/SB05/transcripts/source-assertions.txt`
- `bundle://proof/SB05/transcripts/changed-file-hashes.txt`
