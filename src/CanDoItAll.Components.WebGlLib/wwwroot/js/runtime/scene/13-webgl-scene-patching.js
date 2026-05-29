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
    return applyPatchDetailed(state, patch).success;
}

export function applyPatchDetailed(state, patch) {
    const result = createCommandResult(state, "patch", patch?.commandId || patch?.metadata?.commandId || "");
    const normalized = normalizePatch(patch);
    if (!normalized) {
        return completeCommandResult(state, failPatch(state, result, "Patch is missing."));
    }

    if (normalized.sceneId && normalized.sceneId !== state.sceneModel.sceneId) {
        return completeCommandResult(state, failPatch(state, result, `Patch scene '${normalized.sceneId}' does not match '${state.sceneModel.sceneId}'.`));
    }

    let changed = false;
    for (const objectId of normalized.removeObjectIds) {
        removeSceneObject(state, objectId);
        result.affectedObjectIds.push(objectId);
        changed = true;
    }

    for (const sceneObject of normalized.addObjects) {
        addSceneObject(state, sceneObject);
        result.affectedObjectIds.push(sceneObject?.id || "");
        changed = true;
    }

    for (const objectPatch of normalized.objectPatches) {
        const patchResult = applyObjectPatch(state, objectPatch, result);
        if (patchResult) {
            result.affectedObjectIds.push(objectPatch.objectId);
        }

        changed = patchResult || changed;
    }

    for (const linkId of normalized.removeLinkIds) {
        state.sceneModel.links = state.sceneModel.links.filter(link => link.id !== linkId);
        removeLinkGroup(state, linkId);
        result.affectedLinkIds.push(linkId);
        changed = true;
    }

    for (const link of normalized.addLinks) {
        state.sceneModel.links = state.sceneModel.links.filter(item => item.id !== link.id);
        state.sceneModel.links.push(link);
        addLinkGroup(state, link);
        result.affectedLinkIds.push(link.id || "");
        changed = true;
    }

    if (!changed) {
        return completeCommandResult(state, result);
    }

    state.sceneModel.uiState.revision = normalized.nextRevision > 0
        ? normalized.nextRevision
        : (state.sceneModel.uiState.revision || 0) + 1;
    state.sceneModel.revision = state.sceneModel.uiState.revision;
    notifyStateChanged(state);
    state.scheduleRender("patch");
    return completeCommandResult(state, result);
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

function applyObjectPatch(state, patch, result) {
    if (!patch?.objectId) {
        failPatch(state, result, "Object patch id is missing.");
        return false;
    }

    const sceneObject = state.objectLookup.get(patch.objectId);
    if (!sceneObject) {
        failPatch(state, result, `Object '${patch.objectId}' was not found.`);
        return false;
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

function createCommandResult(state, commandKind, commandId) {
    return {
        commandId: commandId || `${commandKind}:${Date.now()}`,
        success: true,
        succeeded: true,
        sceneId: state.sceneModel.sceneId || "",
        commandKind,
        revision: state.sceneModel.uiState?.revision || 0,
        errors: [],
        warnings: [],
        affectedObjectIds: [],
        affectedLinkIds: [],
        diagnostics: {}
    };
}

function completeCommandResult(state, result) {
    result.success = result.errors.length === 0;
    result.succeeded = result.success;
    result.revision = state.sceneModel.uiState?.revision || 0;
    result.affectedObjectIds = unique(result.affectedObjectIds);
    result.affectedLinkIds = unique(result.affectedLinkIds);
    result.diagnostics = {
        renderCount: String(state.diagnostics.renderCount || 0),
        activeMotionCount: String(state.motions?.size || 0)
    };
    state.commandResults.push(result);
    return result;
}

function failPatch(state, result, message) {
    result.errors.push(message);
    state.diagnostics.failedPatchCommands.add(message);
    state.diagnostics.failedCommandDetails.push({
        commandId: result.commandId,
        success: false,
        succeeded: false,
        sceneId: result.sceneId,
        commandKind: result.commandKind,
        revision: state.sceneModel.uiState?.revision || 0,
        errors: [message],
        warnings: [],
        affectedObjectIds: [],
        affectedLinkIds: [],
        diagnostics: {}
    });
    state.diagnostics.lastError = message;
    state.notifyRuntimeError?.("WebGL scene patch failed.", new Error(message));
    state.scheduleRender("patch-failed");
    return result;
}

function unique(values) {
    return Array.from(new Set(values.filter(Boolean)));
}

export function cloneSceneForExport(scene) {
    return clonePayload(scene);
}
