# Genericity Checks

## Forbidden in generic source

The following example terms must not appear in generic engine files:

```text
shared-well
near-household
far-household
well-keeper
waterAccessFairness
farmer.small
farmer.expander
oligarchy
landConcentrationHhi
```

Allowed locations:

- test fixtures
- example input packs
- scenario factory/test code
- docs explaining probes

## Required bridge checks

- `Components` does not reference `Economy`.
- Economy abstractions do not reference WebGL or Components.
- Only future bridge project may reference both `Economy.Visualization` and `Components.WebGlRunLib`.
- Visual mapping uses neutral keys, not direct GLB asset IDs, until bridge layer.
