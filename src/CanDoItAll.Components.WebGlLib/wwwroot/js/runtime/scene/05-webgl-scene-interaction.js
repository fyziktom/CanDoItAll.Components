import { THREE, focusHost } from "./02-webgl-scene-core.js";

function resolvePointer(state, event) {
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

function findHit(state, event) {
    if (!state.hitMeshes.length) {
        return null;
    }

    const pointer = resolvePointer(state, event);
    state.raycaster.setFromCamera(pointer.ndc, state.camera);
    const hits = state.raycaster.intersectObjects(state.hitMeshes, true);
    for (const hit of hits) {
        const objectId = hit.object?.userData?.objectId || hit.object?.parent?.userData?.objectId;
        if (objectId) {
            return {
                objectId,
                point: hit.point,
                screenX: pointer.x,
                screenY: pointer.y
            };
        }
    }

    return null;
}

export function handlePointerMove(state, event) {
    if (state.sceneModel.interaction?.allowHover === false) {
        return;
    }

    const hit = findHit(state, event);
    const nextHoveredObjectId = hit?.objectId || "";
    if (nextHoveredObjectId === state.hoveredObjectId) {
        return;
    }

    state.hoveredObjectId = nextHoveredObjectId;
    syncSelectionVisuals(state);
    notifyHoverChanged(state, {
        objectId: nextHoveredObjectId || null,
        screenX: hit?.screenX || 0,
        screenY: hit?.screenY || 0
    });
    state.scheduleRender();
}

export function handlePointerDown(state, event) {
    state.pointerDown = {
        clientX: event.clientX,
        clientY: event.clientY
    };
}

export function handleClick(state, event) {
    if (state.sceneModel.interaction?.allowClickSelection === false) {
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
    if (next.has(hit.objectId)) {
        next.delete(hit.objectId);
    } else {
        next.add(hit.objectId);
    }

    selectObjects(state, Array.from(next));
}

export function handleDoubleClick(state, event) {
    if (state.sceneModel.interaction?.focusOnDoubleClick === false) {
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
    syncSelectionVisuals(state);
    notifySelectionChanged(state);
    notifyStateChanged(state);
    focusHost(state);
    state.scheduleRender();
}

export function syncSelectionVisuals(state) {
    for (const [objectId, group] of state.objectGroups.entries()) {
        const selected = state.selectedObjectIds.has(objectId);
        const hovered = state.hoveredObjectId === objectId;
        if (group.userData.selectionRing) {
            group.userData.selectionRing.visible = selected;
        }

        if (group.userData.hoverRing) {
            group.userData.hoverRing.visible = hovered && !selected;
        }
    }
}

function notifySelectionChanged(state) {
    if (!state.dotNetRef) {
        return;
    }

    const selectedObjectIds = Array.from(state.selectedObjectIds);
    state.dotNetRef
        .invokeMethodAsync("OnSceneSelectionChanged", JSON.stringify({
            primaryObjectId: selectedObjectIds[0] || null,
            selectedObjectIds
        }))
        .catch(error => console.warn("WebGL scene selection callback failed.", error));
}

function notifyHoverChanged(state, args) {
    if (!state.dotNetRef) {
        return;
    }

    state.dotNetRef
        .invokeMethodAsync("OnSceneHoverChanged", JSON.stringify(args))
        .catch(error => console.warn("WebGL scene hover callback failed.", error));
}

function notifyStateChanged(state) {
    if (!state.dotNetRef) {
        return;
    }

    state.dotNetRef
        .invokeMethodAsync("OnSceneStateChanged", JSON.stringify(state.sceneModel.uiState || {}))
        .catch(error => console.warn("WebGL scene state callback failed.", error));
}

