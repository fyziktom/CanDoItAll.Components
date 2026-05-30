import { primitiveKinds, resolveActiveAssetProfile, resolveFiniteNumber, resolveString } from "./02-webgl-scene-core.js";
import { createPrimitiveVisual } from "./09-webgl-scene-primitives.js";
import { buildModelInstance, loadModelAsset } from "./16-webgl-scene-models.js";
import { disposeSceneObjectTree } from "./17-webgl-scene-resources.js";

export function buildAssetLookup(catalog) {
    const lookup = new Map();
    for (const asset of catalog?.assets || []) {
        if (asset?.id) {
            lookup.set(asset.id, asset);
        }
    }

    return lookup;
}

export function resolveAssetDefinition(state, assetId) {
    if (!assetId) {
        return resolveFallbackAsset(state, null);
    }

    const asset = state.assetLookup.get(assetId);
    if (asset) {
        return asset;
    }

    state.diagnostics.missingAssetIds.add(assetId);
    return resolveFallbackAsset(state, assetId);
}

function resolveFallbackAsset(state, missingAssetId) {
    const fallbackId = state.sceneModel.assetCatalog?.defaultFallbackAssetId || "";
    const fallback = fallbackId ? state.assetLookup.get(fallbackId) : null;
    if (fallback) {
        return fallback;
    }

    if (fallbackId) {
        state.diagnostics.missingFallbackAssetIds.add(fallbackId);
    }

    return {
        id: missingAssetId ? `${missingAssetId}.fallback` : "asset.primitive.fallback",
        kind: "primitive",
        format: "primitive",
        primitiveKind: primitiveKinds.box,
        displayName: "Fallback box",
        color: "#94a3b8",
        supportsTint: true,
        boundsHint: { x: 1, y: 1, z: 1 },
        importOptions: {}
    };
}

export function resolveAssetForObject(state, sceneObject, options = {}) {
    const baseAsset = resolveAssetDefinition(state, sceneObject?.assetId || "");
    const profile = resolveActiveAssetProfile(state);
    const explicitVariantId = sceneObject?.metadata?.assetVariantId || sceneObject?.metadata?.preferredAssetVariantId || "";
    const variant = findVariant(baseAsset, explicitVariantId, profile);
    const resolved = variant
        ? mergeVariant(state.sceneModel.assetCatalog, baseAsset, variant, profile)
        : mergeBaseAsset(state.sceneModel.assetCatalog, baseAsset, profile);

    if (profile === "primitive" && resolved.format !== "primitive") {
        return resolvePrimitiveFallback(state, baseAsset, resolved);
    }

    if (options.symbol && resolved.format !== "primitive" && profile === "primitive") {
        return resolvePrimitiveFallback(state, baseAsset, resolved);
    }

    return resolved;
}

export function syncAssetVisual(state, sceneObject, group, options = {}) {
    const asset = resolveAssetForObject(state, sceneObject, options);
    const objectId = sceneObject?.id || asset.id;

    if (asset.format !== "glb" && asset.format !== "gltf") {
        const primitive = createPrimitiveVisual(asset, sceneObject, options);
        primitive.name = `${objectId}-primitive`;
        group.add(primitive);
        state.diagnostics.primitiveInstanceIds.add(objectId);
        addPerformanceHint(state, asset);
        return primitive;
    }

    const fallback = createPrimitiveVisual(resolvePrimitiveFallback(state, asset, asset), sceneObject, options);
    fallback.name = `${objectId}-fallback`;
    group.add(fallback);
    state.diagnostics.fallbackObjectIds.add(objectId);
    state.diagnostics.primitiveInstanceIds.add(`${objectId}:fallback`);

    loadModelAsset(state, asset)
        .then(assetTemplate => {
            if (group.userData.disposed) {
                return;
            }

            const instance = buildModelInstance(assetTemplate, sceneObject, asset, options, state.diagnostics);
            group.add(instance);
            if (fallback.parent === group) {
                group.remove(fallback);
                disposeSceneObjectTree(fallback);
            }

            state.diagnostics.fallbackObjectIds.delete(objectId);
            state.diagnostics.primitiveInstanceIds.delete(`${objectId}:fallback`);
            state.diagnostics.modelInstanceIds.add(objectId);
            addPerformanceHint(state, asset);
            state.scheduleRender("asset-loaded");
        })
        .catch(() => {
            state.scheduleRender("asset-fallback");
        });

    return fallback;
}

function findVariant(asset, explicitVariantId, profile) {
    const variants = Array.isArray(asset?.variants) ? asset.variants : [];
    if (explicitVariantId) {
        const explicit = variants.find(variant => same(variant.id, explicitVariantId));
        if (explicit) {
            return explicit;
        }
    }

    const exact = variants.find(variant => same(variant.qualityTier, profile));
    if (exact) {
        return exact;
    }

    if (profile === "primitive") {
        return variants.find(variant => same(variant.format, "primitive") || same(variant.qualityTier, "primitive"));
    }

    if (profile === "model-high") {
        return variants.find(variant => same(variant.qualityTier, "model-medium")) ||
            variants.find(variant => same(variant.qualityTier, "model-low"));
    }

    return variants.find(variant => same(variant.qualityTier, "model-low"));
}

function mergeBaseAsset(catalog, asset, profile) {
    return {
        ...asset,
        variantId: "",
        qualityTier: resolveString(asset?.qualityTier, profile),
        performanceHint: asset?.performanceHint || {},
        importOptions: resolveImportOptions(catalog, asset, null),
        requestedProfile: profile
    };
}

function mergeVariant(catalog, asset, variant, profile) {
    return {
        ...asset,
        id: asset.id,
        variantId: variant.id || "",
        uri: variant.uri || asset.uri || "",
        format: variant.format || asset.format || "primitive",
        primitiveKind: variant.primitiveKind || asset.primitiveKind || primitiveKinds.box,
        fallbackAssetId: variant.fallbackAssetId || asset.fallbackAssetId || "",
        color: variant.color || asset.color || "#94a3b8",
        qualityTier: variant.qualityTier || asset.qualityTier || profile,
        scale: variant.scale || { x: 1, y: 1, z: 1 },
        importOptions: resolveImportOptions(catalog, asset, variant),
        performanceHint: variant.performanceHint || asset.performanceHint || {},
        requestedProfile: profile
    };
}

function resolvePrimitiveFallback(state, baseAsset, resolvedAsset) {
    const fallbackId = resolvedAsset?.fallbackAssetId || baseAsset?.fallbackAssetId || state.sceneModel.assetCatalog?.defaultFallbackAssetId || "";
    const fallback = fallbackId ? state.assetLookup.get(fallbackId) : null;
    if (fallback) {
        return {
            ...fallback,
            variantId: resolvedAsset?.variantId || "",
            requestedProfile: resolveActiveAssetProfile(state)
        };
    }

    if (fallbackId) {
        state.diagnostics.missingFallbackAssetIds.add(fallbackId);
    }

    return {
        id: baseAsset?.id ? `${baseAsset.id}.primitive-fallback` : "asset.primitive.fallback",
        kind: "primitive",
        format: "primitive",
        primitiveKind: baseAsset?.primitiveKind || primitiveKinds.box,
        displayName: "Fallback primitive",
        color: baseAsset?.color || "#94a3b8",
        supportsTint: true,
        boundsHint: baseAsset?.boundsHint || { x: 1, y: 1, z: 1 },
        importOptions: {},
        requestedProfile: resolveActiveAssetProfile(state),
        performanceHint: {
            qualityTier: "primitive"
        }
    };
}

function addPerformanceHint(state, asset) {
    const hint = asset?.performanceHint || {};
    state.diagnostics.estimatedTriangleCount += Math.max(0, resolveFiniteNumber(hint.triangleCountHint, 0));
    state.diagnostics.estimatedVertexCount += Math.max(0, resolveFiniteNumber(hint.vertexCountHint, 0));
}

function same(left, right) {
    return String(left || "").toLowerCase() === String(right || "").toLowerCase();
}

function resolveImportOptions(catalog, asset, variant) {
    return {
        ...resolveRecipeOptions(catalog, asset?.importRecipeId),
        ...(asset?.importOptions || {}),
        ...resolveRecipeOptions(catalog, variant?.importRecipeId),
        ...(variant?.importOptions || {})
    };
}

function resolveRecipeOptions(catalog, recipeId) {
    if (!catalog || !recipeId) {
        return {};
    }

    const recipe = (catalog.modelImportRecipes || []).find(item => same(item.id, recipeId));
    return recipe?.options || {};
}
