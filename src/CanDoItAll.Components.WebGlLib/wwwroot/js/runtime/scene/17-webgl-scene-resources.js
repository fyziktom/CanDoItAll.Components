const ownershipKey = "webglResourceOwnership";

export function markSharedTemplateResource(object) {
    markResourceTree(object, { ownsGeometry: false, ownsMaterial: false, sharedTemplate: true });
    return object;
}

export function markInstanceResource(object, { ownsGeometry = true, ownsMaterial = true } = {}) {
    markResourceTree(object, { ownsGeometry, ownsMaterial, sharedTemplate: false });
    return object;
}

export function markOwnedMaterial(material) {
    for (const item of normalizeMaterials(material)) {
        if (!item) {
            continue;
        }

        item.userData = {
            ...item.userData,
            [ownershipKey]: {
                ...(item.userData?.[ownershipKey] || {}),
                ownsMaterial: true
            }
        };
    }

    return material;
}

export function disposeSceneObjectTree(object, diagnostics = null) {
    if (!object) {
        return;
    }

    object.traverse(child => {
        const ownership = resolveOwnership(child);
        if (ownership.ownsGeometry) {
            disposeOwnedGeometry(child.geometry, diagnostics);
        }

        disposeOwnedMaterial(child.material, ownership.ownsMaterial, diagnostics);
    });
}

export function disposeOwnedMaterial(material, force = true, diagnostics = null) {
    for (const item of normalizeMaterials(material)) {
        if (!item) {
            continue;
        }

        const materialOwnership = item.userData?.[ownershipKey] || {};
        if (!force && materialOwnership.ownsMaterial !== true) {
            continue;
        }

        disposeMaterialTextures(item, diagnostics);
        item.dispose?.();
        if (diagnostics) {
            diagnostics.disposedMaterialCount = (diagnostics.disposedMaterialCount || 0) + 1;
        }
    }
}

export function disposeOwnedGeometry(geometry, diagnostics = null) {
    if (geometry && diagnostics) {
        diagnostics.disposedGeometryCount = (diagnostics.disposedGeometryCount || 0) + 1;
    }

    geometry?.dispose?.();
}

function markResourceTree(object, ownership) {
    object?.traverse(child => {
        child.userData = {
            ...child.userData,
            [ownershipKey]: {
                ownsGeometry: ownership.ownsGeometry,
                ownsMaterial: ownership.ownsMaterial,
                sharedTemplate: ownership.sharedTemplate
            }
        };
    });
}

function resolveOwnership(child) {
    const childOwnership = child.userData?.[ownershipKey] || {};
    return {
        ownsGeometry: childOwnership.ownsGeometry !== false,
        ownsMaterial: childOwnership.ownsMaterial !== false
    };
}

function normalizeMaterials(material) {
    if (!material) {
        return [];
    }

    return Array.isArray(material) ? material : [material];
}

function disposeMaterialTextures(material, diagnostics = null) {
    const textureKeys = [
        "map",
        "alphaMap",
        "aoMap",
        "bumpMap",
        "displacementMap",
        "emissiveMap",
        "envMap",
        "lightMap",
        "metalnessMap",
        "normalMap",
        "roughnessMap"
    ];

    for (const key of textureKeys) {
        if (material[key] && diagnostics) {
            diagnostics.disposedTextureCount = (diagnostics.disposedTextureCount || 0) + 1;
        }

        material[key]?.dispose?.();
    }
}
