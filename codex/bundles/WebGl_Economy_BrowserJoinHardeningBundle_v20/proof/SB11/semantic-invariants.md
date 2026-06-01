# SB11 Semantic Invariants

## Browser Smoke Scope

- Browser proof is desktop-only at `1440x900`.
- Mobile proof is not produced or implied.
- Browser smoke success does not mean the full UI demo is complete.

## Runtime Proof

- Initial scene proof must be captured before browser apply.
- Applied-frame proof must exercise a non-empty browser frame through the real route.
- Runtime proof must include browser-observed counters, not headless projection-only data.

## Analysis Proof

- Snapshot analysis proof must be visible in the browser after snapshot/analyze actions.
- Readiness must record failed host/runtime/apply blockers if the browser route is not hostable.

## Validator Markers

## Invariant ID

SB11-BROWSER-SMOKE-DESKTOP-ONLY.

## Shallow-pass trap

A headless projection-only test would pass without proving the Blazor route, WebGL canvas/context, browser runtime apply, or visible snapshot analysis.

## Adversarial negative proof

`bundle://proof/SB11/transcripts/browser-artifact-assertions.txt` rejects mobile proof claims, failed responses, page errors, and full UI demo readiness overclaiming.

## Semantic positive proof

`bundle://proof/SB11/applied-frame-proof.json` and `bundle://proof/SB11/snapshot-analysis-proof.json` prove a live large-screen browser applied frame 2 and displayed analysis findings.

## Anti-stub audit

`bundle://proof/SB11/transcripts/anti-stub-audit.txt` checks that browser proof artifacts contain real observed values instead of placeholder success markers.
