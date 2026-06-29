# SB10 Fake-Proof Resistance

## Rejected Shallow Evidence Patterns

- Route-load-only browser proof was rejected; SB05-SB08 require scenario counts, real interactions, geometry measurements, screenshots, console logs, and reopen decisions.
- Static screenshot-only proof was rejected; browser scripts assert DOM/runtime state, callbacks, action results, viewport bounds, and console quality.
- README-only publishing proof was rejected; SB09 requires approval tests, generated asset verification, focused release builds, package creation, package content inspection, source assertions, and runtime dependency proof.
- Source-only pure-JS claims were rejected; SB04, SB09, and SB10 inspect runtime JS and root npm metadata.
- Overlay-only proof was rejected for Canvas floating windows; SB07 proves both OverlayWindow and CanvasFloatingWindow lifecycle behavior and runtime alias ownership.
- Benchmark-only renderer claims were rejected; SB08 documents benchmark as route-health/draw-cost evidence only, not renderer-migration approval.
- WebGL drift was rejected as in-scope work; WebGL source/package/test/tool changes are source-asserted as absent and future WebGL restore/build work is separated.

## Stronger Evidence Accepted

- Contract tests cover malformed state, normalization, layout, calendar request records, and overlay state behavior.
- Browser matrix evidence covers Canvas, benchmark, and overlays across desktop, tablet, and mobile with 20 screenshots and zero warnings/errors/pageerrors.
- Package proof inspects actual `.nupkg` contents rather than trusting project metadata.
- Runtime dependency proof checks `package.json`, project package references, and runtime JS import/require patterns.
- Completed-stage validation is run after final report and proof artifacts are in place.
