# Preparation self-review

## Senior architecture review

This bundle deliberately shifts from WebGL hardening to experiment trustworthiness. The main question is no longer "can the sandbox run?" but "can the result be trusted?"

## QA concerns addressed

- Pause/stop bug is handled as a runtime invariant, not only a UI issue.
- Experiment validity is separated from browser visualization validity.
- Strict semantic mode prevents warnings from hiding model errors.
- Golden oracles provide known-answer tests.
- Store resolution and behavior expansion are treated as economic policy, not neutral plumbing.
- Metrics and invariants become typed/validated instead of implicit fallback logic.
- Proof integrity is a first-class gate.

## Remaining uncertainty

Some exact implementation paths may differ depending on Codex's latest local changes. Codex must start SB01 with a current-state audit and adjust source references accordingly without weakening the invariants.
