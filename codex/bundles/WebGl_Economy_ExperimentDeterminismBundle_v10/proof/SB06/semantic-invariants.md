# SB06 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB06-INV-001 | WebGL run documents can carry generic input provenance. | Introducing Economy references into WebGlRunLib. | `Run_document_provenance_validator_keeps_generic_input_refs_domain_neutral` |
| SB06-INV-002 | Economy/ledger/water/farmer/tax/market terms are rejected in generic provenance. | Domain terms leak into generic WebGL runtime. | Same validator test negative case. |

## Production Behavior Artifact Matrix

See `bundle://proof/SB06/manifest.md`.
