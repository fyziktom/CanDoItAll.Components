# One-shot Codex prompt

You are working across two local repositories:

- `CanDoItAll.Components`
- `CanDoItAll.Economy`

Do not create a new branch in either repository. First run `git branch --show-current` in both repositories and continue in the checked-out branches.

Implement the follow-up described in this bundle. Work in phases and stop after each refactoring gate to validate. Do not connect Economy to WebGL yet.

In Components:
- harden current `webgl-engine` implementation;
- finish asset-cache disposal and scene-index synchronization;
- unify command results;
- split `WebGlSceneDocumentSerializer`;
- mature `WebGlRunLib` into a reusable generic run/playback/action layer;
- move sandbox playback logic into `WebGlRunLib`;
- add generic visual action/event mapping without economy concepts.

In Economy:
- harden and split new `Simulation.*` preparation projects;
- add loadable scenario definitions;
- add simulation event stream contracts;
- add economy visual action contracts without WebGL dependency;
- convert shared-well and entrepreneur examples from hardcoded frame seeds into scenario definitions plus materialization proof;
- preserve strict project reference boundaries.

Use `05_spreadsheets/implementation_matrix.xlsx` as the authoritative checklist. All source code comments must be in English.
