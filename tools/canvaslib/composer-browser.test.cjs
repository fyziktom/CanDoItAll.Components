const assert = require("node:assert/strict");
const path = require("node:path");
const { after, before, test } = require("node:test");
const { chromium } = require("playwright");

const root = path.join(__dirname, "../../src/CanDoItAll.Components.CanvasLib/wwwroot");
const manifest = require("./asset-manifest.json");
let browser;
before(async () => { browser = await chromium.launch(); });
after(async () => { await browser?.close(); });

async function fixture(t, edit = false, extra = {}) {
    const page = await browser.newPage({ viewport: { width: 1920, height: 1080 } });
    page.setDefaultTimeout(10000);
    t.after(() => page.close());
    const errors = [];
    page.on("pageerror", error => errors.push(error.message));
    t.after(() => assert.deepEqual(errors, []));
    await page.route("https://canvas.test/", route => route.fulfill({ contentType: "text/html",
        body: '<!doctype html><div data-ui-theme="dark"><div class="cw-workbench-shell"><div class="cw-workbench-frame"><div class="cw-toolbar">Canvas</div><div id="host" class="cw-canvas-host" tabindex="0"></div></div></div></div>' }));
    await page.goto("https://canvas.test/");
    await page.addStyleTag({ path: path.join(root, "../../CanDoItAll.Components.BaseLib/wwwroot/css/output.css") });
    for (const asset of manifest.styles) {
        await page.addStyleTag({ path: path.join(root, asset.source) });
    }
    await page.addStyleTag({ content: ".cw-workbench-frame{height:850px}.cw-canvas-host{height:850px;width:1600px;position:relative}" });
    if (extra.hostStyle) {
        await page.addStyleTag({ content: extra.hostStyle });
    }
    if (extra.scrollY) {
        await page.evaluate(y => window.scrollTo(0, y), extra.scrollY);
    }
    for (const asset of manifest.runtimeScripts) {
        await page.addScriptTag({ path: path.join(root, asset.source) });
    }
    await page.evaluate(({ edit, extra }) => {
        window.calls = [];
        const host = document.querySelector("#host");
        CanDoItAll.canvasWorkbench.create(host, {
            invokeMethodAsync(method, ...args) {
                calls.push({ method, args });
                return extra.holdSubmission && method === "OnCreateAction"
                    ? new Promise(resolve => { window.releaseSubmission = resolve; }) : Promise.resolve();
            }
        }, { surfaceId: "composer-proof", nodes: [], links: [], uiState: { zoom: 1, panX: 0, panY: 0 } }, 0, 0, extra.runtimeOptions || {});
        window.state = host.__canvasWorkbenchState;
        window.runtime = CanDoItAll.canvasWorkbenchModule;
        const action = { actionId: "sample:create", label: edit ? "Edit sample" : "sample", titleLabel: "Name",
            submitLabel: edit ? "Save" : "Create", inputFields: [
                { key: "target", label: "Target", inputMode: "url", isRequired: true, sectionKey: "required", sectionTitle: "Connection" },
                { key: "optional", label: "Optional URL", inputMode: "url", sectionKey: "settings", sectionTitle: "Settings" },
                { key: "mode", label: "Mode", inputMode: "select", sectionKey: "settings", options: [{ value: "safe", label: "Safe" }, { value: "fast", label: "Fast" }] }
            ], ...extra.action };
        runtime.openCreateComposer(state, action, { actionId: action.actionId, placementKind: edit ? "edit" : "child",
            title: "Initial name", subtitle: "Description", notes: "Initial notes", x: 200, y: 160, ...extra.request });
    }, { edit, extra });
    await page.waitForFunction(() => {
        const composer = state.composer;
        const expected = composer.titleInput || composer.inputFieldEntries.find(entry => !entry.input.hidden && !entry.input.disabled)?.input || composer.createButton;
        return document.activeElement === expected;
    });
    return page;
}

test("creation shows one step, focuses name, preserves drafts and puts required settings before notes", async t => {
    const page = await fixture(t);
    assert.equal(await page.locator(".cw-canvas-composer__section:visible").count(), 1);
    assert.match(await page.locator(".cw-canvas-composer__step-status").textContent(), /Step 1.*Details/);
    await page.getByLabel("Name", { exact: true }).fill("Kept name");
    await page.keyboard.press("Enter");
    assert.match(await page.locator(".cw-canvas-composer__step-status").textContent(), /Step 2.*Connection/);
    await page.getByRole("button", { name: "Next", exact: true }).click();
    assert.equal(await page.getByLabel("Target *", { exact: true }).evaluate(input => input === document.activeElement), true);
    assert.equal(await page.evaluate(() => calls.some(call => call.method === "OnCreateAction")), false);
    await page.getByLabel("Target *", { exact: true }).fill("https://example.test/target");
    await page.getByRole("button", { name: "Next", exact: true }).focus();
    await page.keyboard.press("Enter");
    assert.match(await page.locator(".cw-canvas-composer__step-status").textContent(), /Step 3.*Settings.*optional/);
    await page.getByRole("button", { name: "Skip", exact: true }).click();
    assert.match(await page.locator(".cw-canvas-composer__step-status").textContent(), /Step 4.*Notes.*optional/);
    await page.getByLabel("Notes", { exact: true }).fill("Kept notes");
    await page.getByRole("button", { name: "Back", exact: true }).click();
    await page.getByRole("button", { name: "Back", exact: true }).click();
    await page.getByRole("button", { name: "Back", exact: true }).click();
    assert.equal(await page.getByLabel("Name", { exact: true }).inputValue(), "Kept name");
    await page.getByRole("button", { name: "Create", exact: true }).click();
    await page.waitForFunction(() => calls.some(call => call.method === "OnCreateAction"));
    const payload = await page.evaluate(() => JSON.parse(calls.find(call => call.method === "OnCreateAction").args[0]));
    assert.equal(payload.title, "Kept name");
    assert.equal(payload.notes, "Kept notes");
    assert.deepEqual(payload.inputValues, [{ key: "target", value: "https://example.test/target" }, { key: "optional", value: "" }, { key: "mode", value: "" }]);
});

test("finishing reveals an invalid hidden optional field without dispatching", async t => {
    const page = await fixture(t, false, { request: { inputValues: [{ key: "target", value: "https://example.test" }, { key: "optional", value: "unfinished" }] } });
    await page.getByRole("button", { name: "Create", exact: true }).click();
    assert.match(await page.locator(".cw-canvas-composer__step-status").textContent(), /Settings/);
    assert.equal(await page.getByLabel("Optional URL", { exact: true }).evaluate(input => input === document.activeElement), true);
    assert.equal(await page.evaluate(() => calls.some(call => call.method === "OnCreateAction")), false);
    await page.getByLabel("Optional URL", { exact: true }).fill("");
    await page.getByRole("button", { name: "Create", exact: true }).click();
    await page.waitForFunction(() => calls.some(call => call.method === "OnCreateAction"));
});

test("editing is a name-first form with populated optional settings visible and empty settings disclosed", async t => {
    const page = await fixture(t, true, { request: { inputValues: [{ key: "target", value: "https://example.test" }] } });
    assert.equal(await page.locator(".cw-canvas-composer__step-status").count(), 0);
    assert.equal(await page.getByRole("button", { name: "Next", exact: true }).count(), 0);
    assert.equal(await page.getByLabel("Notes", { exact: true }).isVisible(), true);
    const settings = page.locator("details.cw-canvas-composer__section");
    assert.equal(await settings.evaluate(element => element.open), false);
    await settings.locator("summary").click();
    await page.getByLabel("Optional URL", { exact: true }).fill("unfinished");
    await settings.locator("summary").click();
    await page.getByRole("button", { name: "Save", exact: true }).click();
    assert.equal(await settings.evaluate(element => element.open), true);
    assert.equal(await page.getByLabel("Optional URL", { exact: true }).evaluate(input => input === document.activeElement), true);
    await page.getByLabel("Optional URL", { exact: true }).fill("https://example.test/configured");
    await page.getByRole("button", { name: "Save", exact: true }).click();
    await page.waitForFunction(() => calls.some(call => call.method === "OnCreateAction"));
    const payload = await page.evaluate(() => JSON.parse(calls.find(call => call.method === "OnCreateAction").args[0]));
    assert.equal(payload.placementKind, "edit");
    assert.equal(payload.title, "Initial name");
    assert.equal(payload.notes, "Initial notes");
    const populated = await fixture(t, true, { request: { inputValues: [{ key: "optional", value: "https://example.test/configured" }] } });
    assert.equal(await populated.locator("details.cw-canvas-composer__section").evaluate(element => element.open), true);
});

test("IME and native textarea/select/button keys retain their meaning during creation", async t => {
    const page = await fixture(t, false, { request: { inputValues: [{ key: "target", value: "https://example.test" }] } });
    const ime = await page.getByLabel("Name", { exact: true }).evaluate(input => {
        const event = new KeyboardEvent("keydown", { key: "Enter", isComposing: true, bubbles: true, cancelable: true });
        input.dispatchEvent(event);
        return { prevented: event.defaultPrevented, step: state.composer.stepIndex };
    });
    assert.deepEqual(ime, { prevented: false, step: 0 });
    await page.getByRole("button", { name: "Next", exact: true }).click();
    await page.getByRole("button", { name: "Next", exact: true }).click();
    const select = await page.getByRole("combobox").evaluate(input => {
        const event = new KeyboardEvent("keydown", { key: "Enter", bubbles: true, cancelable: true });
        input.dispatchEvent(event);
        return { prevented: event.defaultPrevented, step: state.composer.stepIndex };
    });
    assert.deepEqual(select, { prevented: false, step: 2 });
    await page.getByRole("button", { name: "Skip", exact: true }).click();
    await page.getByLabel("Notes", { exact: true }).fill("Line one");
    await page.getByLabel("Notes", { exact: true }).press("End");
    await page.keyboard.press("Enter");
    assert.equal(await page.getByLabel("Notes", { exact: true }).inputValue(), "Line one\n");
    assert.equal(await page.evaluate(() => calls.some(call => call.method === "OnCreateAction")), false);
    await page.getByRole("button", { name: "Cancel", exact: true }).focus();
    await page.keyboard.press("Enter");
    assert.equal(await page.locator(".cw-canvas-composer").count(), 0);
    assert.equal(await page.evaluate(() => calls.some(call => call.method === "OnCreateAction")), false);
});

test("required attachment precedes optional steps and cannot be skipped or omitted", async t => {
    const page = await fixture(t, false, { action: { requiresFile: true }, request: { inputValues: [{ key: "target", value: "https://example.test" }] } });
    assert.equal(await page.getByRole("button", { name: "Create", exact: true }).isDisabled(), true);
    await page.getByRole("button", { name: "Next", exact: true }).click();
    await page.getByRole("button", { name: "Next", exact: true }).click();
    assert.match(await page.locator(".cw-canvas-composer__step-status").textContent(), /Step 3.*Attachment/);
    await page.getByRole("button", { name: "Next", exact: true }).click();
    assert.match(await page.getByRole("alert").textContent(), /required file/);
    await page.locator('input[type="file"]').setInputFiles({ name: "note.txt", mimeType: "text/plain", buffer: Buffer.from("durable payload") });
    await page.waitForFunction(() => !state.composer.createButton.disabled);
    await page.getByRole("button", { name: "Create", exact: true }).click();
    await page.waitForFunction(() => calls.some(call => call.method === "OnCreateAction"));
    const payload = await page.evaluate(() => JSON.parse(calls.find(call => call.method === "OnCreateAction").args[0]));
    assert.equal(payload.uploadedFile.fileName, "note.txt");
    assert.equal(Buffer.from(payload.uploadedFile.base64Data, "base64").toString(), "durable payload");
});

test("inline note uses readable theme foregrounds and preserves native note save/cancel keys", async t => {
    const page = await fixture(t);
    await page.getByRole("button", { name: "Cancel", exact: true }).click();
    await page.evaluate(() => runtime.openInlineNoteComposer(state, {
        kind: "note-edit", nodeId: "note", value: "Original note", anchorWorld: { x: 300, y: 180 }
    }));
    const input = page.locator(".cw-note-editor__input");
    await input.waitFor();
    for (const theme of ["dark", "light"]) {
        await page.locator("[data-ui-theme]").evaluate((element, theme) => element.dataset.uiTheme = theme, theme);
        const paint = await input.evaluate(element => ({ color: getComputedStyle(element).color,
            expected: getComputedStyle(element).getPropertyValue("--cw-text").trim(),
            hint: getComputedStyle(element.parentElement.querySelector(".cw-note-editor__hint")).color,
            background: getComputedStyle(element).backgroundColor }));
        const normalized = await page.evaluate(paint => {
            const canvas = document.createElement("canvas");
            const context = canvas.getContext("2d");
            context.fillStyle = paint.expected;
            const expected = context.fillStyle;
            context.fillStyle = paint.color;
            return { expected, actual: context.fillStyle };
        }, paint);
        assert.equal(normalized.actual, normalized.expected);
        assert.equal(await input.inputValue(), "Original note");
    }
    await input.press("End");
    await page.keyboard.press("Shift+Enter");
    await page.keyboard.type("Second line");
    await page.keyboard.press("Enter");
    await page.waitForFunction(() => calls.some(call => call.method === "OnNodeEdited"));
    const payload = await page.evaluate(() => JSON.parse(calls.find(call => call.method === "OnNodeEdited").args[0]));
    assert.equal(payload.notes, "Original note\nSecond line");
    assert.equal(await page.locator(".cw-note-editor").count(), 0);
});

test("long creation and edit forms keep their header and footer outside the one scrolling body", async t => {
    const fields = Array.from({ length: 18 }, (_, index) => ({ key: `field${index}`, label: `Value ${index}`,
        sectionKey: "required", sectionTitle: "Required settings", isRequired: true }));
    for (const edit of [false, true]) {
        const page = await fixture(t, edit, { action: { inputFields: fields } });
        if (!edit) {
            await page.getByRole("button", { name: "Next", exact: true }).click();
        }
        const title = page.locator(".cw-canvas-composer__title");
        const actions = page.locator(".cw-canvas-composer__actions");
        const before = { title: await title.boundingBox(), actions: await actions.boundingBox() };
        const body = await page.locator(".cw-canvas-composer__scroll").evaluate(element => {
            element.scrollTop = element.scrollHeight;
            return { client: element.clientHeight, height: element.scrollHeight, top: element.scrollTop };
        });
        assert.ok(body.height > body.client && body.top > 0);
        assert.deepEqual(await title.boundingBox(), before.title);
        assert.deepEqual(await actions.boundingBox(), before.actions);
        assert.ok(before.title.y >= 0 && before.actions.y + before.actions.height <= 1080);
    }
});

test("native optional disclosure expansion and collapse keep edit actions visible without replacing fields", async t => {
    const page = await fixture(t, true, {
        hostStyle: ".cw-workbench-frame{height:1007px;margin-top:73px}.cw-canvas-host{height:1007px}",
        action: { inputFields: Array.from({ length: 8 }, (_, index) => ({ key: `optional${index}`, label: `Setting ${index}`,
            sectionKey: "settings", sectionTitle: "Optional settings" })) },
        request: { x: 900, y: 270 }
    });
    const composer = page.locator(".cw-canvas-composer");
    const section = composer.locator("details");
    const initial = await composer.boundingBox();
    const firstInput = await page.getByLabel("Setting 0", { exact: true }).elementHandle();
    for (const open of [true, false, true]) {
        await section.evaluate(element => {
            window.disclosureToggled = new Promise(resolve => element.addEventListener("toggle", () => resolve(), { once: true }));
        });
        await section.locator("summary").click();
        await page.evaluate(() => disclosureToggled);
        assert.equal(await section.evaluate(element => element.open), open);
        const bounds = await composer.boundingBox();
        const footer = await composer.locator(".cw-canvas-composer__actions").boundingBox();
        assert.ok(footer.y + footer.height <= 1062.5, JSON.stringify({ initial, bounds, footer }));
        if (open) {
            assert.ok(bounds.height > initial.height);
            await page.getByLabel("Setting 0", { exact: true }).fill("Retained setting");
        }
        assert.equal(await firstInput.evaluate(input => input === state.composer.inputFieldEntries[0].input), true);
    }
    assert.equal(await page.getByLabel("Setting 0", { exact: true }).inputValue(), "Retained setting");
    await page.getByRole("button", { name: "Save", exact: true }).click();
    await page.waitForFunction(() => calls.some(call => call.method === "OnCreateAction"));
    const payload = await page.evaluate(() => JSON.parse(calls.find(call => call.method === "OnCreateAction").args[0]));
    assert.equal(payload.title, "Initial name");
    assert.equal(payload.inputValues[0].value, "Retained setting");
});

test("creation and edit footers fit the visible viewport when the canvas extends below and to its right", async t => {
    for (const edit of [false, true]) {
        const page = await fixture(t, edit, {
            hostStyle: ".cw-workbench-frame{margin-top:460px;margin-left:500px;width:1600px}",
            scrollY: 160,
            request: { x: 1500, y: 760, inputValues: [{ key: "target", value: "https://example.test" }] }
        });
        const bounds = await page.locator(".cw-canvas-composer").boundingBox();
        const toolbar = await page.locator(".cw-toolbar").boundingBox();
        const footer = await page.locator(".cw-canvas-composer__actions").boundingBox();
        assert.ok(bounds.x >= 18 && bounds.x + bounds.width <= 1902.5, JSON.stringify(bounds));
        assert.ok(bounds.y >= toolbar.y + toolbar.height + 17.5, JSON.stringify(bounds));
        assert.ok(footer.y + footer.height <= 1062.5, JSON.stringify(footer));
        assert.equal(await page.getByLabel("Name", { exact: true }).evaluate(input => input === document.activeElement), true);
        await page.evaluate(() => window.scrollTo(0, 0));
        await page.waitForFunction(() => window.scrollY === 0 &&
            state.composer.element.getBoundingClientRect().bottom <= 1062.5);
        await page.getByRole("button", { name: edit ? "Save" : "Create", exact: true }).click();
        await page.waitForFunction(() => calls.some(call => call.method === "OnCreateAction"));
        const payload = await page.evaluate(() => JSON.parse(calls.find(call => call.method === "OnCreateAction").args[0]));
        assert.equal(payload.title, "Initial name");
        assert.equal(payload.inputValues[0].value, "https://example.test");
        await page.evaluate(() => runtime.openInlineNoteComposer(state, {
            kind: "note-edit", nodeId: "note", value: "Retained note", anchorWorld: { x: 1500, y: 760 }
        }));
        const note = page.getByRole("textbox", { name: "Note text", exact: true });
        const originalNote = await note.elementHandle();
        await page.evaluate(() => {
            window.scrollTo(0, 160);
            document.querySelector("[data-ui-theme]").dataset.uiTheme = "light";
        });
        await page.waitForFunction(() => window.scrollY === 160);
        await page.evaluate(() => {
            window.scrollTo(0, 0);
            document.querySelector("[data-ui-theme]").dataset.uiTheme = "dark";
        });
        await page.waitForFunction(() => window.scrollY === 0 &&
            state.composer.element.getBoundingClientRect().bottom <= 1062.5);
        assert.equal(await originalNote.evaluate(input => input === document.querySelector(".cw-note-editor__input")), true);
        assert.equal(await note.inputValue(), "Retained note");
        const cleanup = await page.evaluate(() => {
            const remove = document.removeEventListener;
            let removedScrollHandler = false;
            document.removeEventListener = function (type, handler, options) {
                removedScrollHandler ||= type === "scroll" && handler === state.nodeToolbarScroll && options === true;
                return remove.call(this, type, handler, options);
            };
            CanDoItAll.canvasWorkbench.dispose(state.host);
            document.removeEventListener = remove;
            window.scrollTo(0, 160);
            return removedScrollHandler;
        });
        assert.equal(cleanup, true);
    }
});

test("reviewed draft application keeps controls and caret, exposes configured settings, and still waits for save acknowledgement", async t => {
    const page = await fixture(t, true, { runtimeOptions: { tracksComposerOpenings: true, requiresComposerAcknowledgement: true },
        holdSubmission: true, request: { inputValues: [{ key: "target", value: "https://example.test" }] } });
    const result = await page.evaluate(() => {
        const composer = state.composer;
        const title = composer.titleInput;
        const optional = composer.inputFieldEntries[1].input;
        title.setSelectionRange(2, 5);
        const draft = runtime.readComposerDraft(state);
        const request = { openingId: draft.openingId, expectedRevision: draft.revision,
            createRequest: { ...draft.createRequest, title: "Reviewed name", inputValues: draft.createRequest.inputValues.map(value =>
                value.key === "optional" ? { ...value, value: "https://example.test/configured" } : value) } };
        const applied = runtime.applyComposerDraft(state, request);
        return { status: applied.status, sameTitle: title === state.composer.titleInput,
            sameOptional: optional === state.composer.inputFieldEntries[1].input, focused: document.activeElement === title,
            caret: [title.selectionStart, title.selectionEnd], disclosed: optional.closest("details").open };
    });
    assert.deepEqual(result, { status: "Applied", sameTitle: true, sameOptional: true, focused: true, caret: [2, 5], disclosed: true });
    await page.getByRole("button", { name: "Save", exact: true }).click();
    await page.waitForFunction(() => typeof releaseSubmission === "function");
    assert.equal(await page.getByRole("button", { name: "Save", exact: true }).isDisabled(), true);
    await page.evaluate(() => {
        const composer = state.composer;
        runtime.acknowledgeComposer(state, { openingId: composer.openingId,
            submissionId: composer.pendingSubmission.id, kind: "Committed", message: "Saved" });
        releaseSubmission();
    });
    assert.equal(await page.locator(".cw-canvas-composer").count(), 0);
});

test("a fixed required choice is context in Details without an extra wizard step or payload loss", async t => {
    const page = await fixture(t, false, { action: { inputFields: [
        { key: "kind", label: "Kind", inputMode: "select", isRequired: true, sectionKey: "kind", options: [{ value: "host", label: "Host" }] },
        { key: "target", label: "Target", isRequired: true, sectionKey: "target", sectionTitle: "Connection" }
    ] } });
    assert.equal(await page.locator(".cw-canvas-composer__fixed-value").textContent(), "Host");
    assert.equal(await page.locator("select").count(), 1);
    assert.equal(await page.locator("select").inputValue(), "host");
    assert.equal(await page.locator("select").isVisible(), false);
    assert.match(await page.locator(".cw-canvas-composer__step-status").textContent(), /Step 1 of 3.*Details/);
    await page.getByRole("button", { name: "Next", exact: true }).click();
    assert.match(await page.locator(".cw-canvas-composer__step-status").textContent(), /Step 2 of 3.*Connection/);
    await page.getByLabel("Target *", { exact: true }).fill("Target");
    await page.getByRole("button", { name: "Create", exact: true }).click();
    await page.waitForFunction(() => calls.some(call => call.method === "OnCreateAction"));
    const payload = await page.evaluate(() => JSON.parse(calls.find(call => call.method === "OnCreateAction").args[0]));
    assert.deepEqual(payload.inputValues, [{ key: "kind", value: "host" }, { key: "target", value: "Target" }]);
});

test("fixed-only context never becomes a step or steals focus without default text fields", async t => {
    const page = await fixture(t, false, { action: { showDefaultTextFields: false, inputFields: [
        { key: "kind", label: "Kind", inputMode: "select", isRequired: true, sectionKey: "kind", options: [{ value: "host", label: "Host" }] },
        { key: "driver", label: "Driver", inputMode: "select", isRequired: true, sectionKey: "connection", options: [{ value: "native", label: "Native" }] },
        { key: "address", label: "Address", isRequired: true, sectionKey: "connection" }
    ] } });
    assert.equal(await page.locator(".cw-canvas-composer__step-status").count(), 0);
    assert.equal(await page.getByRole("button", { name: "Next", exact: true }).count(), 0);
    assert.equal(await page.getByLabel("Address *", { exact: true }).evaluate(input => input === document.activeElement), true);
    assert.equal(await page.locator(".cw-canvas-composer__overview .cw-canvas-composer__fixed-value").textContent(), "Host");
    await page.getByLabel("Address *", { exact: true }).fill("Local host");
    await page.getByRole("button", { name: "Create", exact: true }).click();
    await page.waitForFunction(() => calls.some(call => call.method === "OnCreateAction"));
    const payload = await page.evaluate(() => JSON.parse(calls.find(call => call.method === "OnCreateAction").args[0]));
    assert.deepEqual(payload.inputValues, [{ key: "kind", value: "host" }, { key: "driver", value: "native" }, { key: "address", value: "Local host" }]);
});
