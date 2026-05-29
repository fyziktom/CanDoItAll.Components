import { THREE, buildDiagnosticsSnapshot, resolveObjectPosition, resolveObjectSize, round } from "./02-webgl-scene-core.js";

export function syncOverlays(state) {
    syncLabels(state);
    syncDiagnostics(state);
}

function syncLabels(state) {
    const labelsEnabled = state.options.showLabels !== false && state.sceneModel.uiState?.showLabels !== false;
    state.shell.labelLayer.classList.toggle("is-hidden", !labelsEnabled);
    if (!labelsEnabled) {
        return;
    }

    const visibleIds = new Set();
    for (const sceneObject of state.sceneModel.objects || []) {
        if (!sceneObject?.id) {
            continue;
        }

        visibleIds.add(sceneObject.id);
        const label = ensureObjectLabel(state, sceneObject);
        const size = resolveObjectSize(sceneObject);
        const position = resolveObjectPosition(sceneObject);
        const projected = worldToScreen(state, new THREE.Vector3(position.x, position.y + size.y + 0.18, position.z));
        label.style.transform = `translate(${round(projected.x)}px, ${round(projected.y)}px) translate(-50%, -100%)`;
        label.classList.toggle("is-selected", state.selectedObjectIds.has(sceneObject.id));
        label.classList.toggle("is-hovered", state.hoveredObjectId === sceneObject.id);
    }

    for (const [objectId, label] of state.labelElements.entries()) {
        if (!visibleIds.has(objectId)) {
            label.remove();
            state.labelElements.delete(objectId);
        }
    }
}

function ensureObjectLabel(state, sceneObject) {
    const existing = state.labelElements.get(sceneObject.id);
    if (existing) {
        return existing;
    }

    const label = document.createElement("div");
    label.className = "wgl-scene-label";
    label.dataset.objectId = sceneObject.id;
    const title = document.createElement("p");
    title.className = "wgl-scene-label__title";
    title.textContent = sceneObject.title || sceneObject.id;
    const meta = document.createElement("p");
    meta.className = "wgl-scene-label__meta";
    meta.textContent = sceneObject.subtitle || sceneObject.kind || "object";
    label.append(title, meta);
    state.shell.labelLayer.appendChild(label);
    state.labelElements.set(sceneObject.id, label);
    return label;
}

function syncDiagnostics(state) {
    state.shell.diagnosticsPanel.classList.toggle("is-hidden", state.options.showDiagnosticsPanel === false);
    if (state.options.showDiagnosticsPanel === false) {
        return;
    }

    const diagnostics = buildDiagnosticsSnapshot(state);
    state.shell.diagnosticsMeta.textContent = [
        `${diagnostics.objectCount} objects`,
        `${diagnostics.symbolCount} symbols`,
        `${diagnostics.loadedAssetCount} loaded`,
        `${diagnostics.fallbackObjectCount} fallback`
    ].join(" / ");
}

export function worldToScreen(state, vector) {
    const projected = vector.clone().project(state.camera);
    return {
        x: ((projected.x + 1) / 2) * state.viewport.width,
        y: ((1 - projected.y) / 2) * state.viewport.height
    };
}

