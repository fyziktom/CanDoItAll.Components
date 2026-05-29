import {
    createState,
    dispose,
    exportImageData,
    exportScene,
    importScene,
    notifyCreateError,
    resolveState,
    updateState
} from "./10-webgl-scene-lifecycle.js";
import { selectObjects } from "./05-webgl-scene-interaction.js";
import { focusObject, fitView, resetCamera } from "./06-webgl-scene-camera.js";
import { getProofSnapshot } from "./08-webgl-scene-proof.js";
import { applyPatch, moveObject, setObjectTransform } from "./13-webgl-scene-patching.js";
import { clearMotions, enqueueMotion } from "./14-webgl-scene-motion.js";
import { buildDiagnosticsSnapshot } from "./02-webgl-scene-core.js";

const root = window.CanDoItAll = window.CanDoItAll || {};

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
            notifyCreateError(dotNetRef, scene?.sceneId || "", error);
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
            state.notifyRuntimeError("WebGL scene update failed.", error);
            return false;
        }
    },
    importScene(host, scene, options) {
        const state = resolveState(host);
        if (!state) {
            return false;
        }

        try {
            return importScene(state, scene, options);
        } catch (error) {
            state.notifyRuntimeError("WebGL scene import failed.", error);
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
            state.scheduleRender("fit-view");
        }
    },
    focusObject(host, objectId) {
        const state = resolveState(host);
        if (state) {
            focusObject(state, objectId);
            state.scheduleRender("focus-object");
        }
    },
    resetCamera(host) {
        const state = resolveState(host);
        if (state) {
            resetCamera(state);
            state.scheduleRender("reset-camera");
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
    exportScene(host) {
        const state = resolveState(host);
        return state ? exportScene(state) : null;
    },
    exportImageData(host) {
        const state = resolveState(host);
        return state ? exportImageData(state) : null;
    },
    applyPatch(host, patch) {
        const state = resolveState(host);
        return state ? applyPatch(state, patch) : false;
    },
    setObjectTransform(host, objectId, transform) {
        const state = resolveState(host);
        return state ? setObjectTransform(state, objectId, transform) : false;
    },
    moveObject(host, objectId, position) {
        const state = resolveState(host);
        return state ? moveObject(state, objectId, position) : false;
    },
    enqueueMotion(host, command) {
        const state = resolveState(host);
        return state ? enqueueMotion(state, command) : false;
    },
    clearMotions(host, objectId) {
        const state = resolveState(host);
        return state ? clearMotions(state, objectId) : false;
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
