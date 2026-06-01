# Original Request

User asked Codex to execute the prepared bundle at:

`C:\repositories\CanDoItAll.Components\codex\bundles\WebGl_Economy_IntegratedSimulationReadinessBundle_v16`

The requested workflow is to use `candoitall-bundle-workflow` to implement, validate, and test everything defined by the bundle.

Hard constraints preserved from the bundle:

- Work in the currently checked-out branches in both repositories.
- Do not create a new branch.
- Keep `CanDoItAll.Components` generic and UI/WebGL-only.
- Keep joined simulation plus visualization orchestration in `CanDoItAll.Economy`.
- Do not add Economy references to Components.
- Keep WebGL validation desktop and large-screen only.
- Do not implement the final shared-resource or constrained-resource demo UI in this wave.
