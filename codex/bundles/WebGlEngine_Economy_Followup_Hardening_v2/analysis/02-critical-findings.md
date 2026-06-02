# Critical findings and follow-up targets

## F01 — Economy sandbox runtime still depends on test fixture paths

`EconomySimulationSandboxPage` resolves the default fixture by walking upward from `AppContext.BaseDirectory` until it finds `tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/{scenarioId}/{fileName}`. This makes the Node route work only from a repository checkout that contains test fixtures. It is not suitable for package consumption, deployment, Docker, or enterprise use.

Expected direction: move scenario discovery/loading behind an app-owned provider such as `IEconomySimulationScenarioCatalog`, with default embedded or content-root sample packs. Tests may continue using test fixtures, but runtime UI must not hard-code test directories.

## F02 — WebGlRun frame application may silently drop direct frame commands when stages exist

`WebGlRunFrameApplyResult.FromFrame` emits direct `frame.ScenePatches` and `frame.Motions` only when `frame.Stages.Count == 0`. A frame that contains both direct commands and staged commands will silently lose the direct commands. This must become either a validator error or a deterministic generated stage.

## F03 — Scene revision normalization is still not fully canonical

The policy says top-level `Scene.Revision` is canonical and `UiState.Revision` mirrors it when committed. However `WebGlSceneRevisionPolicy.Normalize` only sets `scene.Revision = Resolve(scene)` and does not sync `scene.UiState.Revision`. `WebGlSceneDocumentNormalizer` calls `Normalize` before optionally replacing UI state, creating room for divergent persisted documents.

## F04 — Initial scene runtime options are ignored by browser reset path

`WebGlRunBrowserApplyAdapter` accepts a `WebGlSceneDocument`, but `WebGlSceneViewBrowserRuntime.ImportSceneAsync` imports only `sceneDocument.Scene`. The document's `RuntimeOptions` are currently ignored during reset. The architecture must explicitly decide whether runtime options are external-only or part of replay/reset semantics, then enforce that decision with tests.

## F05 — Patch transaction modes are not named or documented

JS patching now pre-validates, which is good. But `missingLinkEndpointMode = warn` can still result in a partially applied patch where object changes land and invalid links are skipped. That can be valid for permissive visual overlays, but only if named and tested as a partial-warning mode. The current API does not make transaction mode explicit enough.

## F06 — Generic WebGlRun boundary validator and domain provenance are in tension

`WebGlRunDocumentValidator` forbids terms such as economy, ledger, market, account, buyer, seller, and price in metadata values. Economy bridge outputs intentionally carry `source.*` provenance such as source action kind, event kind, input hash, and bridge names. The system needs a precise rule: either generic validators allow `source.*` provenance while blocking generic contract pollution, or domain bridge validators own domain provenance separately.

## F07 — Economy WebGL bridge validation assumes a static initial scene

`EconomyWebGlRunValidator` validates motion and patch references only against initial scene object ids. That is adequate for a static layout but will reject future dynamic simulations where a patch adds an object and later stages reference it. The follow-up must decide whether Economy v1 officially forbids dynamic object creation or supports evolving object ids.

## F08 — Resource ownership improved, but async load/dispose race proof is still thin

The new ownership model avoids disposing shared textures from tinted instances. It still needs explicit stress proof for async GLB load completion after group disposal, asset cache disposal while promises are pending, repeated dispose/recreate loops, and future global/shared cache extension points.

## F09 — Proof artifacts are numerous but need evidence-quality audit

The previous execution added a large number of proof files, screenshots, transcripts, and bundle artifacts. Some transcripts in compare output have zero line changes and some generated logs are very large. The follow-up must audit whether proof manifests cite existing, non-empty, semantically meaningful artifacts, and whether the repository is accumulating transient proof noise.

## F10 — Economy README and package surface are stale relative to the new simulation stack

The root README is still ledger-centric. The solution now contains a broad simulation subsystem with abstractions, simple account backend, ledger backend, visualization, WebGL bridge, sandbox service, components, and Node route. Public docs need a current map and package-boundary rules so future simulation backends do not leak into the wrong layer.

## F11 — Browser UI proof is tied to a demo fixture path and raw HTML controls

The sandbox page uses direct service construction and direct buttons/CSS. That may be acceptable for a proof host, but if this component is part of `CanDoItAll.Economy.Components`, it should either use DI + app-owned scenario provider and shared UI conventions or be explicitly marked as sandbox-only.

## F12 — Component package consumption strategy is inconsistent across packages

Economy components can switch WebGL packages between project/package mode, but BaseLib/Charts/Mermaid remain package references. The bridge project only references WebGlRunLib directly in package mode, relying on transitive WebGlLib availability. This may be valid, but the package proof must explicitly test stale-cache prevention and transitive compile assets.
