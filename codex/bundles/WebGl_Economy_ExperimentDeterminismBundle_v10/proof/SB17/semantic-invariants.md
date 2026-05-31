# SB17 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB17-INV-001 | Future bridge mapping is documented without adding direct project references. | Implementing Economy -> Components bridge in this bundle. | `07_references/economy_webgl_bridge_mapping_design.md` |
| SB17-INV-002 | Input pack provenance maps to generic WebGL metadata only at the future bridge boundary. | Adding economy vocabulary into generic WebGL runtime code. | `Run_document_provenance_validator_keeps_generic_input_refs_domain_neutral` |

## Production Behavior Artifact Matrix

No production signal, state, record, or event is introduced by SB17.
