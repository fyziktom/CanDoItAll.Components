# SB06 Semantic Invariants

| Invariant ID | Shallow-pass trap | Adversarial negative proof | Semantic positive proof | Anti-stub audit |
| --- | --- | --- | --- | --- |
| SB06-INV-01 | A scan that only checks project references could miss renderer terms embedded in abstraction source. | `audit-simulation-boundaries.ps1` now checks source text for Components/WebGL/GLB coupling outside the bridge. | `renderer-neutral-source-scan.txt` proves no forbidden renderer terms in Abstractions or Visualization. | The scan is source-based and fails on concrete forbidden strings, not on documentation claims. |
| SB06-INV-02 | Renaming a variable without preserving validation would pass a superficial boundary review. | Semantic key validation still rejects renderer-specific asset markers. | `EconomyVisualMappingValidation.cs` keeps renderer-specific marker checks using neutral naming. | Existing visual mapping loader tests remain covered by Economy test runs. |
