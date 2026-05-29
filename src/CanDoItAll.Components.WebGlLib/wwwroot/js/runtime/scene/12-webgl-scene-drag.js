import { THREE, round, resolveObjectPosition } from "./02-webgl-scene-core.js";
import { notifyObjectsMoved, notifyStateChanged, resolvePointer } from "./05-webgl-scene-interaction.js";
import { updateObjectRuntimeTransform } from "./11-webgl-scene-graph.js";

const dragThresholdPixels = 6;
const clickSuppressMs = 250;

export function beginDrag(state, event, hit) {
    if (state.sceneModel.interaction?.allowDragOnGroundPlane !== true || !hit?.objectId) {
        state.dragState = null;
        return false;
    }

    const sceneObject = state.objectLookup.get(hit.objectId);
    if (!sceneObject?.isDraggable) {
        state.dragState = null;
        return false;
    }

    const startPosition = resolveObjectPosition(sceneObject);
    const groundPoint = intersectGroundPlane(state, event, startPosition.y);
    if (!groundPoint) {
        state.dragState = null;
        return false;
    }

    state.dragState = {
        pointerId: event.pointerId,
        objectId: hit.objectId,
        started: false,
        startClientX: event.clientX,
        startClientY: event.clientY,
        startPosition,
        offset: new THREE.Vector3(
            groundPoint.x - startPosition.x,
            0,
            groundPoint.z - startPosition.z),
        suppressClickUntil: 0
    };

    try {
        event.currentTarget?.setPointerCapture?.(event.pointerId);
    } catch {
    }
    return true;
}

export function updateDrag(state, event) {
    const drag = state.dragState;
    if (!drag || drag.pointerId !== event.pointerId) {
        return false;
    }

    const moved = Math.hypot(event.clientX - drag.startClientX, event.clientY - drag.startClientY);
    if (!drag.started && moved < dragThresholdPixels) {
        return false;
    }

    const sceneObject = state.objectLookup.get(drag.objectId);
    const groundPoint = intersectGroundPlane(state, event, drag.startPosition.y);
    if (!sceneObject || !groundPoint) {
        return false;
    }

    drag.started = true;
    state.controls.enabled = false;
    const nextPosition = {
        x: round(groundPoint.x - drag.offset.x, 3),
        y: round(drag.startPosition.y, 3),
        z: round(groundPoint.z - drag.offset.z, 3)
    };
    sceneObject.position = nextPosition;
    updateObjectRuntimeTransform(state, drag.objectId, true);
    notifyStateChanged(state);
    event.preventDefault();
    return true;
}

export function completeDrag(state, event) {
    const drag = state.dragState;
    if (!drag || drag.pointerId !== event.pointerId) {
        return false;
    }

    try {
        event.currentTarget?.releasePointerCapture?.(event.pointerId);
    } catch {
    }
    state.controls.enabled = true;
    state.dragState = null;
    if (!drag.started) {
        return false;
    }

    const sceneObject = state.objectLookup.get(drag.objectId);
    if (!sceneObject) {
        return false;
    }

    drag.suppressClickUntil = performance.now() + clickSuppressMs;
    state.lastDragSuppressClickUntil = drag.suppressClickUntil;
    notifyObjectsMoved(state, [{
        objectId: drag.objectId,
        position: sceneObject.position
    }]);
    notifyStateChanged(state);
    state.scheduleRender("drag-commit");
    return true;
}

export function cancelDrag(state, event) {
    const drag = state.dragState;
    if (!drag) {
        return false;
    }

    const sceneObject = state.objectLookup.get(drag.objectId);
    if (sceneObject) {
        sceneObject.position = drag.startPosition;
        updateObjectRuntimeTransform(state, drag.objectId, true);
        notifyStateChanged(state);
    }

    if (event?.pointerId === drag.pointerId) {
        try {
            event.currentTarget?.releasePointerCapture?.(event.pointerId);
        } catch {
        }
    }

    state.controls.enabled = true;
    state.dragState = null;
    state.scheduleRender("drag-cancel");
    return true;
}

export function isClickSuppressed(state) {
    return performance.now() < (state.lastDragSuppressClickUntil || 0);
}

function intersectGroundPlane(state, event, y) {
    const pointer = resolvePointer(state, event);
    const plane = new THREE.Plane(new THREE.Vector3(0, 1, 0), -y);
    const point = new THREE.Vector3();
    state.raycaster.setFromCamera(pointer.ndc, state.camera);
    return state.raycaster.ray.intersectPlane(plane, point);
}
