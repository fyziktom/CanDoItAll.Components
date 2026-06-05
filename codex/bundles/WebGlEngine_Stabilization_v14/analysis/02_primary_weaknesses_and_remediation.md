# Primary Weaknesses And Remediation

## W1: API drift risk

Without API approval files, later Economy-driven changes may silently alter generic contracts. This is the largest strategic risk because the whole goal is to stop changing Components for every Economy use case.

**Fix:** Generate and approve public API manifests for `WebGlLib`, `WebGlRunLib`, and `window.CanDoItAll.webglScene`. Add CI checks that fail on unapproved changes.

## W2: Package scope ambiguity

The repo-level `IsPackable=true` is convenient but risky. It can accidentally make demo/sandbox/test projects look like package outputs.

**Fix:** Set package intent explicitly per project. Only intended library packages should be packable by default. Samples may be buildable but should not be accidentally published unless there is a deliberate sample package path.

## W3: WebGlSceneView boundary bloat

`WebGlSceneView` is now a critical public boundary. Keeping too much logic inside a `.razor` file increases regression risk.

**Fix:** Preserve public API but move interop calls/lifecycle key logic/result annotation into internal partial classes/services.

## W4: Runtime idle proof ambiguity

Final scheduled render drain can be valid, but it should not be silently accepted in every proof mode.

**Fix:** Add explicit idle policies and require strict mode in CI browser proof.

## W5: Domain provenance leakage

`source.*` metadata is allowed as traceability. That is useful, but raw domain values can leak into generic artifacts.

**Fix:** Add opaque provenance mode and a generic trace-map reference model. Domain drivers own raw trace maps.

## W6: Generic sample coverage gap

`WebGlLibOnlyViewer` supports package mode. `WebGlRunLibGenericSample` should also prove package-mode consumption.

**Fix:** Add project/package switch and CI proof.

## W7: Weak freeze handoff

The current system still lacks a single “Components RC frozen” signoff.

**Fix:** Add release-candidate freeze manifest covering public API, JS API, package contents, domain boundary, runtime proof, samples, docs, and known deferred issues.
