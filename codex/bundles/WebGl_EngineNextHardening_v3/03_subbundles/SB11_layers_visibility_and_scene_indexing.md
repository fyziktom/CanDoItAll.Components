# SB11 — Layers, Visibility, and Scene Indexing

## Goal

Make scenes scalable for tycoon-like visualizations by using layers and indexes.

## Current issue

`WebGlSceneModel` has layers, but runtime primarily renders all objects and links. Future scenarios need toggles such as people/buildings/resources/flows/symbols/debug.

## Tasks

1. Implement layer visibility:
   - `WebGlSceneLayer.IsVisible` or metadata flag if the model already has equivalent.
   - object belongs to one or more layers.
   - hidden layers skip object, link, symbol, and hit-test registration.
   - proof snapshot includes visible/hidden counts.

2. Add scene index helpers:
   - object by id
   - links by source/target
   - symbols by owner
   - layers by id
   - asset usage counts
   - tag queries

3. Avoid rebuilding entire scene for simple visibility toggles when possible.
   - layer toggle should hide/show groups when possible.
   - full rebuild only when asset/geometry changes.

4. Add sandbox controls:
   - show/hide buildings
   - show/hide people
   - show/hide props
   - show/hide symbols
   - show/hide model diagnostics/debug bounds

## Done criteria

- Layer visibility works.
- Hit testing respects hidden layers.
- Proof snapshot exposes visibility counts.
