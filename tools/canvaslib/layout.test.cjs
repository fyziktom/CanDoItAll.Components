const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const source = fs.readFileSync(path.join(__dirname,
    "../../src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/01-foundation.js"), "utf8");

function resolve(nodes, manualPositions = {}) {
    const root = { selectionModel: {} };
    vm.runInNewContext(source, { window: { CanDoItAll: root } });
    const runtime = root.canvasWorkbenchModule;
    const state = {
        ui: { manualPositions },
        lookups: runtime.buildNodeLookup(nodes),
        measuredNodeSizes: new Map(nodes.map(node => [node.id, { width: 272, height: 196 }])),
        interaction: null
    };
    return runtime.computeResolvedNodePositions(state, nodes);
}

for (const y of [-400, 400]) {
    test(`safe vertical child at y=${y} keeps its saved position`, () => {
        const nodes = [{ id: "parent", x: 100, y: 0 }, { id: "child", parentId: "parent", x: 100, y }];
        const positions = resolve(nodes);
        assert.equal(positions.get("child").x, 100);
        assert.equal(positions.get("child").y, y);
        assert.equal(positions.get("parent").x, 100);
    });
}

for (const x of [-500, 500]) {
    test(`safe horizontal child at x=${x} keeps its saved position`, () => {
        const positions = resolve([{ id: "parent", x: 0, y: 0 }, { id: "child", parentId: "parent", x, y: 0 }]);
        assert.equal(positions.get("child").x, x);
        assert.equal(positions.get("child").y, 0);
    });
}

test("real parent-child overlap still receives clearance", () => {
    const positions = resolve([{ id: "parent", x: 0, y: 0 }, { id: "child", parentId: "parent", x: 25, y: 10 }]);
    const parent = positions.get("parent");
    const child = positions.get("child");
    assert.ok(Math.abs(parent.x - child.x) >= 272 + 42 || Math.abs(parent.y - child.y) >= 196 + 42);
});

test("deliberate phase columns and their vertical tasks are stable across node order", () => {
    const nodes = [{ id: "root", x: 620, y: -400 }];
    for (let column = 0; column < 3; column++) {
        const id = `phase-${column}`;
        nodes.push({ id, parentId: "root", x: column * 620, y: 0 });
        for (let row = 1; row < 4; row++) {
            nodes.push({ id: `${id}-task-${row}`, parentId: id, x: column * 620 + 170, y: row * 260 });
        }
    }
    for (const order of [nodes, [...nodes].reverse()]) {
        const positions = resolve(order);
        for (const node of nodes) {
            assert.equal(positions.get(node.id).x, node.x, node.id);
            assert.equal(positions.get(node.id).y, node.y, node.id);
        }
    }
});

test("a safely separated manual position remains stable", () => {
    const nodes = [{ id: "parent", x: 0, y: 0 }, { id: "child", parentId: "parent", x: 500, y: 0 }];
    const positions = resolve(nodes, { child: { x: 0, y: 500 } });
    assert.equal(positions.get("child").x, 0);
    assert.equal(positions.get("child").y, 500);
});
