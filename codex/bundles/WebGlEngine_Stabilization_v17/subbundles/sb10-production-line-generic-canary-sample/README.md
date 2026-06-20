# SB10 — Production-line generic canary sample

        ## Purpose

        Add a non-economy canary using a domain driver/fixture for stations, buffers, WIP tokens, alarms and simple operator interactions without adding domain terms to generic source.

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
        6. Produce proof artifacts under `proof/SB10/`.


## Required implementation details

Add a production-line canary as a sample/test fixture, not generic source semantics.

The canary should model:
- process nodes/stations as generic scene objects
- buffers/queues as generic groups/layers
- WIP/parts as generic tokens
- alarms as generic symbols
- work movement as generic directed flows/motions
- simple click/selection/hover command interaction

Allowed domain vocabulary locations:
- canary sample code
- canary fixture JSON
- canary documentation
- domain-driver fixture

Forbidden:
- adding `machine`, `station`, `work-order`, `conveyor`, `wip`, `factory`, or equivalent terms to generic WebGlLib/WebGlRunLib source.


        ## Done criteria

        - Code builds.
        - Relevant tests pass.
        - Domain-boundary hard gates pass.
        - Proof manifest exists and references non-empty files.
        - No unchecked domain terms appear in generic source.
        - A short QA note states why this does not make Components economy-specific.

        ## Required proof files

        - `proof/SB10/manifest.md`
        - `proof/SB10/transcripts/*.txt`
        - `proof/SB10/changed-file-hashes.txt`
        - optional browser JSON/screenshots if this subbundle touches runtime/browser behavior
