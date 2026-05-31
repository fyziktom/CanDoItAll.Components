# SB15 — Performance and scalability proofs

## Goal
Catch bottlenecks before building the UI.

## Required probes
- 250 actors
- 500 stores
- 1000 scheduled events
- 1000 visual actions
- 500 staged WebGL commands
- 100 snapshots

## Metrics
- simulation materialization time
- visual mapping time
- bridge projection time
- snapshot export/import time
- command batch normalization time
- average/peak WebGL frame time if browser proof is used

## Policy
Large-screen only: 1440x900 or larger.
