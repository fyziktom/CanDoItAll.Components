import {
    THREE,
    normalizeColor,
    resolveFiniteNumber,
    resolveObjectSize
} from "./02-webgl-scene-core.js";
import { GLTFLoader } from "../../../vendor/GLTFLoader.js";
import { clone as cloneSkeleton } from "../../../vendor/utils/SkeletonUtils.js";

const modelLoader = new GLTFLoader();

export function loadModelAsset(state, asset) {
    const uri = buildAbsoluteUri(asset?.uri);
    if (!uri) {
        return Promise.reject(new Error(`Asset '${asset?.id || ""}' has no URI.`));
    }

    if (state.assetCache.has(uri)) {
        return state.assetCache.get(uri);
    }

    const promise = modelLoader.loadAsync(uri)
        .then(gltf => buildLoadedAssetTemplate(state, gltf, asset, uri))
        .catch(error => {
            state.assetCache.delete(uri);
            state.diagnostics.missingAssetIds.add(asset.id);
            state.diagnostics.failedAssetUris.add(uri);
            state.diagnostics.lastError = error?.message || String(error);
            throw error;
        });

    state.assetCache.set(uri, promise);
    return promise;
}

export function buildModelInstance(assetTemplate, sceneObject, asset, options = {}) {
    const instance = assetTemplate.hasSkinnedMesh ? cloneSkeleton(assetTemplate.template) : assetTemplate.template.clone(true);
    const size = resolveObjectSize(sceneObject);
    const fit = options.fit || 0.92;
    const scale = Math.min(
        (size.x * fit) / Math.max(assetTemplate.size.x, 0.01),
        (size.y * fit) / Math.max(assetTemplate.size.y, 0.01),
        (size.z * fit) / Math.max(assetTemplate.size.z, 0.01));
    const defaultScale = Math.max(0.01, resolveFiniteNumber(asset.defaultScale, 1));
    const variantScale = resolveFiniteNumber(asset.scale?.x, 1);
    const resolvedScale = scale * defaultScale * variantScale;
    instance.scale.setScalar(resolvedScale);
    instance.position.set(
        -assetTemplate.center.x * resolvedScale,
        -(assetTemplate.min.y * resolvedScale),
        -assetTemplate.center.z * resolvedScale);
    markModelInstance(instance, sceneObject?.color || asset?.color, asset?.supportsTint !== false);
    return instance;
}

function buildAbsoluteUri(uri) {
    if (!uri) {
        return "";
    }

    try {
        return new URL(uri, document.baseURI).href;
    } catch {
        return uri;
    }
}

function buildLoadedAssetTemplate(state, gltf, asset, uri) {
    const template = gltf.scene || gltf.scenes?.[0];
    if (!template) {
        throw new Error(`Asset '${asset.id}' did not contain a scene.`);
    }

    const bounds = new THREE.Box3().setFromObject(template);
    const size = bounds.getSize(new THREE.Vector3());
    const center = bounds.getCenter(new THREE.Vector3());
    let hasSkinnedMesh = false;
    template.traverse(child => {
        hasSkinnedMesh ||= !!child.isSkinnedMesh;
        child.userData = { ...child.userData, skipDispose: true };
    });

    const loadedId = asset.variantId ? `${asset.id}:${asset.variantId}` : asset.id;
    state.diagnostics.loadedAssetIds.add(loadedId);
    state.diagnostics.missingAssetIds.delete(asset.id);
    rememberLargestAsset(state, loadedId, asset?.performanceHint?.byteSizeHint || 0, uri);
    return { template, size, center, min: bounds.min.clone(), hasSkinnedMesh };
}

function markModelInstance(instance, tintColor, supportsTint) {
    instance.traverse(child => {
        child.frustumCulled = false;
        child.userData = { ...child.userData, skipDispose: true };
        if (!child.isMesh || !supportsTint || !tintColor) {
            return;
        }

        const materials = Array.isArray(child.material) ? child.material : [child.material];
        child.material = Array.isArray(child.material) ? materials.map(material => material.clone()) : child.material?.clone?.();
        for (const material of Array.isArray(child.material) ? child.material : [child.material]) {
            material?.color?.lerp?.(normalizeColor(tintColor), 0.38);
            if (material) {
                material.needsUpdate = true;
            }
        }
    });
}

function rememberLargestAsset(state, assetId, byteSize, uri) {
    const size = Math.max(0, resolveFiniteNumber(byteSize, 0));
    if (size >= (state.diagnostics.largestLoadedAssetBytes || 0)) {
        state.diagnostics.largestLoadedAssetBytes = size;
        state.diagnostics.largestLoadedAssetId = assetId || uri || "";
    }
}
