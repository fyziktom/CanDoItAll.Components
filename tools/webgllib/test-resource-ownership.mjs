import { disposeAssetCache } from "../../src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/21-webgl-scene-asset-cache.js";
import {
    disposeSceneObjectTree,
    markInstanceResource,
    markOwnedMaterial,
    markSharedTemplateResource
} from "../../src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/17-webgl-scene-resources.js";

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

async function verifyTemplateCacheDisposal() {
    const counters = {};
    const texture = disposable("templateTexture", counters);
    const material = createMaterial("templateMaterial", texture, counters);
    const geometry = disposable("templateGeometry", counters);
    const template = treeWithChildren([child(material, geometry)]);
    markSharedTemplateResource(template);
    const state = {
        diagnostics: {},
        assetCache: {
            mode: "state-local",
            entries: new Map([["template", Promise.resolve({ template })]]),
            hitCount: 2,
            missCount: 1,
            disposedTemplateCount: 0
        }
    };

    disposeAssetCache(state);
    await new Promise(resolve => setTimeout(resolve, 0));
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
            state.diagnostics.assetCacheMode === "state-local"
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
    verifyDuplicateResourceDisposalIsDeduped(),
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
