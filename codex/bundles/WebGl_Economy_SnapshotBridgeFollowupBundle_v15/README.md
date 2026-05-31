# CanDoItAll WebGL + Economy Snapshot/Bridge Follow-up Bundle v15

This follow-up bundle is for **two already-pushed repositories**:

- `CanDoItAll.Components` on the currently checked-out WebGL branch, expected `webgl-engine`
- `CanDoItAll.Economy` on the currently checked-out branch, expected `main`

## Validation Summary

Bundle preparation status: `Ready after repair`
Bundle readiness gate: `Passed prepared-stage validator`
Execution status: `Completed through SB16`
Subbundle gate review: `SB01-SB16 passed`
Final closure gate: `Passed completed-stage validator`
Browser validation analytics: `SB01-SB16 passed; WebGL proof is large-screen desktop only when required`

## Primary outcome

Stabilize the current generic WebGL run layer, Economy simulation kernel, Economy-side WebGL bridge, and snapshot workflow so the next phase can safely build a connected simulation visualization sandbox **inside the Economy repository**.

## Non-negotiable rules

1. **Do not create a new branch.**
   Work only in the branch that is already checked out in each repository.
2. **Do not put Economy references into Components.**
   `CanDoItAll.Components` remains a generic UI/WebGL component repository.
3. **Connected simulation + visualization belongs in Economy.**
   The combined version should live under `CanDoItAll.Economy.SimulationSandbox` and related Economy projects.
4. **WebGL is desktop / large-screen only.**
   Do not spend time optimizing WebGL for phone, tablet, small screen, medium screen, responsive mobile layout, or mobile screenshot proof.
5. **Keep all generic layers free of example-specific vocabulary.**
   Terms like `water`, `well`, `farmer`, `land`, `parcel`, `oligarchy`, `near-household`, `far-household` are allowed in fixtures/tests/probes, not in generic runtime/kernel code.
6. **Source code comments must be in English.**

## What this bundle is not

This bundle does not ask Codex to build the final demo UI. It prepares the bridge, snapshot, diagnostics, stage execution, and sandbox workflow so a later demo can be built without hacks.

## Highest-risk findings

- JS stage runner and per-object motion queue now exist, but they still need stronger proof for long ordered visual sequences.
- Economy-side WebGL bridge now creates initial scene and stages, but it needs stronger mapping validation, no silent fallback, and end-to-end command proof.
- Snapshot contracts exist, but snapshot creation/analysis needs reusable services instead of test-local helper logic.
- Economy SimulationSandbox exists, but it is currently tied to `SimpleSimulationStateTransitionEngine`; it needs a backend-neutral orchestration seam.
- Large test files and broad files are emerging; they need scheduled split/refactor gates before the codebase becomes hard to maintain.

## Main spreadsheet

See:

```text
05_spreadsheets/implementation_matrix.xlsx
```

It contains the subbundle matrix, risk register, snapshot readiness matrix, bridge readiness matrix, and source references.
