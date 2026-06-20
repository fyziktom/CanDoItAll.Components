# SB14 — Instancing and LOD design checkpoint

        ## Purpose

        Do not implement heavy instancing unless justified; define adapter contract, metrics and fallback rules for a future generic repeated-object backend.

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
        6. Produce proof artifacts under `proof/SB14/`.


## Required implementation details

Do not implement a heavy instancing backend unless the design review proves it is needed.

Produce:
- a repeated-object abstraction proposal
- a mapping from current primitive/model fallback to potential instanced rendering
- a list of diagnostics required before implementation
- no public API widening unless approved by checkpoint C

Use Three.js `InstancedMesh`/`LOD` as benchmark concepts, but keep Components API backend-neutral.


        ## Done criteria

        - Code builds.
        - Relevant tests pass.
        - Domain-boundary hard gates pass.
        - Proof manifest exists and references non-empty files.
        - No unchecked domain terms appear in generic source.
        - A short QA note states why this does not make Components economy-specific.

        ## Required proof files

        - `proof/SB14/manifest.md`
        - `proof/SB14/transcripts/*.txt`
        - `proof/SB14/changed-file-hashes.txt`
        - optional browser JSON/screenshots if this subbundle touches runtime/browser behavior
