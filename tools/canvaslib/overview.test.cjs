const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const runtimePath = path.join(__dirname, "../../src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench");
function load(file, shared) {
    vm.runInNewContext(fs.readFileSync(path.join(runtimePath, file), "utf8"), {
        window: { CanDoItAll: { canvasWorkbenchModule: shared } }
    });
}

for (const zoom of [0.2, 0.33, 0.5, 0.55]) {
    test(`overview at ${zoom} uses a bounded title without overlapping badges`, () => {
        const shared = {
            round: value => Math.round(value * 100) / 100,
            getTextMeasureService: () => null,
            resolveProgressDisplay: () => ({ title: "Not started" }),
            resolveCanvasNodePaletteStyle: () => ({}),
            getCanvasRuntimePrimitives: () => ({ wrapText: (_context, text, width, count) => {
                assert.equal(text, "Prepare the evening musician and sound check");
                assert.equal(width, 88);
                assert.equal(count, 4);
                return ["Prepare the evening", "musician and", "sound check"];
            } })
        };
        load("06-canvas-renderers.js", shared);
        const state = { ui: { zoom }, selectedIds: new Set() };
        assert.equal(shared.resolveCanvasNodeDetailMode(state, 41), "micro");
        const drawn = [];
        const context = {
            save() {}, restore() {}, beginPath() {}, roundRect() {}, fill() {}, stroke() {},
            fillText(text, x, y) { drawn.push({ text, x, y }); }
        };
        const bounds = { left: 10, top: 20, width: 100, height: 70 };
        shared.renderCanvasMicroNode(context, state, { id: "task", title: "Prepare the evening musician and sound check" }, bounds, "", {});
        assert.equal(drawn.length, 3);
        assert.ok(drawn.every(item => item.x === 60 && item.y > bounds.top && item.y < bounds.top + bounds.height));
    });
}

test("normal zoom retains full card details", () => {
    const shared = {};
    load("06-canvas-renderers.js", shared);
    assert.equal(shared.resolveCanvasNodeDetailMode({ ui: { zoom: 0.75 } }, 41), "full");
});

for (const service of [false, true]) {
    test(`fit keeps the whole scene below the toolbar, viewport service=${service}`, () => {
        const bounds = { minX: 0, minY: -1000, maxX: 2400, maxY: 1500 };
        let target;
        const shared = {
            MIN_ZOOM: 0.15, MAX_ZOOM: 1.75,
            clamp: (value, min, max) => Math.max(min, Math.min(max, value)),
            getVisibleNodes: () => [{}], getSceneBounds: () => bounds,
            animateViewportTransition: (_state, value) => { target = value; },
            getViewportControllerService: () => service ? { createFitViewTarget(request) {
                assert.equal(request.hostHeight, 916);
                const zoom = (request.hostHeight - 120) / 2500;
                return { zoom, panX: 960 - 1200 * zoom, panY: request.hostHeight / 2 - 250 * zoom };
            } } : null
        };
        load("05-viewport-and-events.js", shared);
        shared.fitView({
            host: { getBoundingClientRect: () => ({ top: 132, width: 1920, height: 1000 }) },
            shell: { querySelector: () => ({ getBoundingClientRect: () => ({ bottom: 216 }) }) }
        });
        assert.ok(bounds.minY * target.zoom + target.panY >= 84 + 59);
        assert.ok(bounds.maxY * target.zoom + target.panY <= 941);
    });
}
