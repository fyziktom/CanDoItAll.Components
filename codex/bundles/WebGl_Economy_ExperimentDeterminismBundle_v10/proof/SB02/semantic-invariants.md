# SB02 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB02-INV-001 | Same object can move out and return in the same frame when stages differ. | Keeping only the final motion because object id duplicates. | `WebGlRunActionCompilerTests.Compiler_preserves_home_well_admin_home_motion_sequence_for_same_actor` |
| SB02-INV-002 | Stage group and coalescing metadata survives into batch stages. | Contract properties exist but are ignored by planning/runtime. | `WebGlRunActionCompilerTests.Compiler_projects_stage_group_and_coalescing_scope_to_command_batch_stages` |

## Production Behavior Artifact Matrix

See `bundle://proof/SB02/manifest.md`.
