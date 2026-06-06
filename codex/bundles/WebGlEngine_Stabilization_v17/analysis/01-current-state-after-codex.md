# Current-state analysis after Codex v16

## Confirmed good progress

1. The repository now has a better package boundary:
   - `Directory.Build.props` sets `IsPackable=false`.
   - WebGlLib and WebGlRunLib opt in with `IsPackable=true`.
   - Samples and sandboxes should not be published accidentally.

2. A single RC validation command exists:
   - `npm run webgl:validate-rc`
   - `scripts/validate-webgl-rc.ps1`
   - `scripts/webgl-engine/validate-release-candidate.ps1`

3. Domain-boundary infrastructure is much stronger:
   - GitHub workflow has source hard gate, public API hard gate, package-content hard gate and docs/bundle soft audit.
   - Audit config distinguishes hard source/package scopes from historical docs/bundle paths.

4. Freeze approval tests exist:
   - WebGlLib public API.
   - WebGlLib JS surface.
   - WebGlLib JS API manifest.
   - WebGlLib package content.
   - WebGlRunLib public API.
   - WebGlRunLib action vocabulary.
   - WebGlRunLib domain-driver manifest schema.
   - WebGlRunLib package content.

5. Runtime idle semantics are no longer a single boolean:
   - `semanticIdle`
   - `visualIdle`
   - `finalRenderDrained`
   - policy modes: `semanticOnly`, `visualStrict`, `allowFinalRenderDrain`

## Remaining release-candidate blockers

### RC-BLOCKER-01: WebGlRunLib package-mode proof must be made true

The RC script claims to restore/build/run WebGlRunLib sample in package mode using:
- `UseComponentsWebGlRunLibPackage=true`
- `ComponentsWebGlRunLibPackageVersion=<version>`

However the inspected sample project currently has an unconditional `ProjectReference` to `src/CanDoItAll.Components.WebGlRunLib`.

Codex must add the same local/project-vs-package switch pattern as WebGlLibOnlyViewer.

### RC-BLOCKER-02: Proof hygiene must validate every RC step

A single RC script is good, but the output must include:
- non-empty transcript per step
- machine-readable assertion result per proof
- artifact hash per proof
- explicit no-op detection
- final manifest linking proof files to RC summary

### RC-BLOCKER-03: Production-line canary is required before freeze

The current generic sample is too small: two nodes and one motion.
A production-line canary should test:
- many repeated tokens
- buffers/queues
- station-like objects
- directed flow
- alarms/symbols
- hover/select/click
- pause/stop/seek
- no domain terms in generic source

The canary may contain production-line terms only in sample/test/domain-driver fixture code.

### RC-BLOCKER-04: WebGlSceneView still needs internal decomposition

Do not change public API unless approval intentionally changes. Split implementation risk:
- lifecycle/keying
- JS invocations
- callbacks
- runtime idle/stop
- command API
- import/export
- external import preservation

### RC-BLOCKER-05: JS behavior contract must be stricter than method-name approval

The JS surface should be frozen with:
- method name
- parameter shape
- return shape
- missing runtime behavior
- error behavior
- lifecycle state expectation
- whether it may schedule work
- whether it can return before idle
