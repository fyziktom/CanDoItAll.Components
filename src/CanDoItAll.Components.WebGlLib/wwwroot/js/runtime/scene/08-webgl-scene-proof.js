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
        modelInstanceCount: state.diagnostics.modelInstanceIds.size,
        primitiveInstanceCount: state.diagnostics.primitiveInstanceIds.size,
        estimatedTriangleCount: state.diagnostics.estimatedTriangleCount || 0,
        estimatedVertexCount: state.diagnostics.estimatedVertexCount || 0,
        activeMotionCount: state.motions?.size || 0,
        activeAssetProfile: state.sceneModel.uiState?.activeAssetProfile || state.options.assetQualityProfile || "primitive",
        largestLoadedAssetId: state.diagnostics.largestLoadedAssetId || "",
        selectedObjectIds: Array.from(state.selectedObjectIds || []),
        hoveredObjectId: state.hoveredObjectId || "",
        viewportWidth: state.viewport.width,
        viewportHeight: state.viewport.height,
        metadata: {
            runtime: "webglScene",
            deterministicMode: String(state.options.deterministicMode),
            renderMode: state.options.renderMode || "auto",
            lastFrameReason: state.diagnostics.lastFrameReason || "",
            frameTimeMs: String(Math.round((state.diagnostics.frameTimeMs || 0) * 100) / 100)
        }
    };
}
