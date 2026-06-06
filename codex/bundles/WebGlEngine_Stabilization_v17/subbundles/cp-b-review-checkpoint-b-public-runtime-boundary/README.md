# CP-B — Review checkpoint B: public/runtime boundary

## Purpose

Run API, JS, idle and WebGlSceneView regression proof before adding canaries or performance work.

## Required checkpoint review

Codex must stop here and produce:

- `proof/CP-B/manifest.md`
- `proof/CP-B/checkpoint-review.md`
- a changed-files list
- a summary of what was proven and what remains risky
- a go/no-go decision for continuing

## No-go conditions

- Any RC blocker remains unclassified.
- Any proof transcript is empty.
- Any implementation added domain terms to generic source.
- Any package-mode proof still uses local project references while claiming NuGet mode.
- Any API approval file changed without an explicit human-readable reason.
