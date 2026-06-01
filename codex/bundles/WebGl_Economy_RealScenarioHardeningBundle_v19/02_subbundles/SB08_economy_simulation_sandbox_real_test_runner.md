# SB08 - Economy simulation sandbox real test runner

Codex must add a real headless runner command or test helper that produces artifacts:

- `input-pack.validation.json`
- `simulation.frames.json`
- `simulation.deltas.json`
- `visual.frames.json`
- `webgl.run-document.json`
- `snapshots/<snapshot-id>.json`
- `snapshot-analysis/<snapshot-id>.json`
- `readiness-report.json`

Use existing shared-resource and finite-resource probes, but keep the runner generic.
