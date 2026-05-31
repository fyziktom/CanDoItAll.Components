# SB14 — Components large-screen-only guard

## Goal
Prevent wasted effort on mobile responsiveness for WebGL.

## Tasks
- Keep large-screen-only policy in docs and audit.
- Add or keep minimum viewport warning only.
- No mobile/tablet screenshot gates.
- No responsive layout optimization work for WebGL surfaces.

## Tests
- audit detects mobile/small-screen optimization instructions in WebGL bundle docs unless explicitly forbidden/out-of-scope.
