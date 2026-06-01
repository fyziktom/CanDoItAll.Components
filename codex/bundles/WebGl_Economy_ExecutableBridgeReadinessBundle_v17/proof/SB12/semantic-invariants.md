# SB12 Semantic Invariants

| Invariant ID | Shallow-pass trap | Adversarial negative proof | Semantic positive proof | Anti-stub audit |
| --- | --- | --- | --- | --- |
| SB12-INV-01 | A README-only claim could miss example terms in reusable code. | The boundary audit fails on forbidden terms in generic simulation projects except approved scenario factories/materializers. | `generic-domain-term-scan.txt` reports no forbidden terms outside approved files. | The transcript is generated from source files, not manual inspection. |
| SB12-INV-02 | Fixing domain terms could break renderer boundary checks. | SB12 reruns the full boundary audit after SB06 changes. | `domain-leakage-boundary-audit.txt` passes. | Same script checks references, file size gates, renderer coupling, backend coupling, randomness, and domain leakage. |
