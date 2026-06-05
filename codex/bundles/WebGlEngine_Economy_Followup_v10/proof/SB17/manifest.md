# Proof manifest SB17

Status: pass

Required proof: Runbook includes CLI commands, expected artifacts, failure interpretation, and domain-driver guidance.

Artifacts attached:
- `source-scan-operator-runbook.txt` - source scan proving the runbook contains CLI commands, expected artifacts, failure interpretation, status guidance, schema references, and domain-driver guidance.
- `changed-file-hashes.txt` - SHA-256 hash for the updated Economy readiness documentation.
- `anti-stub-scan.txt` - anti-stub scan for the updated Economy readiness documentation.

Result:
Pass. `docs/simulation/experiment-readiness.md` now includes an operator runbook for exploratory, headless-valid, oracle-valid, browser-observer-valid, and research-ready workflows; exact CLI commands; expected headless artifacts; failure interpretation for `failed`, `not-comparable`, and exploratory states; manifest-diff usage; and domain-driver guidance that keeps Economy semantics in the Economy bridge.

Production Behavior Artifact Matrix:

| Behavior | Documentation artifact | Proof artifact |
| --- | --- | --- |
| Operators can run and interpret credible experiment evidence | `docs/simulation/experiment-readiness.md` | `source-scan-operator-runbook.txt` |
| Documentation names the current readiness and headless manifest schemas | `docs/simulation/experiment-readiness.md` | `source-scan-operator-runbook.txt` |
| Domain-driver guidance keeps Components generic and Economy semantics in the Economy bridge | `docs/simulation/experiment-readiness.md` | `source-scan-operator-runbook.txt` |
