# Proof manifest - SB03

Status: completed

Required artifacts:
- `proof/SB03/transcripts/command-lifecycle-tests.txt` - passed; covers C# command lifecycle result deserialization and existing result round-trips.
- `proof/SB03/browser/staged-batch-settled-proof.json` - passed; normal staged apply returns `scheduled`/`settled=false`, settled proof apply returns `settled`/`settled=true` with no idle blockers.

Additional artifacts:
- `proof/SB03/browser/staged-batch-settled-proof.cjs`
- `proof/SB03/browser/staged-batch-settled-proof.png`
- `proof/SB03/transcripts/staged-batch-settled-proof-playwright.txt`
- `proof/SB03/transcripts/webgllib-tests.txt`
- `proof/SB03/transcripts/webgllib-audit-scene-runtime.txt`
- `proof/SB03/transcripts/webgllib-audit-scene-runtime-imports.txt`
- `proof/SB03/transcripts/webgllib-audit-stage-runner.txt`
- `proof/SB03/transcripts/command-lifecycle-js-check.txt`
- `proof/SB03/transcripts/source-assertion-command-lifecycle-scan.txt`
- `proof/SB03/transcripts/anti-stub-audit.txt`
- `proof/SB03/transcripts/changed-file-hashes.txt`
- `proof/SB03/refactor-gate-review.md`

Production behavior artifact matrix:
- `WebGlSceneCommandResult` exposes `LifecycleState` and `Settled` typed fields; JS result metadata and diagnostics mirror them.
- `applyCommandBatch` derives `scheduled` versus `settled` from SB02 runtime idle blockers.
- `applyCommandBatchAndWait` and `WebGlSceneView.ApplyCommandBatchAndWaitAsync` provide proof paths that wait for runtime idle before reporting settled.
- Motion enqueue results distinguish active and queued/scheduled work; failed commands retain `failed`.
