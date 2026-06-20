import { focusHost } from "./02-webgl-scene-core.js";
import { beginDrag, cancelDrag, completeDrag, isClickSuppressed, updateDrag } from "./12-webgl-scene-drag.js";
import { syncObjectRings } from "./11-webgl-scene-graph.js";
import { notifyStateChanged, resolvePointer } from "./24-webgl-scene-notifications.js";

export function findHit(state, event) {
    if (!state.hitMeshes.length) {
        return null;
    }

    const pointer = resolvePointer(state, event);
    state.raycaster.setFromCamera(pointer.ndc, state.camera);
    const hits = state.raycaster.intersectObjects(state.hitMeshes, true);
    for (const hit of hits) {
        const objectId = hit.object?.userData?.objectId || hit.object?.parent?.userData?.objectId;
        if (objectId) {
            return { objectId, point: hit.point, screenX: pointer.x, screenY: pointer.y };
        }
    }

    return null;
}

export function handlePointerMove(state, event) {
    if (updateDrag(state, event)) {
        return;
    }

    if (state.sceneModel.interaction?.allowHover === false) {
        return;
    }

    const hit = findHit(state, event);
    const nextHoveredObjectId = hit?.objectId || "";
    if (nextHoveredObjectId === state.hoveredObjectId) {
        return;
    }

    state.hoveredObjectId = nextHoveredObjectId;
    state.sceneModel.uiState.hoveredObjectId = nextHoveredObjectId;
    syncObjectRings(state);
    notifyHoverChanged(state, {
        objectId: nextHoveredObjectId || null,
        screenX: hit?.screenX || 0,
        screenY: hit?.screenY || 0
    });
    state.scheduleRender("hover");
}

export function handlePointerDown(state, event) {
    state.pointerDown = {
        clientX: event.clientX,
        clientY: event.clientY
    };
    beginDrag(state, event, findHit(state, event));
}

export function handlePointerUp(state, event) {
    completeDrag(state, event);
}

export function handlePointerCancel(state, event) {
    cancelDrag(state, event);
}

export function handleKeyDown(state, event) {
    if (event.key === "Escape") {
        cancelDrag(state, event);
    }
}

export function handleClick(state, event) {
    if (state.sceneModel.interaction?.allowClickSelection === false || isClickSuppressed(state)) {
        return;
    }

    if (state.pointerDown) {
        const moved = Math.hypot(event.clientX - state.pointerDown.clientX, event.clientY - state.pointerDown.clientY);
        if (moved > 6) {
            return;
        }
    }

    const hit = findHit(state, event);
    if (!hit?.objectId) {
        selectObjects(state, []);
        return;
    }

    const sceneObject = state.objectLookup.get(hit.objectId);
    if (sceneObject?.isSelectable === false) {
        return;
    }

    const multi = !!(event.ctrlKey || event.metaKey || event.shiftKey) &&
        state.sceneModel.interaction?.allowMultiSelect === true;
    if (!multi) {
        selectObjects(state, [hit.objectId]);
        return;
    }

    const next = new Set(state.selectedObjectIds);
    next.has(hit.objectId) ? next.delete(hit.objectId) : next.add(hit.objectId);
    selectObjects(state, Array.from(next));
}

export function handleDoubleClick(state, event) {
    if (state.sceneModel.interaction?.focusOnDoubleClick === false || isClickSuppressed(state)) {
        return;
    }

    const hit = findHit(state, event);
    if (hit?.objectId) {
        state.focusObject(hit.objectId);
    }
}

export function selectObjects(state, objectIds) {
    const selectableIds = objectIds.filter(id => state.objectLookup.get(id)?.isSelectable !== false);
    state.selectedObjectIds = new Set(selectableIds);
    const primaryObjectId = selectableIds[0] || "";
    state.sceneModel.uiState.selection = {
        selectedObjectIds: selectableIds,
        primaryObjectId,
        contextActionId: ""
    };
    syncObjectRings(state);
    notifySelectionChanged(state);
    notifyStateChanged(state);
    focusHost(state);
    state.scheduleRender("selection");
}

function notifySelectionChanged(state) {
    const selectedObjectIds = Array.from(state.selectedObjectIds);
    state.dotNetRef?.invokeMethodAsync("OnSceneSelectionChanged", JSON.stringify({
        primaryObjectId: selectedObjectIds[0] || null,
        selectedObjectIds
    })).catch(error => console.warn("WebGL scene selection callback failed.", error));
}

function notifyHoverChanged(state, args) {
    state.dotNetRef?.invokeMethodAsync("OnSceneHoverChanged", JSON.stringify(args))
        .catch(error => console.warn("WebGL scene hover callback failed.", error));
}
