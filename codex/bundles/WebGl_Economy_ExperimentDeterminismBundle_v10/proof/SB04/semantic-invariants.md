# SB04 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB04-INV-001 | Generic anchors such as use/trade/home resolve without economy vocabulary. | Hardcoding wells, farms, or markets into WebGL runtime. | `Target_resolver_reports_resolution_metadata_for_trade_anchor_and_explicit_position` |
| SB04-INV-002 | Distance and optional duration estimates are deterministic metadata. | Movement succeeds without measurable travel proof. | `Planner_adds_deterministic_distance_estimate_to_targeted_motion` |

## Production Behavior Artifact Matrix

See `bundle://proof/SB04/manifest.md`.
