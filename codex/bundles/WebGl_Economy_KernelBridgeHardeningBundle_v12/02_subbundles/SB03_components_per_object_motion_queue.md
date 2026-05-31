# SB03 - Components Per-Object Motion Queue

## Problem

`queueMode=append` currently keeps multiple motions active, but it does not create a sequential queue. Multiple motions for the same object can fight each other in `advanceMotions`.

## Goal

Implement a generic per-object motion queue.

## Required behavior

- Default behavior remains replace-existing for simple commands.
- `queueMode=append` should enqueue after the last pending motion for the same object.
- Active motion per object should be single by default.
- Completion of one motion should start the next queued motion.
- Each queued motion should compute start position from the final state of the previous motion, not the old position at enqueue time.
- Cancellation can cancel:
  - one motion
  - all motions for object
  - whole queue
- Diagnostics:
  - active motion count
  - queued motion count
  - max queue length
  - cancelled motion count

## Tests

Add a test/proof where the same object receives two motions in order:

```text
A -> well
A -> home
```

Final position must be home and intermediate proof must show that both motions completed in order.
