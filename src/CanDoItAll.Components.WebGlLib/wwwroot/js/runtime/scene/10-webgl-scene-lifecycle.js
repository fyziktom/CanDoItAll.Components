import {
    THREE,
    buildDiagnosticsSnapshot,
    clonePayload,
    normalizeOptions,
    normalizeScene,
    resolveActiveAssetProfile
} from "./02-webgl-scene-core.js";
import { buildAssetLookup } from "./03-webgl-scene-assets.js";
import {
    handleClick,
    handleDoubleClick,
    handleKeyDown,
    handlePointerCancel,
    handlePointerDown,
    handlePointerMove,
    handlePointerUp
} from "./05-webgl-scene-interaction.js";
import {
    applyCameraState,
    createCamera,
    createControls,
    fitView,
    focusObject,
    syncViewport,
    updateCameraModel
} from "./06-webgl-scene-camera.js";
import { buildDecorations, clearDynamicScene, rebuildScene } from "./11-webgl-scene-graph.js";
import { attachRenderLoop } from "./15-webgl-scene-render-loop.js";
import { disposeSceneObjectTree } from "./17-webgl-scene-resources.js";
import { buildHostShell } from "./19-webgl-scene-shell.js";
import { completeCommandResult, createCommandResult } from "./20-webgl-scene-command-results.js";
import { createAssetCache, disposeAssetCache } from "./21-webgl-scene-asset-cache.js";
import { createDiagnostics } from "./25-webgl-scene-diagnostics.js";

export function createState(host, dotNetRef, scene, options) {
    const sceneModel = normalizeScene(scene);
    const runtimeOptions = normalizeOptions(options);
    sceneModel.uiState.activeAssetProfile = resolveActiveAssetProfile({ sceneModel, options: runtimeOptions });
    const shell = buildHostShell(host);
    const threeScene = new THREE.Scene();
    const environment = sceneModel.environment || {};
    threeScene.background = new THREE.Color(environment.backgroundColor || "#0f172a");
    if (environment.fogEnabled !== false) {
        threeScene.fog = new THREE.Fog(
            environment.fogColor || environment.backgroundColor || "#0f172a",
            environment.fogNear || 80,
            environment.fogFar || 180);
    }

    const renderer = new THREE.WebGLRenderer({
        antialias: runtimeOptions.enableAntialiasing,
        alpha: true,
        preserveDrawingBuffer: runtimeOptions.preserveDrawingBuffer
    });
    renderer.setPixelRatio(Math.min(window.devicePixelRatio || 1, runtimeOptions.maximumDevicePixelRatio));
    renderer.outputColorSpace = THREE.SRGBColorSpace;
    renderer.domElement.style.touchAction = "none";
    shell.stage.appendChild(renderer.domElement);

    const viewport = {
        width: Math.max(host.clientWidth || 1, 1),
        height: Math.max(host.clientHeight || 1, 1)
    };
    const camera = createCamera(sceneModel, viewport);
    const controls = createControls(camera, renderer.domElement, host, sceneModel);
    threeScene.add(camera);
    addLights(threeScene, environment);

    const state = buildState(host, dotNetRef, sceneModel, runtimeOptions, shell, threeScene, renderer, camera, controls, viewport);
    state.scheduleRender = () => {};
    state.notifyRuntimeError = (message, error) => notifyRuntimeError(state, message, error);
    state.focusObject = objectId => {
        focusObject(state, objectId);
        state.scheduleRender("focus-object");
    };

    buildDecorations(state);
    syncViewport(state, true);
    applyCameraState(state);
    rebuildScene(state);
    if (runtimeOptions.autoFitOnCreate && sceneModel.interaction?.fitViewOnCreate !== false) {
        fitView(state);
    }

    attachHandlers(state);
    host.__webglSceneState = state;
    attachRenderLoop(state);
    notifyRuntimeReady(state);
    return state;
}

export function updateState(state, scene, options) {
    state.sceneModel = normalizeScene(scene);
    state.options = normalizeOptions(options);
    state.sceneModel.uiState.activeAssetProfile = resolveActiveAssetProfile(state);
    state.assetLookup = buildAssetLookup(state.sceneModel.assetCatalog);
    state.selectedObjectIds = new Set(state.sceneModel.uiState?.selection?.selectedObjectIds || []);
    state.hoveredObjectId = state.sceneModel.uiState?.hoveredObjectId || "";
    state.diagnostics.updateCount += 1;
    rebuildScene(state);
    applyCameraState(state);
    state.scheduleRender("scene-update");
}

export function importScene(state, scene, options) {
    return importSceneDetailed(state, scene, options).success;
}

export function importSceneDetailed(state, scene, options) {
    const result = createCommandResult(state, "scene-import", scene?.sceneId || "");
    updateState(state, scene, options || state.options);
    state.scheduleRender("scene-import");
    result.affectedObjectIds.push(...(state.sceneModel.objects || []).map(item => item.id));
    result.affectedLinkIds.push(...(state.sceneModel.links || []).map(item => item.id));
    return completeCommandResult(state, result);
}

export function resolveState(host) {
    return host?.__webglSceneState || null;
}

export function exportImageData(state) {
    return state.renderer.domElement
        .toDataURL("image/png")
        .replace(/^data:image\/png;base64,/, "");
}

export function exportScene(state) {
    updateCameraModel(state);
    const exported = clonePayload(state.sceneModel);
    exported.uiState = exported.uiState || {};
    exported.uiState.hoveredObjectId = state.hoveredObjectId || "";
    exported.uiState.selection = {
        selectedObjectIds: Array.from(state.selectedObjectIds || []),
        primaryObjectId: Array.from(state.selectedObjectIds || [])[0] || "",
        contextActionId: ""
    };
    exported.uiState.activeAssetProfile = resolveActiveAssetProfile(state);
    exported.uiState.revision = exported.uiState.revision || state.sceneModel.uiState?.revision || 0;
    return exported;
}

export function notifyCreateError(dotNetRef, sceneId, error) {
    if (!dotNetRef) {
        return;
    }

    dotNetRef
        .invokeMethodAsync("OnRuntimeError", JSON.stringify({
            sceneId: sceneId || "",
            message: "WebGL scene create failed.",
            detail: error?.message || String(error || "Create returned false.")
        }))
        .catch(callbackError => console.warn("WebGL scene create error callback failed.", callbackError));
}

export function notifyRuntimeError(state, message, error) {
    state.diagnostics.lastError = error?.message || String(error || message);
    if (!state.dotNetRef) {
        return;
    }

    state.dotNetRef
        .invokeMethodAsync("OnRuntimeError", JSON.stringify({
            sceneId: state.sceneModel.sceneId || "",
            message,
            detail: state.diagnostics.lastError
        }))
        .catch(callbackError => console.warn("WebGL scene error callback failed.", callbackError));
}

export function dispose(state) {
    if (!state) {
        return;
    }

    if (state.animationHandle) {
        cancelAnimationFrame(state.animationHandle);
    }

    state.resizeObserver?.disconnect();
    state.renderer.domElement.removeEventListener("pointermove", state.handlers.pointerMove);
    state.renderer.domElement.removeEventListener("pointerdown", state.handlers.pointerDown);
    state.renderer.domElement.removeEventListener("pointerup", state.handlers.pointerUp);
    state.renderer.domElement.removeEventListener("pointercancel", state.handlers.pointerCancel);
    state.renderer.domElement.removeEventListener("click", state.handlers.click);
    state.renderer.domElement.removeEventListener("dblclick", state.handlers.doubleClick);
    state.host.removeEventListener("keydown", state.handlers.keyDown);
    state.controls.removeEventListener("change", state.handlers.controlsChange);
    state.controls.dispose();
    clearDynamicScene(state);
    disposeAssetCache(state);
    for (const decoration of Object.values(state.decorations || {})) {
        state.scene.remove(decoration);
        disposeSceneObjectTree(decoration, state.diagnostics);
    }

    state.renderer.dispose();
    state.diagnostics.disposeCount += 1;
    state.host.replaceChildren();
    delete state.host.__webglSceneState;
}

function addLights(scene, environment) {
    scene.add(new THREE.AmbientLight("#f8fafc", environment.ambientLightIntensity || 0.62));
    scene.add(new THREE.HemisphereLight("#dbeafe", "#111827", 0.9));
    const directional = new THREE.DirectionalLight("#ffffff", environment.directionalLightIntensity || 1.2);
    directional.position.set(22, 38, 18);
    const rim = new THREE.DirectionalLight("#38bdf8", 0.32);
    rim.position.set(-18, 18, -24);
    scene.add(directional, rim);
}

function buildState(host, dotNetRef, sceneModel, options, shell, scene, renderer, camera, controls, viewport) {
    return {
        host,
        dotNetRef,
        shell,
        scene,
        renderer,
        camera,
        controls,
        viewport,
        raycaster: new THREE.Raycaster(),
        sceneModel,
        initialCamera: clonePayload(sceneModel.camera || {}),
        options,
        assetLookup: buildAssetLookup(sceneModel.assetCatalog),
        assetCache: createAssetCache(),
        objectLookup: new Map(),
        objectGroups: new Map(),
        objectPositions: new Map(),
        hitMeshes: [],
        linkGroups: [],
        linkGroupsByObjectId: new Map(),
        symbolGroups: new Map(),
        labelElements: new Map(),
        selectedObjectIds: new Set(sceneModel.uiState?.selection?.selectedObjectIds || []),
        hoveredObjectId: sceneModel.uiState?.hoveredObjectId || "",
        motions: new Map(),
        motionQueuesByObjectId: new Map(),
        pendingCommandStages: [],
        dragState: null,
        animationHandle: 0,
        renderRequested: true,
        renderReason: "create",
        frame: 0,
        lastRenderTimestamp: 0,
        cameraDampingFrames: 0,
        diagnostics: createDiagnostics(),
        commandResults: [],
        nextCommandSequence: 0,
        nextMotionSequence: 0,
        decorations: {},
        handlers: {}
    };
}

function attachHandlers(state) {
    state.handlers.pointerMove = event => handlePointerMove(state, event);
    state.handlers.pointerDown = event => handlePointerDown(state, event);
    state.handlers.pointerUp = event => handlePointerUp(state, event);
    state.handlers.pointerCancel = event => handlePointerCancel(state, event);
    state.handlers.keyDown = event => handleKeyDown(state, event);
    state.handlers.click = event => handleClick(state, event);
    state.handlers.doubleClick = event => handleDoubleClick(state, event);
    state.handlers.controlsChange = () => {
        updateCameraModel(state);
        state.cameraDampingFrames = 12;
        state.scheduleRender("camera");
    };
    state.handlers.resize = () => state.scheduleRender("resize");

    state.renderer.domElement.addEventListener("pointermove", state.handlers.pointerMove);
    state.renderer.domElement.addEventListener("pointerdown", state.handlers.pointerDown);
    state.renderer.domElement.addEventListener("pointerup", state.handlers.pointerUp);
    state.renderer.domElement.addEventListener("pointercancel", state.handlers.pointerCancel);
    state.renderer.domElement.addEventListener("click", state.handlers.click);
    state.renderer.domElement.addEventListener("dblclick", state.handlers.doubleClick);
    state.host.addEventListener("keydown", state.handlers.keyDown);
    state.controls.addEventListener("change", state.handlers.controlsChange);
    state.resizeObserver = new ResizeObserver(state.handlers.resize);
    state.resizeObserver.observe(state.host);
}

function notifyRuntimeReady(state) {
    state.dotNetRef?.invokeMethodAsync("OnRuntimeReady", JSON.stringify({
        sceneId: state.sceneModel.sceneId || "",
        diagnostics: buildDiagnosticsSnapshot(state)
    })).catch(error => console.warn("WebGL scene ready callback failed.", error));
}
