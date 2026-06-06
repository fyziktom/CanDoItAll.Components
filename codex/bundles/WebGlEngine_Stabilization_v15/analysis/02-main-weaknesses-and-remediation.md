# Main weaknesses and remediation

## F01 Scope (High)

Keep v15 Components-only. Economy work from v13/v14 after SB08 is intentionally deferred.

**Remediation:** Add bundle guardrails: no CanDoItAll.Economy repo changes, no economy-specific behavior, only generic production-line canary specs/tests.

## F02 Freeze (High)

Freeze approval tests exist, but release-candidate gates must be elevated into a single pass/fail script and CI job.

**Remediation:** Create scripts/webgl-engine/validate-release-candidate.* that runs build/test/pack/audit/browser proof and fails on missing proof artifacts.

## F03 WebGlSceneView (High)

WebGlSceneView remains the broadest boundary file: lifecycle, callbacks, interop, imports, commands, idle/stop, external import keying.

**Remediation:** Keep public API but split implementation into internal partial/facade services or helper classes with approval tests proving API unchanged.

## F04 JS API (High)

window.CanDoItAll.webglScene is a global object with many methods. Freeze tests exist but typed result-shape compatibility needs stronger fixture coverage.

**Remediation:** Add JS API manifest with method, arguments, result shape, missing-runtime result, lifecycle state, and error behavior for every public JS method.

## F05 Runtime idle (High)

Idle model now has semantic/visual/final-drain modes. This is powerful but can hide visual work if policies are not documented and tested.

**Remediation:** Define three explicit modes: semanticOnly for headless/replay, visualStrict for browser proof, allowFinalRenderDrain for UI ergonomics; require visualStrict in release proof.

## F06 Packaging (Medium)

WebGlRun generic sample uses project reference only. WebGlLibOnly sample supports package mode.

**Remediation:** Add package-mode switch for WebGlRunLibGenericSample and proof that WebGlRunLib consumes WebGlLib via packages without Economy or sandbox references.

## F07 Domain drivers (Medium)

Domain driver contract is a good base but needs non-economy canary to prove it is not Economy-shaped.

**Remediation:** Add ProductionLineDomainMappingDriver test/sample using generic machine/station/job vocabulary only in driver fixture, mapping to generic actions.

## F08 Production line future (Medium)

Manufacturing simulations require stations, conveyors, queues, buffers, WIP, machine state, alarms, maintenance and throughput overlays. Current engine can represent most via generic objects/links/symbols, but queue/throughput overlays are not proven.

**Remediation:** Add generic production-line canary run document: stations as scene objects, conveyor links, WIP token motions, status symbols, utilization bars as symbols/metadata, and operator click/drag events.

## F09 Open-source gaps (Medium)

Compared to PlayCanvas/Three/Babylon, missing formal asset lifecycle docs, ECS/entity separation proof, instancing/LOD stress proof and profiler-like diagnostics budget.

**Remediation:** Add asset registry/resource lifecycle approval tests, instanced object stress, LOD/profile proof and performance counter summary.

## F10 Interop (Medium)

C# methods often call string-named JS functions directly. Approval tests cover JS surface but not C# wrapper-to-JS parity.

**Remediation:** Add parity audit: every public WebGlSceneView interop method maps to an approved JS API method and result type.

## F11 Domain leakage (Medium)

Audit config is strong but historical bundle/docs allowlists are broad and time-limited. This is acceptable short-term but should not mask source/package leaks.

**Remediation:** Separate source/package hard gates from soft docs/bundle audit; source/package gates must not inherit broad historical allowlists.

## F12 Docs (Low)

Docs are now rich but need a concise consumer guide for external app authors.

**Remediation:** Add `docs/webgl/consumer-quickstart.md` and `docs/webgl/domain-driver-authoring.md` with minimal examples and versioning rules.
