const assert = require("node:assert/strict");
const path = require("node:path");
const { after, before, test } = require("node:test");
const { chromium } = require("playwright");

const root = path.join(__dirname, "../../src/CanDoItAll.Components.CanvasLib/wwwroot");
const manifest = require("./asset-manifest.json");
let browser;
before(async () => { browser = await chromium.launch(); });
after(async () => { await browser?.close(); });

async function fixture(t) {
    const page = await browser.newPage({ viewport: { width: 1920, height: 1080 } });
    t.after(() => page.close());
    const errors = [];
    page.on("pageerror", error => errors.push(error.message));
    t.after(() => assert.deepEqual(errors, []));
    await page.setContent('<button id="outside">Outside</button><div class="cw-workbench-shell"><div class="cw-workbench-frame"><div class="cw-toolbar">Toolbar</div><div id="host" class="cw-canvas-host" tabindex="0"></div></div></div>');
    for (const asset of manifest.styles) {
        await page.addStyleTag({ path: path.join(root, asset.source) });
    }
    await page.addStyleTag({ content: ".cw-workbench-frame{height:900px}.cw-canvas-host{height:900px;width:1600px;position:relative}" });
    for (const asset of manifest.runtimeScripts) {
        await page.addScriptTag({ path: path.join(root, asset.source) });
    }
    await page.evaluate(() => {
        window.calls = [];
        window.surface = {
            surfaceId: "test", mode: "authoring",
            uiState: { selectedNodeIds: ["a"], zoom: 0.2, panX: 300, panY: 250 },
            clipboard: { allowCopy: true, allowCut: true, allowPaste: true, allowDuplicate: true },
            nodes: [{ id: "a", title: "Selected", x: 400, y: 400, width: 250, height: 150,
                toolbarActions: [
                    { actionId: "host:identifier", label: "Copy identifier", icon: "copy", keyboardCommand: "CopyIdentifier" },
                    { actionId: "host:delete", label: "Delete node", icon: "delete", tone: "danger" }
                ] }], links: []
        };
        CanDoItAll.canvasWorkbench.create(document.querySelector("#host"), {
            invokeMethodAsync(method, ...args) { calls.push({ method, args }); return Promise.resolve(); }
        }, surface, 0, 0, { hasClipboardHandler: true });
        window.state = document.querySelector("#host").__canvasWorkbenchState;
        window.runtime = CanDoItAll.canvasWorkbenchModule;
    });
    return page;
}

test("fixed-size toolbar follows zoom, stays below chrome, and dispatches host actions", async t => {
    const page = await fixture(t);
    await page.locator("#host").focus();
    await page.keyboard.press("Control+m");
    const toolbar = page.getByRole("toolbar", { name: "Selected node actions" });
    const initial = await toolbar.boundingBox();
    assert.ok(initial && initial.width > 200 && initial.height >= 38);
    await page.evaluate(() => CanDoItAll.canvasWorkbench.setZoomPercent(state.host, 100));
    const zoomed = await toolbar.boundingBox();
    assert.equal(zoomed.width, initial.width);
    assert.equal(zoomed.height, initial.height);
    const chrome = await page.locator(".cw-toolbar").boundingBox();
    assert.ok(zoomed.y >= chrome.y + chrome.height);
    assert.ok(zoomed.x >= 0 && zoomed.x + zoomed.width <= 1920 && zoomed.y + zoomed.height <= 1080);
    await page.getByRole("button", { name: "Copy identifier", exact: true }).click();
    assert.equal(await toolbar.isVisible(), false);
    const requests = await page.evaluate(() => calls.filter(call => call.method === "OnContextAction"));
    assert.deepEqual(requests.map(call => call.args.slice(0, 2)), [["a", "host:identifier"]]);
    assert.equal(await page.evaluate(() => document.activeElement === state.host), true);
});

test("Ctrl+I preserves the host command and accepted repeats do not redispatch", async t => {
    const page = await fixture(t);
    const result = await page.evaluate(() => {
        const send = (key, repeat = false) => {
            const event = new KeyboardEvent("keydown", { key, ctrlKey: true, repeat, bubbles: true, cancelable: true });
            state.host.dispatchEvent(event);
            return event.defaultPrevented;
        };
        return { accepted: [send("i"), send("i", true), send("c"), send("c", true), send("x"), send("v"), send("d")],
            actions: calls.filter(call => call.method === "OnContextAction"),
            clipboard: calls.filter(call => call.method === "OnClipboardAction").map(call => JSON.parse(call.args[0]).action) };
    });
    assert.deepEqual(result.accepted, [true, true, true, true, true, true, true]);
    assert.equal(result.actions.length, 1);
    assert.deepEqual(result.clipboard, ["Copy", "Cut", "Paste", "Duplicate"]);
});

test("native inputs, overlays, nested canvases and unimplemented commands retain their keys", async t => {
    const page = await fixture(t);
    const result = await page.evaluate(() => {
        const results = [];
        for (const html of ['<input>', '<textarea></textarea>', '<select><option>Value</option></select>',
            '<div contenteditable="true">Text</div>', '<div role="dialog"><button>Dialog</button></div>',
            '<div class="cw-floating-window"><button>Window</button></div>', '<div class="cw-canvas-host" tabindex="0"></div>']) {
            const container = document.createElement("div");
            container.innerHTML = html;
            state.host.appendChild(container);
            const target = container.querySelector("button") || container.firstElementChild;
            const event = new KeyboardEvent("keydown", { key: "i", ctrlKey: true, bubbles: true, cancelable: true });
            target.dispatchEvent(event);
            results.push(event.defaultPrevented);
        }
        for (const options of [{ key: "z", ctrlKey: true }, { key: "y", ctrlKey: true },
            { key: "m", metaKey: true }, { key: "i", ctrlKey: true, isComposing: true },
            { key: "i", ctrlKey: true, altKey: true }]) {
            const event = new KeyboardEvent("keydown", { ...options, bubbles: true, cancelable: true });
            state.host.dispatchEvent(event);
            results.push(event.defaultPrevented);
        }
        const prevented = new KeyboardEvent("keydown", { key: "i", ctrlKey: true, bubbles: true, cancelable: true });
        prevented.preventDefault();
        state.host.dispatchEvent(prevented);
        return { results, actions: calls.filter(call => call.method === "OnContextAction") };
    });
    assert.ok(result.results.every(value => value === false));
    assert.deepEqual(result.actions, []);
});

test("toolbar native Tab and Enter activate buttons without creating a note; Escape restores focus", async t => {
    const page = await fixture(t);
    await page.locator("#host").focus();
    await page.keyboard.press("Control+m");
    await page.keyboard.press("Tab");
    assert.equal(await page.evaluate(() => document.activeElement.getAttribute("aria-label")), "Delete node");
    await page.keyboard.press("Enter");
    assert.deepEqual(await page.evaluate(() => calls.filter(call => call.method === "OnContextAction").map(call => call.args[1])), ["host:delete"]);
    assert.equal(await page.locator(".cw-canvas-composer").count(), 0);
    await page.keyboard.press("Control+m");
    await page.keyboard.press("Escape");
    assert.equal(await page.getByRole("toolbar", { name: "Selected node actions" }).isVisible(), false);
    assert.equal(await page.evaluate(() => document.activeElement === state.host), true);
});

test("selection removal and outside click retire toolbar content and stale actions", async t => {
    const page = await fixture(t);
    await page.locator("#host").focus();
    await page.keyboard.press("Control+m");
    await page.locator("#outside").click();
    assert.equal(await page.evaluate(() => state.nodeToolbarActions === null && state.nodeToolbar.childElementCount === 0), true);
    await page.locator("#host").focus();
    await page.keyboard.press("Control+m");
    const result = await page.evaluate(() => {
        const oldButton = state.nodeToolbar.querySelector("button");
        surface.nodes = [];
        CanDoItAll.canvasWorkbench.update(state.host, surface, { hasClipboardHandler: true });
        oldButton.click();
        return { hidden: state.nodeToolbar.hidden, actions: state.nodeToolbarActions,
            calls: calls.filter(call => call.method === "OnContextAction") };
    });
    assert.equal(result.hidden, true);
    assert.equal(result.actions, null);
    assert.deepEqual(result.calls, []);
});

test("multiple or unsupported selections do not consume toolbar and identifier shortcuts", async t => {
    const page = await fixture(t);
    const result = await page.evaluate(() => {
        const attempt = () => ["m", "i"].map(key => {
            const event = new KeyboardEvent("keydown", { key, ctrlKey: true, bubbles: true, cancelable: true });
            state.host.dispatchEvent(event);
            return event.defaultPrevented;
        });
        state.selectedIds.add("second");
        const multiple = attempt();
        state.selectedIds.delete("second");
        state.lookups.byId.get("a").toolbarActions = [];
        return [...multiple, ...attempt()];
    });
    assert.deepEqual(result, [false, false, false, false]);
});

test("a canvas inside a dialog owns its keys; teardown removes toolbar listeners and held actions", async t => {
    const page = await fixture(t);
    const result = await page.evaluate(() => {
        const dialog = document.createElement("div");
        dialog.setAttribute("role", "dialog");
        state.host.parentElement.appendChild(dialog);
        dialog.appendChild(state.host);
        const event = new KeyboardEvent("keydown", { key: "m", ctrlKey: true, bubbles: true, cancelable: true });
        state.host.dispatchEvent(event);
        const opened = !state.nodeToolbar.hidden;
        const oldButton = state.nodeToolbar.querySelector("button");
        CanDoItAll.canvasWorkbench.dispose(state.host);
        oldButton.click();
        document.dispatchEvent(new Event("scroll"));
        return { opened, accepted: event.defaultPrevented, actions: state.nodeToolbarActions,
            hidden: state.nodeToolbar.hidden, calls: calls.filter(call => call.method === "OnContextAction") };
    });
    assert.equal(result.opened, true);
    assert.equal(result.accepted, true);
    assert.equal(result.hidden, true);
    assert.equal(result.actions, null);
    assert.deepEqual(result.calls, []);
});

test("refreshing an open toolbar retains focus on a remaining action or restores the canvas", async t => {
    const page = await fixture(t);
    await page.locator("#host").focus();
    await page.keyboard.press("Control+m");
    const result = await page.evaluate(() => {
        surface.nodes[0].toolbarActions = [surface.nodes[0].toolbarActions[1]];
        CanDoItAll.canvasWorkbench.update(state.host, surface, { hasClipboardHandler: true });
        const remainingFocus = document.activeElement.getAttribute("aria-label");
        surface.nodes[0].toolbarActions = [];
        CanDoItAll.canvasWorkbench.update(state.host, surface, { hasClipboardHandler: true });
        return { remainingFocus, hidden: state.nodeToolbar.hidden, canvasFocused: document.activeElement === state.host };
    });
    assert.equal(result.remainingFocus, "Delete node");
    assert.equal(result.hidden, true);
    assert.equal(result.canvasFocused, true);
});
