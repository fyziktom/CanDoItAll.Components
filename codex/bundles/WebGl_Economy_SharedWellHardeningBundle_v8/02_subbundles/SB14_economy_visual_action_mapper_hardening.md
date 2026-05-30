# SB14 — Economy: visual action mapper hardening

## Problems
Visual actions may duplicate sequence children as top-level actions and may sort events by ID rather than timing.

## Required work
- Ensure `Sequence` contains steps, but child steps are not also top-level unless explicitly marked as expanded debug output.
- Order actions by simulation timing.
- Include target place/location binding in action metadata.
- Add action diagnostics:
  - unresolved subject;
  - unresolved target;
  - missing location;
  - missing symbol/pose category.
- Add tests for shared-well:
  - actor moves to well;
  - actor uses resource;
  - actor returns home;
  - admin-writing action appears for rule/admin event;
  - action order is deterministic.
