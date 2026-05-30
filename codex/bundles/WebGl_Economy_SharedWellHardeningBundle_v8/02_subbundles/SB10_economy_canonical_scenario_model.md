# SB10 — Economy: canonical scenario model

## Problem
Scenario definitions contain migration aliases:
- Entities vs Actors;
- Places vs Locations;
- Stores vs InitialStores;
- EventTemplates vs ScheduledEvents.

## Required work
- Add `SimulationScenarioDefinitionNormalizer`.
- Choose canonical internal model:
  - Actors;
  - Locations;
  - InitialStores;
  - ScheduledEvents.
- Keep aliases only for import/export compatibility.
- Validators should validate canonical normalized form.
- Add tests for alias conflict detection and normalization.

## Do not
Do not reference Components/WebGL.
