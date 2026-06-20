# Normalized requirements

- R01: Resolve F01 (RunPlayback pause is not a real runtime stop) through SB01,SB02,SB03; proof must include failing-first where severity is P0/P1.
- R02: Resolve F02 (Playback UI event handler can monopolize component command flow) through SB03; proof must include failing-first where severity is P0/P1.
- R03: Resolve F03 (No public stop-all WebGL runtime operation) through SB02; proof must include failing-first where severity is P0/P1.
- R04: Resolve F04 (MotionCompleted callback can overwrite paused status) through SB03; proof must include failing-first where severity is P0/P1.
- R05: Resolve F05 (ApplyPlaybackAsync lacks playback transaction/cancellation summary) through SB05; proof must include failing-first where severity is P0/P1.
- R06: Resolve F06 (Economy UI deterministic replay is O(n) per step and O(n²) across long playback) through SB06; proof must include failing-first where severity is P0/P1.
- R07: Resolve F07 (Scenario API is improved but still path-biased) through SB07; proof must include failing-first where severity is P0/P1.
- R08: Resolve F08 (Scenario manifest locks only part of pack semantics) through SB08; proof must include failing-first where severity is P0/P1.
- R09: Resolve F09 (Proof hygiene still needs machine enforcement) through SB10,SB12; proof must include failing-first where severity is P0/P1.
- R10: Resolve F10 (Large simulation performance budgets are implicit) through SB09; proof must include failing-first where severity is P0/P1.
- R11: Resolve F11 (WebGlRun runner state lacks first-class playback lifecycle) through SB04; proof must include failing-first where severity is P0/P1.
- R12: Resolve F12 (Documentation needs a user-facing playback troubleshooting section) through SB11; proof must include failing-first where severity is P0/P1.
- R13: Preserve generic Components boundary; no Economy-specific types or terms in WebGlLib/WebGlRunLib public contracts.
- R14: Final bundle validation must reject empty proof transcripts and screenshot-only browser proof.
