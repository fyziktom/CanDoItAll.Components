# SB10 Semantic Invariants

## Invariant SB10-GENERICITY-001

Raw notes:
- RN-006: "Validate shared-resource and finite-resource probes without hardcoding example-specific concepts into generic models."
- RN-012: "Remove example terms from generic production code; example names belong only in factories, fixtures, tests, docs, or probes."

Expected behavior:
- Generic simulation abstractions, visualization, and bridge code use resource-scoped vocabulary: resource requirements, resource limits, capacity, ownership, transfer, rule, relationship, issue, metric, invariant, and snapshot.
- Example-domain terms are not present in generic production files except allowlisted scenario factories/materializers/policies.
- Boundary audit continues to enforce coupling and example-domain leakage checks.

Shallow-pass trap:
- A search can pass by scanning only new files or by ignoring generated runtime paths while stale example-domain terms remain in shared production code.

Adversarial negative proof:
- `bundle://proof/SB10/source-assertions/strict-generic-domain-term-scan.txt` scans `Simulation.Abstractions`, `Simulation.SimpleAccounts`, `Simulation.Visualization`, and `Simulation.WebGlBridge` production files for the forbidden example-domain term list, excluding only scenario factory/materializer/policy allowlists.
- `bundle://proof/SB10/transcripts/simulation-boundary-audit.txt` independently fails if non-allowlisted generic production files contain those terms.

Semantic positive proof:
- `bundle://proof/SB10/source-assertions/generic-resource-concept-source-scan.txt` shows the production vocabulary remains resource/capacity/ownership/transfer/rule/relationship/issue oriented.
- `bundle://proof/SB10/source-assertions/allowlisted-domain-term-scan.txt` records remaining example-domain terms only in allowlisted factories/tests/probe contexts.

Anti-stub audit:
- SB10 is audit-only; no replacement code or stub implementation was introduced. The validator evidence is the strict term scan plus boundary audit transcript.

Changed source files:
- No production source files changed for SB10.

Downstream dependency check:
- SB15 may proceed because generic projects contain no unallowlisted example-domain terms and the boundary audit passes.
