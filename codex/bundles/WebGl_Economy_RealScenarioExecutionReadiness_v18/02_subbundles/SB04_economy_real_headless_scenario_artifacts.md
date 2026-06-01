# SB04 — Economy Real Headless Scenario Artifacts

## Goal

Produce real artifacts for the current generic probes without building the final browser UI.

## Required probes

- shared-resource probe using the existing shared-well input pack
- finite-resource probe using the existing farmer-land input pack

## Required artifacts per probe

```text
artifacts/economy/real-probe/<probe>/input-pack.validation.json
artifacts/economy/real-probe/<probe>/simulation.frames.json
artifacts/economy/real-probe/<probe>/simulation.deltas.json
artifacts/economy/real-probe/<probe>/visual.frames.json
artifacts/economy/real-probe/<probe>/webgl.run-document.json
artifacts/economy/real-probe/<probe>/snapshots/<snapshot-id>.json
artifacts/economy/real-probe/<probe>/analysis/<snapshot-id>.json
artifacts/economy/real-probe/<probe>/readiness-report.json
```

## Acceptance

The report must answer:

- Was the input pack valid?
- How many simulation frames/deltas were produced?
- How many visual frames/actions were produced?
- How many WebGL frames/stages/motions/patches were produced?
- How many snapshots were produced?
- Which warnings/errors remain?
- Is the probe ready for large-screen browser execution?
