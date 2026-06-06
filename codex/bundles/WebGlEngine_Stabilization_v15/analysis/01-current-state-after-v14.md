
# Current-state analysis after v14

## Strong progress

- `Directory.Build.props` now defaults `IsPackable=false`; `WebGlLib` and `WebGlRunLib` opt in explicitly.
- Solution includes package projects, sandbox, WebGlLib-only sample, WebGlRunLib generic sample, and tests.
- `WebGlRunActionKinds` exposes approved generic vocabulary through `All`.
- `ResourceTransferVisual` has been generalized to `DirectedFlowVisual`.
- `IWebGlRunDomainMappingDriver` exists with manifest/hash/scrubber/validator.
- `WebGlSceneView` now exposes runtime idle policy mode parameters and richer idle/command behavior.
- Domain-boundary CI now has hard source, public API, package content, and soft docs/bundle profiles.
- Freeze approval tests exist for public API, JS surface, JS API manifest, package content, action kinds, and driver manifest.

## Remaining release-candidate risks

1. `WebGlSceneView` still concentrates many responsibilities in one boundary file. Keep the public API, but split internals.
2. JS API compatibility must be stronger than method-name approval: require argument/result/missing-runtime/lifecycle shape manifests.
3. Package-mode proof for WebGlRunLib generic sample must be explicit, not just project reference.
4. Domain-boundary hard gates must not depend on broad active-bundle allowlists.
5. Runtime idle modes need hard semantics: release proof should use `visualStrict`; UI convenience may use `allowFinalRenderDrain`.
6. Production-line canary is still needed to prove genericity outside Economy.
7. One release-candidate validation command is missing.
