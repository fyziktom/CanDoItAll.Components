# Phase Plan

## Subbundle Dependency Map

```mermaid
flowchart TD
    SB01["SB01 Inventory and branch guard"]
    SB02["SB02 Stage execution runtime"]
    SB03["SB03 Per-object motion queue"]
    SB04["SB04 Action plan to staged batch"]
    SB05["SB05 Normalizer parity fixtures"]
    SB06["SB06 Run playback hardening"]
    SB07["SB07 Strict input pack mode"]
    SB08["SB08 Experiment pack loader"]
    SB09["SB09 Generic leakage audit"]
    SB10["SB10 Policy and event handler registry"]
    SB11["SB11 Transition diagnostics"]
    SB12["SB12 Metrics and invariants"]
    SB13["SB13 Visual mapping contracts"]
    SB14["SB14 Bridge adapter skeleton"]
    SB15["SB15 Large-screen policy regression"]
    SB16["SB16 Large-file gate"]
    SB17["SB17 Shared-well and farmer-land readiness"]
    SB18["SB18 Performance bottleneck proofs"]
    SB19["SB19 Validation and closure"]

    SB01 --> SB02 --> SB04 --> SB06
    SB01 --> SB03 --> SB04
    SB04 --> SB05
    SB05 --> SB06
    SB01 --> SB07 --> SB08
    SB08 --> SB10 --> SB11 --> SB12
    SB08 --> SB13 --> SB14
    SB09 --> SB10
    SB12 --> SB17
    SB13 --> SB17
    SB14 --> SB17
    SB02 --> SB15
    SB03 --> SB16
    SB11 --> SB18
    SB06 --> SB18
    SB15 --> SB19
    SB16 --> SB19
    SB17 --> SB19
    SB18 --> SB19
```

## Critical Subbundles

All subbundles are treated as critical because this bundle is explicitly about bridge readiness and hardening foundations. The highest-risk critical foundations are:

| Subbundle | Why it is critical |
|---|---|
| SB02 | Later staged playback, run playback, and bridge projection depend on real stage barriers. |
| SB03 | Later sequential visual actions depend on motions not fighting each other. |
| SB07 | Loader, readiness, and deterministic experiment proof depend on strict input integrity. |
| SB08 | Readiness probes and bridge inputs depend on one high-level loading path. |
| SB10 | Genericity depends on policies and handlers being plugin-friendly rather than core special cases. |
| SB11 | Downstream metrics and readiness proof depend on trustworthy diagnostics. |
| SB12 | Final readiness must interpret results with generic metrics and invariants. |
| SB14 | The bridge boundary must be isolated before any final demo work. |
| SB17 | Confirms both example probes use the same generic path. |
| SB19 | Closes proof consistency and final validation. |

## Phase Gates

| Gate | Required pass condition |
|---|---|
| Prepared gate | `scripts/validate_bundle.py --stage prepared` passes and no required bundle scaffold is missing. |
| Entry gate | Current subbundle prerequisites are completed or explicitly N/A, source references still exist, and scope still matches the bundle. |
| Closure gate | Acceptance checklist, proof artifacts, transcripts, changed-file hashes, source assertions, semantic invariants, and execution-report rows are updated. |
| Cross-repo bridge gate | Components has no Economy references; low-level Economy abstractions have no WebGL/Components references; only the bridge project may reference both. |
| Large-screen gate | WebGL work has no unguarded small/medium/mobile/tablet optimization drift. |
| Final gate | Components tests/audits, Economy tests/audits, cross-repo scans, readiness artifacts, performance notes, and `scripts/validate_bundle.py --stage completed` pass or record an honest blocker. |

