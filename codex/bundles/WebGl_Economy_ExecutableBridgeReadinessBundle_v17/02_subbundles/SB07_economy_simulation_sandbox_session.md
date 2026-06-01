# SB07 — Economy SimulationSandbox session model

## Goal

Prepare the Economy-side sandbox for a real connected simulation + visualization UI without building the final UI yet.

## Required concepts

Add a session model/service:

- `EconomySimulationSandboxSession`
- `IEconomySimulationSandboxSessionService`
- session state: input, backend, visual frames, run document, snapshots, diagnostics, current step/frame/stage
- operations: load, project, step, seek, pause, resume, snapshot, analyze
- no direct dependency on Blazor components
- no browser-only assumptions

## Required behavior

A session should support:

- simple account backend;
- future ledger backend via registry;
- creating snapshot for current step;
- returning current visual frame and run frame;
- returning analysis summary.

## Closure proof

- headless session test with shared-resource fixture;
- headless session test with finite-resource/farmer-land fixture.
