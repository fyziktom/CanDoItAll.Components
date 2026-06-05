# Release-Candidate Freeze Gates

The Components freeze is complete only if all gates pass:

1. **C# API gate** — public API baseline generated and approved for `WebGlLib` and `WebGlRunLib`.
2. **JS API gate** — `window.CanDoItAll.webglScene` exported method manifest generated and approved.
3. **Action-kind gate** — `WebGlRunActionKinds.All` is approved and no domain terms appear in generic action names.
4. **Package gate** — package contents for WebGlLib/WebGlRunLib are listed and approved, samples/sandboxes/tests are not accidentally packable.
5. **Static asset gate** — package-mode consumers can load JS/CSS/static web assets from packages.
6. **Runtime idle gate** — strict browser proof shows settled state with no semantic or visual blockers unless an explicitly approved final-render-drain mode is used.
7. **Domain boundary gate** — source/package CI scan proves no configured domain terms in generic shipping source.
8. **Domain driver gate** — generic driver manifest/hash contracts pass tests, including rejection of unknown generic action kinds.
9. **Samples gate** — WebGlLib-only and WebGlRunLib generic samples build/run both in local project mode and package mode.
10. **Docs gate** — host integration docs explain frozen contracts, upgrade path, and when a future change belongs in a domain driver.
