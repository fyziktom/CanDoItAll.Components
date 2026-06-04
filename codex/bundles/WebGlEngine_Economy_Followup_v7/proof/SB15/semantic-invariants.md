# Semantic Invariants - SB15

| Invariant | Implementation surface | Guardrail | Proof |
|---|---|---|---|
| Economic truth is headless and gated | Economy strict policy, oracle suite, readiness report, headless runner, manifest, design harness | Browser/WebGL proof can support observer claims only; it cannot promote or mutate headless economic conclusions | `proof/SB15/final-red-team-report.md`, `proof/SB13/browser/observer-boundary-proof.json` |
| Failure classes remain separate | Economy readiness gates, performance budget gate, Components runtime diagnostics | Unknown events, ambiguous stores, unknown metrics, performance overages, browser non-idle state, and scenario hash drift are classified by their own gates or diagnostics | `proof/SB15/artifacts/final-readiness-summary.json` |
| Browser non-idle must be proven to settle | RunPlayback browser proof and WebGL runtime diagnostics | Pause proof must begin from active runtime work and then prove UI paused, runtime stop generation advanced, idle did not time out, and no queued work remained | `proof/SB15/browser/pause-idle-proof.json` |
| Performance noise cannot become an economic claim | SB14/SB15 performance reports | Over-budget headless runs are `not-comparable`; browser timing is observer/runtime proof; neither is reported as scenario/simulation/metric economics | `proof/SB14/artifacts/performance-budget-report.json`, `proof/SB15/browser/performance-budget-browser-proof.json` |
| Final readiness claim is bounded | SB15 final report and summary JSON | The final verdict must state both what can be claimed and what cannot be claimed | `proof/SB15/final-red-team-report.md` |

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative-test citation |
|---|---|---|---|---|
| `conditionally-research-ready` verdict | SB15 final readiness summary | Execution report and bundle README | Emitted only after focused tests, red-team classifications, browser proof, performance proof, and final validator pass | `proof/SB15/artifacts/final-readiness-summary.json` |
| Red-team failure classifications | Economy hardening tests, Components runtime/browser tests, SB15 report | Researchers deciding whether an output is publishable evidence | Each failure mode is tagged as simulation/config/runtime/comparability drift with `economicConclusionAllowed = false` | `proof/SB15/final-red-team-report.md` |
| Browser idle assertions | SB15 `pause-idle-proof.cjs` | Final report and browser analytics | Captured from the live RunPlayback page after active motion and Pause; late-drain state must remain paused | `proof/SB15/browser/pause-idle-proof.json` |
