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

function comparisonFixture() {
    function input(value, type = "text", allowed) {
        return {
            value, type, tagName: type === "select-one" ? "SELECT" : "INPUT", valid: true,
            selectionStart: type === "text" ? 3 : null, selectionEnd: type === "text" ? 7 : null,
            selectionDirection: "forward",
            checkValidity() { return this.valid && (!allowed || allowed.includes(this.value)); },
            cloneNode() { return { ...this }; },
            setSelectionRange(start, end, direction) {
                assert.equal(type, "text");
                Object.assign(this, { selectionStart: start, selectionEnd: end, selectionDirection: direction });
            }
        };
    }
    const shared = { layoutComposer() {} };
    vm.runInNewContext(fs.readFileSync(path.join(__dirname,
        "../../src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/04-context-menu-and-composer.js"), "utf8"),
        { window: { CanDoItAll: { canvasWorkbenchModule: shared } } });
    const composer = {
        kind: "create", openingId: "opening", requiresAcknowledgement: true, draftRevision: 9,
        request: { actionId: "edit:original", sourceNodeId: "original", parentNodeId: "parent", objectSubtype: "note", placementKind: "edit" },
        titleInput: input("Original title"), subtitleInput: input(""), notesInput: input(" Local raw draft \n"),
        inputFieldEntries: [{ key: "amount", input: input("2.00", "number") }, { key: "mode", input: input("first", "select-one", ["first", "second"]) }],
        createButton: {}, status: {}
    };
    const state = { composer, document: { activeElement: composer.notesInput } };
    const request = () => ({ openingId: "opening", expectedRevision: 9,
        createRequest: { ...shared.readComposerDraft(state).createRequest, title: "Merged title", notes: "Combined" } });
    return { shared, state, composer, request };
}

test("comparison snapshot retains raw text, actual revision and pending native input", () => {
    const { shared, state, composer } = comparisonFixture();
    composer.inputFieldEntries[0].input.valid = false;
    const snapshot = shared.readComposerDraft(state);
    assert.equal(snapshot.revision, 9);
    assert.equal(snapshot.createRequest.notes, " Local raw draft \n");
    assert.equal(snapshot.hasPendingInput, true);
    assert.equal(composer.notesInput.value, snapshot.createRequest.notes);
});

for (const [property, value, status] of [["openingId", "retired", "Retired"], ["expectedRevision", 8, "InputChanged"]]) {
    test(`local apply refuses ${property} mismatch before changing any input`, () => {
        const { shared, state, composer, request } = comparisonFixture();
        const candidate = { ...request(), [property]: value };
        assert.equal(shared.applyComposerDraft(state, candidate).status, status);
        assert.equal(composer.titleInput.value, "Original title");
        assert.equal(composer.notesInput.value, " Local raw draft \n");
        assert.equal(composer.draftRevision, 9);
    });
}

test("unfinished native input blocks local apply without clearing the original control", () => {
    const { shared, state, composer, request } = comparisonFixture();
    composer.inputFieldEntries[0].input.valid = false;
    assert.equal(shared.applyComposerDraft(state, request()).status, "PendingInput");
    assert.equal(composer.inputFieldEntries[0].input.value, "2.00");
    assert.equal(composer.titleInput.value, "Original title");
});

for (const kind of ["Unconfirmed", "Committed"]) {
    test(`${kind} outcome cannot become retryable from a local comparison`, () => {
        const { shared, state, composer, request } = comparisonFixture();
        composer.submissionFinished = true;
        composer.lastResultKind = kind;
        assert.equal(shared.applyComposerDraft(state, request()).status, "OutcomeRequiresReview");
        assert.equal(composer.submissionFinished, true);
        assert.equal(composer.notesInput.value, " Local raw draft \n");
    });
}

test("an unknown field or invalid last result cannot partially change earlier fields", () => {
    for (const extra of [false, true]) {
        const { shared, state, composer, request } = comparisonFixture();
        const candidate = request();
        if (extra) {
            candidate.createRequest.inputValues.push({ key: "foreign", value: "unsent" });
        } else {
            candidate.createRequest.inputValues[1].value = "invalid option";
        }
        assert.equal(shared.applyComposerDraft(state, candidate).status, "ShapeChanged");
        assert.equal(composer.titleInput.value, "Original title");
        assert.equal(composer.notesInput.value, " Local raw draft \n");
        assert.equal(composer.inputFieldEntries[1].input.value, "first");
    }
});

test("reviewed local apply keeps the actual controls, active input and caret", () => {
    const { shared, state, composer, request } = comparisonFixture();
    const title = composer.titleInput;
    const notes = composer.notesInput;
    const result = shared.applyComposerDraft(state, request());
    assert.equal(result.status, "Applied");
    assert.equal(result.revision, 10);
    assert.equal(composer.titleInput, title);
    assert.equal(composer.notesInput, notes);
    assert.equal(state.document.activeElement, notes);
    assert.equal(notes.value, "Combined");
    assert.equal(notes.selectionStart, 3);
    assert.equal(notes.selectionEnd, 7);
    assert.equal(composer.submissionFinished, false);
});

test("focused select result is applied without invoking text selection APIs", () => {
    const { shared, state, composer, request } = comparisonFixture();
    const select = composer.inputFieldEntries[1].input;
    state.document.activeElement = select;
    const candidate = request();
    candidate.createRequest.inputValues[1].value = "second";
    assert.equal(shared.applyComposerDraft(state, candidate).status, "Applied");
    assert.equal(state.document.activeElement, select);
    assert.equal(select.value, "second");
});

test("only an explicit reviewed apply can rearm a definitely rejected opening", () => {
    const { shared, state, composer, request } = comparisonFixture();
    composer.submissionFinished = true;
    composer.lastResultKind = "Rejected";
    assert.equal(shared.readComposerDraft(state).canApply, true);
    assert.equal(shared.applyComposerDraft(state, request()).status, "Applied");
    assert.equal(composer.submissionFinished, false);
    assert.equal(composer.lastResultKind, null);
});

test("inline reviewed title is explicit and cannot retarget a different note", () => {
    const { shared, state, composer } = comparisonFixture();
    composer.kind = "note-edit";
    composer.nodeId = "original";
    composer.textInput = composer.notesInput;
    const candidate = { openingId: "opening", expectedRevision: 9,
        editRequest: { nodeId: "foreign", title: "Reviewed title", notes: "Reviewed note", hasExplicitTitle: true } };
    assert.equal(shared.applyComposerDraft(state, candidate).status, "ShapeChanged");
    candidate.editRequest.nodeId = "original";
    assert.equal(shared.applyComposerDraft(state, candidate).status, "Applied");
    const snapshot = shared.readComposerDraft(state);
    assert.equal(snapshot.editRequest.title, "Reviewed title");
    assert.equal(snapshot.editRequest.hasExplicitTitle, true);
    assert.equal(snapshot.editRequest.notes, "Reviewed note");
});

function closeGuardFixture() {
    const calls = [];
    let decide;
    const decision = new Promise(resolve => { decide = resolve; });
    const shared = {};
    const window = { CanDoItAll: { canvasWorkbenchModule: shared }, requestAnimationFrame: callback => callback() };
    const context = vm.createContext({ window });
    const root = "../../src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/";
    for (const name of ["03-interaction-and-state.js", "04-context-menu-and-composer.js"]) {
        vm.runInContext(fs.readFileSync(path.join(__dirname, root, name), "utf8"), context);
    }
    const composer = {
        openingId: "original-opening", requiresAcknowledgement: true, draftRevision: 2, opened: Promise.resolve(),
        element: { remove() { this.removed = true; }, setAttribute() {} }, status: {}, createButton: {}
    };
    const state = {
        composer, supportsComposerCloseGuard: true,
        host: { querySelectorAll: () => [], focus() {} },
        dotNetRef: { invokeMethodAsync(method, payload) {
            calls.push({ method, payload });
            return method === "OnComposerCloseRequested" ? decision : Promise.resolve();
        } }
    };
    return { shared, state, composer, calls, decide };
}

const flushCloseDecision = async () => { for (let index = 0; index < 12; index++) { await Promise.resolve(); } };

test("Continue editing retains the actual composer and asks only one close question", async () => {
    const { shared, state, composer, calls, decide } = closeGuardFixture();
    assert.equal(shared.closeComposer(state), false);
    assert.equal(shared.closeComposer(state), false);
    await flushCloseDecision();
    assert.equal(calls.filter(call => call.method === "OnComposerCloseRequested").length, 1);
    assert.equal(state.composer, composer);
    decide(false);
    await flushCloseDecision();
    assert.equal(state.composer, composer);
    assert.equal(composer.element.removed, undefined);
});

test("a discard decision for older raw input cannot close newer input", async () => {
    const { shared, state, composer, decide } = closeGuardFixture();
    shared.closeComposer(state);
    await flushCloseDecision();
    composer.draftRevision++;
    decide(true);
    await flushCloseDecision();
    assert.equal(state.composer, composer);
    assert.equal(composer.element.removed, undefined);
});

test("an approved current close removes the original composer and runs its replacement once", async () => {
    const { shared, state, composer, calls, decide } = closeGuardFixture();
    let replacements = 0;
    shared.closeComposer(state, { onClosed: () => { replacements++; } });
    await flushCloseDecision();
    decide(true);
    await flushCloseDecision();
    assert.equal(state.composer, null);
    assert.equal(composer.element.removed, true);
    assert.equal(replacements, 1);
    assert.equal(calls.filter(call => call.method === "OnComposerClosed").length, 1);
});

test("only an acknowledged unchanged commit bypasses the optional close decision", () => {
    const { shared, state, composer, calls } = closeGuardFixture();
    composer.pendingSubmission = { id: "accepted", revision: composer.draftRevision };
    shared.acknowledgeComposer(state, { openingId: composer.openingId, submissionId: "accepted", kind: "Committed", message: "Saved" });
    assert.equal(state.composer, null);
    assert.equal(calls.filter(call => call.method === "OnComposerCloseRequested").length, 0);
});
