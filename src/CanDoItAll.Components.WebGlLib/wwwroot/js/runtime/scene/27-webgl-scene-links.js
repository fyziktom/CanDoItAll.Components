import {
    THREE,
    resolveFiniteNumber,
    resolveObjectPosition
} from "./02-webgl-scene-core.js";

export function createLinkGroup(state, link) {
    const source = state.objectLookup.get(link.sourceObjectId);
    const target = state.objectLookup.get(link.targetObjectId);
    if (!source || !target) {
        return null;
    }

    const material = new THREE.LineBasicMaterial({
        color: link.color || "#94a3b8",
        transparent: true,
        opacity: resolveFiniteNumber(link.opacity, 0.75),
        linewidth: Math.max(1, resolveFiniteNumber(link.width, 1))
    });
    const line = new THREE.Line(buildLinkGeometry(source, target), material);
    const group = new THREE.Group();
    group.userData = {
        linkId: link.id || "",
        sourceObjectId: link.sourceObjectId,
        targetObjectId: link.targetObjectId
    };
    group.add(line);
    return group;
}

export function syncLinksForObject(state, objectId) {
    const groups = state.linkGroupsByObjectId?.get(objectId) || [];
    state.diagnostics.linkUpdateCount = (state.diagnostics.linkUpdateCount || 0) + groups.length;
    for (const group of groups) {
        const source = state.objectLookup.get(group.userData.sourceObjectId);
        const target = state.objectLookup.get(group.userData.targetObjectId);
        const line = group.children[0];
        if (source && target && line) {
            line.geometry.dispose();
            line.geometry = buildLinkGeometry(source, target);
        }
    }
}

export function indexLinkGroup(state, group) {
    state.linkGroupsByObjectId ||= new Map();
    addLinkIndex(state.linkGroupsByObjectId, group.userData.sourceObjectId, group);
    addLinkIndex(state.linkGroupsByObjectId, group.userData.targetObjectId, group);
}

export function unindexLinkGroup(state, group) {
    removeLinkIndex(state.linkGroupsByObjectId, group.userData.sourceObjectId, group);
    removeLinkIndex(state.linkGroupsByObjectId, group.userData.targetObjectId, group);
}

function addLinkIndex(index, objectId, group) {
    if (!objectId) {
        return;
    }

    const groups = index.get(objectId) || [];
    groups.push(group);
    index.set(objectId, groups);
}

function removeLinkIndex(index, objectId, group) {
    if (!index || !objectId) {
        return;
    }

    const groups = (index.get(objectId) || []).filter(item => item !== group);
    if (groups.length) {
        index.set(objectId, groups);
    } else {
        index.delete(objectId);
    }
}

function buildLinkGeometry(source, target) {
    const sourcePosition = resolveObjectPosition(source);
    const targetPosition = resolveObjectPosition(target);
    return new THREE.BufferGeometry().setFromPoints([
        new THREE.Vector3(sourcePosition.x, 0.04, sourcePosition.z),
        new THREE.Vector3(targetPosition.x, 0.04, targetPosition.z)
    ]);
}
