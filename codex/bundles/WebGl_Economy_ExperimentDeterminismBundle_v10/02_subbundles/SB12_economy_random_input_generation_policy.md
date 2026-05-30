# SB12 — Economy: random input generation policy

## Problem

Randomization is useful for experimental treatments, but not inside a run.

## Tasks

1. Add `ISimulationInputGenerator`.
2. Add generator request/result DTOs.
3. Generated placement/parameters must include:
   - generator id/version
   - seed
   - generated at
   - output hash
4. Generator writes JSON files.
5. Simulation run accepts only saved generated files.

## Done criteria

- A random placement can be generated once and then replayed deterministically from JSON.
- Tests prove two runs from the same JSON produce identical hashes.
