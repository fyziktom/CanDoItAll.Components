# SB07 - Economy strict visual mapping completion

## Goal

Reduce permissive fallback use for the first real browser smoke.

## Tasks

- Complete fixture visual mappings for:
  - admin/write pose,
  - risk/warning symbol,
  - rule/fee/tax symbol,
  - resource transfer visual,
  - relationship/conflict pulse.
- Add strict mapping profile that disables no-op/fallback where expected.
- Keep fallback modes available for diagnostics, but not for the strict browser smoke.

## Acceptance

- At least one probe runs with no fallback object and no no-op pose/symbol fallbacks.
- Any fallback use is explicit, visible, and treated as a warning/error according to options.
