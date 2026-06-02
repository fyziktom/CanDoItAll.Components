import { disposeSceneObjectTree } from "./17-webgl-scene-resources.js";

export function createAssetCache() {
    return {
        mode: "state-local",
        entries: new Map(),
        hitCount: 0,
        missCount: 0,
        disposedTemplateCount: 0
    };
}

export function getOrLoadTemplate(state, key, loader) {
    const cache = ensureAssetCache(state);
    if (cache.entries.has(key)) {
        cache.hitCount += 1;
        syncAssetCacheDiagnostics(state);
        return cache.entries.get(key);
    }

    cache.missCount += 1;
    const promise = loader()
        .catch(error => {
            cache.entries.delete(key);
            syncAssetCacheDiagnostics(state);
            throw error;
        });
    cache.entries.set(key, promise);
    syncAssetCacheDiagnostics(state);
    return promise;
}

export function deleteCachedTemplate(state, key) {
    const cache = ensureAssetCache(state);
    cache.entries.delete(key);
    syncAssetCacheDiagnostics(state);
}

export function disposeAssetCache(state) {
    const cache = ensureAssetCache(state);
    const promises = Array.from(cache.entries.values());
    cache.entries.clear();

    for (const promise of promises) {
        Promise.resolve(promise)
            .then(template => {
                if (template?.template) {
                    disposeSceneObjectTree(template.template, state.diagnostics);
                    cache.disposedTemplateCount += 1;
                    syncAssetCacheDiagnostics(state);
                }
            })
            .catch(() => {
            });
    }

    syncAssetCacheDiagnostics(state);
}

export function syncAssetCacheDiagnostics(state) {
    if (!state?.diagnostics) {
        return;
    }

    const cache = ensureAssetCache(state);
    state.diagnostics.assetCacheMode = cache.mode || "state-local";
    state.diagnostics.assetCacheEntryCount = cache.entries.size;
    state.diagnostics.assetCacheHitCount = cache.hitCount;
    state.diagnostics.assetCacheMissCount = cache.missCount;
    state.diagnostics.disposedTemplateCount = cache.disposedTemplateCount;
}

function ensureAssetCache(state) {
    state.assetCache = state.assetCache?.entries instanceof Map ? state.assetCache : createAssetCache();
    return state.assetCache;
}
