# SB05 Semantic Invariants

## Invariant ID

SB05-economy-desktop-sandbox-page

## Shallow-pass trap

A page can render labels while never using `EconomySimulationSandboxSessionService`, never applying a WebGL frame through the generic browser adapter, or only showing placeholder diagnostics.

## Adversarial negative proof

`EconomySimulationSandboxPage` keeps explicit status paths for missing fixture files, failed fixture loads, missing current frames, missing `WebGlSceneView`, failed browser apply results, and missing snapshots before analysis.

## Semantic positive proof

`bundle://proof/SB05/browser-action-proof.json` proves a 1440x900 browser session loaded the shared-well fixture, applied frame 2, paused, stepped, sought first and last, captured a snapshot, and displayed 8 analysis findings with a live WebGL canvas.

## Anti-stub audit

`bundle://proof/SB05/transcripts/anti-stub-audit.txt` confirms no placeholder/stub markers in the SB05 page, route, CSS, or test source.

## Raw-note literal closure

- Joined simulation plus visualization belongs in Economy: implemented in the Economy components and Node host repos.
- WebGL is desktop/large-screen only: proof captured at `1440x900`, and no mobile optimization was added.
- Can load and apply at least one frame: browser proof applied frame 2 with 9 stages and 8 patches.
- Can pause, step, seek, snapshot, analyze: all actions are captured in `bundle://proof/SB05/browser-action-proof.json`.
