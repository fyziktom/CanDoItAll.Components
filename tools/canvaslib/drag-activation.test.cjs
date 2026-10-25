const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const runtimeRoot = path.join(__dirname,
    "../../src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench");

function fixture(zoom = 1) {
    let clock = 1000;
    const calls = { renders: 0, patches: 0, snaps: 0, contexts: 0, moves: [], effects: [] };
    const window = { CanDoItAll: { selectionModel: {} }, addEventListener() {} };
    const context = vm.createContext({ window, Date: class extends Date { static now() { return clock; } } });
    const load = name => vm.runInContext(fs.readFileSync(path.join(runtimeRoot, name), "utf8"), context);
    load("01-foundation.js");
    const runtime = window.CanDoItAll.canvasWorkbenchModule;
    const nodes = [
        { id: "root", x: 0, y: 0, family: "root" },
        { id: "a", x: 40, y: 10, parentId: "root" },
        { id: "b", x: 60, y: 20, parentId: "root", isRequired: true },
        { id: "locked", x: 1300, y: 200, isReadOnly: true }
    ];
    const state = {
        document: { addEventListener() {} },
        host: {
            dataset: {},
            addEventListener() {},
            getBoundingClientRect: () => ({ left: 0, top: 0, right: 1920, bottom: 1080 })
        },
        surface: { nodes, links: [], chrome: { collapseOnDoubleClick: false } },
        ui: { zoom, manualPositions: {} },
        lookups: runtime.buildNodeLookup(nodes),
        measuredNodeSizes: new Map(nodes.map(node => [node.id, { width: 272, height: 232 }])),
        selectedIds: new Set(["a"]),
        collapsedIds: new Set(),
        interaction: null,
        dotNetRef: { invokeMethodAsync: async (...args) => { calls.effects.push(args); } }
    };
    Object.assign(runtime, {
        getVisibleNodes: () => nodes,
        render: current => {
            calls.renders++;
            runtime.ensureLayoutPositions(current, nodes);
        },
        buildActiveDragContext: () => {
            calls.contexts++;
            return {};
        },
        resolveSnapAdjustment: (_state, _interaction, deltaX, deltaY) => {
            calls.snaps++;
            return { deltaX, deltaY, guides: [] };
        },
        clearSnapGuides: current => { current.snapGuides = []; },
        publishNodesMoved: async (_state, ids, positions) => { calls.moves.push({ ids: [...ids], positions }); },
        publishState() {},
        publishSelection() {},
        selectSingleNode: (current, id) => { current.selectedIds = new Set([id]); },
        clearContextMenu() {},
        cancelViewportAnimation() {},
        ensureHostFocus() {},
        deferHostFocus() {},
        clearScenePopoverHover() {},
        isOverlayTarget: () => false,
        getHostPoint: (_state, x, y) => ({ x, y }),
        getSceneHitAtEvent: () => ({ type: "node-body", nodeId: "a" }),
        resolveHitNode: (current, hit) => current.lookups.byId.get(hit?.nodeId),
        hitTestNode: current => current.lookups.byId.get("a"),
        hitTestFrameHandle: () => null,
        hitTestProgressBadge: () => null,
        workbenchInternals: {
            overlayRenderer: { clearSnapGuides() {} },
            scenePatching: { renderActiveDrag: () => { calls.patches++; } }
        }
    });
    load("05-viewport-and-events.js");
    const positions = () => Array.from(runtime.ensureLayoutPositions(state, nodes),
        ([id, position]) => [id, position.x, position.y]);
    const event = (dx = 0, dy = 0) => ({
        button: 0, clientX: 100 + dx, clientY: 100 + dy,
        target: { closest: () => null }, preventDefault() {}
    });
    return { runtime, state, calls, positions, event, load, advance: ms => { clock += ms; } };
}

for (const kind of ["drag", "frame-drag", "dependency-drag"]) {
    test(`${kind}: pressing a crowded node leaves the resolved layout unchanged`, async () => {
        const f = fixture();
        const before = f.positions();
        f.runtime.startDragForNodeIds(f.state, f.event(), ["a"], { kind });
        assert.deepEqual(f.positions(), before);
        assert.equal(f.calls.renders, 0);
        assert.equal(f.calls.contexts, 0);
        await f.runtime.finishInteraction(f.state);
        assert.deepEqual(f.positions(), before);
        assert.equal(f.calls.moves.length, 0);
    });
}

for (const zoom of [0.25, 1, 2]) {
    test(`pointer jitter cannot snap or move a node at zoom ${zoom}`, async () => {
        const f = fixture(zoom);
        const before = f.positions();
        f.runtime.startDragForNodeIds(f.state, f.event(), ["a"], { kind: "drag" });
        f.advance(700);
        for (const [x, y] of [[0, 0], [2, 2], [5.9, 0], [0, -5.9]]) {
            f.runtime.updateDrag(f.state, f.event(x, y));
            assert.deepEqual(f.positions(), before);
        }
        assert.equal(f.calls.snaps, 0);
        assert.equal(f.state.interaction.moved, false);
        assert.deepEqual(f.state.ui.manualPositions, {});
        await f.runtime.finishInteraction(f.state);
        assert.equal(f.calls.moves.length, 0);
    });
}

test("intentional movement starts from the displayed position and freezes its neighbours", async () => {
    const f = fixture(0.25);
    const before = f.positions();
    f.state.lastPointerTarget = { nodeId: "a", timestamp: 1000 };
    f.runtime.startDragForNodeIds(f.state, f.event(), ["a"], { kind: "drag" });
    const start = f.state.interaction.startPositions.a;
    f.runtime.updateDrag(f.state, f.event(6, 0));
    assert.equal(f.state.interaction.moved, true);
    assert.equal(f.state.lastPointerTarget, null);
    assert.ok(Math.abs(f.state.ui.manualPositions.a.x - start.x - 24) < 0.02);
    assert.ok(Math.abs(f.state.ui.manualPositions.a.y - start.y) < 0.02);
    assert.deepEqual(f.positions().filter(([id]) => id !== "a"), before.filter(([id]) => id !== "a"));
    assert.equal(f.calls.contexts, 1);
    f.runtime.updateDrag(f.state, f.event(10, 0));
    assert.equal(f.calls.contexts, 1);
    assert.equal(f.calls.patches, 1);
    assert.equal(f.calls.moves.length, 0);
    await f.runtime.finishInteraction(f.state);
    await f.runtime.finishInteraction(f.state);
    assert.equal(f.calls.moves.length, 1);
    assert.deepEqual(f.calls.moves[0].ids, ["a"]);
});

test("group dragging keeps read-only nodes stationary and commits only editable identities", async () => {
    const f = fixture();
    const before = f.positions();
    f.runtime.startDragForNodeIds(f.state, f.event(), ["a", "b", "locked"], { kind: "frame-drag" });
    assert.deepEqual(f.positions(), before);
    f.runtime.updateDrag(f.state, f.event(12, 0));
    assert.equal(f.state.ui.manualPositions.locked, undefined);
    await f.runtime.finishInteraction(f.state);
    assert.deepEqual(f.calls.moves[0].ids, ["a", "b"]);
});

test("dependency click retains its click action without a node move", async () => {
    const f = fixture();
    f.runtime.startDragForNodeIds(f.state, f.event(), ["a"], {
        kind: "dependency-drag", sourceNodeId: "root", targetNodeId: "a"
    });
    f.runtime.updateDrag(f.state, f.event(2, 0));
    await f.runtime.finishInteraction(f.state);
    assert.equal(f.calls.moves.length, 0);
    assert.equal(f.calls.effects.length, 1);
    assert.equal(f.calls.effects[0][0], "OnContextActionRequest");
    assert.equal(JSON.parse(f.calls.effects[0][1]).actionId, "dependency:create");
});

test("a surface update while pressed rebases activation on the current displayed layout", () => {
    const f = fixture();
    f.runtime.startDragForNodeIds(f.state, f.event(), ["a"], { kind: "drag" });
    f.state.ui.manualPositions.a = { x: 1800, y: 700 };
    const displayed = f.runtime.getNodePosition(f.state, f.state.lookups.byId.get("a"));
    f.runtime.updateDrag(f.state, f.event(12, 0));
    assert.ok(Math.abs(f.state.ui.manualPositions.a.x - displayed.x - 12) < 0.02);
    assert.ok(Math.abs(f.state.ui.manualPositions.a.y - displayed.y) < 0.02);
});

for (const change of ["removed", "read-only"]) {
    test(`a node made ${change} while pressed cannot activate a stale drag`, async () => {
        const f = fixture();
        f.runtime.startDragForNodeIds(f.state, f.event(), ["a"], { kind: "drag" });
        if (change === "removed") {
            f.state.lookups.byId.delete("a");
        } else {
            f.state.lookups.byId.get("a").isReadOnly = true;
        }
        f.runtime.updateDrag(f.state, f.event(12, 0));
        assert.equal(f.state.interaction, null);
        assert.equal(f.state.ui.manualPositions.a, undefined);
        await f.runtime.finishInteraction(f.state);
        assert.equal(f.calls.moves.length, 0);
    });
}

for (const renderer of ["canvas", "legacy"]) {
    for (const interval of [180, 450]) {
        test(`${renderer}: double activation at ${interval}ms opens once without moving`, async () => {
            const f = fixture();
            if (renderer === "canvas") {
                f.load("07a-runtime-interaction-router.js");
                f.runtime.attachEvents(f.state);
            } else {
                f.runtime.legacyAttachEvents(f.state);
            }
            const before = f.positions();
            f.state.handlers.pointerDown(f.event());
            await f.runtime.finishInteraction(f.state);
            f.advance(interval);
            f.state.handlers.pointerDown(f.event());
            await f.runtime.finishInteraction(f.state);
            f.state.handlers.doubleClick(f.event());
            assert.deepEqual(f.positions(), before);
            assert.equal(f.calls.moves.length, 0);
            assert.equal(f.calls.effects.filter(([method]) => method === "OnNodeOpened").length, 1);
        });
    }

    test(`${renderer}: a drag cannot become a double activation`, async () => {
        const f = fixture();
        if (renderer === "canvas") {
            f.load("07a-runtime-interaction-router.js");
            f.runtime.attachEvents(f.state);
        } else {
            f.runtime.legacyAttachEvents(f.state);
        }
        f.state.handlers.pointerDown(f.event());
        f.runtime.updateDrag(f.state, f.event(12, 0));
        await f.runtime.finishInteraction(f.state);
        f.state.handlers.doubleClick(f.event());
        assert.equal(f.calls.effects.filter(([method]) => method === "OnNodeOpened").length, 0);
    });
}
