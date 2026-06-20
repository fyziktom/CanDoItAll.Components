# SB03 — Package-mode proof correctness

        ## Purpose

        Fix WebGlRunLibGenericSample package-mode support and prove it truly consumes freshly packed NuGet packages without local project references.

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
        6. Produce proof artifacts under `proof/SB03/`.


## Required implementation details

- Add package-mode properties to `samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj`:
  - `ComponentsWebGlRunLibProject`
  - `UseComponentsWebGlRunLibPackage`
  - `ComponentsWebGlRunLibPackageVersion`
- In package mode, use `PackageReference Include="CanDoItAll.Components.WebGlRunLib"`.
- In project mode, use local `ProjectReference`.
- Add a validation target that fails when project mode is requested but project path is missing.
- Add a source assertion proof that package-mode restore/build/run uses the packed package and not the local project.

## Required tests/proof

- `dotnet pack` WebGlLib/WebGlRunLib.
- Restore and build WebGlRunLibGenericSample against the local `.nupkg`.
- Run the sample and capture output.
- Inspect assets/refs/lockfile or build log to prove the package version is used.


        ## Done criteria

        - Code builds.
        - Relevant tests pass.
        - Domain-boundary hard gates pass.
        - Proof manifest exists and references non-empty files.
        - No unchecked domain terms appear in generic source.
        - A short QA note states why this does not make Components economy-specific.

        ## Required proof files

        - `proof/SB03/manifest.md`
        - `proof/SB03/transcripts/*.txt`
        - `proof/SB03/changed-file-hashes.txt`
        - optional browser JSON/screenshots if this subbundle touches runtime/browser behavior
