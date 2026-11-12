const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const runtimeRoot = path.join(__dirname,
    "../../src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench");

function fixture() {
    const clone = value => JSON.parse(JSON.stringify(value));
    const shared = {
        render() {},
        syncMenuScaleCss() {},
        invalidateMeasuredLayout() {},
        toCollapsedSet: ids => new Set(ids || []),
        serializeState: state => JSON.stringify(state.ui),
        workbenchInternals: {
            runtime: {},
            stateStore: {
                normalizeSurface: clone,
                buildNodeLookup: nodes => ({ byId: new Map(nodes.map(node => [node.id, node])) }),
                reconcileSelection() {}
            },
            instrumentation: { now: () => 0 },
            sceneLayout: {
                cancelViewportAnimation() {}, resize() {}, applySceneTransform() {}, ensureLayoutPositions() {},
                getVisibleNodes: () => [], getProjectedNodes: () => []
            },
            scenePatching: { renderGroupFrames() {}, renderLinks() {}, renderNodes() {}, scheduleNodeMeasurement() {} },
            overlayRenderer: {
                clearContextMenu() {}, hidePopover() {}, renderSnapGuides() {}, renderEmptyStateOverlay() {},
                renderConnectorAnchorOverlay() {}, renderTransformHandlesOverlay() {}, renderDebugDecorations() {},
                renderDiagnosticsOverlay() {}, renderMinimap() {}, layoutComposer() {}
            }
        }
    };
    const window = { CanDoItAll: { canvasWorkbenchModule: shared }, clearTimeout() {} };
    const context = vm.createContext({ window });
    for (const name of ["03-interaction-and-state.js", "05-viewport-and-events.js"]) {
        vm.runInContext(fs.readFileSync(path.join(runtimeRoot, name), "utf8"), context);
    }
    const surface = {
        surfaceId: "first",
        nodes: [{ id: "parent" }, { id: "child", parentId: "parent" }],
        uiState: { selectedNodeIds: [], collapsedNodeIds: [], panX: 0, panY: 0, zoom: 1 }
    };
    const publications = [];
    const state = {
        surface: clone(surface),
        collapsedIds: new Set(),
        estimatedNodeSizes: new Map(),
        nodeTextLayouts: new Map(),
        maximizedApplied: false,
        stateDispatchId: 0,
        collapseStateDispatchId: 0,
        publishStateDebounced: snapshot => publications.push(snapshot)
    };
    state.ui = state.surface.uiState;
    const refresh = (incoming = surface, committedDispatchId = 0) =>
        shared.refresh(state, incoming, false, true, committedDispatchId);
    return { state, surface, publications, refresh, toggle: () => shared.toggleCollapse(state, "parent") };
}

test("an older host refresh cannot undo a pending collapse", () => {
    const f = fixture();
    f.toggle();
    f.refresh();
    assert.deepEqual([...f.state.collapsedIds], ["parent"]);
    assert.deepEqual([...f.state.ui.collapsedNodeIds], ["parent"]);
    assert.deepEqual(JSON.parse(f.publications[0].stateJson).collapsedNodeIds, ["parent"]);
});

test("an older acknowledgement cannot undo a newer expansion", () => {
    const f = fixture();
    f.toggle();
    f.toggle();
    f.surface.uiState.collapsedNodeIds = ["parent"];
    f.refresh(f.surface, 1);
    assert.deepEqual([...f.state.collapsedIds], []);
    assert.deepEqual([...f.state.ui.collapsedNodeIds], []);
});

test("the acknowledged owner can normalize the collapse state", () => {
    const f = fixture();
    f.toggle();
    f.refresh(f.surface, 1);
    assert.deepEqual([...f.state.collapsedIds], []);
});

test("an acknowledged collapse does not block a later owner expansion", () => {
    const f = fixture();
    f.toggle();
    f.surface.uiState.collapsedNodeIds = ["parent"];
    f.refresh(f.surface, 1);
    f.surface.uiState.collapsedNodeIds = [];
    f.refresh(f.surface, 1);
    assert.deepEqual([...f.state.collapsedIds], []);
});

test("pending collapse state belongs only to its original surface", () => {
    const f = fixture();
    f.toggle();
    f.surface.surfaceId = "second";
    f.refresh();
    assert.deepEqual([...f.state.collapsedIds], []);
});

test("a late pre-acknowledgement refresh cannot undo an accepted collapse", () => {
    const f = fixture();
    f.toggle();
    f.surface.uiState.collapsedNodeIds = ["parent"];
    f.refresh(f.surface, 1);
    f.surface.uiState.collapsedNodeIds = [];
    f.refresh(f.surface, 0);
    assert.deepEqual([...f.state.collapsedIds], ["parent"]);
});

test("a removed node cannot remain pending in the collapsed set", () => {
    const f = fixture();
    f.toggle();
    f.surface.nodes = [];
    f.refresh();
    assert.deepEqual([...f.state.collapsedIds], []);
});

test("preserving collapse does not discard a newer owner selection", () => {
    const f = fixture();
    f.toggle();
    f.surface.uiState.selectedNodeIds = ["child"];
    f.refresh();
    assert.deepEqual([...f.state.collapsedIds], ["parent"]);
    assert.deepEqual([...f.state.ui.selectedNodeIds], ["child"]);
});

test("owner collapse applies when there is no pending local toggle", () => {
    const f = fixture();
    f.surface.uiState.collapsedNodeIds = ["parent"];
    f.refresh();
    assert.deepEqual([...f.state.collapsedIds], ["parent"]);
});
