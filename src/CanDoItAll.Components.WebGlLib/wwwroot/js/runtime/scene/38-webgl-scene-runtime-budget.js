export function normalizeRuntimeBudget(value) {
    return {
        profile: resolveString(value?.profile, "standard"),
        maxSceneObjects: Math.max(1, resolveFiniteNumber(value?.maxSceneObjects, 500)),
        maxLoadedAssets: Math.max(1, resolveFiniteNumber(value?.maxLoadedAssets, 128)),
        maxAssetCacheEntries: Math.max(1, resolveFiniteNumber(value?.maxAssetCacheEntries, 128)),
        maxActiveMotions: Math.max(1, resolveFiniteNumber(value?.maxActiveMotions, 256)),
        maxQueuedMotions: Math.max(1, resolveFiniteNumber(value?.maxQueuedMotions, 512)),
        maxQueuedCommandStages: Math.max(1, resolveFiniteNumber(value?.maxQueuedCommandStages, 128)),
        maxEstimatedTriangles: Math.max(1, resolveFiniteNumber(value?.maxEstimatedTriangles, 250000)),
        degradeWhenExceeded: value?.degradeWhenExceeded !== false
    };
}

export function evaluateRuntimeBudget(state, diagnostics) {
    const limits = normalizeRuntimeBudget(state?.options?.runtimeBudget);
    const warnings = [];
    pushBudgetWarning(warnings, "scene objects", state?.sceneModel?.objects?.length || 0, limits.maxSceneObjects);
    pushBudgetWarning(warnings, "loaded assets", diagnostics.loadedAssetIds?.size || 0, limits.maxLoadedAssets);
    pushBudgetWarning(warnings, "asset cache entries", diagnostics.assetCacheEntryCount || 0, limits.maxAssetCacheEntries);
    pushBudgetWarning(warnings, "active motions", state?.motions?.size || 0, limits.maxActiveMotions);
    pushBudgetWarning(warnings, "queued motions", diagnostics.queuedMotionCount || 0, limits.maxQueuedMotions);
    pushBudgetWarning(warnings, "queued command stages", diagnostics.queuedCommandStageCount || 0, limits.maxQueuedCommandStages);
    pushBudgetWarning(warnings, "estimated triangles", diagnostics.estimatedTriangleCount || 0, limits.maxEstimatedTriangles);

    return {
        profile: limits.profile,
        limits,
        warnings,
        degraded: limits.degradeWhenExceeded && warnings.length > 0
    };
}

export function buildRuntimeBudgetDiagnostics(budget) {
    return {
        runtimeBudgetProfile: budget.profile,
        degradedRenderingActive: budget.degraded,
        runtimeBudgetWarningCount: budget.warnings.length,
        runtimeBudgetWarnings: budget.warnings,
        runtimeBudgetMaxSceneObjects: budget.limits.maxSceneObjects,
        runtimeBudgetMaxLoadedAssets: budget.limits.maxLoadedAssets,
        runtimeBudgetMaxActiveMotions: budget.limits.maxActiveMotions,
        runtimeBudgetMaxQueuedMotions: budget.limits.maxQueuedMotions
    };
}

function resolveFiniteNumber(value, fallback) {
    const number = Number(value);
    return Number.isFinite(number) ? number : fallback;
}

function resolveString(value, fallback = "") {
    return typeof value === "string" && value.trim().length > 0
        ? value
        : fallback;
}

function pushBudgetWarning(warnings, label, actual, limit) {
    if (actual > limit) {
        warnings.push(`${label} ${actual} exceeds budget ${limit}`);
    }
}
