const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const source = fs.readFileSync(path.join(__dirname,
    "../../src/CanDoItAll.Components.OverlayLib/wwwroot/js/runtime/overlay-window.js"), "utf8");

function fixture() {
    function events() {
        const listeners = new Map();
        return {
            addEventListener(name, handler) {
                if (!listeners.has(name)) {
                    listeners.set(name, new Set());
                }
                listeners.get(name).add(handler);
            },
            removeEventListener(name, handler) { listeners.get(name)?.delete(handler); },
            emit(name, event) {
                for (const handler of listeners.get(name) || []) {
                    handler(event);
                }
            },
            count() { return [...listeners.values()].reduce((sum, handlers) => sum + handlers.size, 0); }
        };
    }
    const timers = new Map();
    let timer = 0;
    let observer;
    const container = { clientWidth: 1400, clientHeight: 1000, matches: () => false,
        querySelector: () => null, closest: () => null, getBoundingClientRect: () => ({ left: 0, top: 0 }) };
    const handle = events();
    const classes = new Set();
    const host = { ...events(), isConnected: true, dataset: {},
        closest: () => container, querySelector: () => handle,
        style: {
            setProperty(name, value) { this[name] = value; },
            removeProperty(name) { delete this[name]; }
        },
        classList: { add: name => classes.add(name), remove: name => classes.delete(name),
            contains: name => classes.has(name), toggle(name, value) { value ? classes.add(name) : classes.delete(name); } },
        get offsetWidth() { return parseFloat(this.style.width) || 200; },
        get offsetHeight() { return parseFloat(this.style.height) || 50; },
        getBoundingClientRect() {
            const left = parseFloat(this.style.left) || 0;
            const top = parseFloat(this.style.top) || 0;
            return { left, top, width: this.offsetWidth, height: this.offsetHeight,
                right: left + this.offsetWidth, bottom: top + this.offsetHeight };
        }
    };
    const window = { ...events(),
        setTimeout(callback) {
            timers.set(++timer, callback);
            return timer;
        },
        clearTimeout(id) { timers.delete(id); },
        requestAnimationFrame(callback) { timers.set(++timer, callback); },
        ResizeObserver: class {
            constructor(callback) {
                observer = this;
                this.callback = callback;
            }
            observe() {}
            disconnect() { this.disconnected = true; }
        }
    };
    const calls = [];
    const dotNet = { invokeMethodAsync(name, left, top, width, height) {
        calls.push({ left, top, width, height });
        return Promise.resolve();
    } };
    const options = { defaultWidth: 760, defaultHeight: 720, minWidth: 360, minHeight: 300,
        maxWidth: 1100, maxHeight: 920, state: { isVisible: true } };
    vm.runInNewContext(source, { window });
    const api = window.CanDoItAll.overlayWindow;
    function flush() {
        while (timers.size) {
            const batch = [...timers.values()];
            timers.clear();
            batch.forEach(callback => callback());
        }
    }
    api.create(host, dotNet, options);
    flush();
    return { api, host, handle, window, calls, options, observer, flush,
        begin() {
            const rect = host.getBoundingClientRect();
            host.emit("pointerdown", { button: 0, pointerId: 1, clientX: rect.right - 1, clientY: rect.bottom - 1 });
        },
        resize(width, height) {
            host.style.width = `${width}px`;
            host.style.height = `${height}px`;
            observer.callback([{ target: host }]);
            flush();
        },
        end() {
            window.emit("pointerup", { pointerId: 1 });
            flush();
        },
        echo(geometry) { api.update(host, { ...options, state: { isVisible: true, ...geometry } }); }
    };
}

test("first native resize retains every local size and publishes only on release", () => {
    const f = fixture();
    f.begin();
    for (const width of [780, 810, 860]) {
        f.resize(width, 650);
        assert.equal(f.host.offsetWidth, width);
        assert.equal(f.calls.length, 0);
    }
    f.end();
    assert.equal(f.calls.length, 1);
    assert.equal(f.calls[0].width, 860);
});

test("delayed acknowledgements cannot rewind a newer gesture, including after release", () => {
    const f = fixture();
    f.begin();
    f.resize(800, 600);
    f.end();
    const first = f.calls[0];
    f.begin();
    f.resize(900, 700);
    f.echo(first);
    assert.equal(f.host.offsetWidth, 900);
    f.end();
    f.echo(f.calls[1]);
    f.echo(first);
    assert.equal(f.host.offsetWidth, 900);
    assert.equal(f.host.offsetHeight, 700);
});

test("reset and minimize remain explicit controlled changes", () => {
    const f = fixture();
    f.begin();
    f.resize(900, 600);
    f.end();
    f.echo(f.calls[0]);
    f.api.update(f.host, f.options);
    assert.equal(f.host.offsetWidth, 760);
    f.echo(f.calls[0]);
    assert.equal(f.host.offsetWidth, 760);
    f.api.update(f.host, { ...f.options, state: { isVisible: true, isMinimized: true } });
    assert.equal(f.host.style.resize, "none");
    f.api.update(f.host, f.options);
    assert.equal(f.host.offsetWidth, 760);
});

test("container resizing clamps the current geometry without restoring old dimensions", () => {
    const f = fixture();
    f.begin();
    f.resize(900, 600);
    f.end();
    const container = f.host.closest();
    container.clientWidth = 800;
    f.observer.callback([{ target: container }]);
    assert.equal(f.host.offsetWidth, 768);
    assert.equal(f.host.offsetHeight, 600);
});

test("disposal removes gesture listeners, observer and pending publications", () => {
    const f = fixture();
    f.begin();
    f.resize(900, 600);
    f.api.dispose(f.host);
    f.end();
    assert.equal(f.calls.length, 0);
    assert.equal(f.window.count(), 0);
    assert.equal(f.host.count(), 0);
    assert.equal(f.handle.count(), 0);
    assert.equal(f.observer.disconnected, true);
});
