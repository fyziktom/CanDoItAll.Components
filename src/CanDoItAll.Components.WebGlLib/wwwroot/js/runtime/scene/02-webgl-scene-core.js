import * as THREE from "../../../vendor/three.module.min.js";

export { THREE };

export const projectionModes = Object.freeze({
    perspective: "perspective",
    orthographic: "orthographic"
});

export const primitiveKinds = Object.freeze({
    box: "box",
    house: "house",
    sphere: "sphere",
    cylinder: "cylinder",
    cone: "cone",
    tree: "tree",
    person: "person",
    marker: "marker",
    gear: "gear"
});

export function clonePayload(value) {
    if (typeof structuredClone === "function") {
        return structuredClone(value || {});
    }

    return JSON.parse(JSON.stringify(value || {}));
}

export function clamp(value, min, max) {
    return Math.min(max, Math.max(min, value));
}

export function round(value, decimals = 2) {
    const multiplier = Math.pow(10, decimals);
    return Math.round((Number(value) || 0) * multiplier) / multiplier;
}

export function resolveFiniteNumber(value, fallback) {
    const number = Number(value);
    return Number.isFinite(number) ? number : fallback;
}

export function resolveString(value, fallback = "") {
    return typeof value === "string" && value.trim().length > 0
        ? value
        : fallback;
}

export function resolveVector3(value, fallback = { x: 0, y: 0, z: 0 }) {
    return {
        x: resolveFiniteNumber(value?.x ?? value?.X, fallback.x || 0),
        y: resolveFiniteNumber(value?.y ?? value?.Y, fallback.y || 0),
        z: resolveFiniteNumber(value?.z ?? value?.Z, fallback.z || 0)
    };
}

export function toThreeVector(value, fallback) {
    const resolved = resolveVector3(value, fallback);
    return new THREE.Vector3(resolved.x, resolved.y, resolved.z);
}

export function normalizeColor(value, fallback = "#ffffff") {
    try {
        return new THREE.Color(resolveString(value, fallback));
    } catch {
        return new THREE.Color(fallback);
    }
}

export function normalizeScene(scene) {
    const normalized = clonePayload(scene);
    normalized.sceneId = resolveString(normalized.sceneId, "webgl-scene");
    normalized.title = resolveString(normalized.title, "WebGL scene");
    normalized.assetCatalog = normalized.assetCatalog || {};
    normalized.assetCatalog.assets = Array.isArray(normalized.assetCatalog.assets)
        ? normalized.assetCatalog.assets
        : [];
    normalized.environment = normalized.environment || {};
    normalized.camera = normalized.camera || {};
    normalized.uiState = normalized.uiState || {};
    normalized.interaction = normalized.interaction || {};
    normalized.objects = Array.isArray(normalized.objects) ? normalized.objects : [];
    normalized.links = Array.isArray(normalized.links) ? normalized.links : [];
    normalized.layers = Array.isArray(normalized.layers) ? normalized.layers : [];
    return normalized;
}

export function normalizeOptions(options) {
    return {
        deterministicMode: options?.deterministicMode !== false,
        preserveDrawingBuffer: options?.preserveDrawingBuffer !== false,
        enableAntialiasing: options?.enableAntialiasing !== false,
        maximumDevicePixelRatio: clamp(resolveFiniteNumber(options?.maximumDevicePixelRatio, 2), 1, 3),
        showDiagnosticsPanel: options?.showDiagnosticsPanel !== false,
        showLabels: options?.showLabels !== false,
        showSymbols: options?.showSymbols !== false,
        autoFitOnCreate: options?.autoFitOnCreate !== false,
        runtimeKey: resolveString(options?.runtimeKey, "")
    };
}

export function resolveObjectPosition(sceneObject) {
    return toThreeVector(sceneObject?.position, { x: 0, y: 0, z: 0 });
}

export function resolveObjectSize(sceneObject) {
    const size = resolveVector3(sceneObject?.size, { x: 1, y: 1, z: 1 });
    return {
        x: Math.max(0.05, size.x),
        y: Math.max(0.05, size.y),
        z: Math.max(0.05, size.z)
    };
}

export function resolveObjectScale(sceneObject) {
    const scale = resolveVector3(sceneObject?.scale, { x: 1, y: 1, z: 1 });
    return {
        x: Math.max(0.01, scale.x),
        y: Math.max(0.01, scale.y),
        z: Math.max(0.01, scale.z)
    };
}

export function resolveObjectRotation(sceneObject) {
    return resolveVector3(sceneObject?.rotation, { x: 0, y: 0, z: 0 });
}

export function applyObjectTransform(group, sceneObject) {
    const position = resolveObjectPosition(sceneObject);
    const rotation = resolveObjectRotation(sceneObject);
    const scale = resolveObjectScale(sceneObject);
    group.position.copy(position);
    group.rotation.set(rotation.x, rotation.y, rotation.z);
    group.scale.set(scale.x, scale.y, scale.z);
}

export function createMaterial(color, options = {}) {
    return new THREE.MeshStandardMaterial({
        color: normalizeColor(color, options.fallbackColor || "#ffffff"),
        roughness: resolveFiniteNumber(options.roughness, 0.58),
        metalness: resolveFiniteNumber(options.metalness, 0.08),
        emissive: normalizeColor(options.emissive || "#000000", "#000000"),
        emissiveIntensity: resolveFiniteNumber(options.emissiveIntensity, 0),
        transparent: !!options.transparent,
        opacity: resolveFiniteNumber(options.opacity, 1),
        depthWrite: options.depthWrite !== false
    });
}

export function disposeObject3D(object) {
    if (!object) {
        return;
    }

    object.traverse(child => {
        if (child.userData?.skipDispose) {
            return;
        }

        child.geometry?.dispose?.();
        if (Array.isArray(child.material)) {
            for (const material of child.material) {
                material?.map?.dispose?.();
                material?.dispose?.();
            }
        } else {
            child.material?.map?.dispose?.();
            child.material?.dispose?.();
        }
    });
}

export function focusHost(state) {
    try {
        state?.host?.focus?.({ preventScroll: true });
    } catch {
        state?.host?.focus?.();
    }
}

export function buildDiagnosticsSnapshot(state) {
    return {
        createCount: state.diagnostics.createCount,
        updateCount: state.diagnostics.updateCount,
        renderCount: state.diagnostics.renderCount,
        loadedAssetCount: state.diagnostics.loadedAssetIds.size,
        missingAssetCount: state.diagnostics.missingAssetIds.size,
        fallbackObjectCount: state.diagnostics.fallbackObjectIds.size,
        objectCount: state.sceneModel.objects.length,
        symbolCount: state.symbolGroups.size,
        deterministicMode: state.options.deterministicMode,
        lastError: state.diagnostics.lastError || "",
        missingAssetIds: Array.from(state.diagnostics.missingAssetIds)
    };
}

