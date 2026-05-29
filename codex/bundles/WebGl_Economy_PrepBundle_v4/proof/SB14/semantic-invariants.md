# SB14 Semantic Invariants

- Invariant ID: `SB14-cross-repo-validation`
- Source raw note: Validate WebGL rendering with screenshots and keep Economy uncoupled from Components/WebGL.
- Expected behavior: Components and Economy validation gates pass, WebGL scenes render, and no forbidden cross-repo references are introduced.
- Disallowed shallow implementation: A build-only closeout with no screenshot, pixel, dependency, or full-test evidence.
- Failing-first test: N/A process-only no production behavior; dependency scans cover the negative coupling case.
- Passing test: `dotnet test`, `npm run webgllib:*`, and boundary scan commands recorded in `bundle://proof/SB14/transcripts/cross-repo-validation.md`.
- Changed source files: `repo://artifacts/webgl-engine-prep-v4/BROWSER_WEBGL_PROOF.md`
- Production assertions: WebGL model lab renders `models 1` and run playback renders frame 2; Economy simulation layers compile and tests pass.
- Red-team negative case: Scans reject Economy references in Components WebGL libraries and Components/WebGL references in new Economy simulation projects.
- Downstream dependency check: SB15 closure proceeds only after the full validation transcript passes.
