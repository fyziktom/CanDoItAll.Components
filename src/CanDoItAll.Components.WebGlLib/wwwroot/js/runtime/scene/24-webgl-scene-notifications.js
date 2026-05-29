import { THREE } from "./02-webgl-scene-core.js";

export function resolvePointer(state, event) {
    const rect = state.renderer.domElement.getBoundingClientRect();
    return {
        rect,
        x: event.clientX - rect.left,
        y: event.clientY - rect.top,
        ndc: new THREE.Vector2(
            ((event.clientX - rect.left) / Math.max(rect.width, 1)) * 2 - 1,
            -(((event.clientY - rect.top) / Math.max(rect.height, 1)) * 2 - 1))
    };
}

export function notifyStateChanged(state) {
    state.dotNetRef?.invokeMethodAsync("OnSceneStateChanged", JSON.stringify(state.sceneModel.uiState || {}))
        .catch(error => console.warn("WebGL scene state callback failed.", error));
}

export function notifyObjectsMoved(state, positions) {
    if (!positions.length) {
        return;
    }

    state.dotNetRef?.invokeMethodAsync("OnObjectsMoved", JSON.stringify({ positions }))
        .catch(error => console.warn("WebGL scene objects moved callback failed.", error));
}
