# SB08 Asset Lifecycle Report

Validation command:

- `npm run webgllib:test-resource-ownership`

Covered cases:

- tinted instance keeps shared texture;
- template and instance ownership are separate;
- duplicate disposal is deduped;
- pending template promise disposal records diagnostics;
- state-local template cache disposes resources and counters.

The command passed in `proof/SB18/transcripts/rc-validation-transcript.txt`.
