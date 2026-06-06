# Proof manifest - SB18

Status: completed

## Scope

Diagnostics and profiler-lite dashboard for generic large-scene proof.

## Artifacts

- Browser proof JSON: `bundle://proof/SB18/browser/performance-proof-browser.json`
- Page screenshot: `bundle://proof/SB18/browser/performance-proof.png`
- Canvas screenshot: `bundle://proof/SB18/browser/performance-proof-canvas.png`
- Canvas pixel proof: `bundle://proof/SB18/browser/performance-proof-pixels.json`
- Transcript: `bundle://proof/SB18/transcripts/performance-proof-browser-transcript.txt`
- Sandbox route: `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/PerformanceProof.razor`
- Sandbox code-behind: `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/PerformanceProof.razor.cs`

## Commands

- Playwright performance route proof recorded in `bundle://proof/SB18/transcripts/performance-proof-browser-transcript.txt`.
- `npm run webgllib:audit-large-scene-performance` executed in the RC wrapper.

## Result

- Browser proof passed.
- Route status: `Applied 202 commands with 0 queued stage(s).`
- Metrics: 202 commands, 100 objects, 100 coalesced patches, 100 dropped duplicate motions, render count greater than zero.
- Canvas pixel proof passed: 81 non-blank samples, 19 distinct sample colors.

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Command batch metrics | WebGlLib command batch result | performance dashboard | after large scene apply | missing 202 commands fails browser proof |
| Proof canvas pixels | Playwright canvas screenshot | pixel sampler | after route settles | blank or low-color canvas fails pixel proof |
| Generic diagnostics | WebGlSandbox performance route | RC reviewers | dashboard render | domain metrics are absent by design |
