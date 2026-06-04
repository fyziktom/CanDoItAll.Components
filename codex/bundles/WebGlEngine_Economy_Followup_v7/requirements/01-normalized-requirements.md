# Normalized requirements

| Id | Requirement | Source findings | Subbundles |
|---|---|---|---|
| R01 | Runtime stop must be settled-state proven | F01 | SB01,SB02,SB03 |
| R02 | Command lifecycle must distinguish accepted/scheduled/settled/cancelled/failed | F02 | SB02,SB03 |
| R03 | Readiness report must produce hard status levels and machine-readable gates | F03 | SB04,SB10,SB15 |
| R04 | Research strict mode must be a single first-class policy | F04 | SB05 |
| R05 | Store resolution must be explicit and auditable in strict mode | F05 | SB06 |
| R06 | Metric/invariant kinds must be registry-backed and unknowns must fail | F06 | SB07 |
| R07 | Behavior expansion profile must be declared, versioned, and hashed | F07 | SB08 |
| R08 | Golden oracles must validate core economic semantics | F03,F04,F05,F06,F07 | SB09 |
| R09 | Headless runner must be the source of truth for research runs | F03,F10 | SB10,SB13 |
| R10 | Every run must emit a reproducibility manifest | F11 | SB11 |
| R11 | Experiment design harness must isolate stochastic/config noise | F12 | SB12 |
| R12 | Browser visualization must be observer-only | F10 | SB13 |
| R13 | Performance thresholds must produce hard readiness states | F09 | SB14 |
| R14 | Final red-team must prove no simulator-noise failure path remains unclassified | All | SB15 |
