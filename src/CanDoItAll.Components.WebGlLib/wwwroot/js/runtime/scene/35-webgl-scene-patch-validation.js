import { resolveSceneRevision } from "./34-webgl-scene-revisions.js";

export function validatePatchForApply(state, normalized) {
    const errors = [];
    const warnings = [];
    if (normalized.sceneId && normalized.sceneId !== state.sceneModel.sceneId) {
        errors.push(`Patch scene '${normalized.sceneId}' does not match '${state.sceneModel.sceneId}'.`);
    }

    const currentRevision = resolveSceneRevision(state.sceneModel);
    if (normalized.baseRevision > 0 && normalized.baseRevision !== currentRevision) {
        const message = `Patch base revision ${normalized.baseRevision} does not match scene revision ${currentRevision}.`;
        if (normalized.strictBaseRevision) {
            errors.push(message);
        } else {
            warnings.push(message);
        }
    }

    const availableObjectIds = new Set((state.sceneModel.objects || [])
        .map(item => item.id)
        .filter(Boolean));
    for (const objectId of normalized.removeObjectIds) {
        availableObjectIds.delete(objectId);
    }

    for (const sceneObject of normalized.addObjects) {
        if (!sceneObject?.id) {
            errors.push("Added object id is missing.");
        } else {
            availableObjectIds.add(sceneObject.id);
        }
    }

    for (const objectPatch of normalized.objectPatches) {
        if (!objectPatch?.objectId) {
            errors.push("Object patch id is missing.");
            continue;
        }

        if (!availableObjectIds.has(objectPatch.objectId)) {
            errors.push(`Object patch target '${objectPatch.objectId}' was not found.`);
        }
    }

    for (const link of normalized.addLinks) {
        if (!link?.id) {
            errors.push("Added link id is missing.");
            continue;
        }

        if (availableObjectIds.has(link.sourceObjectId) && availableObjectIds.has(link.targetObjectId)) {
            continue;
        }

        const message = `Link '${link.id}' references missing endpoint(s): '${link.sourceObjectId}' -> '${link.targetObjectId}'.`;
        if (normalized.missingLinkEndpointMode === "warn") {
            warnings.push(message);
        } else {
            errors.push(message);
        }
    }

    return { valid: errors.length === 0, errors, warnings };
}

export function linkEndpointsExist(state, link) {
    const sourceExists = state.sceneModel.objects.some(item => item.id === link?.sourceObjectId);
    const targetExists = state.sceneModel.objects.some(item => item.id === link?.targetObjectId);
    return sourceExists && targetExists;
}
