# Critical findings

| Id | Finding | Severity | Area | Detail |
|---|---|---|---|---|
| F01 | Runtime pause/stop proof incomplete | High | Components | Stop API exists but proof must show browser idle/settled state and no stale callbacks. |
| F02 | Command batch apply conflates scheduled and settled | Critical | Components | Stage scheduling can return before all barriers/motions complete. |
| F03 | Readiness report is not yet a research certificate | High | Economy | It answers useful questions but still declares browser actions missing and lacks golden oracle coverage. |
| F04 | Strict mode is not end-to-end | Critical | Economy | Warnings and fallbacks can still influence scenario outcomes. |
| F05 | Implicit store resolution can change outcomes | Critical | Economy | First-match/fallback store selection can mask incorrect scenario wiring. |
| F06 | Unknown metric/invariant fallback can create false positives | Critical | Economy | Unknown metric/invariant kinds should fail in research mode. |
| F07 | Behavior expansion is hidden economic policy | High | Economy | Expansion profile must be declared, versioned, and hashed. |
| F08 | Scenario source is improved but path compatibility remains mixed | Medium | Economy | Path fields remain in session/export; require migration guardrails. |
| F09 | Performance budgets are not hard gates | High | Cross-repo | Warning-only thresholds are insufficient for comparable experiments. |
| F10 | Visualization can still contaminate interpretation | High | Cross-repo | WebGL proof must be observer-only and never mutate economic state. |
| F11 | Artifact reproducibility not yet complete | High | Economy | Need full reproducibility manifest, source hashes, config hashes, model profile hashes. |
| F12 | No experiment design matrix yet | Medium | Economy | Need seed/repetition/factor-runner support for robust comparative experiments. |
