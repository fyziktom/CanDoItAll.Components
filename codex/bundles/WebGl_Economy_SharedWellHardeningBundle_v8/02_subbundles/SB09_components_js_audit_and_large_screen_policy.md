# SB09 — Components: JS audit and large-screen policy

## Required work
- Keep WebGL runtime module line thresholds.
- Keep forbidden domain word scan for generic WebGL runtime.
- Keep large-screen-only policy scan.
- Add audit that rejects new WebGL mobile/tablet/small-screen tasks unless explicitly marked unsupported-size warning only.
- Use desktop proof viewports only.

## Acceptance
No WebGL task should spend time on small/medium/mobile optimization.
