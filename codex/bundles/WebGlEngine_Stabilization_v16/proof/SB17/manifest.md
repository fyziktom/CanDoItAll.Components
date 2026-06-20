# Proof manifest - SB17

Status: completed

## Scope

Browser observer proof RC for `/run-playback`.

## Artifacts

- Browser proof JSON: `bundle://proof/SB17/browser/browser-observer-proof.json`
- Screenshot: `bundle://proof/SB17/browser/run-playback.png`
- Server logs: `bundle://proof/SB17/browser/server/`
- Proof runner: `repo://tools/webgllib/run-browser-observer-proof.cjs`
- PowerShell wrapper: `repo://scripts/webgl-engine/run-browser-observer-proof.ps1`
- Final changed-file hashes: `bundle://proof/SB22/changed-file-hashes.txt`

## Commands

- `powershell -ExecutionPolicy Bypass -File scripts\webgl-engine\run-browser-observer-proof.ps1 -NoBuild -TimeoutMs 120000`

## Result

- Browser observer proof passed.
- Assertions true: browser runtime valid, UI valid, observer proof valid, document hash match, scene content hash match, driver hash match, runtime idle, final positions compared, cancellation stopped runtime, console errors empty.
- Completed stages: `run.move.target`, `run.pose.restore`, `run.pose.work`, `run.return.anchor`, `run.symbol.hide`, `run.symbol.show`.

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Browser-loaded document hash | `/run-playback` proof bridge | observer proof validator | browser proof run | hash mismatch would set `observerProofValid=false` |
| Runtime idle result | WebGlLib runtime idle API | observer report | after playback/cancel flows | idle false or blockers fail proof assertions |
| Final object positions | WebGlLib proof snapshot | observer report | after browser playback | final position mismatch fails proof assertions |
