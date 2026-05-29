export function getProofSnapshot(state) {
    const symbolCount = Array.from(state.sceneModel.objects || [])
        .reduce((count, sceneObject) => count + (sceneObject.symbols || []).filter(symbol => symbol?.isVisible !== false).length, 0);
    return {
        sceneId: state.sceneModel.sceneId || "",
        objectCount: state.sceneModel.objects?.length || 0,
        linkCount: state.sceneModel.links?.length || 0,
        symbolCount,
        loadedAssetCount: state.diagnostics.loadedAssetIds.size,
        missingAssetCount: state.diagnostics.missingAssetIds.size,
        fallbackObjectCount: state.diagnostics.fallbackObjectIds.size,
        selectedObjectIds: Array.from(state.selectedObjectIds || []),
        hoveredObjectId: state.hoveredObjectId || "",
        viewportWidth: state.viewport.width,
        viewportHeight: state.viewport.height,
        metadata: {
            runtime: "webglScene",
            deterministicMode: String(state.options.deterministicMode)
        }
    };
}

