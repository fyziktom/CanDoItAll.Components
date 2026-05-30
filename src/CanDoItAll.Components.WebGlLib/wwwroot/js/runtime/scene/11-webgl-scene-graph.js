import {
    THREE,
    applyObjectTransform,
    createMaterial,
    resolveFiniteNumber,
    resolveObjectSize
} from "./02-webgl-scene-core.js";
import { syncAssetVisual } from "./03-webgl-scene-assets.js";
import { rebuildSymbols, syncSymbolPositionsForObject } from "./04-webgl-scene-symbols.js";
import { disposeSceneObjectTree } from "./17-webgl-scene-resources.js";
import {
    buildSceneIndexes,
    buildVisibilityCounts,
    isLinkVisible,
    isObjectVisible
} from "./23-webgl-scene-indexes.js";
import {
    createLinkGroup,
    indexLinkGroup,
    syncLinksForObject,
    unindexLinkGroup
} from "./27-webgl-scene-links.js";

export function buildDecorations(state) {
    const environment = state.sceneModel.environment || {};
    const groundSize = Math.max(8, resolveFiniteNumber(environment.groundSize, 36));
    const gridDivisions = Math.max(4, resolveFiniteNumber(environment.gridDivisions, 24));

    const ground = new THREE.Mesh(
        new THREE.PlaneGeometry(groundSize, groundSize),
        createMaterial(environment.groundColor || "#1f2937", {
            transparent: true,
            opacity: 0.82,
            roughness: 0.9
        }));
    ground.rotation.x = -Math.PI / 2;
    ground.position.y = -0.02;

    const grid = new THREE.GridHelper(groundSize, gridDivisions, environment.gridColor || "#94a3b8", "#334155");
    grid.position.y = 0.01;
    const materials = Array.isArray(grid.material) ? grid.material : [grid.material];
    for (const material of materials) {
        material.transparent = true;
        material.opacity = 0.34;
    }

    state.scene.add(ground, grid);
    state.decorations = { ground, grid };
}

export function syncDecorations(state) {
    const uiState = state.sceneModel.uiState || {};
    state.decorations.ground.visible = uiState.showGround !== false;
    state.decorations.grid.visible = uiState.showGrid !== false;
}

export function rebuildScene(state) {
    clearDynamicScene(state);
    syncSceneIndexes(state, "scene-rebuild");
    state.objectLookup = new Map();
    state.objectPositions = new Map();

    for (const sceneObject of state.sceneModel.objects || []) {
        state.objectLookup.set(sceneObject.id, sceneObject);
        if (!isObjectVisible(state, sceneObject)) {
            continue;
        }

        addSceneObjectGroup(state, sceneObject);
    }

    for (const link of state.sceneModel.links || []) {
        if (!isLinkVisible(state, link)) {
            continue;
        }

        addLinkGroup(state, link);
    }

    rebuildSymbols(state);
    syncObjectRings(state);
    state.shell.emptyState.classList.toggle("is-visible", (state.sceneModel.objects || []).length === 0);
    state.scheduleRender("scene-rebuild");
}

export function syncSceneIndexes(state, reason = "scene-index-sync") {
    state.sceneIndexes = buildSceneIndexes(state.sceneModel);
    state.diagnostics.visibilityCounts = buildVisibilityCounts(state);
    state.diagnostics.sceneIndexSyncCount = (state.diagnostics.sceneIndexSyncCount || 0) + 1;
    state.diagnostics.lastSceneIndexSyncReason = reason;
    return state.sceneIndexes;
}

export function addSceneObjectGroup(state, sceneObject) {
    if (!sceneObject?.id) {
        return null;
    }

    state.objectLookup.set(sceneObject.id, sceneObject);
    const group = createSceneObjectGroup(state, sceneObject);
    state.objectGroups.set(sceneObject.id, group);
    state.scene.add(group);
    return group;
}

export function createSceneObjectGroup(state, sceneObject) {
    const group = new THREE.Group();
    group.userData = { objectId: sceneObject.id };
    applyObjectTransform(group, sceneObject);
    state.objectPositions.set(sceneObject.id, group.position.clone());

    const size = resolveObjectSize(sceneObject);
    const hitMesh = new THREE.Mesh(
        new THREE.BoxGeometry(size.x * 1.18, size.y * 1.12, size.z * 1.18),
        new THREE.MeshBasicMaterial({ transparent: true, opacity: 0, depthWrite: false }));
    hitMesh.position.y = size.y / 2;
    hitMesh.userData = { objectId: sceneObject.id };

    const selectionRing = createGroundRing(size, sceneObject.color || "#38bdf8", 0.92);
    selectionRing.visible = false;
    const hoverRing = createGroundRing(size, "#facc15", 0.42);
    hoverRing.visible = false;
    group.userData.selectionRing = selectionRing;
    group.userData.hoverRing = hoverRing;
    group.userData.hitMesh = hitMesh;

    group.add(selectionRing, hoverRing, hitMesh);
    state.hitMeshes.push(hitMesh);
    syncAssetVisual(state, sceneObject, group);
    return group;
}

export function removeSceneObjectGroup(state, objectId) {
    const group = state.objectGroups.get(objectId);
    if (!group) {
        return;
    }

    state.scene.remove(group);
    group.userData.disposed = true;
    state.hitMeshes = state.hitMeshes.filter(mesh => mesh.userData?.objectId !== objectId);
    state.objectGroups.delete(objectId);
    state.objectLookup.delete(objectId);
    state.objectPositions.delete(objectId);
    disposeSceneObjectTree(group, state.diagnostics);
}

export function replaceSceneObjectGroup(state, sceneObject) {
    removeSceneObjectGroup(state, sceneObject.id);
    return addSceneObjectGroup(state, sceneObject);
}

export function updateObjectRuntimeTransform(state, objectId, positionOnly = false) {
    const group = state.objectGroups.get(objectId);
    const sceneObject = state.objectLookup.get(objectId);
    if (!group || !sceneObject) {
        return false;
    }

    if (positionOnly) {
        const position = resolveObjectPosition(sceneObject);
        group.position.copy(position);
    } else {
        applyObjectTransform(group, sceneObject);
    }

    state.objectPositions.set(objectId, group.position.clone());
    syncLinksForObject(state, objectId);
    syncSymbolPositionsForObject(state, objectId);
    state.scheduleRender("object-transform");
    return true;
}

export function addLinkGroup(state, link) {
    const linkGroup = createLinkGroup(state, link);
    if (!linkGroup) {
        return null;
    }

    state.linkGroups.push(linkGroup);
    indexLinkGroup(state, linkGroup);
    state.scene.add(linkGroup);
    return linkGroup;
}

export function removeLinkGroup(state, linkId) {
    const index = state.linkGroups.findIndex(group => group.userData.linkId === linkId);
    if (index < 0) {
        return;
    }

    const [group] = state.linkGroups.splice(index, 1);
    unindexLinkGroup(state, group);
    state.scene.remove(group);
    disposeSceneObjectTree(group, state.diagnostics);
}

export function syncObjectRings(state) {
    for (const [objectId, group] of state.objectGroups.entries()) {
        const selected = state.selectedObjectIds.has(objectId);
        const hovered = state.hoveredObjectId === objectId;
        if (group.userData.selectionRing) {
            group.userData.selectionRing.visible = selected;
        }

        if (group.userData.hoverRing) {
            group.userData.hoverRing.visible = hovered && !selected;
        }
    }
}

export function clearDynamicScene(state) {
    for (const group of state.objectGroups.values()) {
        state.scene.remove(group);
        group.userData.disposed = true;
        disposeSceneObjectTree(group, state.diagnostics);
    }

    for (const linkGroup of state.linkGroups) {
        state.scene.remove(linkGroup);
        disposeSceneObjectTree(linkGroup, state.diagnostics);
    }

    for (const symbolGroup of state.symbolGroups.values()) {
        state.scene.remove(symbolGroup);
        symbolGroup.userData.disposed = true;
        disposeSceneObjectTree(symbolGroup, state.diagnostics);
    }

    for (const label of state.labelElements.values()) {
        label.remove();
    }

    state.objectGroups.clear();
    state.linkGroups.length = 0;
    state.linkGroupsByObjectId?.clear();
    state.symbolGroups.clear();
    state.labelElements.clear();
    state.hitMeshes.length = 0;
    resetInstanceDiagnostics(state);
    state.diagnostics.visibilityCounts = buildVisibilityCounts(state);
}

function createGroundRing(size, color, opacity) {
    const radius = Math.max(size.x, size.z) * 0.62;
    const ring = new THREE.Mesh(
        new THREE.TorusGeometry(radius, Math.max(0.018, radius * 0.035), 8, 48),
        createMaterial(color, {
            transparent: true,
            opacity,
            emissive: color,
            emissiveIntensity: 0.18,
            depthWrite: false
        }));
    ring.position.y = 0.04;
    ring.rotation.x = Math.PI / 2;
    return ring;
}

function resetInstanceDiagnostics(state) {
    state.diagnostics.fallbackObjectIds.clear();
    state.diagnostics.modelInstanceIds.clear();
    state.diagnostics.primitiveInstanceIds.clear();
    state.diagnostics.estimatedTriangleCount = 0;
    state.diagnostics.estimatedVertexCount = 0;
}
