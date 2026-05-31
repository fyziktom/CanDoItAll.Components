# SB13 - Headless bridge end-to-end proof

## Goal
Prove joined simulation + visualization without building the final UI demo.

## Required test flow

```text
load experiment input pack
validate strict inputs
run selected backend
produce frames/deltas
map visual frames/actions
project WebGlRunDocument
validate initial scene
validate stage patches/motions
compile representative frame command batches
verify source traceability
build snapshot with visual state
export/import snapshot
analyze snapshot
```

## Acceptance criteria

- No browser needed.
- No final demo UI needed.
- The run document is actually executable by WebGlRunLib concepts.
