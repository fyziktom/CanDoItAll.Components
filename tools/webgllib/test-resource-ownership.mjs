import { disposeAssetCache } from "../../src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/21-webgl-scene-asset-cache.js";
import {
    disposeSceneObjectTree,
    markInstanceResource,
    markOwnedMaterial,
    markSharedTemplateResource
} from "../../src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/17-webgl-scene-resources.js";

function createDeferred() {
    const deferred = {};
    deferred.promise = new Promise((resolve, reject) => {
        deferred.resolve = resolve;
        deferred.reject = reject;
    });
    return deferred;
}

function disposable(label, counters) {
    return {
        label,
        disposed: false,
        dispose() {
            this.disposed = true;
            counters[label] = (counters[label] || 0) + 1;
        }
    };
}

function createMaterial(label, texture, counters) {
    return {
        userData: {},
        map: texture,
        dispose() {
            counters[label] = (counters[label] || 0) + 1;
        }
    };
}

function treeWithChildren(children) {
    return {
        userData: {},
        children,
        traverse(callback) {
            callback(this);
            for (const child of children) {
                callback(child);
            }
        }
    };
}

function child(material, geometry) {
    return { userData: {}, material, geometry };
}

function createAssetCache(entries) {
    return {
        mode: "state-local",
        entries,
        hitCount: 0,
        missCount: entries.size,
        disposedTemplateCount: 0,
        pendingDisposalCount: 0,
        disposedPromiseCount: 0,
        disposalErrorCount: 0
    };
}

async function flushPromises() {
    await new Promise(resolve => setTimeout(resolve, 0));
}

async function verifyTemplateCacheDisposal() {
    const counters = {};
    const texture = disposable("templateTexture", counters);
    const material = createMaterial("templateMaterial", texture, counters);
    const geometry = disposable("templateGeometry", counters);
    const template = treeWithChildren([child(material, geometry)]);
    markSharedTemplateResource(template);
    const state = {
        diagnostics: {},
        assetCache: createAssetCache(new Map([["template", Promise.resolve({ template })]]))
    };
    state.assetCache.hitCount = 2;

    disposeAssetCache(state);
    await flushPromises();
    return {
        name: "template-cache-disposal",
        counters,
        diagnostics: state.diagnostics,
        pass: counters.templateGeometry === 1 &&
            counters.templateMaterial === 1 &&
            counters.templateTexture === 1 &&
            state.assetCache.disposedTemplateCount === 1 &&
            state.diagnostics.disposedGeometryCount === 1 &&
            state.diagnostics.disposedMaterialCount === 1 &&
            state.diagnostics.disposedTextureCount === 1 &&
            state.diagnostics.assetCachePendingDisposalCount === 0 &&
            state.diagnostics.assetCacheDisposedPromiseCount === 1 &&
            state.diagnostics.assetCacheDisposalErrorCount === 0 &&
            state.diagnostics.assetCacheMode === "state-local"
    };
}

async function verifyPendingTemplatePromiseDisposalDiagnostics() {
    const counters = {};
    const texture = disposable("pendingTemplateTexture", counters);
    const material = createMaterial("pendingTemplateMaterial", texture, counters);
    const geometry = disposable("pendingTemplateGeometry", counters);
    const template = treeWithChildren([child(material, geometry)]);
    markSharedTemplateResource(template);
    const pendingTemplate = createDeferred();
    const state = {
        diagnostics: {},
        assetCache: createAssetCache(new Map([["pending-template", pendingTemplate.promise]]))
    };

    disposeAssetCache(state);
    const scheduled = {
        entryCount: state.diagnostics.assetCacheEntryCount,
        pendingDisposalCount: state.diagnostics.assetCachePendingDisposalCount,
        disposedTemplateCount: state.diagnostics.disposedTemplateCount || 0
    };
    pendingTemplate.resolve({ template });
    await flushPromises();

    return {
        name: "pending-template-promise-disposal-diagnostics",
        scheduled,
        counters,
        diagnostics: state.diagnostics,
        pass: scheduled.entryCount === 0 &&
            scheduled.pendingDisposalCount === 1 &&
            scheduled.disposedTemplateCount === 0 &&
            counters.pendingTemplateGeometry === 1 &&
            counters.pendingTemplateMaterial === 1 &&
            counters.pendingTemplateTexture === 1 &&
            state.diagnostics.assetCachePendingDisposalCount === 0 &&
            state.diagnostics.assetCacheDisposedPromiseCount === 1 &&
            state.diagnostics.assetCacheDisposalErrorCount === 0 &&
            state.diagnostics.disposedTemplateCount === 1
    };
}

function verifyTintedInstanceRetainsSharedTexture() {
    const counters = {};
    const texture = disposable("sharedTexture", counters);
    const material = createMaterial("clonedMaterial", texture, counters);
    const instance = treeWithChildren([child(material, null)]);
    markInstanceResource(instance, { ownsGeometry: false, ownsMaterial: false, ownsTexture: false });
    markOwnedMaterial(material, { ownsTexture: false });
    const diagnostics = {};

    disposeSceneObjectTree(instance, diagnostics);
    return {
        name: "tinted-instance-retains-shared-texture",
        counters,
        diagnostics,
        pass: counters.clonedMaterial === 1 &&
            (counters.sharedTexture || 0) === 0 &&
            diagnostics.disposedMaterialCount === 1 &&
            (diagnostics.disposedTextureCount || 0) === 0 &&
            diagnostics.retainedSharedTextureCount === 1
    };
}

function verifyTintedInstanceTemplateOwnershipSeparation() {
    const counters = {};
    const sharedTexture = disposable("templateSharedTexture", counters);
    const templateMaterial = createMaterial("templateMaterial", sharedTexture, counters);
    const clonedMaterial = createMaterial("ownedInstanceMaterial", sharedTexture, counters);
    const templateGeometry = disposable("templateGeometry", counters);
    const template = treeWithChildren([child(templateMaterial, templateGeometry)]);
    const instance = treeWithChildren([child(clonedMaterial, null)]);
    markSharedTemplateResource(template);
    markInstanceResource(instance, { ownsGeometry: false, ownsMaterial: false, ownsTexture: false });
    markOwnedMaterial(clonedMaterial, { ownsTexture: false });
    const instanceDiagnostics = {};
    const templateDiagnostics = {};

    disposeSceneObjectTree(instance, instanceDiagnostics);
    const textureRetainedAfterInstanceDispose = (counters.templateSharedTexture || 0) === 0;
    disposeSceneObjectTree(template, templateDiagnostics);

    return {
        name: "tinted-instance-template-ownership-separation",
        counters,
        instanceDiagnostics,
        templateDiagnostics,
        pass: counters.ownedInstanceMaterial === 1 &&
            counters.templateMaterial === 1 &&
            counters.templateGeometry === 1 &&
            counters.templateSharedTexture === 1 &&
            textureRetainedAfterInstanceDispose &&
            instanceDiagnostics.disposedMaterialCount === 1 &&
            (instanceDiagnostics.disposedTextureCount || 0) === 0 &&
            instanceDiagnostics.retainedSharedTextureCount === 1 &&
            templateDiagnostics.disposedMaterialCount === 1 &&
            templateDiagnostics.disposedTextureCount === 1
    };
}

function verifyDuplicateResourceDisposalIsDeduped() {
    const counters = {};
    const texture = disposable("sharedTexture", counters);
    const material = createMaterial("sharedMaterial", texture, counters);
    const geometry = disposable("sharedGeometry", counters);
    const template = treeWithChildren([
        child(material, geometry),
        child(material, geometry)
    ]);
    markSharedTemplateResource(template);
    const diagnostics = {};

    disposeSceneObjectTree(template, diagnostics);
    return {
        name: "duplicate-resource-disposal-deduped",
        counters,
        diagnostics,
        pass: counters.sharedGeometry === 1 &&
            counters.sharedMaterial === 1 &&
            counters.sharedTexture === 1 &&
            diagnostics.disposedGeometryCount === 1 &&
            diagnostics.disposedMaterialCount === 1 &&
            diagnostics.disposedTextureCount === 1
    };
}

const results = [
    verifyTintedInstanceRetainsSharedTexture(),
    verifyTintedInstanceTemplateOwnershipSeparation(),
    verifyDuplicateResourceDisposalIsDeduped(),
    await verifyPendingTemplatePromiseDisposalDiagnostics(),
    await verifyTemplateCacheDisposal()
];
const summary = {
    proof: "WebGlLib resource ownership semantics",
    results,
    pass: results.every(result => result.pass)
};

console.log(JSON.stringify(summary, null, 2));
if (!summary.pass) {
    process.exit(1);
}
