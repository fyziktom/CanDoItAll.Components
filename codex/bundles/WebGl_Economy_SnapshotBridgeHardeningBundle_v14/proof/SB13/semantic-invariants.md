# SB13 Semantic Invariants

Status: Completed

## Shallow-pass trap

A timing file alone is not enough. SB13 must prove the probe exercises the required sizes and stages: 100 actors, 1000 resource stores, 300 visual actions, more than 1000 visual links, scenario normalization, transition materialization, visual frame mapping, WebGL bridge projection, and command batch generation.

## Adversarial negative proof

The probe would fail the semantic gate if it only materialized frames, if it omitted visual mapping or WebGL projection, if it produced fewer than 300 visual actions, or if the bridge resolved nodes/links by repeated list scans instead of dictionary lookups. Source assertions point to `NodeObjectIds.TryGetValue`, dictionary action merging in `SelectFrameActions`, and cached `ObjectBindings`.

## Semantic positive proof

`economy-simulation-performance-proof.json` records all required stages. The command-batch stage reports 100 actors, 1000 resource stores, 1200 links, 300 visual actions, 300 stages, 300 commands, and 1100 node-object mappings. The mapped-frame projection also reports 1200 objects, 1001 links, and dictionary-backed node-object mapping count.

## Anti-stub audit

The anti-stub scan found no TODO, `NotImplementedException`, skip, placeholder, stub, or fake markers in the SB13 test and bridge files. The targeted test and Components runtime audit both executed and wrote JSON artifacts.
