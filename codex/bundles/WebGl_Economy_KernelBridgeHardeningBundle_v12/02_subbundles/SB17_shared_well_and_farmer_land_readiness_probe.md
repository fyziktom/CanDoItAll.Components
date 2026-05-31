# SB17 - Shared-Well and Farmer-Land Readiness Probe

## Goal

Do not build a demo. Build a readiness probe that proves both examples can pass through the same generic pipeline.

## Required assertion flow

For shared-well:

```text
input pack -> load -> validate hashes -> apply placement/parameters -> normalize -> compile events -> materialize frames -> map visual actions
```

For farmer-land:

```text
input pack -> load -> validate hashes -> apply placement/parameters -> normalize -> compile events -> materialize frames -> evaluate concentration metric/invariant
```

## Genericity assertions

- no scenario-id branching in core transition engine
- no resource-id branching in visual mapper
- no actor-id branching in event handler registry
- visual action kinds are generic
- all example-specific data comes from JSON fixtures or scenario factories

## Output

A readiness report artifact:

```text
artifacts/economy/readiness/shared-well-and-farmer-land-readiness.json
```
