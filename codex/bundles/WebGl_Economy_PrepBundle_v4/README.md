# CanDoItAll WebGL Engine + Economy Preparation Bundle v4

## Validation Summary

- Bundle preparation status: `Prepared`
- Bundle readiness gate: `Passed`
- Execution status: `Completed`
- Subbundle gate review: `Passed`
- Final closure gate: `Passed`
- Browser validation analytics: `Completed with WebGL screenshots and pixel checks`

## Purpose

This bundle is for the next Codex execution wave after the `webgl-engine` branch hardening.

It has two parallel tracks:

1. `CanDoItAll.Components`
   - harden the generic `WebGlLib` scene engine;
   - keep it fully domain-neutral;
   - prepare the minimum generic `WebGlRunLib` foundation without economy references.

2. `CanDoItAll.Economy`
   - prepare shared simulation abstractions that can be used by both simple-account simulations and ledger-backed simulations;
   - keep ledger-backed and simple-account engines isolated;
   - prepare generic visualization contracts inside the Economy repo without referencing the Components/WebGL packages yet.

## Critical execution rules

- Work in the currently checked-out branch in each repository.
- Do not create a new branch.
- Do not run `git switch -c`, `git checkout -b`, or equivalent branch-creation commands.
- Do not connect the two repositories yet.
- Do not add package references from `CanDoItAll.Economy` to `CanDoItAll.Components` in this wave.
- Do not add economy, ledger, process, or game semantics into `CanDoItAll.Components.WebGlLib`.
- Do not move existing simulator behavior into `Core`, `Ledger`, `BusinessObjects`, `SDK`, or `Node`.
- All source code comments must be in English.

## Recommended execution order

Use `05_spreadsheets/implementation_matrix.xlsx` first. It is the execution map and includes repo, project, dependency boundary, subbundle, validation, and source-reference columns.

Then follow the subbundles in order:

- SB01 Components current-branch and runtime audit guard
- SB02 Components WebGL engine cleanup and command consistency
- SB03 Components scene document and render scheduler hardening
- SB04 Components generic WebGlRunLib foundation
- SB05 Components refactoring gate A
- SB06 Economy inventory and dependency boundary guard
- SB07 Economy shared simulation abstractions
- SB08 Economy simple-account simulation backend prep
- SB09 Economy ledger-backed simulation adapter prep
- SB10 Economy generic visualization prep without WebGL dependency
- SB11 Economy scenario seeds: shared well and small entrepreneur community
- SB12 Cross-repo no-coupling validation
- SB13 Refactoring gate B and evidence closure

## Expected output

Codex should produce an implementation report in each repo:

- `CanDoItAll.Components/artifacts/webgl-engine-prep-v4/IMPLEMENTATION_REPORT.md`
- `CanDoItAll.Economy/artifacts/economy-simulation-prep-v4/IMPLEMENTATION_REPORT.md`

Both reports must include:
- changed files;
- dependency graph before/after;
- validation command outputs;
- explicit statement that no new branch was created;
- explicit statement that no cross-repo coupling was introduced.
