# SB01 semantic invariants

| Invariant | Requirement | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB01-INV-001 | Execution stays on existing branches. | Creating or switching to a new branch while claiming compliance. | `bundle://proof/SB01/transcripts/inventory.txt` records branch status for Components, Economy, and CanDoItAll. |
| SB01-INV-002 | The bundle is structurally ready before implementation. | Implementing directly from notes without the prepared validator. | Prepared validator transcript in `bundle://proof/SB01/transcripts/inventory.txt`. |
| SB01-INV-003 | Required repo surfaces exist before dependent subbundles start. | Assuming project paths from the bundle without inspecting current repos. | Inventory observations in `bundle://proof/SB01/transcripts/inventory.txt`. |

## Production Behavior Artifact Matrix

SB01 does not introduce production signals, state, records, or events. It only records repository inventory and workflow gate proof.
