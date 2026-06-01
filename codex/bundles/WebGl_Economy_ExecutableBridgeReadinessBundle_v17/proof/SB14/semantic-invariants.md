# SB14 Semantic Invariants

| Invariant ID | Shallow-pass trap | Adversarial negative proof | Semantic positive proof | Anti-stub audit |
| --- | --- | --- | --- | --- |
| SB14-INV-01 | Passing only focused tests could hide cross-repo build failures. | Final validation runs full Components solution build and full Economy solution build. | `components-build.txt` and `economy-build.txt` both show successful builds. | Build transcripts are command outputs captured from the local repos. |
| SB14-INV-02 | Passing targeted tests could miss unrelated regression in Economy. | Final Economy command runs the complete `CanDoItAll.Economy.Tests` project. | `economy-tests.txt` shows 519 passing tests. | The transcript includes the heavy performance probe in the full suite. |
| SB14-INV-03 | Closure could ignore architectural boundaries. | Final checks include boundary audit and Components Economy-reference scan. | `economy-boundary-audit.txt` passes and `final-branch-and-boundary-checks.txt` records no Economy references in Components source/tests. | The audit script checks source files, project references, file sizes, domain leakage, and renderer coupling. |
