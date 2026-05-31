# SB03 - Components stage barriers and motion queue hardening

## Goal
Ensure action sequences can represent real scenario flows such as:

```text
move to target -> perform action -> show symbol/pose -> return home
```

without relying only on guessed durations.

## Required actions

1. Add generic stage barrier policy:
   - `none`
   - `wait-seconds`
   - `wait-for-active-motions`
   - `wait-for-object-motions`
   - `wait-for-render-idle`
   - `wait-for-event`
2. Extend stage runner diagnostics with current barrier type, wait target and unresolved blockers.
3. Add per-object motion queue proof for at least three motions on the same object.
4. Ensure `append` means sequential object queue, not competing active motions.
5. Ensure cancel/clear operations remove both active and queued motions.

## Acceptance criteria

- Sequential object actions execute in order.
- Stage runner does not advance while required object motions remain active/queued.
- Delayed stage completion is visible in diagnostics.
