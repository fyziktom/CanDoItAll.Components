# SB05 Semantic Invariants

Status: Completed

| Invariant ID | Shallow-pass trap | Adversarial negative proof | Semantic positive proof | Anti-stub audit |
|---|---|---|---|---|
| SB05-STRICT-001 | A permissive validator can appear green while fallback-dependent or unresolved executable run documents still advance without structured errors. | `bundle://proof/SB05/transcripts/economy-strict-validator-tests.txt` exercises invalid strict inputs and requires error diagnostics with non-empty severity, code, and path fields. | The same transcript proves valid permissive fallback behavior remains warning-based only when explicitly enabled, while strict validation rejects the invalid executable documents required by SB05. | The SB05 source/proof set keeps validation logic in production testable code and does not rely on placeholder TODO, fake, stub, or NotImplemented behavior for acceptance. |

## Invariants

| Invariant | Status | Evidence |
|---|---|---|
| Strict validation diagnostics expose non-empty severity, code, and path. | Passed | `bundle://proof/SB05/transcripts/economy-strict-validator-tests.txt` |
| Every listed invalid strict input is rejected as an error. | Passed | `bundle://proof/SB05/transcripts/economy-strict-validator-tests.txt` |
| Permissive diagnostic fallback behavior remains warning-based when explicitly enabled. | Passed | `bundle://proof/SB05/transcripts/economy-strict-validator-tests.txt` |

## Contract

Economy bridge output cannot advance to real-scenario runner proof unless strict validation rejects fallback-dependent or unresolved executable run documents with structured diagnostics.
