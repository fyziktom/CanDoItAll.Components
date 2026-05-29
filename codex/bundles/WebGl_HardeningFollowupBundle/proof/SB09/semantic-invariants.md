# SB09 semantic invariants

- `WebGlLib` remains domain-neutral and does not reference economy/process projects.
- `window.CanDoItAll.webglWorkbench` and `window.CanDoItAll.webglScene` coexist.
- Primitive asset profile remains available as the fastest default/fallback path.
- Mixed/high profiles load optional GLB models without removing primitive fallback.
- Transform-only motion and drag update object positions without requiring a full scene rebuild.
- Missing asset ids produce diagnostics and primitive fallback instead of a runtime crash.
- Browser validation is large-screen only for this bundle.
