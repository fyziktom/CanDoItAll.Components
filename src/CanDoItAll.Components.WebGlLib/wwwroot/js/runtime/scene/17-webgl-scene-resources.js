const ownershipKey = "webglResourceOwnership";
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

export function markSharedTemplateResource(object) {
    markResourceTree(object, { ownsGeometry: true, ownsMaterial: true, ownsTexture: true, sharedTemplate: true });
    return object;
}

export function markInstanceResource(object, { ownsGeometry = true, ownsMaterial = true, ownsTexture = ownsMaterial } = {}) {
    markResourceTree(object, { ownsGeometry, ownsMaterial, ownsTexture, sharedTemplate: false });
    return object;
}

export function markOwnedMaterial(material, { ownsTexture = false } = {}) {
    for (const item of normalizeMaterials(material)) {
        if (!item) {
            continue;
        }

        item.userData = {
            ...item.userData,
            [ownershipKey]: {
                ...(item.userData?.[ownershipKey] || {}),
                ownsMaterial: true,
                ownsTexture: ownsTexture === true
            }
        };
    }

    return material;
}

export function disposeSceneObjectTree(object, diagnostics = null) {
    if (!object) {
        return;
    }

    const tracker = createDisposalTracker();
    object.traverse(child => {
        const ownership = resolveOwnership(child);
        if (ownership.ownsGeometry) {
            disposeOwnedGeometry(child.geometry, diagnostics, tracker.geometries);
        }

        disposeOwnedMaterial(child.material, ownership.ownsMaterial, diagnostics, {
            forceTexture: ownership.ownsTexture,
            disposedMaterials: tracker.materials,
            disposedTextures: tracker.textures,
            retainedTextures: tracker.retainedTextures
        });
    });
}

export function disposeOwnedMaterial(material, force = true, diagnostics = null, options = {}) {
    for (const item of normalizeMaterials(material)) {
        if (!item) {
            continue;
        }

        const materialOwnership = item.userData?.[ownershipKey] || {};
        const shouldDisposeMaterial = force === true || materialOwnership.ownsMaterial === true;
        if (!shouldDisposeMaterial) {
            continue;
        }

        const shouldDisposeTextures = materialOwnership.ownsTexture === true ||
            (force === true && materialOwnership.ownsTexture !== false && options.forceTexture !== false) ||
            options.forceTexture === true;
        if (shouldDisposeTextures) {
            disposeMaterialTextures(item, diagnostics, options.disposedTextures);
        } else {
            retainMaterialTextures(item, diagnostics, options.retainedTextures);
        }

        if (options.disposedMaterials?.has?.(item)) {
            continue;
        }

        options.disposedMaterials?.add?.(item);
        item.dispose?.();
        if (diagnostics) {
            diagnostics.disposedMaterialCount = (diagnostics.disposedMaterialCount || 0) + 1;
        }
    }
}

export function disposeOwnedGeometry(geometry, diagnostics = null, disposedGeometries = null) {
    if (!geometry) {
        return;
    }

    if (disposedGeometries?.has?.(geometry)) {
        return;
    }

    disposedGeometries?.add?.(geometry);
    if (diagnostics) {
        diagnostics.disposedGeometryCount = (diagnostics.disposedGeometryCount || 0) + 1;
    }

    geometry.dispose?.();
}

function markResourceTree(object, ownership) {
    object?.traverse(child => {
        child.userData = {
            ...child.userData,
            [ownershipKey]: {
                ownsGeometry: ownership.ownsGeometry,
                ownsMaterial: ownership.ownsMaterial,
                ownsTexture: ownership.ownsTexture,
                sharedTemplate: ownership.sharedTemplate
            }
        };
    });
}

function resolveOwnership(child) {
    const childOwnership = child.userData?.[ownershipKey] || {};
    const ownsMaterial = childOwnership.ownsMaterial !== false;
    return {
        ownsGeometry: childOwnership.ownsGeometry !== false,
        ownsMaterial,
        ownsTexture: ownsMaterial && childOwnership.ownsTexture !== false
    };
}

function normalizeMaterials(material) {
    if (!material) {
        return [];
    }

    return Array.isArray(material) ? material : [material];
}

function disposeMaterialTextures(material, diagnostics = null, disposedTextures = null) {
    for (const key of textureKeys) {
        const texture = material[key];
        if (!texture || disposedTextures?.has?.(texture)) {
            continue;
        }

        disposedTextures?.add?.(texture);
        if (diagnostics) {
            diagnostics.disposedTextureCount = (diagnostics.disposedTextureCount || 0) + 1;
        }

        texture.dispose?.();
    }
}

function retainMaterialTextures(material, diagnostics = null, retainedTextures = null) {
    if (!diagnostics) {
        return;
    }

    for (const key of textureKeys) {
        const texture = material[key];
        if (!texture || retainedTextures?.has?.(texture)) {
            continue;
        }

        retainedTextures?.add?.(texture);
        diagnostics.retainedSharedTextureCount = (diagnostics.retainedSharedTextureCount || 0) + 1;
    }
}

function createDisposalTracker() {
    return {
        geometries: new WeakSet(),
        materials: new WeakSet(),
        textures: new WeakSet(),
        retainedTextures: new WeakSet()
    };
}
