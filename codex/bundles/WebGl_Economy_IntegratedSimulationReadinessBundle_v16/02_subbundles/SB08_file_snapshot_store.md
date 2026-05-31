# SB08 - File-based snapshot store

## Goal
Support realistic pause/export/analyze workflows.

## Required actions

1. Add file-based snapshot store under Economy abstractions or a small persistence package.
2. Store descriptors separately from full snapshot JSON.
3. Use safe relative paths and deterministic filenames.
4. Support list by run id, scenario id and step range.
5. Include integrity verification on load.

## Acceptance criteria

- A snapshot can be saved to disk and reloaded later.
- The descriptor list can be shown without loading every full snapshot.
