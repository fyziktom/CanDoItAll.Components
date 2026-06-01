# SB04 — Components executable run document contract

## Goal

Make `WebGlRunDocument` executable by a generic runner, not only a DTO.

## Required additions

Add or harden a generic executor/controller that can:

- accept `WebGlRunDocument`;
- load/apply `InitialScene`;
- apply a selected frame;
- apply stages as command batches;
- step, pause, resume, seek;
- produce runtime diagnostics;
- expose current frame/stage/action IDs;
- export a generic runtime snapshot/proof snapshot.

This must remain generic and must not know Economy.

## Required tests

- create a minimal generic `WebGlRunDocument` with initial scene and two stages;
- execute first frame headlessly or through existing runtime abstraction;
- verify command batches are generated/applied;
- verify current frame/stage state is observable.

## Note

This can be a headless C# contract proof first. Browser proof can come later when Economy sandbox UI is built.
