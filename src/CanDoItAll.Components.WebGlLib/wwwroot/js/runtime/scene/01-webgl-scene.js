import {
    THREE,
    applyObjectTransform,
    buildDiagnosticsSnapshot,
    clonePayload,
    createMaterial,
    disposeObject3D,
    normalizeOptions,
    normalizeScene,
    resolveFiniteNumber,
    resolveObjectPosition,
    resolveObjectSize,
    round
} from "./02-webgl-scene-core.js";
import { buildAssetLookup, syncAssetVisual } from "./03-webgl-scene-assets.js";
import { rebuildSymbols, syncSymbolAnimation } from "./04-webgl-scene-symbols.js";
import {
    handleClick,
    handleDoubleClick,
    handlePointerDown,
    handlePointerMove,
    selectObjects,
    syncSelectionVisuals
} from "./05-webgl-scene-interaction.js";
import {
    applyCameraState,
    createCamera,
    createControls,
    fitView,
    focusObject,
    resetCamera,
    syncViewport,
    updateCameraModel
} from "./06-webgl-scene-camera.js";
import { syncOverlays } from "./07-webgl-scene-overlays.js";
import { getProofSnapshot } from "./08-webgl-scene-proof.js";

const root = window.CanDoItAll = window.CanDoItAll || {};

function buildHostShell(host) {
    host.innerHTML = "";
    host.classList.add("wgl-scene-runtime-host");

    const stage = document.createElement("div");
    stage.className = "wgl-scene-stage";

    const labelLayer = document.createElement("div");
    labelLayer.className = "wgl-scene-label-layer";

    const emptyState = document.createElement("div");
    emptyState.className = "wgl-scene-empty-state";
    const emptyCard = document.createElement("div");
    emptyCard.className = "wgl-scene-empty-state__card";
    const emptyTitle = document.createElement("p");
    emptyTitle.className = "wgl-scene-empty-state__title";
    emptyTitle.textContent = "No scene objects";
    const emptyBody = document.createElement("p");
    emptyBody.className = "wgl-scene-empty-state__body";
    emptyBody.textContent = "Add generic scene objects to render the WebGL proof surface.";
    emptyCard.append(emptyTitle, emptyBody);
    emptyState.appendChild(emptyCard);

    const diagnosticsPanel = document.createElement("div");
    diagnosticsPanel.className = "wgl-scene-diagnostics";
    const diagnosticsTitle = document.createElement("p");
    diagnosticsTitle.className = "wgl-scene-diagnostics__title";
    diagnosticsTitle.textContent = "Runtime";
    const diagnosticsMeta = document.createElement("p");
    diagnosticsMeta.className = "wgl-scene-diagnostics__meta";
    diagnosticsPanel.append(diagnosticsTitle, diagnosticsMeta);

    host.append(stage, labelLayer, emptyState, diagnosticsPanel);
    return {
        stage,
        labelLayer,
        emptyState,
        diagnosticsPanel,
        diagnosticsMeta
    };
}

function createState(host, dotNetRef, scene, options) {
    const sceneModel = normalizeScene(scene);
    const runtimeOptions = normalizeOptions(options);
    const shell = buildHostShell(host);
    const threeScene = new THREE.Scene();
    const environment = sceneModel.environment || {};
    threeScene.background = new THREE.Color(environment.backgroundColor || "#0f172a");
    if (environment.fogEnabled !== false) {
        threeScene.fog = new THREE.Fog(
            environment.fogColor || environment.backgroundColor || "#0f172a",
            resolveFiniteNumber(environment.fogNear, 80),
            resolveFiniteNumber(environment.fogFar, 180));
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

    const ambient = new THREE.AmbientLight("#f8fafc", resolveFiniteNumber(environment.ambientLightIntensity, 0.62));
    const hemisphere = new THREE.HemisphereLight("#dbeafe", "#111827", 0.9);
    const directional = new THREE.DirectionalLight("#ffffff", resolveFiniteNumber(environment.directionalLightIntensity, 1.2));
    directional.position.set(22, 38, 18);
    const rim = new THREE.DirectionalLight("#38bdf8", 0.32);
    rim.position.set(-18, 18, -24);
    threeScene.add(ambient, hemisphere, directional, rim);

    const state = {
        host,
        dotNetRef,
        shell,
        scene: threeScene,
        renderer,
        camera,
        controls,
        viewport,
        raycaster: new THREE.Raycaster(),
        sceneModel,
        initialCamera: clonePayload(sceneModel.camera || {}),
        options: runtimeOptions,
        assetLookup: buildAssetLookup(sceneModel.assetCatalog),
        assetCache: new Map(),
        objectLookup: new Map(),
        objectGroups: new Map(),
        objectPositions: new Map(),
        hitMeshes: [],
        linkGroups: [],
        symbolGroups: new Map(),
        labelElements: new Map(),
        selectedObjectIds: new Set(sceneModel.uiState?.selection?.selectedObjectIds || []),
        hoveredObjectId: sceneModel.uiState?.hoveredObjectId || "",
        pointerDown: null,
        animationHandle: 0,
        renderRequested: false,
        frame: 0,
        diagnostics: {
            createCount: 1,
            updateCount: 0,
            renderCount: 0,
            loadedAssetIds: new Set(),
            missingAssetIds: new Set(),
            fallbackObjectIds: new Set(),
            lastError: ""
        },
        decorations: {},
        handlers: {}
    };

    state.scheduleRender = () => scheduleRender(state);
    state.focusObject = objectId => {
        focusObject(state, objectId);
        scheduleRender(state);
    };

    buildDecorations(state);
    syncViewport(state, true);
    applyCameraState(state);
    rebuildScene(state);
    if (runtimeOptions.autoFitOnCreate && sceneModel.interaction?.fitViewOnCreate !== false) {
        fitView(state);
    }

    state.handlers.pointerMove = event => handlePointerMove(state, event);
    state.handlers.pointerDown = event => handlePointerDown(state, event);
    state.handlers.click = event => handleClick(state, event);
    state.handlers.doubleClick = event => handleDoubleClick(state, event);
    state.handlers.controlsChange = () => {
        updateCameraModel(state);
        scheduleRender(state);
    };
    state.handlers.resize = () => scheduleRender(state);

    renderer.domElement.addEventListener("pointermove", state.handlers.pointerMove);
    renderer.domElement.addEventListener("pointerdown", state.handlers.pointerDown);
    renderer.domElement.addEventListener("click", state.handlers.click);
    renderer.domElement.addEventListener("dblclick", state.handlers.doubleClick);
    controls.addEventListener("change", state.handlers.controlsChange);
    state.resizeObserver = new ResizeObserver(state.handlers.resize);
    state.resizeObserver.observe(host);

    host.__webglSceneState = state;
    notifyRuntimeReady(state);
    startRenderLoop(state);
    return state;
}

function buildDecorations(state) {
    const environment = state.sceneModel.environment || {};
    const groundSize = Math.max(8, resolveFiniteNumber(environment.groundSize, 36));
    const gridDivisions = Math.max(4, resolveFiniteNumber(environment.gridDivisions, 24));

    const ground = new THREE.Mesh(
        new THREE.PlaneGeometry(groundSize, groundSize),
        createMaterial(environment.groundColor || "#1f2937", {
            transparent: true,
            opacity: 0.82,
            roughness: 0.9
        }));
    ground.rotation.x = -Math.PI / 2;
    ground.position.y = -0.02;

    const grid = new THREE.GridHelper(
        groundSize,
        gridDivisions,
        environment.gridColor || "#94a3b8",
        "#334155");
    grid.position.y = 0.01;
    const materials = Array.isArray(grid.material) ? grid.material : [grid.material];
    for (const material of materials) {
        material.transparent = true;
        material.opacity = 0.34;
    }

    state.scene.add(ground, grid);
    state.decorations = {
        ground,
        grid
    };
}

function syncDecorations(state) {
    const uiState = state.sceneModel.uiState || {};
    state.decorations.ground.visible = uiState.showGround !== false;
    state.decorations.grid.visible = uiState.showGrid !== false;
}

function rebuildScene(state) {
    clearDynamicScene(state);
    state.objectLookup = new Map();
    state.objectPositions = new Map();

    for (const sceneObject of state.sceneModel.objects || []) {
        if (!sceneObject?.id) {
            continue;
        }

        state.objectLookup.set(sceneObject.id, sceneObject);
        const group = createSceneObjectGroup(state, sceneObject);
        state.objectGroups.set(sceneObject.id, group);
        state.scene.add(group);
    }

    for (const link of state.sceneModel.links || []) {
        const linkGroup = createLinkGroup(state, link);
        if (linkGroup) {
            state.linkGroups.push(linkGroup);
            state.scene.add(linkGroup);
        }
    }

    rebuildSymbols(state);
    syncSelectionVisuals(state);
    state.shell.emptyState.classList.toggle("is-visible", (state.sceneModel.objects || []).length === 0);
    scheduleRender(state);
}

function createSceneObjectGroup(state, sceneObject) {
    const group = new THREE.Group();
    group.userData = {
        objectId: sceneObject.id
    };
    applyObjectTransform(group, sceneObject);
    state.objectPositions.set(sceneObject.id, group.position.clone());

    const size = resolveObjectSize(sceneObject);
    const hitMesh = new THREE.Mesh(
        new THREE.BoxGeometry(size.x * 1.18, size.y * 1.12, size.z * 1.18),
        new THREE.MeshBasicMaterial({
            transparent: true,
            opacity: 0,
            depthWrite: false
        }));
    hitMesh.position.y = size.y / 2;
    hitMesh.userData = {
        objectId: sceneObject.id
    };

    const selectionRing = createGroundRing(size, sceneObject.color || "#38bdf8", 0.92);
    selectionRing.visible = false;
    const hoverRing = createGroundRing(size, "#facc15", 0.42);
    hoverRing.visible = false;
    group.userData.selectionRing = selectionRing;
    group.userData.hoverRing = hoverRing;

    group.add(selectionRing, hoverRing, hitMesh);
    state.hitMeshes.push(hitMesh);
    syncAssetVisual(state, sceneObject, group);
    return group;
}

function createGroundRing(size, color, opacity) {
    const radius = Math.max(size.x, size.z) * 0.62;
    const ring = new THREE.Mesh(
        new THREE.TorusGeometry(radius, Math.max(0.018, radius * 0.035), 8, 48),
        createMaterial(color, {
            transparent: true,
            opacity,
            emissive: color,
            emissiveIntensity: 0.18,
            depthWrite: false
        }));
    ring.position.y = 0.04;
    ring.rotation.x = Math.PI / 2;
    return ring;
}

function createLinkGroup(state, link) {
    const source = state.objectLookup.get(link.sourceObjectId);
    const target = state.objectLookup.get(link.targetObjectId);
    if (!source || !target) {
        return null;
    }

    const sourcePosition = resolveObjectPosition(source);
    const targetPosition = resolveObjectPosition(target);
    const points = [
        new THREE.Vector3(sourcePosition.x, 0.04, sourcePosition.z),
        new THREE.Vector3(targetPosition.x, 0.04, targetPosition.z)
    ];
    const geometry = new THREE.BufferGeometry().setFromPoints(points);
    const material = new THREE.LineBasicMaterial({
        color: link.color || "#94a3b8",
        transparent: true,
        opacity: resolveFiniteNumber(link.opacity, 0.75),
        linewidth: Math.max(1, resolveFiniteNumber(link.width, 1))
    });
    const line = new THREE.Line(geometry, material);
    const group = new THREE.Group();
    group.userData = {
        linkId: link.id || ""
    };
    group.add(line);
    return group;
}

function clearDynamicScene(state) {
    for (const group of state.objectGroups.values()) {
        state.scene.remove(group);
        group.userData.disposed = true;
        disposeObject3D(group);
    }

    for (const linkGroup of state.linkGroups) {
        state.scene.remove(linkGroup);
        disposeObject3D(linkGroup);
    }

    for (const symbolGroup of state.symbolGroups.values()) {
        state.scene.remove(symbolGroup);
        symbolGroup.userData.disposed = true;
        disposeObject3D(symbolGroup);
    }

    for (const label of state.labelElements.values()) {
        label.remove();
    }

    state.objectGroups.clear();
    state.linkGroups.length = 0;
    state.symbolGroups.clear();
    state.labelElements.clear();
    state.hitMeshes.length = 0;
    state.diagnostics.fallbackObjectIds.clear();
}

function render(state) {
    syncViewport(state);
    syncDecorations(state);
    state.controls.update();
    const elapsedSeconds = state.options.deterministicMode
        ? state.frame / 60
        : performance.now() / 1000;
    syncSymbolAnimation(state, elapsedSeconds);
    state.renderer.render(state.scene, state.camera);
    syncOverlays(state);
    state.diagnostics.renderCount += 1;
    state.frame += 1;
}

function scheduleRender(state) {
    state.renderRequested = true;
}

function startRenderLoop(state) {
    const loop = () => {
        if (!state.host.__webglSceneState) {
            return;
        }

        render(state);
        state.animationHandle = requestAnimationFrame(loop);
    };
    state.animationHandle = requestAnimationFrame(loop);
}

function updateState(state, scene, options) {
    state.sceneModel = normalizeScene(scene);
    state.options = normalizeOptions(options);
    state.assetLookup = buildAssetLookup(state.sceneModel.assetCatalog);
    state.selectedObjectIds = new Set(state.sceneModel.uiState?.selection?.selectedObjectIds || []);
    state.hoveredObjectId = state.sceneModel.uiState?.hoveredObjectId || "";
    state.diagnostics.updateCount += 1;
    rebuildScene(state);
    applyCameraState(state);
}

function resolveState(host) {
    return host?.__webglSceneState || null;
}

function exportImageData(state) {
    return state.renderer.domElement
        .toDataURL("image/png")
        .replace(/^data:image\/png;base64,/, "");
}

function notifyRuntimeReady(state) {
    if (!state.dotNetRef) {
        return;
    }

    state.dotNetRef
        .invokeMethodAsync("OnRuntimeReady", JSON.stringify({
            sceneId: state.sceneModel.sceneId || "",
            diagnostics: buildDiagnosticsSnapshot(state)
        }))
        .catch(error => console.warn("WebGL scene ready callback failed.", error));
}

function notifyRuntimeError(state, message, error) {
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

function dispose(state) {
    if (!state) {
        return;
    }

    if (state.animationHandle) {
        cancelAnimationFrame(state.animationHandle);
    }

    state.resizeObserver?.disconnect();
    state.renderer.domElement.removeEventListener("pointermove", state.handlers.pointerMove);
    state.renderer.domElement.removeEventListener("pointerdown", state.handlers.pointerDown);
    state.renderer.domElement.removeEventListener("click", state.handlers.click);
    state.renderer.domElement.removeEventListener("dblclick", state.handlers.doubleClick);
    state.controls.removeEventListener("change", state.handlers.controlsChange);
    state.controls.dispose();
    clearDynamicScene(state);
    disposeObject3D(state.decorations.ground);
    disposeObject3D(state.decorations.grid);
    state.renderer.dispose();
    state.host.innerHTML = "";
    delete state.host.__webglSceneState;
}

root.webglScene = {
    create(host, dotNetRef, scene, options) {
        if (!host) {
            return false;
        }

        try {
            const existing = resolveState(host);
            if (existing) {
                dispose(existing);
            }

            createState(host, dotNetRef, scene, options);
            return true;
        } catch (error) {
            console.error("CanDoItAll WebGL scene failed to create.", error);
            return false;
        }
    },
    update(host, scene, options) {
        const state = resolveState(host);
        if (!state) {
            return false;
        }

        try {
            updateState(state, scene, options);
            return true;
        } catch (error) {
            notifyRuntimeError(state, "WebGL scene update failed.", error);
            return false;
        }
    },
    dispose(host) {
        dispose(resolveState(host));
    },
    fitView(host) {
        const state = resolveState(host);
        if (state) {
            fitView(state);
        }
    },
    focusObject(host, objectId) {
        const state = resolveState(host);
        if (state) {
            focusObject(state, objectId);
        }
    },
    resetCamera(host) {
        const state = resolveState(host);
        if (state) {
            resetCamera(state);
        }
    },
    getState(host) {
        const state = resolveState(host);
        return state ? JSON.stringify(state.sceneModel.uiState || {}) : "{}";
    },
    getDiagnostics(host) {
        const state = resolveState(host);
        return state ? buildDiagnosticsSnapshot(state) : null;
    },
    getProofSnapshot(host) {
        const state = resolveState(host);
        return state ? getProofSnapshot(state) : null;
    },
    exportImageData(host) {
        const state = resolveState(host);
        return state ? exportImageData(state) : null;
    },
    triggerSelectObject(host, objectId) {
        const state = resolveState(host);
        if (!state) {
            return false;
        }

        selectObjects(state, objectId ? [objectId] : []);
        return true;
    }
};

