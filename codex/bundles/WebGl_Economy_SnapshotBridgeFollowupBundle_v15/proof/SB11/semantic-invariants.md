# Semantic invariants SB11

Status: Completed.

## Visual Mapping Contract Boundary

Invariant ID: SB11-visual-mapping-boundary

Abstraction-layer visual mapping types stay renderer-neutral. They may describe semantic node categories, action mappings, pose keys, symbol keys, anchor aliases, relationship categories, fallback policy, loading, and validation, but they must not depend on Components or renderer bridge packages.

## Shallow-Pass Trap

Simply splitting the original 314-line file would satisfy a size scan while leaving renderer-specific vocabulary or asset-path semantics in the abstraction layer. The validator and tests now assert the semantic boundary, not just file count.

## Adversarial Negative Proof

`VisualMappingContract_RoundTripsAndRejectsRendererSpecificKeys` injects a renderer path/file-name style pose key and asset id. `EconomyVisualMappingValidator` rejects both with `renderer-specific-visual-key`.

## Semantic Positive Proof

Fixture mappings for `shared-well` and `farmer-land` still load with `EconomyVisualMappingLoader.LoadStrict`, preserving the shared neutral contract and existing bridge consumers after the split.

## Anti-Stub Audit

`bundle://proof/SB11/source-assertions/anti-stub-scan.txt` records no TODO, NotImplementedException, NotSupportedException, stub, or placeholder markers in the split mapping files or updated tests.
