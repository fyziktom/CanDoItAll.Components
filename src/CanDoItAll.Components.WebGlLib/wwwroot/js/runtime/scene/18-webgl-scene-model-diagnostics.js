import {
    THREE,
    resolveFiniteNumber,
    resolveVector3
} from "./02-webgl-scene-core.js";

export function normalizeModelImportOptions(asset) {
    const options = asset?.importOptions || {};
    return {
        unitScale: Math.max(0.0001, resolveFiniteNumber(options.unitScale, 1)),
        fitMode: normalizeOption(options.fitMode, "fit-bounds", ["fit-bounds", "original-scale", "fixed-scale"]),
        centerMode: normalizeOption(options.centerMode, "center-bottom", ["center-bottom", "center-bounds", "preserve-origin"]),
        fixedScale: Math.max(0.0001, resolveFiniteNumber(options.fixedScale, 1)),
        rotationOffset: resolveVector3(options.rotationOffset, { x: 0, y: 0, z: 0 }),
        positionOffset: resolveVector3(options.positionOffset, { x: 0, y: 0, z: 0 }),
        forceDoubleSidedMaterial: options.forceDoubleSidedMaterial === true,
        normalizeMaterialVisibility: options.normalizeMaterialVisibility === true,
        debugBounds: options.debugBounds === true,
        disableTint: options.disableTint === true
    };
}

export function diagnoseModelTemplate({ asset, uri, template, camera }) {
    const diagnostics = createDiagnostics(asset, uri);
    diagnostics.hasScene = !!template;
    if (!template) {
        diagnostics.errors.push("GLB did not contain a default scene.");
        return diagnostics;
    }

    const invalidTransformNodes = [];
    let invisibleMeshCount = 0;
    let transparentMaterialCount = 0;
    let materialCount = 0;
    template.updateWorldMatrix(true, true);
    template.traverse(child => {
        if (!hasFiniteTransform(child)) {
            invalidTransformNodes.push(child.name || child.uuid || "unnamed-node");
        }

        if (!child.isMesh && !child.isSkinnedMesh) {
            return;
        }

        diagnostics.meshCount += 1;
        if (child.visible !== false) {
            diagnostics.visibleMeshCount += 1;
        } else {
            invisibleMeshCount += 1;
        }

        for (const material of normalizeMaterials(child.material)) {
            materialCount += 1;
            if (isTransparentMaterial(material)) {
                transparentMaterialCount += 1;
            }
        }
    });

    diagnostics.materialCount = materialCount;
    diagnostics.transparentMaterialCount = transparentMaterialCount;
    diagnostics.bounds = measureBounds(template);
    diagnostics.center = vectorPayload(diagnostics.bounds.center);
    diagnostics.size = vectorPayload(diagnostics.bounds.size);
    diagnostics.min = vectorPayload(diagnostics.bounds.min);
    diagnostics.max = vectorPayload(diagnostics.bounds.max);

    if (diagnostics.meshCount === 0) {
        diagnostics.errors.push("GLB scene contains no mesh nodes.");
    }

    if (diagnostics.meshCount > 0 && invisibleMeshCount === diagnostics.meshCount) {
        diagnostics.warnings.push("All mesh nodes are hidden.");
    }

    if (materialCount > 0 && transparentMaterialCount === materialCount) {
        diagnostics.warnings.push("All mesh materials appear transparent or near-zero opacity.");
    }

    if (invalidTransformNodes.length > 0) {
        diagnostics.errors.push(`Non-finite transform values detected on ${invalidTransformNodes.length} node(s).`);
        diagnostics.metadata.invalidTransformNodes = invalidTransformNodes.join(",");
    }

    const maxAxis = Math.max(diagnostics.bounds.size.x, diagnostics.bounds.size.y, diagnostics.bounds.size.z);
    const minAxis = Math.min(diagnostics.bounds.size.x, diagnostics.bounds.size.y, diagnostics.bounds.size.z);
    if (!diagnostics.bounds.isEmpty && maxAxis < 0.001) {
        diagnostics.errors.push("Model bounds are zero or near-zero.");
    }

    if (maxAxis > 200 || (minAxis > 0 && maxAxis / minAxis > 250)) {
        diagnostics.warnings.push("Model bounds are extreme; unit scale or axis conversion may be wrong.");
    }

    const distanceFromOrigin = diagnostics.bounds.center.length();
    if (distanceFromOrigin > Math.max(20, maxAxis * 8)) {
        diagnostics.warnings.push("Model bounds are far from origin; center/offset import options may be needed.");
    }

    if (camera?.far && maxAxis > camera.far * 0.7) {
        diagnostics.warnings.push("Model bounds approach the camera far plane; clipping risk is high.");
    }

    if (!diagnostics.errors.length && !diagnostics.warnings.length) {
        diagnostics.metadata.status = "ok";
    }

    return diagnostics;
}

export function normalizeImportedModelMaterials(instance, options) {
    instance.traverse(child => {
        if (!child.isMesh && !child.isSkinnedMesh) {
            return;
        }

        for (const material of normalizeMaterials(child.material)) {
            if (!material) {
                continue;
            }

            if (options.forceDoubleSidedMaterial) {
                material.side = THREE.DoubleSide;
            }

            if (options.normalizeMaterialVisibility && isTransparentMaterial(material)) {
                material.opacity = 1;
                material.transparent = false;
                material.visible = true;
            }

            material.needsUpdate = true;
        }
    });
}

export function addDebugBounds(wrapper, model, color = "#facc15") {
    const bounds = new THREE.Box3().setFromObject(model);
    if (bounds.isEmpty()) {
        return null;
    }

    const size = bounds.getSize(new THREE.Vector3());
    const center = bounds.getCenter(new THREE.Vector3());
    const helper = new THREE.Mesh(
        new THREE.BoxGeometry(size.x, size.y, size.z),
        new THREE.MeshBasicMaterial({
            color,
            wireframe: true,
            transparent: true,
            opacity: 0.85,
            depthWrite: false
        }));
    helper.name = "webgl-model-debug-bounds";
    helper.position.copy(center);
    wrapper.add(helper);
    return helper;
}

function createDiagnostics(asset, uri) {
    return {
        assetId: asset?.id || "",
        variantId: asset?.variantId || "",
        uri: uri || "",
        hasScene: false,
        meshCount: 0,
        visibleMeshCount: 0,
        materialCount: 0,
        transparentMaterialCount: 0,
        min: { x: 0, y: 0, z: 0 },
        max: { x: 0, y: 0, z: 0 },
        size: { x: 0, y: 0, z: 0 },
        center: { x: 0, y: 0, z: 0 },
        errors: [],
        warnings: [],
        metadata: {}
    };
}

function measureBounds(template) {
    const bounds = new THREE.Box3().setFromObject(template);
    const isEmpty = bounds.isEmpty();
    const min = isEmpty ? new THREE.Vector3() : bounds.min.clone();
    const max = isEmpty ? new THREE.Vector3() : bounds.max.clone();
    const size = isEmpty ? new THREE.Vector3() : bounds.getSize(new THREE.Vector3());
    const center = isEmpty ? new THREE.Vector3() : bounds.getCenter(new THREE.Vector3());
    return { isEmpty, min, max, size, center };
}

function hasFiniteTransform(object) {
    const values = [
        object.position.x,
        object.position.y,
        object.position.z,
        object.rotation.x,
        object.rotation.y,
        object.rotation.z,
        object.scale.x,
        object.scale.y,
        object.scale.z
    ];
    return values.every(Number.isFinite);
}

function isTransparentMaterial(material) {
    if (!material) {
        return false;
    }

    return material.visible === false || material.transparent === true && resolveFiniteNumber(material.opacity, 1) <= 0.05;
}

function normalizeMaterials(material) {
    if (!material) {
        return [];
    }

    return Array.isArray(material) ? material : [material];
}

function normalizeOption(value, fallback, allowed) {
    const normalized = String(value || fallback).toLowerCase();
    return allowed.includes(normalized) ? normalized : fallback;
}

function vectorPayload(vector) {
    return {
        x: Math.round(vector.x * 1000) / 1000,
        y: Math.round(vector.y * 1000) / 1000,
        z: Math.round(vector.z * 1000) / 1000
    };
}
