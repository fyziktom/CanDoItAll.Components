import {
    THREE,
    createMaterial,
    disposeObject3D,
    normalizeColor,
    primitiveKinds,
    resolveFiniteNumber,
    resolveObjectSize,
    resolveString
} from "./02-webgl-scene-core.js";
import { GLTFLoader } from "../../../vendor/GLTFLoader.js";
import { clone as cloneSkeleton } from "../../../vendor/utils/SkeletonUtils.js";

const modelLoader = new GLTFLoader();

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
    const catalogFallbackId = state.sceneModel.assetCatalog?.defaultFallbackAssetId;
    const fallback = catalogFallbackId
        ? state.assetLookup.get(catalogFallbackId)
        : null;

    if (fallback) {
        return fallback;
    }

    return {
        id: missingAssetId ? `${missingAssetId}.fallback` : "asset.primitive.fallback",
        kind: "primitive",
        format: "primitive",
        primitiveKind: primitiveKinds.box,
        displayName: "Fallback box",
        color: "#94a3b8",
        supportsTint: true,
        boundsHint: { x: 1, y: 1, z: 1 }
    };
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

function loadModelAsset(state, asset) {
    const uri = buildAbsoluteUri(asset?.uri);
    if (!uri) {
        return Promise.reject(new Error(`Asset '${asset?.id || ""}' has no URI.`));
    }

    if (state.assetCache.has(uri)) {
        return state.assetCache.get(uri);
    }

    const promise = modelLoader.loadAsync(uri)
        .then(gltf => {
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
                child.userData = {
                    ...child.userData,
                    skipDispose: true
                };
            });

            state.diagnostics.loadedAssetIds.add(asset.id);
            state.diagnostics.missingAssetIds.delete(asset.id);
            return {
                template,
                size,
                center,
                min: bounds.min.clone(),
                hasSkinnedMesh
            };
        })
        .catch(error => {
            state.assetCache.delete(uri);
            state.diagnostics.missingAssetIds.add(asset.id);
            state.diagnostics.lastError = error?.message || String(error);
            throw error;
        });

    state.assetCache.set(uri, promise);
    return promise;
}

function markModelInstance(instance, tintColor, supportsTint) {
    instance.traverse(child => {
        child.frustumCulled = false;
        child.userData = {
            ...child.userData,
            skipDispose: true
        };

        if (!child.isMesh || !supportsTint || !tintColor) {
            return;
        }

        if (Array.isArray(child.material)) {
            child.material = child.material.map(material => material.clone());
            for (const material of child.material) {
                material.color?.lerp?.(normalizeColor(tintColor), 0.38);
                material.needsUpdate = true;
            }
            return;
        }

        if (child.material?.clone) {
            child.material = child.material.clone();
            child.material.color?.lerp?.(normalizeColor(tintColor), 0.38);
            child.material.needsUpdate = true;
        }
    });
}

function buildModelInstance(assetTemplate, sceneObject, asset, options = {}) {
    const instance = assetTemplate.hasSkinnedMesh
        ? cloneSkeleton(assetTemplate.template)
        : assetTemplate.template.clone(true);
    const size = resolveObjectSize(sceneObject);
    const fit = options.fit || 0.92;
    const scale = Math.min(
        (size.x * fit) / Math.max(assetTemplate.size.x, 0.01),
        (size.y * fit) / Math.max(assetTemplate.size.y, 0.01),
        (size.z * fit) / Math.max(assetTemplate.size.z, 0.01));

    const defaultScale = Math.max(0.01, resolveFiniteNumber(asset.defaultScale, 1));
    const resolvedScale = scale * defaultScale;
    instance.scale.setScalar(resolvedScale);
    instance.position.set(
        -assetTemplate.center.x * resolvedScale,
        -(assetTemplate.min.y * resolvedScale),
        -assetTemplate.center.z * resolvedScale);
    markModelInstance(instance, sceneObject?.color || asset?.color, asset?.supportsTint !== false);
    return instance;
}

export function createPrimitiveVisual(asset, sceneObject, options = {}) {
    const primitiveKind = resolveString(asset?.primitiveKind, primitiveKinds.box);
    const color = sceneObject?.color || asset?.color || "#94a3b8";
    const size = resolveObjectSize(sceneObject);
    const group = new THREE.Group();

    switch (primitiveKind) {
        case primitiveKinds.house:
            addHousePrimitive(group, size, color);
            break;
        case primitiveKinds.tree:
            addTreePrimitive(group, size, color);
            break;
        case primitiveKinds.person:
            addPersonPrimitive(group, size, color);
            break;
        case primitiveKinds.marker:
            addMarkerPrimitive(group, size, color, options.symbol);
            break;
        case primitiveKinds.gear:
            addGearPrimitive(group, size, color);
            break;
        case primitiveKinds.sphere:
            addSpherePrimitive(group, size, color);
            break;
        case primitiveKinds.cylinder:
            addCylinderPrimitive(group, size, color);
            break;
        case primitiveKinds.cone:
            addConePrimitive(group, size, color);
            break;
        default:
            addBoxPrimitive(group, size, color);
            break;
    }

    return group;
}

function addBoxPrimitive(group, size, color) {
    const mesh = new THREE.Mesh(
        new THREE.BoxGeometry(size.x, size.y, size.z),
        createMaterial(color, { roughness: 0.52 }));
    mesh.position.y = size.y / 2;
    group.add(mesh);
}

function addHousePrimitive(group, size, color) {
    const bodyHeight = size.y * 0.68;
    const roofHeight = size.y * 0.34;
    const body = new THREE.Mesh(
        new THREE.BoxGeometry(size.x, bodyHeight, size.z),
        createMaterial(color, { roughness: 0.62 }));
    body.position.y = bodyHeight / 2;

    const roof = new THREE.Mesh(
        new THREE.ConeGeometry(Math.max(size.x, size.z) * 0.72, roofHeight, 4),
        createMaterial("#b45309", { roughness: 0.7 }));
    roof.position.y = bodyHeight + (roofHeight / 2);
    roof.rotation.y = Math.PI / 4;

    const door = new THREE.Mesh(
        new THREE.BoxGeometry(size.x * 0.22, bodyHeight * 0.46, 0.035),
        createMaterial("#334155", { roughness: 0.82 }));
    door.position.set(0, bodyHeight * 0.23, (size.z / 2) + 0.021);
    group.add(body, roof, door);
}

function addTreePrimitive(group, size, color) {
    const trunk = new THREE.Mesh(
        new THREE.CylinderGeometry(size.x * 0.12, size.x * 0.16, size.y * 0.45, 12),
        createMaterial("#7c4a21", { roughness: 0.8 }));
    trunk.position.y = size.y * 0.225;

    const crown = new THREE.Mesh(
        new THREE.ConeGeometry(size.x * 0.54, size.y * 0.74, 18),
        createMaterial(color || "#16a34a", { roughness: 0.7 }));
    crown.position.y = size.y * 0.78;
    group.add(trunk, crown);
}

function addPersonPrimitive(group, size, color) {
    const body = new THREE.Mesh(
        new THREE.CapsuleGeometry(size.x * 0.22, size.y * 0.42, 8, 16),
        createMaterial(color || "#e2e8f0", { roughness: 0.46 }));
    body.position.y = size.y * 0.44;

    const head = new THREE.Mesh(
        new THREE.SphereGeometry(size.x * 0.22, 18, 18),
        createMaterial("#f8d8bd", { roughness: 0.64 }));
    head.position.y = size.y * 0.82;

    const base = new THREE.Mesh(
        new THREE.CylinderGeometry(size.x * 0.34, size.x * 0.34, size.y * 0.04, 22),
        createMaterial("#334155", { transparent: true, opacity: 0.76 }));
    base.position.y = size.y * 0.02;
    group.add(base, body, head);
}

function addMarkerPrimitive(group, size, color, isSymbol) {
    const radius = Math.max(size.x, size.y, size.z) * (isSymbol ? 0.34 : 0.28);
    const halo = new THREE.Mesh(
        new THREE.SphereGeometry(radius * 1.75, 22, 22),
        createMaterial(color, { transparent: true, opacity: 0.16, depthWrite: false }));
    const core = new THREE.Mesh(
        new THREE.SphereGeometry(radius, 28, 28),
        createMaterial(color, { emissive: color, emissiveIntensity: 0.28, roughness: 0.35 }));
    const ring = new THREE.Mesh(
        new THREE.TorusGeometry(radius * 1.45, radius * 0.08, 10, 36),
        createMaterial(color, { transparent: true, opacity: 0.72 }));
    ring.rotation.x = Math.PI / 2;
    group.add(halo, core, ring);
}

function addGearPrimitive(group, size, color) {
    const radius = Math.max(size.x, size.z) * 0.34;
    const torus = new THREE.Mesh(
        new THREE.TorusGeometry(radius, Math.max(0.04, radius * 0.18), 16, 32),
        createMaterial(color, { metalness: 0.2, roughness: 0.42 }));
    torus.position.y = size.y * 0.5;
    torus.rotation.x = Math.PI / 2;
    const hub = new THREE.Mesh(
        new THREE.CylinderGeometry(radius * 0.42, radius * 0.42, size.y * 0.16, 24),
        createMaterial("#f8fafc", { metalness: 0.18, roughness: 0.5 }));
    hub.position.y = size.y * 0.5;
    group.add(torus, hub);
}

function addSpherePrimitive(group, size, color) {
    const mesh = new THREE.Mesh(
        new THREE.SphereGeometry(Math.max(size.x, size.y, size.z) * 0.5, 28, 28),
        createMaterial(color, { roughness: 0.42 }));
    mesh.position.y = size.y / 2;
    group.add(mesh);
}

function addCylinderPrimitive(group, size, color) {
    const mesh = new THREE.Mesh(
        new THREE.CylinderGeometry(size.x / 2, size.x / 2, size.y, 28),
        createMaterial(color, { roughness: 0.52 }));
    mesh.position.y = size.y / 2;
    group.add(mesh);
}

function addConePrimitive(group, size, color) {
    const mesh = new THREE.Mesh(
        new THREE.ConeGeometry(size.x / 2, size.y, 28),
        createMaterial(color, { roughness: 0.58 }));
    mesh.position.y = size.y / 2;
    group.add(mesh);
}

export function syncAssetVisual(state, sceneObject, group, options = {}) {
    const asset = resolveAssetDefinition(state, sceneObject?.assetId || "");
    const placeholder = createPrimitiveVisual(asset, sceneObject, options);
    placeholder.name = `${sceneObject.id || "object"}-fallback`;
    group.add(placeholder);
    state.diagnostics.fallbackObjectIds.add(sceneObject.id || asset.id);

    const format = resolveString(asset?.format, "primitive").toLowerCase();
    if (format !== "glb" && format !== "gltf") {
        return placeholder;
    }

    loadModelAsset(state, asset)
        .then(assetTemplate => {
            if (group.userData.disposed) {
                return;
            }

            const instance = buildModelInstance(assetTemplate, sceneObject, asset, options);
            group.add(instance);
            if (placeholder.parent === group) {
                group.remove(placeholder);
                disposeObject3D(placeholder);
            }

            state.diagnostics.fallbackObjectIds.delete(sceneObject.id || asset.id);
            state.scheduleRender();
        })
        .catch(() => {
            state.scheduleRender();
        });

    return placeholder;
}

