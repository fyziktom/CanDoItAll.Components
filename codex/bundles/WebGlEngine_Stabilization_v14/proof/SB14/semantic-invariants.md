# SB14 Semantic Invariants: Browser observer generic proof

## SB14-RC

Raw note owned: stabilize and freeze the generic Components WebGL engine without adding consuming-app semantics.

Expected behavior: The /run-playback browser observer proof drives the complete generic timeline, captures diagnostics, screenshot, visible pixels, and strict visual idle.

Disallowed shallow implementation: a change that only updates incomplete proof markers, snapshots, or diagnostics text without enforcing the runtime/source contract.

Failing-first or closed-gap proof: bundle://proof/SB14/transcripts/failing-first-or-closed-gap.txt

Passing proof: bundle://proof/SB14/transcripts/playwright-browser-proof.txt; bundle://proof/SB14/browser-observer-proof.json; bundle://proof/SB14/screenshots/run-playback-1920x1080.png

Changed source files and hashes: bundle://proof/SB16/changed-file-hashes.txt

Production assertions: repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs; bundle://proof/SB14/browser-proof-runner.mjs

Adversarial negative case: Initial browser proof failed until the runner exercised the full timeline; final proof now rejects incomplete playback.

Semantic positive case: Browser proof report pass=true with strictVisualIdle=true and no page errors.

Downstream dependency check: final SB16 build, tests, package proof, browser proof, and domain-boundary audits passed.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| SB14-RC behavior | repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs; bundle://proof/SB14/browser-proof-runner.mjs | bundle://proof/SB14/transcripts/playwright-browser-proof.txt; bundle://proof/SB14/browser-observer-proof.json; bundle://proof/SB14/screenshots/run-playback-1920x1080.png | bundle://proof/SB14/transcripts/implementation-validation.txt | bundle://proof/SB14/transcripts/failing-first-or-closed-gap.txt |

## Closure

Status: Solved. This invariant is backed by existing transcripts and the final bundle closure report.


