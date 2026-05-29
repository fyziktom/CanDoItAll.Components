import {
    THREE,
    clamp,
    resolveFiniteNumber,
    resolveObjectSize,
    resolveString
} from "./02-webgl-scene-core.js";
import { syncAssetVisual } from "./03-webgl-scene-assets.js";
import { disposeSceneObjectTree } from "./17-webgl-scene-resources.js";
import { isObjectVisible } from "./23-webgl-scene-indexes.js";

export function rebuildSymbols(state) {
    clearSymbols(state);
    if (state.options.showSymbols === false || state.sceneModel.uiState?.showSymbols === false) {
        state.diagnostics.animatedSymbolCount = 0;
        return;
    }

    let animatedSymbolCount = 0;
    for (const sceneObject of state.sceneModel.objects || []) {
        if (!isObjectVisible(state, sceneObject)) {
            continue;
        }

        const symbols = (sceneObject.symbols || [])
            .filter(symbol => symbol?.isVisible !== false)
            .sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0));
        if (!symbols.length) {
            continue;
        }

        const size = resolveObjectSize(sceneObject);
        const center = (symbols.length - 1) / 2;
        symbols.forEach((symbol, index) => {
            const symbolGroup = createSymbolGroup(state, sceneObject, symbol, size, index - center);
            state.symbolGroups.set(symbolGroup.userData.symbolRuntimeId, symbolGroup);
            state.scene.add(symbolGroup);
            if (symbolGroup.userData.effectKey && symbolGroup.userData.effectKey !== "none") {
                animatedSymbolCount += 1;
            }
        });
    }

    state.diagnostics.animatedSymbolCount = animatedSymbolCount;
}

export function clearSymbols(state) {
    for (const group of state.symbolGroups.values()) {
        state.scene.remove(group);
        group.userData.disposed = true;
        disposeSceneObjectTree(group, state.diagnostics);
    }

    state.symbolGroups.clear();
    if (state.diagnostics) {
        state.diagnostics.animatedSymbolCount = 0;
    }
}

function createSymbolGroup(state, sceneObject, symbol, objectSize, offsetIndex) {
    const symbolScale = Math.max(0.12, resolveFiniteNumber(symbol.scale, 1));
    const intensity = clamp(resolveFiniteNumber(symbol.intensity, 0), 0, 1);
    const heightOffset = resolveFiniteNumber(symbol.heightOffset, 1.2);
    const symbolSize = 0.64 * symbolScale * (1 + (intensity * 0.26));
    const symbolObject = {
        id: `${sceneObject.id}.symbol.${symbol.id || symbol.semanticKind || "status"}`,
        kind: "symbol",
        family: "status",
        assetId: symbol.symbolAssetId || "asset.symbol.marker.default",
        color: symbol.color || "#facc15",
        size: { x: symbolSize, y: symbolSize, z: symbolSize },
        scale: { x: 1, y: 1, z: 1 }
    };
    const group = new THREE.Group();
    group.userData = {
        symbolRuntimeId: symbolObject.id,
        symbolId: symbol.id || "",
        ownerObjectId: sceneObject.id || "",
        semanticKind: symbol.semanticKind || "",
        effectKey: resolveString(symbol.effectKey, "none"),
        intensity,
        baseScale: symbolScale,
        heightOffset,
        billboardToCamera: symbol.billboardToCamera !== false,
        tooltip: symbol.tooltip || ""
    };

    const spacing = Math.max(0.58, objectSize.x * 0.38);
    const basePosition = state.objectPositions.get(sceneObject.id) || new THREE.Vector3();
    group.position.set(
        basePosition.x + (offsetIndex * spacing),
        basePosition.y + objectSize.y + heightOffset,
        basePosition.z);
    group.userData.basePosition = group.position.clone();
    syncAssetVisual(state, symbolObject, group, { symbol: true, fit: 0.86 });
    return group;
}

export function syncSymbolAnimation(state, elapsedSeconds) {
    for (const group of state.symbolGroups.values()) {
        const effectKey = group.userData.effectKey || "none";
        const intensity = clamp(resolveFiniteNumber(group.userData.intensity, 0), 0, 1);
        const baseScale = Math.max(0.12, resolveFiniteNumber(group.userData.baseScale, 1));
        const phase = elapsedSeconds * (1.2 + intensity);
        let scale = baseScale;
        let offsetY = 0;
        let opacity = 1;

        switch (effectKey) {
            case "pulse":
                scale = baseScale * (1 + (Math.sin(phase * 3.1) * 0.11));
                break;
            case "float":
                offsetY = Math.sin(phase * 1.8) * 0.08;
                break;
            case "spin":
                group.rotation.y = phase * 1.8;
                break;
            case "blink":
                opacity = 0.45 + (Math.abs(Math.sin(phase * 3.4)) * 0.55);
                break;
            case "glow":
                scale = baseScale * (1 + (Math.sin(phase * 2.2) * 0.06));
                break;
            case "shake":
                break;
            case "scale-by-intensity":
                scale = baseScale * (0.82 + intensity * 0.62);
                break;
            default:
                break;
        }

        group.scale.setScalar(scale);
        const basePosition = group.userData.basePosition || group.position;
        const shakeX = effectKey === "shake" ? Math.sin(phase * 8) * 0.04 : 0;
        group.position.set(basePosition.x + shakeX, basePosition.y + offsetY, basePosition.z);
        group.visible = state.options.showSymbols !== false && state.sceneModel.uiState?.showSymbols !== false;
        if (group.userData.billboardToCamera) {
            group.quaternion.copy(state.camera.quaternion);
        }

        group.traverse(child => {
            if (!child.material) {
                return;
            }

            const materials = Array.isArray(child.material) ? child.material : [child.material];
            for (const material of materials) {
                if ("opacity" in material) {
                    material.transparent = opacity < 1 || material.transparent;
                    material.opacity = opacity;
                }

                if ("emissiveIntensity" in material && effectKey === "glow") {
                    material.emissiveIntensity = 0.18 + Math.abs(Math.sin(phase * 2.4)) * 0.36;
                }
            }
        });
    }
}

export function syncSymbolPositionsForObject(state, objectId) {
    const sceneObject = state.objectLookup.get(objectId);
    if (!sceneObject) {
        return;
    }

    const size = resolveObjectSize(sceneObject);
    const ownerSymbols = Array.from(state.symbolGroups.values())
        .filter(group => group.userData.ownerObjectId === objectId);
    const center = (ownerSymbols.length - 1) / 2;
    ownerSymbols.forEach((group, index) => {
        const offsetIndex = index - center;
        const spacing = Math.max(0.58, size.x * 0.38);
        const basePosition = state.objectPositions.get(objectId) || new THREE.Vector3();
        const heightOffset = resolveFiniteNumber(group.userData.heightOffset, 1.2);
        group.position.set(basePosition.x + offsetIndex * spacing, basePosition.y + size.y + heightOffset, basePosition.z);
        group.userData.basePosition = group.position.clone();
    });
}
