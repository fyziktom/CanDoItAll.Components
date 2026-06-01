# SB02 - Components runtime stage barrier hardening

Codex must harden:

- barrier timeout diagnostics,
- cancellation behavior,
- wait-for-object-motions behavior when object id is missing,
- wait-for-render-idle behavior when symbols animate forever,
- wait-for-event behavior so manual-step cannot leak across unrelated batches.

Add tests or audit fixtures for:

- stage A motion -> barrier waits for object motion -> stage B pose patch,
- stage B does not execute until A motion is complete,
- cancel clears queue, active barrier and journal state,
- command journal remains bounded.
