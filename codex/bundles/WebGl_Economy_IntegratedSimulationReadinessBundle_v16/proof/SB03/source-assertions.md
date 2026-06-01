# SB03 Source Assertions

- Stage barriers support canonical policies for none, wait-seconds, active motions, object motions, render-idle, and event/manual-step gating.
- Motion-aware scheduling considers queued and active motions before reporting idle work.
- WebGlRun stage metadata carries barrier policy, object ids, and event ids through frame application.
- Stage runner proof includes adversarial event-barrier behavior where unresolved events do not spin the render loop.

