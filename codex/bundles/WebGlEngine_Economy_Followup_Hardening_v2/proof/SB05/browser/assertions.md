# SB05 Browser Assertions

Route: `http://localhost:5099/tycoon-village`
Viewport: 1600x1000 desktop

Actions:

- Loaded `/tycoon-village` and waited for runtime status `ready`.
- Clicked `Bad link strict`.
- Clicked `Bad link warn`.

Assertions:

- Strict bad-link patch returned `statusText=failed`, `modeText=strict`, `classificationText=mixed-incremental`, `errors=1;warnings=0`.
- Strict bad-link patch left `agent.helper` at `{-8.2, 0, 0.6}` and added no `proof.*` links.
- Warning bad-link patch returned `statusText=success`, `modeText=permissive-invalid-links`, `classificationText=mixed-incremental`, `errors=0;warnings=1`.
- Warning bad-link patch moved `agent.helper` to `{-7.1, 0, 1.2}`, added `proof.good-link.warn`, skipped `proof.bad-link.warn`, and reported `affectedObjectIds=agent.helper`, `affectedLinkIds=proof.good-link.warn`, `skippedLinkIds=proof.bad-link.warn`.
- Console log reported 0 errors and 0 warnings.

Artifacts:

- `bundle://proof/SB05/transcripts/browser-initial-runtime-state.json`
- `bundle://proof/SB05/transcripts/browser-strict-runtime-assertions.json`
- `bundle://proof/SB05/transcripts/browser-warning-runtime-assertions.json`
- `bundle://proof/SB05/browser/tycoon-village-patch-transaction-proof.png`
- `bundle://proof/SB05/browser/tycoon-village-console.txt`
