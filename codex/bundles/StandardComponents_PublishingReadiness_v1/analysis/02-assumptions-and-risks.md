# Assumptions And Risks

## Assumptions

- The new pure repositories will receive polished output later; this bundle prepares and hardens source before transfer.
- WebGL and Canvas implementation work are out of scope and will be handled in separate preparation.
- Existing compatibility components may be retained temporarily only when they are migration aids with clear deprecation or alias policy.
- AppComponents in the main CanDoItAll repo should keep only complex app-specific surfaces after basic primitives are migrated or removed.

## Critical Path Risks

- SB02, SB03, SB04, and SB05 are critical foundations. If Tailwind policy, shared bases, duplicate migration, or sandbox taxonomy are wrong, every later visual hardening phase can produce misleading proof.
- Removing old AppComponents copies without behavior comparison could drop improvements that never made it into BaseLib.
- Visual styling refactors cannot be trusted without real browser screenshots at desktop and narrow widths.
- A sandbox that keeps Canvas and standard components in one undifferentiated catalog can hide missing standard-component coverage.

## Validation Risks

- The repo currently has no standard-component test project, so early proof may start as build plus Playwright visual proof until SB10 adds contract tests.
- Native browser controls such as `select` expose OS/browser styling limits; the proof plan must record what can and cannot be asserted visually.
- Overlay proof needs open-state capture for dialogs, help popovers, tooltips, context menus, notification toasts, and sticky footers; source-only checks are insufficient.
- Tailwind refactors can pass builds while breaking wrapping, width usage, or parent clipping; every affected component needs screenshot review questions answered in the execution report.

## Reopen Triggers

- Reopen SB02 if a later screenshot shows layout, wrapping, or alignment defects caused by a shared Tailwind utility or token decision.
- Reopen SB03 if a later component needs duplicated `Class`, `Style`, `AdditionalAttributes`, enum, service, or CSS-class composition logic.
- Reopen SB04 if old AppComponents behavior is discovered in production usage after a migration/deletion decision.
- Reopen SB05 if a later subbundle cannot capture Playwright proof because the sandbox lacks a focused route, scenario, or test hook.
- Reopen the owning visual-hardening subbundle when Playwright captures text overflow, clipping, dead space, insufficient width/height stretching, or inaccessible interactive state.
