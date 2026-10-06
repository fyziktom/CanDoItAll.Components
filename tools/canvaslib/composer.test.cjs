const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

function fixture(valid) {
    const calls = [];
    const input = { value: "", checkValidity: () => valid, reportValidity: () => calls.push("invalid") };
    const shared = {
        submitCreateRequest: (_state, payload) => calls.push(payload),
        closeComposer: state => { state.composer = null; }
    };
    const source = fs.readFileSync(path.join(__dirname,
        "../../src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/04-context-menu-and-composer.js"), "utf8");
    vm.runInNewContext(source, { window: { CanDoItAll: { canvasWorkbenchModule: shared } } });
    return { calls, shared, state: { composer: { kind: "create", request: { actionId: "create" }, inputFieldEntries: [{ key: "amount", input }] } } };
}

test("invalid raw number cannot dispatch or close the composer", () => {
    const { shared, state, calls } = fixture(false);
    shared.commitComposer(state);
    assert.ok(state.composer);
    assert.deepEqual(calls, ["invalid"]);
});

test("a valid intentionally empty optional field is submitted as a clear", () => {
    const { shared, state, calls } = fixture(true);
    shared.commitComposer(state);
    assert.equal(state.composer, null);
    assert.equal(calls.length, 1);
    assert.equal(calls[0].inputValues[0].value, "");
});

function interactionFixture(services = {}) {
    const calls = [];
    const shared = { ...services };
    const source = fs.readFileSync(path.join(__dirname,
        "../../src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/03-interaction-and-state.js"), "utf8");
    vm.runInNewContext(source, { window: { CanDoItAll: { canvasWorkbenchModule: shared } } });
    let release;
    const opened = new Promise(resolve => { release = resolve; });
    const state = {
        host: { querySelectorAll: () => [] },
        dotNetRef: { invokeMethodAsync: (...args) => calls.push(args) },
        composer: { openingId: "original", opened }
    };
    return { shared, state, calls, release, opened };
}

test("delayed opening acknowledgement delivers the submitted original identity without closing its successor", async () => {
    const { shared, state, calls, release, opened } = interactionFixture();
    shared.submitCreateRequest(state, { actionId: "note", parentNodeId: "original-parent" });
    shared.closeComposer(state, { focusHost: false });
    state.composer = { openingId: "successor" };
    assert.equal(calls.length, 0);
    release();
    await opened;
    assert.equal(state.composer.openingId, "successor");
    assert.equal(calls.length, 1);
    assert.equal(calls[0][0], "OnCreateAction");
    assert.equal(JSON.parse(calls[0][1]).composerOpeningId, "original");
    assert.equal(JSON.parse(calls[0][1]).parentNodeId, "original-parent");
});

test("cancel retires only its original opening after registration", async () => {
    const { shared, state, calls, release, opened } = interactionFixture();
    shared.closeComposer(state, { focusHost: false });
    state.composer = { openingId: "successor" };
    release();
    await opened;
    assert.deepEqual(calls, [["OnComposerClosed", "original"]]);
    assert.equal(state.composer.openingId, "successor");
});

test("composer actions fit the actual canvas below its toolbar", () => {
    const { shared, state } = interactionFixture({ clamp: (n, min, max) => Math.max(min, Math.min(max, n)), round: Math.round });
    const style = { setProperty: (key, value) => { style[key] = value; } };
    state.host = {
        getBoundingClientRect: () => ({ top: 258, bottom: 898, width: 1830, height: 640 }),
        closest: () => ({ querySelector: () => ({ getBoundingClientRect: () => ({ top: 274, bottom: 341 }) }) })
    };
    state.composer = {
        kind: "create", anchorHost: { x: 400, y: 100 },
        element: { style, getBoundingClientRect: () => ({ width: 544, height: Math.min(736, Number.parseInt(style["--cw-composer-available-height"])) }) }
    };
    shared.layoutComposer(state);
    assert.equal(style["--cw-composer-available-height"], "521px");
    assert.equal(style.top, "101px");
    assert.equal(Number.parseInt(style.top) + 521 + 18, 640);
});
