# SB07 — WebGlSceneView internal facade refactor

        ## Purpose

        Split implementation responsibilities while preserving public API and approval snapshots.

        ## Inputs to inspect

        - Components repository, branch `webgl-engine`
        - Existing approval tests and fixtures
        - Existing RC validation script
        - Existing package/sample projects
        - Domain-boundary config
        - Current WebGlSceneView and JS runtime surface

        ## Tasks

        1. Reproduce current state or create a failing-first proof when applicable.
        2. Implement the smallest generic fix that satisfies the subbundle.
        3. Do not modify Economy.
        4. Keep public APIs stable unless this subbundle explicitly says otherwise.
        5. Update docs and approval snapshots only with a clear reason.
        6. Produce proof artifacts under `proof/SB07/`.


## Required implementation details

Refactor without changing public API:
- move lifecycle/keying helpers out of `WebGlSceneView.razor`
- move JS invocation wrapper logic to an internal class/partial
- move runtime idle/stop annotation helper to an internal class/partial
- move callback deserialization helpers out of the main Razor file

## Required tests/proof

- WebGlLib public API approval unchanged unless explicitly justified.
- JS API surface approval unchanged.
- Existing browser proof still passes.
- External import lifecycle proof still passes.


        ## Done criteria

        - Code builds.
        - Relevant tests pass.
        - Domain-boundary hard gates pass.
        - Proof manifest exists and references non-empty files.
        - No unchecked domain terms appear in generic source.
        - A short QA note states why this does not make Components economy-specific.

        ## Required proof files

        - `proof/SB07/manifest.md`
        - `proof/SB07/transcripts/*.txt`
        - `proof/SB07/changed-file-hashes.txt`
        - optional browser JSON/screenshots if this subbundle touches runtime/browser behavior
