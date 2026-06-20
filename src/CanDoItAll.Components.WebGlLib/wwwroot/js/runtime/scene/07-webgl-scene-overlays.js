import { THREE, buildDiagnosticsSnapshot, normalizeLabelVisibilityMode, resolveObjectPosition, resolveObjectSize, round } from "./02-webgl-scene-core.js";

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

    const labelVisibilityMode = normalizeLabelVisibilityMode(state.options.labelVisibilityMode || state.sceneModel.uiState?.labelVisibilityMode);
    const activeLabelObjectId = resolveActiveLabelObjectId(state, labelVisibilityMode);
    state.shell.labelLayer.dataset.labelVisibilityMode = labelVisibilityMode;
    state.shell.labelLayer.dataset.activeLabelObjectId = activeLabelObjectId || "";

    const visibleIds = new Set();
    for (const sceneObject of state.sceneModel.objects || []) {
        if (!sceneObject?.id) {
            continue;
        }

        visibleIds.add(sceneObject.id);
        const label = ensureObjectLabel(state, sceneObject);
        syncObjectLabel(label, sceneObject);
        const size = resolveObjectSize(sceneObject);
        const position = resolveObjectPosition(sceneObject);
        const projected = worldToScreen(state, new THREE.Vector3(position.x, position.y + size.y + 0.18, position.z));
        label.style.transform = `translate(${round(projected.x)}px, ${round(projected.y)}px) translate(-50%, -100%)`;
        label.classList.toggle("is-selected", state.selectedObjectIds.has(sceneObject.id));
        label.classList.toggle("is-hovered", state.hoveredObjectId === sceneObject.id);
        label.classList.toggle("is-hidden-by-mode", labelVisibilityMode === "hover" && sceneObject.id !== activeLabelObjectId);
    }

    for (const [objectId, label] of state.labelElements.entries()) {
        if (!visibleIds.has(objectId)) {
            label.remove();
            state.labelElements.delete(objectId);
        }
    }
}

function resolveActiveLabelObjectId(state, labelVisibilityMode) {
    if (labelVisibilityMode !== "hover") {
        clearLabelHoverHideTimer(state);
        state.labelHoverObjectId = "";
        state.labelHoverExpiresAt = 0;
        return "";
    }

    const now = performance.now();
    if (state.hoveredObjectId) {
        clearLabelHoverHideTimer(state);
        state.labelHoverObjectId = state.hoveredObjectId;
        state.labelHoverExpiresAt = Number.POSITIVE_INFINITY;
        return state.hoveredObjectId;
    }

    if (!state.labelHoverObjectId) {
        state.labelHoverExpiresAt = 0;
        return "";
    }

    if (!Number.isFinite(state.labelHoverExpiresAt)) {
        state.labelHoverExpiresAt = now + resolveLabelHoverHideDelay(state);
    }

    if (now <= state.labelHoverExpiresAt) {
        scheduleLabelHoverHideTimer(state, state.labelHoverExpiresAt);
        return state.labelHoverObjectId;
    }

    clearLabelHoverHideTimer(state);
    state.labelHoverObjectId = "";
    state.labelHoverExpiresAt = 0;
    return "";
}

function resolveLabelHoverHideDelay(state) {
    const delay = Number(state.options.labelHoverHideDelayMilliseconds);
    return Number.isFinite(delay) ? Math.max(0, Math.min(10000, delay)) : 2200;
}

function scheduleLabelHoverHideTimer(state, expiresAt) {
    const delayMs = Math.max(0, expiresAt - performance.now() + 16);
    if (state.labelHoverHideTimer && Math.abs((state.labelHoverHideTimerExpiresAt || 0) - expiresAt) < 12) {
        return;
    }

    clearLabelHoverHideTimer(state);
    state.labelHoverHideTimerExpiresAt = expiresAt;
    state.labelHoverHideTimer = window.setTimeout(() => {
        state.labelHoverHideTimer = 0;
        state.labelHoverHideTimerExpiresAt = 0;
        if (!state.hoveredObjectId) {
            state.labelHoverObjectId = "";
            state.labelHoverExpiresAt = 0;
            state.scheduleRender("label-hover-timeout");
        }
    }, delayMs);
}

function clearLabelHoverHideTimer(state) {
    if (state.labelHoverHideTimer) {
        window.clearTimeout(state.labelHoverHideTimer);
    }

    state.labelHoverHideTimer = 0;
    state.labelHoverHideTimerExpiresAt = 0;
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
    meta.textContent = resolveLabelMeta(sceneObject);
    label.append(title, meta);
    state.shell.labelLayer.appendChild(label);
    state.labelElements.set(sceneObject.id, label);
    return label;
}

function syncObjectLabel(label, sceneObject) {
    label.dataset.objectId = sceneObject.id;
    label.dataset.labelGroup = resolveLabelGroup(sceneObject);
    label.dataset.objectKind = normalizeToken(sceneObject.kind || "object");
    label.dataset.objectFamily = normalizeToken(sceneObject.family || "generic");

    const title = label.querySelector(".wgl-scene-label__title");
    if (title) {
        title.textContent = sceneObject.title || sceneObject.id;
    }

    const meta = label.querySelector(".wgl-scene-label__meta");
    if (meta) {
        meta.textContent = resolveLabelMeta(sceneObject);
    }
}

function resolveLabelMeta(sceneObject) {
    const metadata = sceneObject?.metadata || {};
    return metadata["label.valueLine"] ||
        metadata.labelValueLine ||
        sceneObject.subtitle ||
        sceneObject.kind ||
        "object";
}

function resolveLabelGroup(sceneObject) {
    const metadata = sceneObject?.metadata || {};
    const explicitGroup = normalizeToken(metadata["label.group"] || metadata.labelGroup || "");
    if (explicitGroup) {
        return explicitGroup;
    }

    const visualKind = normalizeToken(metadata["visual.kind"] || metadata.visualKind || "");
    switch (visualKind) {
        case "avatar":
            return "actors";
        case "inventory-prop":
        case "inventory-replica":
            return "supplies";
        case "route-anchor":
            return "shared-locations";
        case "place":
            return "locations";
        case "signal":
        case "diagnostic":
            return "signals";
        default:
            break;
    }

    const kind = normalizeToken(`${sceneObject?.kind || ""}-${sceneObject?.family || ""}-${sceneObject?.subtitle || ""}`);
    if (kind.includes("actor") || kind.includes("person")) {
        return "actors";
    }

    if (kind.includes("inventory") || kind.includes("resource") || kind.includes("supply") || kind.includes("store")) {
        return "supplies";
    }

    if (kind.includes("place") || kind.includes("location") || kind.includes("institution")) {
        return "locations";
    }

    if (kind.includes("signal") || kind.includes("risk") || kind.includes("diagnostic")) {
        return "signals";
    }

    return "other";
}

function normalizeToken(value) {
    return String(value || "")
        .trim()
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, "-")
        .replace(/^-+|-+$/g, "");
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

