const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

function focusFixture(kind) {
    const frames = [];
    const document = {};
    const createElement = () => ({
        children: [],
        dataset: {},
        classList: { add() {} },
        addEventListener() {},
        get childElementCount() { return this.children.length; },
        get isConnected() { return document.body.contains(this); },
        appendChild(child) {
            child.parent = this;
            this.children.push(child);
        },
        contains(child) { return this === child || this.children.some(item => item.contains(child)); },
        remove() { this.parent.children = this.parent.children.filter(child => child !== this); },
        focus() { document.activeElement = this; },
        select() { this.selectCount = (this.selectCount || 0) + 1; }
    });
    document.body = createElement();
    document.activeElement = document.body;
    const shared = {
        createElement,
        round: Math.round,
        clearContextMenu() {},
        layoutComposer() {},
        closeComposer(state) {
            state.composer?.element.remove();
            state.composer = null;
        }
    };
    vm.runInNewContext(fs.readFileSync(path.join(__dirname,
        "../../src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/04-context-menu-and-composer.js"), "utf8"),
        { window: { requestAnimationFrame: callback => frames.push(callback), CanDoItAll: { canvasWorkbenchModule: shared } } });
    const state = { document, host: document.body };
    const open = () => {
        if (kind === "create") {
            shared.openCreateComposer(state, { label: "Link" }, {});
        } else {
            shared.openInlineNoteComposer(state, { kind: "note-create", placeholder: "Note", value: "Initial note", anchorWorld: { x: 0, y: 0 } });
        }
        return state.composer;
    };
    const input = composer => composer.titleInput || composer.textInput;
    return { state, shared, frames, open, input, createElement };
}

for (const kind of ["create", "note"]) {
    test(`${kind} opening focuses its initial input if the user has not moved focus`, () => {
        const { state, frames, open, input } = focusFixture(kind);
        const composer = open();
        frames.shift()();
        assert.equal(state.document.activeElement, input(composer));
        assert.equal(input(composer).selectCount, 1);
    });

    test(`${kind} deferred autofocus preserves an input already selected by the user`, () => {
        const { state, frames, open, input } = focusFixture(kind);
        const composer = open();
        const chosen = composer.subtitleInput || input(composer);
        chosen.focus();
        chosen.value = "Typing already started";
        frames.shift()();
        assert.equal(state.document.activeElement, chosen);
        assert.equal(chosen.value, "Typing already started");
        assert.equal(input(composer).selectCount, undefined);
    });

    test(`${kind} opening cannot steal focus from another surface`, () => {
        const { state, frames, open, createElement } = focusFixture(kind);
        open();
        const outside = createElement();
        state.document.body.appendChild(outside);
        outside.focus();
        frames.shift()();
        assert.equal(state.document.activeElement, outside);
    });

    test(`${kind} closed or replaced opening cannot run its deferred focus`, () => {
        const { state, shared, frames, open, input } = focusFixture(kind);
        const first = open();
        shared.closeComposer(state);
        frames.shift()();
        assert.equal(input(first).selectCount, undefined);
        const second = open();
        const successor = open();
        frames.shift()();
        assert.equal(input(second).selectCount, undefined);
        assert.equal(input(successor).selectCount, undefined);
        frames.shift()();
        assert.equal(state.document.activeElement, input(successor));
        assert.equal(input(successor).selectCount, 1);
    });
}

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
function acknowledgementFixture() {
    const calls = [];
    let release;
    let reject;
    const pending = new Promise((resolve, fail) => { release = resolve; reject = fail; });
    const shared = {
        submitCreateRequest: (_state, payload) => { calls.push(payload); return pending; },
        submitNodeEdit: (_state, payload) => { calls.push(payload); return pending; },
        closeComposer: state => { state.composer = null; },
        layoutComposer: () => {}
    };
    vm.runInNewContext(fs.readFileSync(path.join(__dirname,
        "../../src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/04-context-menu-and-composer.js"), "utf8"),
        { window: { crypto: { randomUUID: () => "submission" }, CanDoItAll: { canvasWorkbenchModule: shared } } });
    const composer = {
        kind: "create", request: { actionId: "edit" }, openingId: "opening", requiresAcknowledgement: true,
        titleInput: { value: "Original title" }, notesInput: { value: "Original draft" }, draftRevision: 0,
        createButton: {}, element: { setAttribute: () => {} }, status: {}
    };
    const state = { composer };
    const acknowledge = kind => shared.acknowledgeComposer(state, { openingId: "opening", submissionId: "submission", kind, message: kind });
    return { shared, state, composer, calls, release, reject, acknowledge };
}

test("rejected save retains all text and cannot dispatch a duplicate from the consumed opening", async () => {
    const { shared, state, composer, calls, release, acknowledge } = acknowledgementFixture();
    shared.commitComposer(state);
    shared.commitComposer(state);
    await Promise.resolve();
    assert.equal(calls.length, 1);
    assert.equal(calls[0].composerSubmissionId, "submission");
    assert.equal(composer.createButton.disabled, true);
    acknowledge("Rejected");
    release();
    assert.equal(state.composer, composer);
    assert.equal(composer.notesInput.value, "Original draft");
    assert.match(composer.status.textContent, /Rejected.*draft is preserved/);
    shared.commitComposer(state);
    assert.equal(calls.length, 1);
});

test("a committed unchanged draft closes only after its matching acknowledgement", async () => {
    const { shared, state, composer, release, acknowledge } = acknowledgementFixture();
    shared.commitComposer(state);
    await Promise.resolve();
    assert.equal(state.composer, composer);
    shared.acknowledgeComposer(state, { openingId: "opening", submissionId: "foreign", kind: "Committed" });
    assert.equal(state.composer, composer);
    acknowledge("Committed");
    release();
    assert.equal(state.composer, null);
});

test("a delayed commit preserves typing after submission and does not replay it", async () => {
    const { shared, state, composer, calls, release, acknowledge } = acknowledgementFixture();
    shared.commitComposer(state);
    await Promise.resolve();
    composer.notesInput.value = "Newer unsaved typing";
    composer.draftRevision++;
    acknowledge("Committed");
    release();
    assert.equal(calls[0].notes, "Original draft");
    assert.equal(state.composer, composer);
    assert.equal(composer.notesInput.value, "Newer unsaved typing");
    assert.match(composer.status.textContent, /Newer text remains/);
    assert.equal(composer.createButton.disabled, true);
});

test("late acknowledgement cannot close a replacement opening", async () => {
    const { shared, state, release, acknowledge } = acknowledgementFixture();
    shared.commitComposer(state);
    await Promise.resolve();
    const replacement = { openingId: "replacement", pendingSubmission: { id: "other", revision: 0 } };
    state.composer = replacement;
    acknowledge("Committed");
    release();
    assert.equal(state.composer, replacement);
});

for (const transportFailure of [false, true]) {
    test(`missing acknowledgement remains explicit and non-retryable (transport failure: ${transportFailure})`, async () => {
        const { shared, state, composer, release, reject, calls } = acknowledgementFixture();
        shared.commitComposer(state);
        await Promise.resolve();
        if (transportFailure) {
            reject(new Error("connection lost"));
        } else {
            release();
        }
        await new Promise(resolve => setImmediate(resolve));
        assert.equal(state.composer, composer);
        assert.match(composer.status.textContent, /could not be confirmed/);
        shared.commitComposer(state);
        assert.equal(calls.length, 1);
    });
}

test("inline note saves also wait for an acknowledgement", async () => {
    const { shared, state, composer, release, acknowledge, calls } = acknowledgementFixture();
    composer.kind = "note-edit";
    composer.textInput = { value: "Inline draft" };
    composer.nodeId = "note";
    shared.commitComposer(state);
    await Promise.resolve();
    assert.equal(state.composer, composer);
    assert.equal(calls[0].notes, "Inline draft");
    acknowledge("Committed");
    release();
    assert.equal(state.composer, null);
});
