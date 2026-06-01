# SB04 - Components runtime snapshot for browser state

## Goal

Expose a browser/runtime snapshot that Economy can attach to simulation snapshots.

## Tasks

- Add or harden a generic runtime snapshot DTO:
  - current frame index,
  - active stages,
  - queued stages,
  - active motions,
  - queued motions,
  - command journal tail,
  - stage barrier state,
  - last runtime errors/warnings.
- Keep it domain-neutral.
- Ensure snapshot size is bounded.

## Acceptance

- A test proves runtime snapshot can be exported from a fake/applied run.
- No Economy terms.
