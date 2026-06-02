export const patchClassifications = Object.freeze({
    noOp: "no-op",
    transformOnly: "transform-only",
    symbolOnly: "symbol-only",
    linkOnly: "link-only",
    visualReplace: "visual-replace",
    mixedIncremental: "mixed-incremental",
    graphStructure: "graph-structure",
    sceneRebuild: "scene-rebuild"
});

export function classifyPatch(normalized) {
    const objectPatches = normalized.objectPatches || [];
    const hasObjectStructure = normalized.addObjects.length > 0 || normalized.removeObjectIds.length > 0;
    const hasLinkStructure = normalized.addLinks.length > 0 || normalized.removeLinkIds.length > 0;
    if (hasObjectStructure) {
        return createClassification(patchClassifications.graphStructure, true, objectPatches.length, hasLinkStructure);
    }

    if (!objectPatches.length) {
        return hasLinkStructure
            ? createClassification(patchClassifications.linkOnly, false, 0, true, true)
            : createClassification(patchClassifications.noOp, false, 0, false);
    }

    const objectKinds = objectPatches.map(classifyObjectPatch);
    const activeKinds = objectKinds.filter(kind => kind !== patchClassifications.noOp);
    if (!activeKinds.length) {
        return hasLinkStructure
            ? createClassification(patchClassifications.linkOnly, false, objectPatches.length, true, true)
            : createClassification(patchClassifications.noOp, false, objectPatches.length, false);
    }

    const allTransform = activeKinds.every(kind => kind === patchClassifications.transformOnly);
    const allSymbols = activeKinds.every(kind => kind === patchClassifications.symbolOnly);
    const allVisual = activeKinds.every(kind => kind === patchClassifications.visualReplace);
    if (!hasLinkStructure && allTransform) {
        return createClassification(patchClassifications.transformOnly, false, objectPatches.length, false);
    }

    if (!hasLinkStructure && allSymbols) {
        return createClassification(patchClassifications.symbolOnly, false, objectPatches.length, false);
    }

    if (!hasLinkStructure && allVisual) {
        return createClassification(patchClassifications.visualReplace, false, objectPatches.length, false);
    }

    if (activeKinds.every(isIncrementalObjectKind)) {
        return createClassification(patchClassifications.mixedIncremental, false, objectPatches.length, hasLinkStructure, hasLinkStructure);
    }

    return createClassification(patchClassifications.sceneRebuild, true, objectPatches.length, hasLinkStructure);
}

export function recordPatchClassificationDiagnostics(state, classification) {
    state.diagnostics.lastPatchClassification = classification.kind;
    switch (classification.kind) {
        case patchClassifications.transformOnly:
            state.diagnostics.transformOnlyPatchCount = (state.diagnostics.transformOnlyPatchCount || 0) + 1;
            break;
        case patchClassifications.symbolOnly:
            state.diagnostics.symbolOnlyPatchCount = (state.diagnostics.symbolOnlyPatchCount || 0) + 1;
            break;
        case patchClassifications.linkOnly:
            state.diagnostics.linkOnlyPatchCount = (state.diagnostics.linkOnlyPatchCount || 0) + 1;
            break;
        case patchClassifications.visualReplace:
            state.diagnostics.visualReplacePatchCount = (state.diagnostics.visualReplacePatchCount || 0) + 1;
            break;
        case patchClassifications.graphStructure:
            state.diagnostics.graphStructurePatchCount = (state.diagnostics.graphStructurePatchCount || 0) + 1;
            break;
        case patchClassifications.sceneRebuild:
            break;
        default:
            state.diagnostics.mixedIncrementalPatchCount = (state.diagnostics.mixedIncrementalPatchCount || 0) + 1;
            break;
    }
}

function classifyObjectPatch(patch) {
    const hasTransform = patch.position !== undefined || patch.rotation !== undefined || patch.scale !== undefined;
    const hasSize = patch.size !== undefined;
    const hasSymbols = patch.symbols !== undefined;
    const hasVisual = patch.assetId !== undefined || patch.color !== undefined || patch.metadata !== undefined;
    if (hasTransform && !hasSize && !hasSymbols && !hasVisual) {
        return patchClassifications.transformOnly;
    }

    if (hasSymbols && !hasTransform && !hasSize && !hasVisual) {
        return patchClassifications.symbolOnly;
    }

    if (hasSize || hasSymbols || hasVisual) {
        return patchClassifications.visualReplace;
    }

    return patchClassifications.noOp;
}

function isIncrementalObjectKind(kind) {
    return kind === patchClassifications.transformOnly ||
        kind === patchClassifications.symbolOnly ||
        kind === patchClassifications.visualReplace;
}

function createClassification(kind, shouldRebuildScene, objectPatchCount, hasLinkChanges, shouldSyncIndexes = false) {
    return {
        kind,
        shouldRebuildScene,
        objectPatchCount,
        hasLinkChanges,
        shouldSyncIndexes
    };
}
