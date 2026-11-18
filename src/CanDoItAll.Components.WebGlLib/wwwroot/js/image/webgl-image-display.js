// WebGlLib image display: a float image view (WebGL2) and its display-window histogram (SVG).
//
// The view draws raw sample planes as they are: samples are uploaded as R32F textures (nearest, texelFetch), an
// optional invalid-sample mask as R8UI and an optional reference plane for the negative log ratio -ln(I / Iref).
// Window, gamma, inversion, smoothing, pan and zoom are display operations only; the samples are never changed.
// Row zero of the image is drawn at the top. Without WebGL2 the samples are still loaded, so values, statistics and
// the histogram keep working.
//
// The histogram bins the displayed values of one view on a scale that stays put while the window changes, so a
// narrowed window can be widened again. Its edges and the window itself are dragged (the linked views follow at once),
// focused edges step with the keyboard, and a double-click fits the scale to the values and the window.
//
// Both are created from Blazor (WebGlFloatImageView, WebGlImageWindowHistogram) and linked by the views' ids.
// globalThis.CanDoItAll.webglImage exposes read-only proof hooks for browser tests.

const SVG_NS = 'http://www.w3.org/2000/svg';

const VERTEX_SHADER = `#version 300 es
precision highp float;
void main() {
    vec2 p = vec2(float((gl_VertexID << 1) & 2), float(gl_VertexID & 2));
    gl_Position = vec4(p * 2.0 - 1.0, 0.0, 1.0);
}`;

const FRAGMENT_SHADER = `#version 300 es
precision highp float;
precision highp sampler2D;
precision highp usampler2D;
uniform sampler2D uImage;
uniform sampler2D uReference;
uniform usampler2D uMask;
uniform vec2 uViewport;      // device pixels
uniform vec3 uView;          // image offset (x, y) at the top-left device pixel, image pixels per device pixel
uniform float uLow;
uniform float uHigh;
uniform float uGamma;
uniform bool uInvert;
uniform int uMode;           // 0 linear, 1 -ln(I / Iref)
uniform vec4 uRegion;        // x, y, w, h in image pixels (w <= 0: none)
uniform float uProfileRow;   // < 0: none
uniform bool uSmooth;        // display-only bilinear interpolation between pixel centres
uniform vec3 uInvalidColor;
uniform vec3 uRegionColor;
uniform vec3 uProfileColor;
out vec4 outColor;
void main() {
    ivec2 size = textureSize(uImage, 0);
    vec2 frag = vec2(gl_FragCoord.x, uViewport.y - gl_FragCoord.y);
    vec2 img = uView.xy + frag * uView.z;
    if (img.x < 0.0 || img.y < 0.0 || img.x >= float(size.x) || img.y >= float(size.y)) discard;
    ivec2 p = ivec2(floor(img));
    float raw = texelFetch(uImage, p, 0).r;
    bool invalid = texelFetch(uMask, p, 0).r != 0u || isnan(raw) || isinf(raw);
    float v = raw;
    if (uMode == 1) {
        float r0 = texelFetch(uReference, p, 0).r;
        if (!(raw > 0.0) || !(r0 > 0.0)) invalid = true; else v = -log(raw / r0);
    }
    if (uSmooth && !invalid) {
        vec2 q = img - 0.5;
        ivec2 q0 = ivec2(floor(q));
        vec2 f = q - vec2(q0);
        float s[4];
        bool ok = true;
        for (int k = 0; k < 4; k++) {
            ivec2 pk = clamp(q0 + ivec2(k & 1, k >> 1), ivec2(0), size - 1);
            float rk = texelFetch(uImage, pk, 0).r;
            float vk = rk;
            if (texelFetch(uMask, pk, 0).r != 0u || isnan(rk) || isinf(rk)) ok = false;
            if (uMode == 1) { float rr = texelFetch(uReference, pk, 0).r; if (!(rk > 0.0) || !(rr > 0.0)) ok = false; else vk = -log(rk / rr); }
            s[k] = vk;
        }
        if (ok) v = mix(mix(s[0], s[1], f.x), mix(s[2], s[3], f.x), f.y);
    }
    vec3 c;
    if (invalid) {
        c = uInvalidColor;
    } else {
        float t = clamp((v - uLow) / (uHigh - uLow), 0.0, 1.0);
        t = pow(t, 1.0 / uGamma);
        if (uInvert) t = 1.0 - t;
        c = vec3(t);
    }
    float px = uView.z * 1.5;
    if (uRegion.z > 0.0) {
        bool inX = img.x >= uRegion.x - px && img.x <= uRegion.x + uRegion.z + px;
        bool inY = img.y >= uRegion.y - px && img.y <= uRegion.y + uRegion.w + px;
        bool edgeX = abs(img.x - uRegion.x) < px || abs(img.x - (uRegion.x + uRegion.z)) < px;
        bool edgeY = abs(img.y - uRegion.y) < px || abs(img.y - (uRegion.y + uRegion.w)) < px;
        if ((edgeX && inY) || (edgeY && inX)) c = uRegionColor;
    }
    if (uProfileRow >= 0.0 && abs(img.y - (uProfileRow + 0.5)) < px * 0.6) c = mix(c, uProfileColor, 0.7);
    outColor = vec4(c, 1.0);
}`;

const DEFAULT_COLORS = { invalid: '#ff00ff', region: '#ffbf00', profile: '#1a99ff' };
const SAMPLE_LIMIT = 200000;

// ---- shared registry: views by id, so a histogram links to views created before or after it ----------------------
const views = new Map();
const hub = new EventTarget();

function proofRoot() {
    const root = globalThis.CanDoItAll || (globalThis.CanDoItAll = {});
    if (!root.webglImage) {
        root.webglImage = {
            // Read-only proof of a view: its image, display state, view transform and counters.
            proof: (target) => { const v = resolve(target); return v ? v.proof() : null; },
            // One RGBA pixel of a view's drawing buffer in device pixels from its top-left corner (null without WebGL2).
            readPixel: (target, x, y) => { const v = resolve(target); return v ? v.readPixel(x, y) : null; },
            // The displayed values of a view (a sample of at most 200000 valid values).
            displayedValues: (target) => { const v = resolve(target); return v ? Array.from(v.displayedValues()) : null; },
            // Read-only proof of a histogram: its scale, window and the values against the window.
            histogram: (target) => { const svg = typeof target === 'string' ? document.querySelector(target) : target; const h = svg && svg.__cdaWindowHistogram; return h ? h.stats() : null; },
        };
    }
    return root.webglImage;
}

function resolve(target) {
    if (!target) return null;
    if (typeof target === 'string') {
        if (views.has(target)) return views.get(target);
        const el = document.querySelector(target);
        return el ? el.__cdaImageView || null : null;
    }
    return target.__cdaImageView || null;
}

// ---- helpers --------------------------------------------------------------------------------------------------------
function parseHexColor(value, fallback) {
    const text = String(value || fallback).trim();
    const m = /^#?([0-9a-f]{6})$/i.exec(text) || /^#?([0-9a-f]{6})$/i.exec(fallback);
    const n = parseInt(m[1], 16);
    return [((n >> 16) & 255) / 255, ((n >> 8) & 255) / 255, (n & 255) / 255];
}

// Raw little-endian samples as display floats (the exact samples stay with their source).
function toFloats(buffer, type) {
    switch (type) {
        case 'float32': return new Float32Array(buffer);
        case 'float64': return Float32Array.from(new Float64Array(buffer));
        case 'uint8': return Float32Array.from(new Uint8Array(buffer));
        case 'int8': return Float32Array.from(new Int8Array(buffer));
        case 'uint16': return Float32Array.from(new Uint16Array(buffer));
        case 'int16': return Float32Array.from(new Int16Array(buffer));
        case 'uint32': return Float32Array.from(new Uint32Array(buffer));
        case 'int32': return Float32Array.from(new Int32Array(buffer));
        case 'uint64': return Float32Array.from(new BigUint64Array(buffer), (x) => Number(x));
        case 'int64': return Float32Array.from(new BigInt64Array(buffer), (x) => Number(x));
        default: return null;
    }
}

async function fetchBytes(url, signal) {
    const response = await fetch(url, { credentials: 'same-origin', headers: { Accept: 'application/octet-stream' }, signal });
    if (!response.ok) throw new Error(`${response.status} ${response.statusText || 'request failed'}`.trim());
    return response.arrayBuffer();
}

function finite(v) { return typeof v === 'number' && Number.isFinite(v); }

// Calls onChange whenever devicePixelRatio changes (browser zoom, another monitor).
function watchDevicePixelRatio(onChange, signal) {
    const arm = () => {
        if (signal.aborted) return;
        const mq = globalThis.matchMedia(`(resolution: ${globalThis.devicePixelRatio || 1}dppx)`);
        mq.addEventListener('change', () => { onChange(); arm(); }, { once: true, signal });
    };
    arm();
}

// ---- float image view -----------------------------------------------------------------------------------------------
class FloatImageView {
    constructor(host, canvas, dotNet, options) {
        const o = options || {};
        this.id = o.id || host.id || '';
        this.host = host;
        this.canvas = canvas;
        this.dotNet = dotNet || null;
        this.emitProbe = !!o.probe;
        this.colors = {
            invalid: parseHexColor(o.invalidColor, DEFAULT_COLORS.invalid),
            region: parseHexColor(o.regionColor, DEFAULT_COLORS.region),
            profile: parseHexColor(o.profileColor, DEFAULT_COLORS.profile),
        };
        this.state = { low: NaN, high: NaN, gamma: 1, invert: false, mode: 0, region: [0, 0, 0, 0], profileRow: -1, smooth: false };
        this.view = { cx: 0, cy: 0, zoom: 1 };   // image-centre offset and zoom relative to fit
        this.image = null;                        // { key, width, height, data, reference, mask, label }
        this.loading = null;                      // { key, abort }
        this.diag = { uploads: 0, loads: 0, contextLost: 0, contextRestored: 0, maxTextureSize: 0, dprChanges: 0, draws: 0 };
        this.status = '';
        this.abort = new AbortController();
        this.probeFrame = 0;
        this.probeLast = '';
        this.pendingProbe = undefined;
        const sig = { signal: this.abort.signal };

        canvas.addEventListener('webglcontextlost', (e) => {
            e.preventDefault();   // ask the browser to restore the context
            this.diag.contextLost++;
            this.gl = null;       // its textures and program died with it
            this._status('context-lost', 'The WebGL context was lost: the display pauses until the browser restores it. Values and statistics are unaffected.');
        }, sig);
        canvas.addEventListener('webglcontextrestored', () => {
            this.diag.contextRestored++;
            this._init();
            if (this.image) this._upload();
            this.draw();
            if (this.gl) this._status('ready', '');
        }, sig);
        watchDevicePixelRatio(() => { this.diag.dprChanges++; this.draw(); }, this.abort.signal);
        canvas.addEventListener('pointermove', (e) => this._probe(e), sig);
        canvas.addEventListener('pointerleave', () => this._sendProbe(null), sig);
        canvas.addEventListener('wheel', (e) => {
            if (!this.image) return;
            e.preventDefault();
            this.view.zoom = Math.min(64, Math.max(0.25, this.view.zoom * (e.deltaY < 0 ? 1.15 : 1 / 1.15)));
            this.draw();
        }, Object.assign({ passive: false }, sig));
        canvas.addEventListener('dblclick', (e) => { if (!this.image) return; e.preventDefault(); this.resetView(); }, sig);
        let drag = null;
        canvas.addEventListener('pointerdown', (e) => {
            if (e.button !== 0 || !this.image) return;
            drag = { id: e.pointerId, x: e.clientX, y: e.clientY, cx: this.view.cx, cy: this.view.cy };
            canvas.setPointerCapture(e.pointerId);
            host.classList.add('is-panning');
        }, sig);
        const endDrag = (e) => {
            if (!drag || e.pointerId !== drag.id) return;
            if (canvas.hasPointerCapture(e.pointerId)) canvas.releasePointerCapture(e.pointerId);
            drag = null;
            host.classList.remove('is-panning');
        };
        canvas.addEventListener('pointerup', endDrag, sig);
        canvas.addEventListener('pointercancel', endDrag, sig);
        canvas.addEventListener('pointermove', (e) => {
            if (!drag || e.pointerId !== drag.id || !this.image) return;
            const s = this._scale();
            const dpr = globalThis.devicePixelRatio || 1;
            this.view.cx = drag.cx - (e.clientX - drag.x) * dpr * s;
            this.view.cy = drag.cy - (e.clientY - drag.y) * dpr * s;
            this.draw();
        }, sig);
        this.resizeObserver = new ResizeObserver(() => this.draw());
        this.resizeObserver.observe(canvas);
        host.__cdaImageView = this;
        this._init();
        this._mark();
        if (this.id) {
            views.set(this.id, this);
            hub.dispatchEvent(new CustomEvent('register', { detail: { id: this.id, view: this } }));
        }
        proofRoot();
    }

    _status(kind, message) {
        this.status = kind;
        this.host.dataset.glState = kind;
        if (this.dotNet) this.dotNet.invokeMethodAsync('NotifyStatus', kind, message || '').catch(() => { });
    }

    _init() {
        let gl = null;
        try { gl = this.canvas.getContext('webgl2', { antialias: false, alpha: true, premultipliedAlpha: false, preserveDrawingBuffer: true }); }
        catch (e) { gl = null; }
        if (!gl) {
            this.gl = null;
            this._status('unavailable', 'WebGL2 is not available in this browser: the image cannot be drawn, but its values and statistics are still available.');
            return;
        }
        // A context lost before this view existed: wait for the browser's restore event instead of building on it.
        if (gl.isContextLost()) { this.gl = null; this._status('context-lost', 'The WebGL context was lost: the display pauses until the browser restores it.'); return; }
        const compile = (type, src) => {
            const s = gl.createShader(type);
            gl.shaderSource(s, src);
            gl.compileShader(s);
            if (!gl.getShaderParameter(s, gl.COMPILE_STATUS)) throw new Error(gl.getShaderInfoLog(s));
            return s;
        };
        const program = gl.createProgram();
        gl.attachShader(program, compile(gl.VERTEX_SHADER, VERTEX_SHADER));
        gl.attachShader(program, compile(gl.FRAGMENT_SHADER, FRAGMENT_SHADER));
        gl.linkProgram(program);
        if (!gl.getProgramParameter(program, gl.LINK_STATUS)) throw new Error(gl.getProgramInfoLog(program));
        this.gl = gl;
        this.program = program;
        this.vao = gl.createVertexArray();
        this.u = {};
        for (const n of ['uImage', 'uReference', 'uMask', 'uViewport', 'uView', 'uLow', 'uHigh', 'uGamma', 'uInvert', 'uMode', 'uRegion', 'uProfileRow', 'uSmooth', 'uInvalidColor', 'uRegionColor', 'uProfileColor'])
            this.u[n] = gl.getUniformLocation(program, n);
        this.textures = { image: gl.createTexture(), reference: gl.createTexture(), mask: gl.createTexture() };
        this.diag.maxTextureSize = gl.getParameter(gl.MAX_TEXTURE_SIZE);
        const hp = gl.getShaderPrecisionFormat(gl.FRAGMENT_SHADER, gl.HIGH_FLOAT);
        this.diag.highpFloatPrecision = hp ? hp.precision : 0;
        this._status('ready', '');
    }

    _texture(texture, internal, format, type, w, h, data) {
        const gl = this.gl;
        gl.bindTexture(gl.TEXTURE_2D, texture);
        gl.pixelStorei(gl.UNPACK_ALIGNMENT, 1);
        gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, false);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        gl.texImage2D(gl.TEXTURE_2D, 0, internal, w, h, 0, format, type, data);
    }

    _upload() {
        const gl = this.gl, im = this.image;
        if (!gl || !im) return;
        if (im.width > this.diag.maxTextureSize || im.height > this.diag.maxTextureSize) {
            this._status('too-large', `The image (${im.width} × ${im.height}) exceeds this GPU's texture size (${this.diag.maxTextureSize}): it cannot be drawn here, but its values and statistics are still available.`);
            this.drawable = false;
            return;
        }
        this._texture(this.textures.image, gl.R32F, gl.RED, gl.FLOAT, im.width, im.height, im.data);
        this._texture(this.textures.reference, gl.R32F, gl.RED, gl.FLOAT, im.width, im.height, im.reference || new Float32Array(im.width * im.height));
        this._texture(this.textures.mask, gl.R8UI, gl.RED_INTEGER, gl.UNSIGNED_BYTE, im.width, im.height, im.mask || new Uint8Array(im.width * im.height));
        this.drawable = true;
        this.diag.uploads++;
    }

    // Loads a source { key, url, width, height, elementType, maskUrl, referenceUrl, referenceElementType, label }. The
    // same key again does nothing; a newer load supersedes one in flight. Resolves to the image's statistics.
    async load(source) {
        if (!source || !source.url) { this.clear(); return { key: '', ok: true, empty: true }; }
        if (this.image && this.image.key === source.key) return this._result(true);
        if (this.loading && this.loading.key === source.key) return this.loading.promise;
        if (this.loading) this.loading.abort.abort();
        const abort = new AbortController();
        const run = async () => {
            this.host.dataset.state = 'loading';
            try {
                const width = source.width | 0, height = source.height | 0, n = width * height;
                if (!(width > 0 && height > 0)) throw new Error('the image has no pixels');
                const [bytes, maskBytes, referenceBytes] = await Promise.all([
                    fetchBytes(source.url, abort.signal),
                    source.maskUrl ? fetchBytes(source.maskUrl, abort.signal) : Promise.resolve(null),
                    source.referenceUrl ? fetchBytes(source.referenceUrl, abort.signal) : Promise.resolve(null),
                ]);
                const data = toFloats(bytes, source.elementType || 'float32');
                if (!data) throw new Error(`the element type ${source.elementType} cannot be displayed`);
                if (data.length !== n) throw new Error(`the image has ${data.length} samples, not ${width} × ${height}`);
                const mask = maskBytes && maskBytes.byteLength === n ? new Uint8Array(maskBytes) : null;
                let reference = null;
                if (referenceBytes) {
                    reference = toFloats(referenceBytes, source.referenceElementType || source.elementType || 'float32');
                    if (!reference || reference.length !== n) reference = null;
                }
                if (abort.signal.aborted || this.disposed) return { key: source.key, ok: false, superseded: true };
                this.image = { key: source.key, width, height, data, mask, reference, label: source.label || '' };
                this.view = { cx: width / 2, cy: height / 2, zoom: 1 };
                this.diag.loads++;
                this._upload();
                this.draw();
                this._mark();
                this._emitValues('image');
                return this._result(true);
            } catch (e) {
                if (abort.signal.aborted) return { key: source.key, ok: false, superseded: true };
                this.host.dataset.state = 'failed';
                const message = 'The image could not be loaded: ' + (e && e.message ? e.message : String(e));
                if (this.dotNet) this.dotNet.invokeMethodAsync('NotifyStatus', 'load-failed', message).catch(() => { });
                return { key: source.key, ok: false, message };
            } finally {
                if (this.loading && this.loading.abort === abort) this.loading = null;
            }
        };
        const promise = run();
        this.loading = { key: source.key, abort, promise };
        return promise;
    }

    clear() {
        if (this.loading) { this.loading.abort.abort(); this.loading = null; }
        this.image = null;
        this.draw();
        this._mark();
        this._emitValues('clear');
    }

    // Display state: { low, high, gamma, invert, mode ('linear' | 'negative-log-ratio'), region [x, y, w, h], profileRow, smooth }.
    setDisplay(display) {
        const d = display || {};
        const before = this.state.mode;
        const next = Object.assign({}, this.state);
        if ('low' in d) next.low = d.low === null || d.low === undefined ? NaN : Number(d.low);
        if ('high' in d) next.high = d.high === null || d.high === undefined ? NaN : Number(d.high);
        if ('gamma' in d) next.gamma = finite(d.gamma) && d.gamma > 0 ? d.gamma : 1;
        if ('invert' in d) next.invert = !!d.invert;
        if ('mode' in d) next.mode = d.mode === 'negative-log-ratio' || d.mode === 1 ? 1 : 0;
        if ('region' in d) next.region = Array.isArray(d.region) && d.region.length === 4 ? d.region.map(Number) : [0, 0, 0, 0];
        if ('profileRow' in d) next.profileRow = finite(d.profileRow) ? d.profileRow : -1;
        if ('smooth' in d) next.smooth = !!d.smooth;
        this.state = next;
        this.draw();
        this._mark();
        if (next.mode !== before) this._emitValues('mode');
    }

    // Displayed quantity of valid pixels (the sample, or -ln(I / Iref)), sampled to at most about 200000 values.
    displayedValues() {
        const im = this.image;
        if (!im) return new Float64Array(0);
        const step = Math.max(1, Math.floor(im.data.length / SAMPLE_LIMIT));
        const out = new Float64Array(Math.ceil(im.data.length / step));
        let k = 0;
        for (let i = 0; i < im.data.length; i += step) {
            if (im.mask && im.mask[i]) continue;
            let v = im.data[i];
            if (this.state.mode === 1) { const r = im.reference ? im.reference[i] : 0; if (!(v > 0) || !(r > 0)) continue; v = -Math.log(v / r); }
            if (Number.isFinite(v)) out[k++] = v;
        }
        return out.subarray(0, k);
    }

    // A robust window over the displayed values: the given quantiles of the valid pixels (0.5 % and 99.5 % by default).
    suggestWindow(lowQuantile, highQuantile) {
        const vals = Float64Array.from(this.displayedValues());
        if (!vals.length) return null;
        vals.sort();
        const ql = finite(lowQuantile) ? Math.min(Math.max(lowQuantile, 0), 1) : 0.005;
        const qh = finite(highQuantile) ? Math.min(Math.max(highQuantile, 0), 1) : 0.995;
        let low = vals[Math.floor(ql * (vals.length - 1))], high = vals[Math.ceil(qh * (vals.length - 1))];
        if (!(high > low)) { const d = Math.abs(low) * 1e-6 || 1e-12; low -= d; high += d; }
        return { low, high };
    }

    // Statistics of the stored samples (valid pixels only: not masked, finite).
    statistics() {
        const im = this.image;
        if (!im) return null;
        let n = 0, sum = 0, min = Infinity, max = -Infinity;
        for (let i = 0; i < im.data.length; i++) {
            if (im.mask && im.mask[i]) continue;
            const v = im.data[i];
            if (!Number.isFinite(v)) continue;
            n++; sum += v;
            if (v < min) min = v;
            if (v > max) max = v;
        }
        return { validCount: n, mean: n ? sum / n : null, minimum: n ? min : null, maximum: n ? max : null };
    }

    _result(ok) {
        const im = this.image;
        if (!im) return { key: '', ok, empty: true };
        const s = this.statistics();
        const w = this.suggestWindow();
        return {
            key: im.key, ok, width: im.width, height: im.height, validCount: s.validCount, mean: s.mean, minimum: s.minimum, maximum: s.maximum,
            suggestedLow: w ? w.low : null, suggestedHigh: w ? w.high : null, drawable: !!this.gl && this.drawable !== false,
        };
    }

    resetView() {
        if (!this.image) return;
        this.view = { cx: this.image.width / 2, cy: this.image.height / 2, zoom: 1 };
        this.draw();
    }

    _scale() {
        // Image pixels per device pixel, preserving aspect ratio (fit) and zoom.
        const im = this.image;
        const w = this.canvas.width || 1, h = this.canvas.height || 1;
        const fit = Math.max(im.width / w, im.height / h);
        return fit / this.view.zoom;
    }

    devicePixelToImage(dx, dy) {
        const s = this._scale();
        const ox = this.view.cx - (this.canvas.width / 2) * s, oy = this.view.cy - (this.canvas.height / 2) * s;
        return [ox + dx * s, oy + dy * s];
    }

    imageToDevicePixel(x, y) {
        const s = this._scale();
        const ox = this.view.cx - (this.canvas.width / 2) * s, oy = this.view.cy - (this.canvas.height / 2) * s;
        return [(x - ox) / s, (y - oy) / s];
    }

    _probe(e) {
        if (!this.image || !this.emitProbe) return;
        const r = this.canvas.getBoundingClientRect();
        const dpr = globalThis.devicePixelRatio || 1;
        const [x, y] = this.devicePixelToImage((e.clientX - r.left) * dpr, (e.clientY - r.top) * dpr);
        const column = Math.floor(x), row = Math.floor(y);
        if (column < 0 || row < 0 || column >= this.image.width || row >= this.image.height) { this._sendProbe(null); return; }
        const i = row * this.image.width + column;
        const value = this.image.data[i];
        const reference = this.image.reference ? this.image.reference[i] : null;
        const masked = !!(this.image.mask && this.image.mask[i]);
        this._sendProbe({
            column, row, value: Number.isFinite(value) ? value : null, reference: reference !== null && Number.isFinite(reference) ? reference : null,
            invalid: masked || !Number.isFinite(value),
        });
    }

    // At most one probe per animation frame, and only when the pixel changed.
    _sendProbe(probe) {
        if (!this.emitProbe || !this.dotNet) return;
        const key = probe ? `${probe.column},${probe.row}` : '';
        if (key === this.probeLast && this.pendingProbe === undefined) return;
        this.probeLast = key;
        this.pendingProbe = probe;
        if (this.probeFrame) return;
        this.probeFrame = requestAnimationFrame(() => {
            this.probeFrame = 0;
            const p = this.pendingProbe;
            this.pendingProbe = undefined;
            if (this.dotNet) this.dotNet.invokeMethodAsync('NotifyProbe', p).catch(() => { });
        });
    }

    draw() {
        const gl = this.gl;
        this.diag.draws++;
        if (!gl) return;
        const dpr = globalThis.devicePixelRatio || 1;
        const cw = Math.max(1, Math.round(this.canvas.clientWidth * dpr)), ch = Math.max(1, Math.round(this.canvas.clientHeight * dpr));
        if (this.canvas.width !== cw || this.canvas.height !== ch) { this.canvas.width = cw; this.canvas.height = ch; }
        gl.viewport(0, 0, cw, ch);
        gl.clearColor(0, 0, 0, 0);
        gl.clear(gl.COLOR_BUFFER_BIT);
        if (!this.image || this.drawable === false) return;
        const st = this.state;
        if (!(Number.isFinite(st.low) && Number.isFinite(st.high) && st.high > st.low)) return;
        gl.useProgram(this.program);
        gl.bindVertexArray(this.vao);
        gl.activeTexture(gl.TEXTURE0); gl.bindTexture(gl.TEXTURE_2D, this.textures.image); gl.uniform1i(this.u.uImage, 0);
        gl.activeTexture(gl.TEXTURE1); gl.bindTexture(gl.TEXTURE_2D, this.textures.reference); gl.uniform1i(this.u.uReference, 1);
        gl.activeTexture(gl.TEXTURE2); gl.bindTexture(gl.TEXTURE_2D, this.textures.mask); gl.uniform1i(this.u.uMask, 2);
        const s = this._scale();
        gl.uniform2f(this.u.uViewport, cw, ch);
        gl.uniform3f(this.u.uView, this.view.cx - (cw / 2) * s, this.view.cy - (ch / 2) * s, s);
        gl.uniform1f(this.u.uLow, st.low);
        gl.uniform1f(this.u.uHigh, st.high);
        gl.uniform1f(this.u.uGamma, st.gamma > 0 ? st.gamma : 1);
        gl.uniform1i(this.u.uInvert, st.invert ? 1 : 0);
        gl.uniform1i(this.u.uMode, st.mode);
        gl.uniform4f(this.u.uRegion, st.region[0], st.region[1], st.region[2], st.region[3]);
        gl.uniform1f(this.u.uProfileRow, st.profileRow);
        gl.uniform1i(this.u.uSmooth, st.smooth ? 1 : 0);
        gl.uniform3fv(this.u.uInvalidColor, this.colors.invalid);
        gl.uniform3fv(this.u.uRegionColor, this.colors.region);
        gl.uniform3fv(this.u.uProfileColor, this.colors.profile);
        gl.drawArrays(gl.TRIANGLES, 0, 3);
    }

    // One RGBA pixel of the drawing buffer (device pixels from the top-left corner), for browser proof.
    readPixel(dx, dy) {
        const gl = this.gl;
        if (!gl) return null;
        const out = new Uint8Array(4);
        gl.readPixels(dx | 0, this.canvas.height - 1 - (dy | 0), 1, 1, gl.RGBA, gl.UNSIGNED_BYTE, out);
        return Array.from(out);
    }

    proof() {
        const im = this.image;
        return {
            id: this.id, status: this.status, key: im ? im.key : '', width: im ? im.width : 0, height: im ? im.height : 0, label: im ? im.label : '',
            state: Object.assign({}, this.state, { mode: this.state.mode === 1 ? 'negative-log-ratio' : 'linear' }),
            view: Object.assign({}, this.view), canvas: { width: this.canvas.width, height: this.canvas.height }, diag: Object.assign({}, this.diag),
        };
    }

    _mark() {
        const ds = this.host.dataset, im = this.image, st = this.state;
        ds.imageKey = im ? im.key : '';
        ds.uploads = String(this.diag.uploads);
        ds.window = Number.isFinite(st.low) && Number.isFinite(st.high) ? `${st.low}|${st.high}` : '';
        ds.mode = st.mode === 1 ? 'negative-log-ratio' : 'linear';
        if (ds.state !== 'loading' || im) ds.state = im ? 'ready' : 'empty';
    }

    // Only the view registered under its id speaks for it (a disposed view with the same id never does).
    _emitValues(reason) {
        if (this.id && !this.disposed && views.get(this.id) === this) hub.dispatchEvent(new CustomEvent('values', { detail: { id: this.id, view: this, reason } }));
    }

    dispose() {
        this.disposed = true;
        if (this.loading) { this.loading.abort.abort(); this.loading = null; }
        if (this.probeFrame) cancelAnimationFrame(this.probeFrame);
        this.abort.abort();
        this.resizeObserver.disconnect();
        const gl = this.gl;
        if (gl) {
            for (const t of Object.values(this.textures)) gl.deleteTexture(t);
            gl.deleteProgram(this.program);
            gl.deleteVertexArray(this.vao);
        }
        this.gl = null;
        this.image = null;
        this.dotNet = null;
        if (this.host.__cdaImageView === this) delete this.host.__cdaImageView;
        if (this.id && views.get(this.id) === this) {
            views.delete(this.id);
            hub.dispatchEvent(new CustomEvent('unregister', { detail: { id: this.id, view: this } }));
        }
    }
}

export function createImageView(host, canvas, dotNet, options) {
    return new FloatImageView(host, canvas, dotNet, options);
}

// ---- display-window histogram ---------------------------------------------------------------------------------------
const MARGIN = { left: 46, right: 8, top: 8, bottom: 22 };   // plot margins in viewBox units
const HINT = 'Drag an edge of the window to change it, or drag inside the window to move it; the arrow keys step a focused edge or the window (Shift: ten steps). Double-click fits the scale to the values and the window.';

function svgElement(name, attrs, text) {
    const e = document.createElementNS(SVG_NS, name);
    setAttributes(e, attrs || {});
    if (text !== undefined) e.textContent = text;
    return e;
}

function setAttributes(e, attrs) {
    for (const [k, v] of Object.entries(attrs)) e.setAttribute(k, String(v));
}

const clamp = (v, a, b) => Math.min(b, Math.max(a, v));

export function formatValue(v) {
    if (!Number.isFinite(v)) return '–';
    const a = Math.abs(v);
    return a !== 0 && (a < 1e-3 || a >= 1e5) ? v.toExponential(3) : v.toPrecision(4);
}

// Index of the first sorted value not below x (strict: the first above x).
function bound(sorted, x, strict) {
    let lo = 0, hi = sorted.length;
    while (lo < hi) {
        const m = (lo + hi) >>> 1;
        if (strict ? sorted[m] <= x : sorted[m] < x) lo = m + 1; else hi = m;
    }
    return lo;
}

// Round steps on a scale of this span: a drag moves in steps finer than a pixel, a key in steps of about a hundredth
// of the scale.
const dragStep = (span) => Math.pow(10, Math.floor(Math.log10(span / 2000)));
const keyStep = (span) => Math.pow(10, Math.floor(Math.log10(span / 100)));
const snap = (v, step) => Number((Math.round(v / step) * step).toPrecision(12));

class WindowHistogram {
    // options: bins (64 over the values' own range, up to four times more when the scale is wider), label (axis label
    // when the view gives none), emitInput (report the window while it moves), emitStats (report the values against the
    // window), valueLabel (the y-axis unit, "pixels" by default).
    constructor(svg, dotNet, options) {
        const o = options || {};
        this.svg = svg;
        this.dotNet = dotNet || null;
        this.bins = o.bins > 0 ? o.bins : 64;
        this.fallbackLabel = o.label || '';
        this.countLabel = o.countLabel || 'pixels';
        this.emitInput = !!o.emitInput;
        this.emitStats = !!o.emitStats;
        this.sorted = new Float64Array(0);
        this.label = '';
        this.low = NaN;
        this.high = NaN;
        this.lo = 0;   // the scale
        this.hi = 1;
        this.drag = null;
        this.focusing = false;   // a drag is focusing its handle
        this.refitOnNextWindow = false;
        this.diag = { binnings: 0, values: 0 };
        this.viewIds = [];
        this.abort = new AbortController();
        this.inputFrame = 0;
        this.statsFrame = 0;
        if (!svg.getAttribute('viewBox')) svg.setAttribute('viewBox', '0 0 400 80');
        svg.setAttribute('role', 'group');   // its edges and the window are controls, not a picture
        this._build();
        const sig = { signal: this.abort.signal };
        svg.addEventListener('pointerdown', (e) => this._down(e), sig);
        svg.addEventListener('pointermove', (e) => this._move(e), sig);
        svg.addEventListener('pointerup', (e) => this._up(e, false), sig);
        svg.addEventListener('pointercancel', (e) => this._up(e, true), sig);
        svg.addEventListener('lostpointercapture', (e) => this._up(e, false), sig);
        svg.addEventListener('pointerleave', () => { if (!this.drag) this._hover(null); }, sig);
        svg.addEventListener('dblclick', (e) => { e.preventDefault(); this.fit(); }, sig);
        svg.addEventListener('keydown', (e) => this._key(e), sig);
        // A handle focused by a drag shows no focus ring until a key is used on it; one reached with Tab shows it.
        for (const k of ['low', 'key', 'high'])
            this.e[k].addEventListener('focus', () => { if (!this.focusing) svg.classList.remove('is-pointer-focus'); }, sig);
        // Esc puts the window back where the drag started, wherever the focus is (and nothing else on the page sees it).
        document.addEventListener('keydown', (e) => {
            if (!this.drag || e.key !== 'Escape') return;
            e.preventDefault();
            e.stopPropagation();
            this._end(true);
        }, Object.assign({ capture: true }, sig));
        hub.addEventListener('values', (e) => { if (e.detail.id === this.viewIds[0]) this._fromView(e.detail.view); }, sig);
        hub.addEventListener('register', (e) => { if (e.detail.id === this.viewIds[0]) this._fromView(e.detail.view); }, sig);
        svg.__cdaWindowHistogram = this;
        this._fit();
        proofRoot();
    }

    // Links the histogram to views by id: the first gives the values, every one follows the window while it moves.
    link(ids) {
        this.viewIds = (ids || []).filter((x) => typeof x === 'string' && x);
        const primary = views.get(this.viewIds[0]);
        if (primary) this._fromView(primary);
        else this.setValues([], this.low, this.high, this.fallbackLabel);
    }

    _fromView(view) {
        const label = (view.image && view.image.label) || this.fallbackLabel;
        this.diag.values++;
        this.setValues(view.image ? view.displayedValues() : [], this.low, this.high, label);
        this.refitOnNextWindow = true;
    }

    // New values with their window: the scale is fitted to both.
    setValues(values, low, high, label) {
        this._drop();
        const sorted = Float64Array.from(values || []).filter(Number.isFinite);
        sorted.sort();
        this.sorted = sorted;
        this.label = label || '';
        this.low = low;
        this.high = high;
        this._fit();
        this._queueStats();
    }

    // A window set by the page. The scale stays, so a narrowed window can be widened again: it grows only to show a
    // window beyond it. fit (or the first window after new values): the scale is fitted to the values and the window.
    setWindow(low, high, fit) {
        const refit = !!fit || this.refitOnNextWindow;
        this.refitOnNextWindow = false;
        const lowValue = low === null || low === undefined ? NaN : Number(low);
        const highValue = high === null || high === undefined ? NaN : Number(high);
        if (!refit && lowValue === this.low && highValue === this.high) return;
        this._drop();
        this.low = lowValue;
        this.high = highValue;
        if (refit) { this._fit(); this._queueStats(); return; }
        if (this._valid() && (this.low < this.lo || this.high > this.hi)) {
            this.lo = Math.min(this.lo, this.low);
            this.hi = Math.max(this.hi, this.high);
            this._bin();
        }
        this._place();
        this._queueStats();
    }

    // The scale fitted to the values and the window (the window itself is kept).
    fit() {
        this._drop();
        this._fit();
        this._queueStats();
    }

    // The values against the window: their range and the fractions drawn black (below low) and white (above high).
    stats() {
        const s = this.sorted, n = s.length;
        const on = this._valid();
        return {
            count: n, minimum: n ? s[0] : null, maximum: n ? s[n - 1] : null, low: on ? this.low : null, high: on ? this.high : null,
            scaleLow: this.lo, scaleHigh: this.hi,
            belowFraction: n && on ? bound(s, this.low, false) / n : 0, aboveFraction: n && on ? (n - bound(s, this.high, true)) / n : 0,
        };
    }

    dispose() {
        this._drop();
        if (this.inputFrame) cancelAnimationFrame(this.inputFrame);
        if (this.statsFrame) cancelAnimationFrame(this.statsFrame);
        this.abort.abort();
        this.dotNet = null;
        if (this.svg.__cdaWindowHistogram === this) delete this.svg.__cdaWindowHistogram;
    }

    // ---- drawing ----------------------------------------------------------------------------------------------------
    _build() {
        const svg = this.svg, vb = svg.viewBox.baseVal;
        while (svg.firstChild) svg.removeChild(svg.firstChild);
        const W = this.W = vb.width || 400, H = this.H = vb.height || 80, bottom = H - MARGIN.bottom, mid = (MARGIN.top + bottom) / 2;
        const add = (name, attrs, text) => svg.appendChild(svgElement(name, attrs, text));
        const edge = (name, label) => {
            const g = add('g', { class: 'ui-webgl-histogram__edge', 'data-edge': name, 'data-part': 'edge', tabindex: 0, role: 'slider', 'aria-label': label });
            g.appendChild(svgElement('line', { y1: MARGIN.top, y2: bottom, class: 'ui-webgl-histogram__edge-line' }));
            g.appendChild(svgElement('rect', { x: -3.5, y: mid - 10, width: 7, height: 20, rx: 2, class: 'ui-webgl-histogram__grip', 'data-part': 'grip' }));
            g.appendChild(svgElement('text', { y: MARGIN.top + 9, class: 'ui-webgl-histogram__edge-value' }));
            return g;
        };
        add('title', {}, HINT);
        add('rect', { x: MARGIN.left, y: MARGIN.top, width: W - MARGIN.left - MARGIN.right, height: bottom - MARGIN.top, class: 'ui-webgl-histogram__frame' });
        this.e = {
            clipLow: add('rect', { y: MARGIN.top, height: bottom - MARGIN.top, class: 'ui-webgl-histogram__clipped' }),
            clipHigh: add('rect', { y: MARGIN.top, height: bottom - MARGIN.top, class: 'ui-webgl-histogram__clipped' }),
            band: add('rect', { y: MARGIN.top, height: bottom - MARGIN.top, class: 'ui-webgl-histogram__band' }),
            bars: add('path', { class: 'ui-webgl-histogram__bars' }),
            xmin: add('text', { x: MARGIN.left, y: H - 6, class: 'ui-webgl-histogram__label' }),
            xmax: add('text', { x: W - MARGIN.right, y: H - 6, 'text-anchor': 'end', class: 'ui-webgl-histogram__label' }),
            xlabel: add('text', { x: (MARGIN.left + W - MARGIN.right) / 2, y: H - 6, 'text-anchor': 'middle', class: 'ui-webgl-histogram__label' }),
            ymax: add('text', { x: 2, y: MARGIN.top + 8, class: 'ui-webgl-histogram__label' }),
            ymin: add('text', { x: 2, y: bottom, class: 'ui-webgl-histogram__label' }, formatValue(0)),
            ylabel: add('text', { x: MARGIN.left + 4, y: MARGIN.top + 10, class: 'ui-webgl-histogram__label' }),
            // Tab order: low edge, the window, high edge.
            low: edge('low', 'Window low: values below it are drawn black'),
            key: add('rect', { y: MARGIN.top, height: bottom - MARGIN.top, class: 'ui-webgl-histogram__band-key', 'data-part': 'window', tabindex: 0, role: 'slider', 'aria-label': 'Display window: the arrow keys move both edges' }),
            high: edge('high', 'Window high: values above it are drawn white'),
            bandValue: add('text', { y: MARGIN.top + 9, class: 'ui-webgl-histogram__band-value', 'text-anchor': 'middle' }),
        };
    }

    _x(v) { return MARGIN.left + (v - this.lo) / (this.hi - this.lo) * (this.W - MARGIN.left - MARGIN.right); }
    _v(x) { return this.lo + (x - MARGIN.left) / (this.W - MARGIN.left - MARGIN.right) * (this.hi - this.lo); }
    _valid() { return Number.isFinite(this.low) && Number.isFinite(this.high) && this.high > this.low; }
    _active() { return this.sorted.length > 0 && this._valid(); }

    // The scale over the values and the window, and the values binned on it.
    _fit() {
        const s = this.sorted, n = s.length, on = this._valid();
        let lo = n ? s[0] : on ? this.low : 0, hi = n ? s[n - 1] : on ? this.high : 1;
        if (on) { lo = Math.min(lo, this.low); hi = Math.max(hi, this.high); }
        if (!(hi > lo)) { const d = Math.abs(lo) * 1e-6 || 1e-12; lo -= d; hi += d; }
        this.lo = lo;
        this.hi = hi;
        this._bin();
        this._place();
    }

    // Equal bins over the scale. Only a new scale bins again: moving the window never changes the bars.
    _bin() {
        const s = this.sorted, n = s.length, e = this.e, lo = this.lo, hi = this.hi, bottom = this.H - MARGIN.bottom;
        this.diag.binnings++;
        this.svg.dataset.scale = `${lo}|${hi}`;
        this.svg.dataset.binnings = String(this.diag.binnings);
        e.xlabel.textContent = this.label;
        if (!n) {
            e.bars.setAttribute('d', '');
            e.xmin.textContent = e.xmax.textContent = e.ymax.textContent = '';
            e.ylabel.textContent = 'no data';
            return;
        }
        const range = s[n - 1] - s[0];
        const bins = Math.round(clamp(range > 0 ? this.bins * (hi - lo) / range : this.bins, this.bins, 4 * this.bins));
        const w = (hi - lo) / bins, counts = new Array(bins);
        let prev = 0;   // the scale starts at or below the smallest value
        for (let i = 0; i < bins; i++) {
            const next = i === bins - 1 ? n : bound(s, lo + (i + 1) * w, false);
            counts[i] = next - prev;
            prev = next;
        }
        const top = Math.max(...counts) * 1.05 || 1, sy = (c) => bottom - c / top * (bottom - MARGIN.top);
        let d = '';
        for (let i = 0; i < bins; i++) {
            if (!counts[i]) continue;
            const x0 = this._x(lo + i * w), x1 = this._x(lo + (i + 1) * w);
            d += `M${x0.toFixed(2)} ${bottom}V${sy(counts[i]).toFixed(2)}H${(x0 + Math.max(0.5, x1 - x0 - 0.3)).toFixed(2)}V${bottom}Z`;
        }
        e.bars.setAttribute('d', d);
        e.xmin.textContent = formatValue(lo);
        e.xmax.textContent = formatValue(hi);
        e.ymax.textContent = formatValue(top);
        e.ylabel.textContent = this.countLabel;
    }

    // The window on the scale: shaded where values are drawn black or white, its edges, and the window itself.
    _place() {
        const e = this.e, on = this._active();
        for (const k of ['clipLow', 'clipHigh', 'band', 'low', 'key', 'high', 'bandValue']) e[k].style.display = on ? '' : 'none';
        this.svg.dataset.window = on ? `${this.low}|${this.high}` : '';
        if (!on) return;
        const x0 = MARGIN.left, x1 = this.W - MARGIN.right, xl = clamp(this._x(this.low), x0, x1), xh = clamp(this._x(this.high), x0, x1);
        const width = this.high - this.low;
        setAttributes(e.clipLow, { x: x0, width: xl - x0 });
        setAttributes(e.clipHigh, { x: xh, width: x1 - xh });
        setAttributes(e.band, { x: xl, width: xh - xl });
        setAttributes(e.key, {
            x: xl, width: xh - xl, 'aria-valuemin': this.lo + width / 2, 'aria-valuemax': this.hi - width / 2,
            'aria-valuenow': this.low + width / 2, 'aria-valuetext': `${formatValue(this.low)} to ${formatValue(this.high)}`,
        });
        this._edge(e.low, xl, this.low, xl - x0 > 60 ? 'end' : 'start', this.lo, this.high);
        this._edge(e.high, xh, this.high, x1 - xh > 60 ? 'start' : 'end', this.low, this.hi);
        e.bandValue.setAttribute('x', ((xl + xh) / 2).toFixed(2));
        e.bandValue.textContent = `${formatValue(this.low)} … ${formatValue(this.high)}`;
    }

    // An edge at x with its value written beside it (on the side with room).
    _edge(g, x, v, anchor, min, max) {
        g.setAttribute('transform', `translate(${x.toFixed(2)} 0)`);
        setAttributes(g, { 'aria-valuemin': min, 'aria-valuemax': max, 'aria-valuenow': v, 'aria-valuetext': formatValue(v) });
        const text = g.lastChild;
        setAttributes(text, { x: anchor === 'end' ? -5 : 5, 'text-anchor': anchor });
        text.textContent = formatValue(v);
    }

    // ---- pointer and keys -------------------------------------------------------------------------------------------
    _point(e) {
        const m = this.svg.getScreenCTM();
        if (!m || !m.a) return null;
        const p = new DOMPoint(e.clientX, e.clientY).matrixTransform(m.inverse());
        return { x: p.x, reach: Math.max(6, 10 / m.a) };   // an edge is taken within 10 px (or beside its grip)
    }

    // What a pointer takes: an edge within reach, else the window when inside it. A window too narrow for all three
    // leaves the edges the outside and the window the inside.
    _zone(p) {
        if (!this._active()) return null;
        const xl = this._x(this.low), xh = this._x(this.high), inside = p.x > xl && p.x < xh;
        if (xh - xl < 3 * p.reach) {
            if (inside) return 'band';
            if (p.x <= xl && xl - p.x <= p.reach) return 'low';
            if (p.x >= xh && p.x - xh <= p.reach) return 'high';
            return null;
        }
        if (Math.abs(p.x - xl) <= p.reach) return 'low';
        if (Math.abs(p.x - xh) <= p.reach) return 'high';
        return inside ? 'band' : null;
    }

    _hover(zone) {
        this.svg.style.cursor = zone === 'band' ? 'grab' : zone ? 'ew-resize' : '';
        this.e.low.classList.toggle('is-hot', zone === 'low');
        this.e.high.classList.toggle('is-hot', zone === 'high');
        this.e.band.classList.toggle('is-hot', zone === 'band');
    }

    _down(e) {
        if (e.button !== 0 || this.drag) return;
        const p = this._point(e), zone = p && this._zone(p);
        if (!zone) return;
        e.preventDefault();
        this.drag = { id: e.pointerId, zone, low: this.low, high: this.high, at: this._v(p.x), moved: false };
        this.svg.setPointerCapture(e.pointerId);
        // Focused, so the keys can fine-tune what was dragged.
        this.focusing = true;
        (zone === 'band' ? this.e.key : this.e[zone]).focus({ preventScroll: true });
        this.focusing = false;
        this.svg.classList.add('is-pointer-focus', 'is-dragging', 'is-drag-' + zone);
        this.svg.style.cursor = zone === 'band' ? 'grabbing' : 'ew-resize';
    }

    _move(e) {
        const p = this._point(e);
        if (!p) return;
        const d = this.drag;
        if (!d) { this._hover(this._zone(p)); return; }
        if (e.pointerId !== d.id) return;
        const span = this.hi - this.lo, step = dragStep(span), gap = span / 1000, dv = this._v(p.x) - d.at;
        let low = this.low, high = this.high;
        if (d.zone === 'band') {
            // The window keeps its width and stays on the scale.
            const shift = clamp(snap(dv, step), this.lo - d.low, this.hi - d.high);
            low = d.low + shift;
            high = d.high + shift;
        } else if (d.zone === 'low') low = clamp(snap(d.low + dv, step), this.lo, this.high - gap);
        else high = clamp(snap(d.high + dv, step), this.low + gap, this.hi);
        if (low === this.low && high === this.high) return;
        d.moved = true;
        this._set(low, high, false);
    }

    _up(e, cancel) {
        if (this.drag && e.pointerId === this.drag.id) this._end(cancel);
    }

    // Ends a drag: released, it is committed; cancelled, the window goes back where it was.
    _end(cancel) {
        const d = this.drag;
        if (!d) return;
        this.drag = null;
        if (this.svg.hasPointerCapture(d.id)) this.svg.releasePointerCapture(d.id);
        this.svg.classList.remove('is-dragging', 'is-drag-' + d.zone);
        this.svg.style.cursor = '';
        if (!d.moved) return;
        if (cancel) this._set(d.low, d.high, false);
        else this._commit();
    }

    // New values or a window from the page during a drag: the drag ends where it is, without a commit.
    _drop() {
        if (!this.drag) return;
        this.drag.moved = false;
        this._end(false);
    }

    _key(e) {
        if (this.drag || !this._active()) return;
        const zone = e.target === this.e.key ? 'band' : e.target === this.e.low ? 'low' : e.target === this.e.high ? 'high' : null;
        if (!zone) return;
        const span = this.hi - this.lo, gap = span / 1000, width = this.high - this.low;
        const dir = { ArrowLeft: -1, ArrowDown: -1, PageDown: -1, ArrowRight: 1, ArrowUp: 1, PageUp: 1 }[e.key];
        let low = this.low, high = this.high;
        if (dir) {
            const step = keyStep(span) * (e.shiftKey || e.key.startsWith('Page') ? 10 : 1);
            if (zone === 'low') low = clamp(snap(low + dir * step, step), this.lo, high - gap);
            else if (zone === 'high') high = clamp(snap(high + dir * step, step), low + gap, this.hi);
            else { const shift = clamp(dir * step, this.lo - low, this.hi - high); low += shift; high += shift; }
        } else if (e.key === 'Home') {
            if (zone === 'high') high = low + gap;
            else { low = this.lo; if (zone === 'band') high = this.lo + width; }
        } else if (e.key === 'End') {
            if (zone === 'low') low = high - gap;
            else { high = this.hi; if (zone === 'band') low = this.hi - width; }
        } else return;
        e.preventDefault();
        this.svg.classList.remove('is-pointer-focus');
        if (low !== this.low || high !== this.high) { this._set(low, high, false); this._commit(); }
    }

    // The window while it moves: the linked views follow at once; the page hears of it at most once per frame.
    _set(low, high) {
        this.low = low;
        this.high = high;
        this._place();
        for (const id of this.viewIds) { const v = views.get(id); if (v) v.setDisplay({ low, high }); }
        this._queueStats();
        if (!this.emitInput || !this.dotNet || this.inputFrame) return;
        this.inputFrame = requestAnimationFrame(() => {
            this.inputFrame = 0;
            if (this.dotNet && this._valid()) this.dotNet.invokeMethodAsync('NotifyWindowInput', this.low, this.high).catch(() => { });
        });
    }

    _commit() {
        if (this.dotNet && this._valid()) this.dotNet.invokeMethodAsync('NotifyWindowChanged', this.low, this.high).catch(() => { });
    }

    _queueStats() {
        if (!this.emitStats || !this.dotNet || this.statsFrame) return;
        this.statsFrame = requestAnimationFrame(() => {
            this.statsFrame = 0;
            if (this.dotNet) this.dotNet.invokeMethodAsync('NotifyStats', this.stats()).catch(() => { });
        });
    }
}

export function createWindowHistogram(svg, dotNet, options) {
    return new WindowHistogram(svg, dotNet, options);
}
