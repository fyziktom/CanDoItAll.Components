# SB08 Source Assertions

- File snapshot store writes full payloads separately from a descriptor index file.
- Listing reads descriptors without loading every full snapshot payload after the index exists.
- Snapshot payload loads still validate deterministic hash and detect tampering.

