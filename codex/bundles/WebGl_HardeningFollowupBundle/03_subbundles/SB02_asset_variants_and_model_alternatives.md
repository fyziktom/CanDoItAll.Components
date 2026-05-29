# SB02 - Asset variants and model alternatives

## Goal

Add user-provided GLB models as optional alternatives without replacing primitive low-poly defaults.

## Tasks

1. Discover all GLB/GLTF files.
2. Add asset metadata:
   - byte size,
   - quality tier,
   - model category,
   - recommended max instance count,
   - fallback asset.
3. Extend `WebGlAssetVariant` and `WebGlAssetDefinition` as needed.
4. Add runtime variant resolver:
   - explicit `object.Metadata["assetVariantId"]`,
   - scene/runtime quality profile,
   - fallback to primitive.
5. Update `WebGlSandboxAssetCatalogFactory`.
6. Add UI selector in sandbox:
   - Primitive
   - GLB mixed
   - GLB high detail
7. Verify fallback still works if GLB fails.

## Acceptance criteria

- Existing primitive village still works as default.
- User-provided GLB models appear as selectable alternatives.
- Proof snapshot includes active asset profile and variant stats.
- Heavy model profile can be disabled quickly.
