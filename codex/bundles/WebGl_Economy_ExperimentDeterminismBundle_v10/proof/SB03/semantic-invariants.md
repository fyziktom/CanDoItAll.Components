# SB03 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB03-INV-001 | Planner consumes canonical `ActionKind`, `SubjectObjectId`, target, pose/symbol, timeline, and stage metadata. | Planner reads compatibility aliases directly and silently accepts conflicts. | `Planner_normalizes_aliases_once_and_warns_on_conflicts` |
| SB03-INV-002 | Invalid target-based actions fail before planning output is emitted. | Unsupported or incomplete actions produce no-op plans. | `Planner_rejects_unsupported_missing_subject_and_missing_target_actions_after_normalization` |

## Production Behavior Artifact Matrix

See `bundle://proof/SB03/manifest.md`.
