# SB10 Semantic Invariants: Domain boundary audit v3

## SB10-RC

Raw note owned: stabilize and freeze the generic Components WebGL engine without adding consuming-app semantics.

Expected behavior: Domain-boundary audit config, term registry, and workflow now scan source, public API snapshots, package content, docs, tools, workflows, and v14 bundle artifacts.

Disallowed shallow implementation: a change that only updates incomplete proof markers, snapshots, or diagnostics text without enforcing the runtime/source contract.

Failing-first or closed-gap proof: bundle://proof/SB10/transcripts/failing-first-or-closed-gap.txt

Passing proof: bundle://proof/SB16/transcripts/domain-boundary-generic-source-hard-gate.txt; bundle://proof/SB16/transcripts/domain-boundary-package-content-hard-gate.txt

Changed source files and hashes: bundle://proof/SB16/changed-file-hashes.txt

Production assertions: repo://tools/webgllib/domain-boundary-audit.config.json; repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/fixtures/domain-leakage-terms.json; repo://.github/workflows/domain-leakage.yml

Adversarial negative case: Hard-gate profiles reject source/package forbidden terms outside explicit allowlists.

Semantic positive case: All four domain-boundary profiles pass, with source and package hard gates reporting zero allowlisted matches.

Downstream dependency check: final SB16 build, tests, package proof, browser proof, and domain-boundary audits passed.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| SB10-RC behavior | repo://tools/webgllib/domain-boundary-audit.config.json; repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/fixtures/domain-leakage-terms.json; repo://.github/workflows/domain-leakage.yml | bundle://proof/SB16/transcripts/domain-boundary-generic-source-hard-gate.txt; bundle://proof/SB16/transcripts/domain-boundary-package-content-hard-gate.txt | bundle://proof/SB10/transcripts/implementation-validation.txt | bundle://proof/SB10/transcripts/failing-first-or-closed-gap.txt |

## Closure

Status: Solved. This invariant is backed by existing transcripts and the final bundle closure report.


