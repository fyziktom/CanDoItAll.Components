const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const source = fs.readFileSync(path.join(__dirname,
    "../../src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/05-viewport-and-events.js"), "utf8");

function fixture() {
    const nodes = [{ id: "readonly", isReadOnly: true }, { id: "editable", isReadOnly: false }];
    const shared = {
        getNodePosition: () => ({ x: 100, y: 200 }),
        clearSnapGuides: () => {},
        render: () => {},
        buildActiveDragContext: () => ({}),
        getVisibleNodes: () => nodes,
        ensureLayoutPositions: () => new Map()
    };
    vm.runInNewContext(source, { window: { CanDoItAll: { canvasWorkbenchModule: shared } } });
    return { shared, state: { lookups: { byId: new Map(nodes.map(node => [node.id, node])) }, interaction: null } };
}

for (const kind of ["drag", "frame-drag", "dependency-drag"]) {
    test(`${kind} refuses a read-only node`, () => {
        const { shared, state } = fixture();
        shared.startDragForNodeIds(state, { clientX: 10, clientY: 20 }, ["readonly"], { kind });
        assert.equal(state.interaction, null);
    });
}

test("mixed selection moves only editable nodes and ignores missing identities", () => {
    const { shared, state } = fixture();
    shared.startDragForNodeIds(state, { clientX: 10, clientY: 20 }, ["readonly", "editable", "missing", "editable"], { kind: "drag" });
    assert.deepEqual(Array.from(state.interaction.nodeIds), ["editable"]);
    assert.equal(state.interaction.startPositions.editable.x, 100);
});
