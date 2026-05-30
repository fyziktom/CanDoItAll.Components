import {
    THREE,
    normalizeColor,
    resolveFiniteNumber,
    resolveObjectSize
} from "./02-webgl-scene-core.js";
import { GLTFLoader } from "../../../vendor/GLTFLoader.js";
import { clone as cloneSkeleton } from "../../../vendor/utils/SkeletonUtils.js";
import {
    addDebugBounds,
    diagnoseModelTemplate,
    normalizeImportedModelMaterials,
    normalizeModelImportOptions
} from "./18-webgl-scene-model-diagnostics.js";
import {
    markInstanceResource,
    markOwnedMaterial,
    markSharedTemplateResource
} from "./17-webgl-scene-resources.js";
import {
    deleteCachedTemplate,
    getOrLoadTemplate
} from "./21-webgl-scene-asset-cache.js";

const modelLoader = new GLTFLoader();

export function loadModelAsset(state, asset) {
    const uri = buildAbsoluteUri(asset?.uri);
    if (!uri) {
        return Promise.reject(new Error(`Asset '${asset?.id || ""}' has no URI.`));
    }

    return getOrLoadTemplate(state, uri, () => modelLoader.loadAsync(uri)
        .then(gltf => buildLoadedAssetTemplate(state, gltf, asset, uri))
        .catch(error => {
            deleteCachedTemplate(state, uri);
            state.diagnostics.missingAssetIds.add(asset.id);
            state.diagnostics.failedAssetUris.add(uri);
            state.diagnostics.lastError = error?.message || String(error);
            throw error;
        }));
}

export function buildModelInstance(assetTemplate, sceneObject, asset, options = {}, diagnostics = null) {
    const model = assetTemplate.hasSkinnedMesh ? cloneSkeleton(assetTemplate.template) : assetTemplate.template.clone(true);
    markInstanceResource(model, { ownsGeometry: false, ownsMaterial: false });
    const size = resolveObjectSize(sceneObject);
    const importOptions = normalizeModelImportOptions(asset);
    const defaultScale = Math.max(0.01, resolveFiniteNumber(asset.defaultScale, 1));
    const variantScale = resolveFiniteNumber(asset.scale?.x, 1);
    const fitScale = resolveFitScale(assetTemplate, size, options.fit || 0.92, importOptions);
    const resolvedScale = fitScale * defaultScale * variantScale * importOptions.unitScale;
    model.scale.setScalar(resolvedScale);
    model.rotation.set(importOptions.rotationOffset.x, importOptions.rotationOffset.y, importOptions.rotationOffset.z);
    model.position.copy(resolveModelOffset(assetTemplate, resolvedScale, importOptions));
    model.position.add(new THREE.Vector3(importOptions.positionOffset.x, importOptions.positionOffset.y, importOptions.positionOffset.z));
    markModelInstance(model, sceneObject?.color || asset?.color, asset?.supportsTint !== false && !importOptions.disableTint, diagnostics);
    normalizeImportedModelMaterials(model, importOptions);

    const wrapper = new THREE.Group();
    wrapper.name = `${sceneObject?.id || asset?.id || "model"}-model-wrapper`;
    wrapper.add(model);
    if (importOptions.debugBounds) {
        addDebugBounds(wrapper, model);
    }

    return wrapper;
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
    const loadedId = asset.variantId ? `${asset.id}:${asset.variantId}` : asset.id;
    if (!template) {
        state.diagnostics.modelDiagnostics.set(loadedId, diagnoseModelTemplate({ asset, uri, template: null, camera: state.camera }));
        throw new Error(`Asset '${asset.id}' did not contain a scene.`);
    }

    const diagnostics = diagnoseModelTemplate({ asset, uri, template, camera: state.camera });
    state.diagnostics.modelDiagnostics.set(loadedId, diagnostics);
    if (diagnostics.errors.length > 0) {
        state.diagnostics.lastError = diagnostics.errors.join(" ");
        throw new Error(`Asset '${asset.id}' failed model diagnostics: ${diagnostics.errors.join(" ")}`);
    }

    const bounds = new THREE.Box3().setFromObject(template);
    const size = bounds.getSize(new THREE.Vector3());
    const center = bounds.getCenter(new THREE.Vector3());
    let hasSkinnedMesh = false;
    template.traverse(child => {
        hasSkinnedMesh ||= !!child.isSkinnedMesh;
    });
    markSharedTemplateResource(template);

    state.diagnostics.loadedAssetIds.add(loadedId);
    state.diagnostics.missingAssetIds.delete(asset.id);
    rememberLargestAsset(state, loadedId, asset?.performanceHint?.byteSizeHint || 0, uri);
    return { template, size, center, min: bounds.min.clone(), hasSkinnedMesh };
}

function markModelInstance(instance, tintColor, supportsTint, diagnostics = null) {
    instance.traverse(child => {
        child.frustumCulled = false;
        if (!child.isMesh || !supportsTint || !tintColor) {
            return;
        }

        const materials = Array.isArray(child.material) ? child.material : [child.material];
        child.material = Array.isArray(child.material) ? materials.map(material => material.clone()) : child.material?.clone?.();
        if (diagnostics) {
            diagnostics.materialCloneCount = (diagnostics.materialCloneCount || 0) + materials.filter(Boolean).length;
        }
        markOwnedMaterial(child.material);
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

function resolveFitScale(assetTemplate, objectSize, fit, importOptions) {
    if (importOptions.fitMode === "original-scale") {
        return 1;
    }

    if (importOptions.fitMode === "fixed-scale") {
        return importOptions.fixedScale;
    }

    return Math.min(
        (objectSize.x * fit) / Math.max(assetTemplate.size.x, 0.01),
        (objectSize.y * fit) / Math.max(assetTemplate.size.y, 0.01),
        (objectSize.z * fit) / Math.max(assetTemplate.size.z, 0.01));
}

function resolveModelOffset(assetTemplate, scale, importOptions) {
    switch (importOptions.centerMode) {
        case "center-bounds":
            return new THREE.Vector3(
                -assetTemplate.center.x * scale,
                -assetTemplate.center.y * scale,
                -assetTemplate.center.z * scale);
        case "preserve-origin":
            return new THREE.Vector3(0, 0, 0);
        default:
            return new THREE.Vector3(
                -assetTemplate.center.x * scale,
                -(assetTemplate.min.y * scale),
                -assetTemplate.center.z * scale);
    }
}
