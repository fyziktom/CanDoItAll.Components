# SB09 - Economy snapshot runtime state attachment

Codex must harden snapshot visual/runtime attachment.

Add fields or metadata for:

- current playback command,
- active motion ids,
- queued motion ids,
- active barrier policy,
- active barrier blockers,
- command journal tail,
- render diagnostics summary,
- selected object/action ids if supplied by UI.

The deterministic snapshot hash should include data-state and optionally visual-runtime state as separate hashes.
