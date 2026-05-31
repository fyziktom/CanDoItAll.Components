import { clonePayload, round, resolveVector3 } from "./02-webgl-scene-core.js";
import { notifyStateChanged } from "./24-webgl-scene-notifications.js";
import {
    addLinkGroup,
    addSceneObjectGroup,
    removeLinkGroup,
    removeSceneObjectGroup,
    rebuildScene,
    replaceSceneObjectGroup,
    updateObjectRuntimeTransform
} from "./11-webgl-scene-graph.js";
import { rebuildSymbolsForObject } from "./04-webgl-scene-symbols.js";
import { clearObjectMotionState } from "./29-webgl-scene-motion-queues.js";
import {
    completeCommandResult,
    createCommandResult,
    failCommand,
    warnCommand
} from "./20-webgl-scene-command-results.js";

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

    if (normalized.baseRevision > 0 && normalized.baseRevision !== (state.sceneModel.uiState?.revision || 0)) {
        const message = `Patch base revision ${normalized.baseRevision} does not match scene revision ${state.sceneModel.uiState?.revision || 0}.`;
        if (normalized.strictBaseRevision) {
            return completeCommandResult(state, failPatch(state, result, message));
        }

        warnCommand(result, message);
    }

    let changed = false;
    for (const objectId of normalized.removeObjectIds) {
        const removed = removeSceneObject(state, objectId, result);
        if (!removed) {
            warnCommand(result, `Object '${objectId}' was not found for removal.`);
            continue;
        }

        result.affectedObjectIds.push(objectId);
        changed = true;
    }

    for (const sceneObject of normalized.addObjects) {
        const added = addSceneObject(state, sceneObject, result);
        if (!added) {
            continue;
        }

        result.affectedObjectIds.push(sceneObject.id);
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
        const exists = state.sceneModel.links.some(link => link.id === linkId);
        if (!exists) {
            warnCommand(result, `Link '${linkId}' was not found for removal.`);
            continue;
        }

        state.sceneModel.links = state.sceneModel.links.filter(link => link.id !== linkId);
        removeLinkGroup(state, linkId);
        result.affectedLinkIds.push(linkId);
        changed = true;
    }

    for (const link of normalized.addLinks) {
        if (!validateLinkForAdd(state, link, normalized, result)) {
            continue;
        }

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
    rebuildScene(state);
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

    const transformChanged = patch.position !== undefined || patch.rotation !== undefined || patch.scale !== undefined;
    const sizeChanged = patch.size !== undefined;
    const symbolOnlyChanged = patch.symbols !== undefined &&
        patch.assetId === undefined &&
        patch.color === undefined &&
        patch.metadata === undefined &&
        !transformChanged &&
        !sizeChanged;
    const visualChanged = patch.assetId !== undefined ||
        patch.color !== undefined ||
        patch.symbols !== undefined ||
        patch.metadata !== undefined;
    state.diagnostics.patchedObjectCount = (state.diagnostics.patchedObjectCount || 0) + 1;
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

    if (transformChanged) {
        cancelObjectMotions(state, patch.objectId);
    }

    if (symbolOnlyChanged) {
        state.diagnostics.symbolOnlyUpdateCount = (state.diagnostics.symbolOnlyUpdateCount || 0) + 1;
        rebuildSymbolsForObject(state, patch.objectId);
        state.scheduleRender("symbol-only-patch");
    } else if (visualChanged || sizeChanged) {
        state.diagnostics.replacedObjectGroupCount = (state.diagnostics.replacedObjectGroupCount || 0) + 1;
        replaceSceneObjectGroup(state, sceneObject);
    } else {
        updateObjectRuntimeTransform(state, patch.objectId, patch.rotation === undefined && patch.scale === undefined);
    }

    return true;
}

function addSceneObject(state, sceneObject, result) {
    if (!sceneObject?.id) {
        failPatch(state, result, "Added object id is missing.");
        return false;
    }

    state.sceneModel.objects = state.sceneModel.objects.filter(item => item.id !== sceneObject.id);
    state.sceneModel.objects.push(sceneObject);
    removeSceneObjectGroup(state, sceneObject.id);
    addSceneObjectGroup(state, sceneObject);
    return true;
}

function removeSceneObject(state, objectId, result) {
    const exists = state.sceneModel.objects.some(item => item.id === objectId);
    if (!exists) {
        return false;
    }

    const removedLinkIds = state.sceneModel.links
        .filter(link => link.sourceObjectId === objectId || link.targetObjectId === objectId)
        .map(link => link.id)
        .filter(Boolean);
    state.sceneModel.objects = state.sceneModel.objects.filter(item => item.id !== objectId);
    state.sceneModel.links = state.sceneModel.links.filter(link => link.sourceObjectId !== objectId && link.targetObjectId !== objectId);
    removeSceneObjectGroup(state, objectId);
    state.linkGroups.filter(group => group.userData.sourceObjectId === objectId || group.userData.targetObjectId === objectId)
        .map(group => group.userData.linkId)
        .forEach(linkId => removeLinkGroup(state, linkId));
    result.affectedLinkIds.push(...removedLinkIds);
    cancelObjectMotions(state, objectId);
    return true;
}

function normalizePatch(patch) {
    if (!patch) {
        return null;
    }

    return {
        sceneId: patch.sceneId || "",
        baseRevision: Number(patch.baseRevision) || 0,
        nextRevision: Number(patch.nextRevision) || 0,
        strictBaseRevision: normalizeBoolean(patch.metadata?.strictBaseRevision) || patch.metadata?.baseRevisionMode === "fail",
        missingLinkEndpointMode: String(patch.metadata?.missingLinkEndpointMode || "fail").toLowerCase(),
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
    return resolveVector3(value, fallback);
}

function failPatch(state, result, message) {
    return failCommand(state, result, message, "WebGL scene patch failed.");
}

function validateLinkForAdd(state, link, normalized, result) {
    if (!link?.id) {
        failPatch(state, result, "Added link id is missing.");
        return false;
    }

    const sourceExists = state.sceneModel.objects.some(item => item.id === link.sourceObjectId);
    const targetExists = state.sceneModel.objects.some(item => item.id === link.targetObjectId);
    if (sourceExists && targetExists) {
        return true;
    }

    const message = `Link '${link.id}' references missing endpoint(s): '${link.sourceObjectId}' -> '${link.targetObjectId}'.`;
    if (normalized.missingLinkEndpointMode === "warn") {
        warnCommand(result, message);
        return false;
    }

    failPatch(state, result, message);
    return false;
}

function cancelObjectMotions(state, objectId) {
    clearObjectMotionState(state, objectId);
}

function normalizeBoolean(value) {
    return value === true || String(value || "").toLowerCase() === "true";
}

export function cloneSceneForExport(scene) {
    return clonePayload(scene);
}
