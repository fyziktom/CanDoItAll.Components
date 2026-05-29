# CanDoItAll.Components WebGL Runtime Hardening Follow-up Bundle v2

Repository: `fyziktom/CanDoItAll.Components`

Primary target: the branch currently checked out in the developer workspace.

This bundle is a second hardening pass for the generic `CanDoItAll.Components.WebGlLib` scene runtime and the standalone `CanDoItAll.Components.WebGlSandbox`. It assumes the previous symbolic tycoon sandbox and WebGL scene hardening work is already merged/present.

## Absolute branch rule

Codex must **not** create a new branch for this work.

At the start of execution, Codex must run:

```powershell
git branch --show-current
git status --short
```

Codex must work in the currently checked-out branch. It must not run `git checkout -b`, `git switch -c`, create a new local branch, create a remote branch, or retarget the work to a different branch. If the current branch is unexpected or dirty in a risky way, Codex must stop and report the current branch/status instead of creating a branch.

## Goal

Prepare the generic WebGL wrapper for future run/simulation/game layers without turning `WebGlLib` into a game engine. The focus is clean JavaScript organization, safe runtime lifecycle, robust model import diagnostics, asset catalog maintainability, deterministic command APIs, and validation guardrails.

## Main deliverables

1. Keep `WebGlLib` domain-neutral.
2. Harden JS module boundaries and helper structure.
3. Add model import diagnostics for invisible/misaligned GLB files.
4. Fix resource ownership/disposal risks for loaded/cloned model resources.
5. Improve idle render-loop behavior.
6. Add richer command results for patches/motion/import/export.
7. Add generic scene document serialization contracts.
8. Split sandbox catalog and large UI/runtime files where needed.
9. Add validation tools that prevent future monolithic JS files and unsafe patterns.
10. Preserve `WebGlWorkbench` compatibility.

## Execution order

Follow the subbundles in `03_subbundles/` in order. Do not skip the inventory and branch guard.

