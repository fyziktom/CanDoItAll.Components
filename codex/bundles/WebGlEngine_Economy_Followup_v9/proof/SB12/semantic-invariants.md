# Semantic invariants - SB12

Status: completed

## Invariants

1. Components executable concepts remain generic.

   Object `kind`, object `family`, and stage ids in the generated WebGL run document must not contain domain terms such as `buyer`, `seller`, `investor`, or `elite`. The strict bridge test and browser assertion file both verify this boundary. Domain terms may remain only in source provenance metadata.

2. Renderer-specific asset choices are isolated from neutral Economy mapping.

   Category asset ids, pose asset ids, symbol asset ids, anchor keys, and diagnostic fallback object ids are carried by `rendererBinding`. Neutral fields such as category `assetId`, pose `assetId`, symbol `symbolAssetId`, and anchor alias `anchorKey` remain empty in the multi-goods fixture.

3. Visual mapping must fail visibly when bridge-bound fields leak into neutral input.

   The failing-first proof records eight `bridge-bound-visual-field` diagnostics from the pre-SB12 multi-goods readiness report. Final focused tests require the fixture to validate without those diagnostics.

4. The browser proof must exercise the generated scenario, not the sandbox default sample.

   The Playwright proof loads `webgl.run-document.json` for run id `sb12-multi-goods.experiment.multi-goods-elite.exchange-investment.v1`, verifies matching expected/browser document hashes, and confirms 23 objects, 12 links, and 1 stage.

5. Browser observer evidence remains separate from headless economic truth.

   The real scenario runner exports canonical headless artifacts and readiness data; the browser proof separately loads the generated document into `/run-playback`, captures screenshot/console/assertion artifacts, and observes runtime idle with completed stage and final positions.

6. Nullable object patches do not overwrite stable object placement.

   Optional null patch fields are treated as absent by the JS scene patcher. The browser proof checks the policy-board object retained `(4, 3, 0)` across scene object, object instance, and runtime group state.

## Validation

- `proof/SB12/transcripts/generic-visualization-tests.txt`: 42 focused Economy tests passed.
- `proof/SB12/transcripts/webgl-sandbox-build.txt`: WebGlSandbox build succeeded with 0 warnings and 0 errors.
- `proof/SB12/transcripts/multi-goods-browser-playwright.txt`: browser observer proof passed.
- `proof/SB12/browser/multi-goods-browser-assertions.json`: observer proof valid, runtime idle true, document hashes matched, and no disallowed console messages.
