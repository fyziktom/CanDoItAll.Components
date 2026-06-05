# SB06 - Runtime idle and playback stop release-candidate proof

Close playback/pause as a freeze-grade browser proof.

Tasks:
- Verify immediate runtime stop occurs before C# task drain.
- Ensure stale callbacks after runtime stop are ignored and counted.
- Require `WaitForRuntimeIdleAsync` after stop.
- Ensure diagnostics prove: active motions zero, queued motions zero, queued stages zero,
  no active barrier, runtime stop generation advanced.
- Add long-window proof that nothing restarts after pause.

Required proof:
- Playwright proof for Play -> Pause -> wait -> diagnostics stable,
- negative test or failing-first proof for stale callback,
- screenshot and JSON diagnostics.

