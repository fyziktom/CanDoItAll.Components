export function buildSceneIndexes(sceneModel) {
    const objectById = new Map();
    const linksBySource = new Map();
    const linksByTarget = new Map();
    const symbolsByOwner = new Map();
    const layersById = new Map();
    const layerIdsByObjectId = new Map();
    const assetUsageCounts = new Map();
    const tags = new Map();

    for (const layer of sceneModel.layers || []) {
        if (!layer?.id) {
            continue;
        }

        layersById.set(layer.id, layer);
        for (const objectId of layer.objectIds || []) {
            if (!layerIdsByObjectId.has(objectId)) {
                layerIdsByObjectId.set(objectId, new Set());
            }

            layerIdsByObjectId.get(objectId).add(layer.id);
        }
    }

    for (const sceneObject of sceneModel.objects || []) {
        if (!sceneObject?.id) {
            continue;
        }

        objectById.set(sceneObject.id, sceneObject);
        if (sceneObject.assetId) {
            assetUsageCounts.set(sceneObject.assetId, (assetUsageCounts.get(sceneObject.assetId) || 0) + 1);
        }

        for (const tag of sceneObject.tags || []) {
            if (!tags.has(tag)) {
                tags.set(tag, new Set());
            }

            tags.get(tag).add(sceneObject.id);
        }

        symbolsByOwner.set(sceneObject.id, sceneObject.symbols || []);
        for (const layerId of parseLayerIds(sceneObject)) {
            if (!layerIdsByObjectId.has(sceneObject.id)) {
                layerIdsByObjectId.set(sceneObject.id, new Set());
            }

            layerIdsByObjectId.get(sceneObject.id).add(layerId);
        }
    }

    for (const link of sceneModel.links || []) {
        addToMultiMap(linksBySource, link.sourceObjectId, link);
        addToMultiMap(linksByTarget, link.targetObjectId, link);
    }

    return {
        objectById,
        linksBySource,
        linksByTarget,
        symbolsByOwner,
        layersById,
        layerIdsByObjectId,
        assetUsageCounts,
        tags
    };
}

export function isObjectVisible(state, sceneObject) {
    if (!sceneObject?.id) {
        return false;
    }

    const layerIds = state.sceneIndexes?.layerIdsByObjectId?.get(sceneObject.id);
    if (!layerIds?.size) {
        return true;
    }

    for (const layerId of layerIds) {
        const layer = state.sceneIndexes.layersById.get(layerId);
        if (layer?.isVisible === false || layer?.metadata?.isVisible === "false") {
            return false;
        }
    }

    return true;
}

export function isLinkVisible(state, link) {
    return isObjectVisible(state, state.sceneIndexes?.objectById?.get(link.sourceObjectId)) &&
        isObjectVisible(state, state.sceneIndexes?.objectById?.get(link.targetObjectId));
}

export function buildVisibilityCounts(state) {
    let visibleObjectCount = 0;
    for (const sceneObject of state.sceneModel.objects || []) {
        if (isObjectVisible(state, sceneObject)) {
            visibleObjectCount += 1;
        }
    }

    let visibleLinkCount = 0;
    for (const link of state.sceneModel.links || []) {
        if (isLinkVisible(state, link)) {
            visibleLinkCount += 1;
        }
    }

    return {
        visibleObjectCount,
        hiddenObjectCount: Math.max(0, (state.sceneModel.objects?.length || 0) - visibleObjectCount),
        visibleLinkCount,
        hiddenLinkCount: Math.max(0, (state.sceneModel.links?.length || 0) - visibleLinkCount)
    };
}

function parseLayerIds(sceneObject) {
    const metadata = sceneObject.metadata || {};
    const layerIds = [
        metadata.layerId,
        ...(metadata.layerIds || "").split(",")
    ];
    return layerIds.map(value => String(value || "").trim()).filter(Boolean);
}

function addToMultiMap(map, key, value) {
    if (!key) {
        return;
    }

    if (!map.has(key)) {
        map.set(key, []);
    }

    map.get(key).push(value);
}
