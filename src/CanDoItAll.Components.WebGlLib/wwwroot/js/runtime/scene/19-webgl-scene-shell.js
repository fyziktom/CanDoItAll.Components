export function buildHostShell(host) {
    host.replaceChildren();
    host.classList.add("wgl-scene-runtime-host");

    const stage = document.createElement("div");
    stage.className = "wgl-scene-stage";

    const labelLayer = document.createElement("div");
    labelLayer.className = "wgl-scene-label-layer";

    const emptyState = document.createElement("div");
    emptyState.className = "wgl-scene-empty-state";
    // webgl-audit: allow-innerHTML-static; literal runtime-owned empty-state markup has no external input.
    emptyState.innerHTML = "<div class=\"wgl-scene-empty-state__card\"><p class=\"wgl-scene-empty-state__title\">No scene objects</p><p class=\"wgl-scene-empty-state__body\">Add generic scene objects to render the WebGL proof surface.</p></div>";

    const diagnosticsPanel = document.createElement("div");
    diagnosticsPanel.className = "wgl-scene-diagnostics";

    const diagnosticsTitle = document.createElement("p");
    diagnosticsTitle.className = "wgl-scene-diagnostics__title";
    diagnosticsTitle.textContent = "Runtime";

    const diagnosticsMeta = document.createElement("p");
    diagnosticsMeta.className = "wgl-scene-diagnostics__meta";

    diagnosticsPanel.append(diagnosticsTitle, diagnosticsMeta);
    host.append(stage, labelLayer, emptyState, diagnosticsPanel);
    return { stage, labelLayer, emptyState, diagnosticsPanel, diagnosticsMeta };
}
