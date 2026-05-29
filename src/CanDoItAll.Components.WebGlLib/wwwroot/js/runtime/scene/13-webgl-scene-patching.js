import { clonePayload, round } from "./02-webgl-scene-core.js";
import { notifyStateChanged } from "./05-webgl-scene-interaction.js";
import {
    addLinkGroup,
    addSceneObjectGroup,
    removeLinkGroup,
    removeSceneObjectGroup,
    replaceSceneObjectGroup,
    updateObjectRuntimeTransform
} from "./11-webgl-scene-graph.js";

export function applyPatch(state, patch) {
    const normalized = normalizePatch(patch);
    if (!normalized) {
        return failPatch(state, "Patch is missing.");
    }

    if (normalized.sceneId && normalized.sceneId !== state.sceneModel.sceneId) {
        return failPatch(state, `Patch scene '${normalized.sceneId}' does not match '${state.sceneModel.sceneId}'.`);
    }

    let changed = false;
    for (const objectId of normalized.removeObjectIds) {
        removeSceneObject(state, objectId);
        changed = true;
    }

    for (const sceneObject of normalized.addObjects) {
        addSceneObject(state, sceneObject);
        changed = true;
    }

    for (const objectPatch of normalized.objectPatches) {
        changed = applyObjectPatch(state, objectPatch) || changed;
    }

    for (const linkId of normalized.removeLinkIds) {
        state.sceneModel.links = state.sceneModel.links.filter(link => link.id !== linkId);
        removeLinkGroup(state, linkId);
        changed = true;
    }

    for (const link of normalized.addLinks) {
        state.sceneModel.links = state.sceneModel.links.filter(item => item.id !== link.id);
        state.sceneModel.links.push(link);
        addLinkGroup(state, link);
        changed = true;
    }

    if (!changed) {
        return true;
    }

    state.sceneModel.uiState.revision = normalized.nextRevision > 0
        ? normalized.nextRevision
        : (state.sceneModel.uiState.revision || 0) + 1;
    state.sceneModel.revision = state.sceneModel.uiState.revision;
    notifyStateChanged(state);
    state.scheduleRender("patch");
    return true;
}

export function setObjectTransform(state, objectId, transform) {
    return applyPatch(state, {
        sceneId: state.sceneModel.sceneId,
        objectPatches: [{
            objectId,
            position: transform?.position,
            rotation: transform?.rotation,
            scale: transform?.scale,
            size: transform?.size
        }]
    });
}

export function moveObject(state, objectId, position) {
    return applyPatch(state, {
        sceneId: state.sceneModel.sceneId,
        objectPatches: [{ objectId, position }]
    });
}

function applyObjectPatch(state, patch) {
    if (!patch?.objectId) {
        return failPatch(state, "Object patch id is missing.");
    }

    const sceneObject = state.objectLookup.get(patch.objectId);
    if (!sceneObject) {
        return failPatch(state, `Object '${patch.objectId}' was not found.`);
    }

    const visualChanged = patch.assetId !== undefined ||
        patch.color !== undefined ||
        patch.symbols !== undefined ||
        patch.metadata !== undefined;
    if (patch.position !== undefined) {
        sceneObject.position = normalizePosition(patch.position);
    }

    if (patch.rotation !== undefined) {
        sceneObject.rotation = normalizeVector(patch.rotation, sceneObject.rotation);
    }

    if (patch.scale !== undefined) {
        sceneObject.scale = normalizeVector(patch.scale, sceneObject.scale);
    }

    if (patch.size !== undefined) {
        sceneObject.size = normalizeVector(patch.size, sceneObject.size);
    }

    if (patch.assetId !== undefined) {
        sceneObject.assetId = patch.assetId || "";
    }

    if (patch.color !== undefined) {
        sceneObject.color = patch.color || sceneObject.color;
    }

    if (patch.symbols !== undefined) {
        sceneObject.symbols = Array.isArray(patch.symbols) ? patch.symbols : [];
    }

    if (patch.metadata !== undefined) {
        sceneObject.metadata = patch.metadata || {};
    }

    if (visualChanged) {
        replaceSceneObjectGroup(state, sceneObject);
    } else {
        updateObjectRuntimeTransform(state, patch.objectId, patch.rotation === undefined && patch.scale === undefined);
    }

    return true;
}

function addSceneObject(state, sceneObject) {
    if (!sceneObject?.id) {
        failPatch(state, "Added object id is missing.");
        return;
    }

    state.sceneModel.objects = state.sceneModel.objects.filter(item => item.id !== sceneObject.id);
    state.sceneModel.objects.push(sceneObject);
    removeSceneObjectGroup(state, sceneObject.id);
    addSceneObjectGroup(state, sceneObject);
}

function removeSceneObject(state, objectId) {
    state.sceneModel.objects = state.sceneModel.objects.filter(item => item.id !== objectId);
    state.sceneModel.links = state.sceneModel.links.filter(link => link.sourceObjectId !== objectId && link.targetObjectId !== objectId);
    removeSceneObjectGroup(state, objectId);
    state.linkGroups.filter(group => group.userData.sourceObjectId === objectId || group.userData.targetObjectId === objectId)
        .map(group => group.userData.linkId)
        .forEach(linkId => removeLinkGroup(state, linkId));
}

function normalizePatch(patch) {
    if (!patch) {
        return null;
    }

    return {
        sceneId: patch.sceneId || "",
        baseRevision: Number(patch.baseRevision) || 0,
        nextRevision: Number(patch.nextRevision) || 0,
        objectPatches: Array.isArray(patch.objectPatches) ? patch.objectPatches : [],
        addObjects: Array.isArray(patch.addObjects) ? patch.addObjects : [],
        removeObjectIds: Array.isArray(patch.removeObjectIds) ? patch.removeObjectIds : [],
        addLinks: Array.isArray(patch.addLinks) ? patch.addLinks : [],
        removeLinkIds: Array.isArray(patch.removeLinkIds) ? patch.removeLinkIds : []
    };
}

function normalizePosition(value) {
    const vector = normalizeVector(value, { x: 0, y: 0, z: 0 });
    return { x: round(vector.x, 3), y: round(vector.y, 3), z: round(vector.z, 3) };
}

function normalizeVector(value, fallback) {
    return {
        x: Number.isFinite(Number(value?.x ?? value?.X)) ? Number(value.x ?? value.X) : Number(fallback?.x ?? fallback?.X ?? 0),
        y: Number.isFinite(Number(value?.y ?? value?.Y)) ? Number(value.y ?? value.Y) : Number(fallback?.y ?? fallback?.Y ?? 0),
        z: Number.isFinite(Number(value?.z ?? value?.Z)) ? Number(value.z ?? value.Z) : Number(fallback?.z ?? fallback?.Z ?? 0)
    };
}

function failPatch(state, message) {
    state.diagnostics.failedPatchCommands.add(message);
    state.diagnostics.lastError = message;
    state.notifyRuntimeError?.("WebGL scene patch failed.", new Error(message));
    state.scheduleRender("patch-failed");
    return false;
}

export function cloneSceneForExport(scene) {
    return clonePayload(scene);
}
