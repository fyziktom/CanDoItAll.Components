# Phase plan and dependencies

```mermaid
flowchart TD
    SB01[SB01 Current-state and pause proof audit]
    SB02[SB02 Runtime idle and stop semantics]
    SB03[SB03 Playback state machine and replay modes]
    SB04[SB04 Strict experiment mode]
    SB05[SB05 Store resolution policy]
    SB06[SB06 Golden oracle suite]
    SB07[SB07 Metric/invariant registry]
    SB08[SB08 Behavior expansion profiles]
    SB09[SB09 Scenario pack hash hardening]
    SB10[SB10 Readiness report and failure classification]
    SB11[SB11 Performance budgets]
    SB12[SB12 Headless experiment runner]
    SB13[SB13 Docs and operator troubleshooting]
    SB14[SB14 Final red-team closure]

    SB01 --> SB02
    SB02 --> SB03
    SB03 --> SB10
    SB04 --> SB06
    SB05 --> SB06
    SB07 --> SB06
    SB08 --> SB06
    SB09 --> SB10
    SB06 --> SB10
    SB10 --> SB11
    SB10 --> SB12
    SB11 --> SB14
    SB12 --> SB14
    SB13 --> SB14
```

## Gate pauses

Codex must pause for a refactor/self-review after:

- SB03: runtime/playback lifecycle stabilized.
- SB06: economics oracle suite established.
- SB10: readiness report implemented.
- SB14: final closure.

No later subbundle may paper over failed invariants from an earlier subbundle.
