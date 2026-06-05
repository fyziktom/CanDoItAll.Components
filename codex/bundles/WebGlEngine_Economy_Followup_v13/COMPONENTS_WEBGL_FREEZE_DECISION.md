# Components WebGL Release-Candidate Freeze Decision

Date: 2026-06-05

Decision: `CanDoItAll.Components.WebGlLib` and `CanDoItAll.Components.WebGlRunLib` are in release-candidate freeze for this bundle after SB08.

## Freeze Scope

- Generic WebGL scene runtime public API, JS `CanDoItAll.webglScene` surface, and package content.
- Generic WebGlRun action vocabulary, document/playback contracts, domain-driver contract, manifest schema, and package content.
- Domain boundary policy: Components generic source and package content remain free of Economy/scenario terms.
- Browser proof behavior for run playback pause/idle and observer real-state verification.

## Allowed Post-Freeze Changes

- Bug fixes required to keep the SB08 gate green.
- CI, packaging, and approval-test maintenance.
- Documentation clarifications that do not change generic behavior.
- Compatibility shims that preserve existing public API and are covered by approval snapshots.
- Browser proof or diagnostic hardening that does not add generic feature scope.

## Disallowed Without A New Architecture Bundle

- New generic WebGlRun action kinds.
- Public API removals or behavior-changing contract edits.
- Economy, market, ledger, claim, investment, farmer/land, or scenario-specific semantics in generic Components code.
- New mandatory runtime assets that are not documented and reflected in package snapshots.
- Generic feature expansion driven by Economy-only research needs.

## Gate Evidence

- SB02 approval snapshots and negative probes passed.
- SB03 package/sample boundary passed; WebGlRunLib package has no static GLB assets or Economy-path entries.
- SB04 hard domain gates passed; forbidden-term injection failed as expected.
- SB05 driver manifest/schema/hash/scrubber tests passed.
- SB06 Play -> Pause browser proof passed with runtime stop generation advanced and zero motion/stage blockers.
- SB07 observer proof passed using browser-loaded document hashes, scene hashes, driver hashes, and final browser positions.
- SB08 consolidated freeze transcript passed.

## CI Command List

See `proof/SB08/transcripts/components-freeze-gate.txt` for exact command lines and output.

## Red-Team Checklist

- [x] Public API changes are snapshotted and intentional.
- [x] Action-kind drift fails approval.
- [x] Unsupported driver-to-generic action mapping fails validation.
- [x] Domain leakage hard gates scan generic source, public approvals, and package content.
- [x] Historical docs/bundles are isolated to a soft audit.
- [x] WebGlLib remains consumable without WebGlRunLib.
- [x] WebGlRunLib remains consumable without Economy.
- [x] Browser pause proof reads live runtime diagnostics.
- [x] Browser observer proof compares actual browser-loaded state, not expected-to-expected fallback.
