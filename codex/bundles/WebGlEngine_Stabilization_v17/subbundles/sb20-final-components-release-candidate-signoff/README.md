# SB20 — Final Components release-candidate signoff

        ## Purpose

        Run full RC command, package proof, browser proof, boundary audits and freeze approvals; produce final signoff manifest.

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
        6. Produce proof artifacts under `proof/SB20/`.


## Required implementation details

Run the full release-candidate validation command and ensure it generates:
- `artifact-manifest.json`
- `validation-summary.json`
- `validation-summary.md`
- non-empty transcript
- package artifacts
- browser observer proof
- domain-boundary audit outputs
- package-mode sample proof
- approval test output

No empty transcript may be accepted as proof.


        ## Done criteria

        - Code builds.
        - Relevant tests pass.
        - Domain-boundary hard gates pass.
        - Proof manifest exists and references non-empty files.
        - No unchecked domain terms appear in generic source.
        - A short QA note states why this does not make Components economy-specific.

        ## Required proof files

        - `proof/SB20/manifest.md`
        - `proof/SB20/transcripts/*.txt`
        - `proof/SB20/changed-file-hashes.txt`
        - optional browser JSON/screenshots if this subbundle touches runtime/browser behavior
