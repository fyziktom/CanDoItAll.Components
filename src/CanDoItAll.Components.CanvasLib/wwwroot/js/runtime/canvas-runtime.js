(function () {
    "use strict";

    const root = window.CanDoItAll = window.CanDoItAll || {};
    if (root.canvasRuntime !== undefined) {
        throw new Error("CanDoItAll.canvasRuntime is already initialized. Include CanvasLibBodyAssets exactly once.");
    }

    function requireObject(value, name) {
        if (value === null || typeof value !== "object" || Array.isArray(value)) {
            throw new TypeError(`${name} must be an object.`);
        }

        return value;
    }

    function requireFunction(value, name) {
        if (typeof value !== "function") {
            throw new TypeError(`${name} must be a function.`);
        }

        return value;
    }

    function readOptionalFunction(value, name) {
        if (value === undefined || value === null) {
            return null;
        }

        return requireFunction(value, name);
    }

    function requireMetadata(value, name) {
        if (value === undefined || value === null) {
            throw new TypeError(`${name} cannot be null or undefined.`);
        }

        return value;
    }

    function requirePngDataUrl(value, name) {
        if (typeof value !== "string" || !value.startsWith("data:image/png;base64,")) {
            throw new TypeError(`${name} must be a base64-encoded image/png data URL.`);
        }

        return value;
    }

    function invokeSynchronous(callback, payload, name) {
        const result = callback(payload);
        if (result && typeof result.then === "function") {
            throw new TypeError(`${name} must be synchronous.`);
        }

        return result;
    }

    function requireFiniteNumber(value, name) {
        if (!Number.isFinite(value)) {
            throw new TypeError(`${name} must be a finite number.`);
        }

        return value;
    }

    function requirePositiveNumber(value, name) {
        const result = requireFiniteNumber(value, name);
        if (result <= 0) {
            throw new RangeError(`${name} must be greater than zero.`);
        }

        return result;
    }

    function requireNonNegativeNumber(value, name) {
        const result = requireFiniteNumber(value, name);
        if (result < 0) {
            throw new RangeError(`${name} cannot be negative.`);
        }

        return result;
    }

    function requireElement(value, name) {
        if (typeof window.Element !== "function") {
            throw new Error("CanDoItAll.canvasRuntime requires the browser Element API.");
        }

        if (!(value instanceof window.Element)) {
            throw new TypeError(`${name} must be a DOM Element.`);
        }

        return value;
    }

    function requireCanvas(value, name) {
        if (typeof window.HTMLCanvasElement !== "function") {
            throw new Error("CanDoItAll.canvasRuntime requires the browser HTMLCanvasElement API.");
        }

        if (!(value instanceof window.HTMLCanvasElement)) {
            throw new TypeError(`${name} must be an HTMLCanvasElement.`);
        }

        return value;
    }

    function requireCanvasContext(canvas, name) {
        const context = canvas.getContext("2d");
        if (!context) {
            throw new Error(`${name} could not create a 2D rendering context.`);
        }

        return context;
    }

    function requireAnimationFrameApi() {
        requireFunction(window.requestAnimationFrame, "window.requestAnimationFrame");
        requireFunction(window.cancelAnimationFrame, "window.cancelAnimationFrame");
    }

    function requireResizeObserverApi() {
        if (typeof window.ResizeObserver !== "function") {
            throw new Error("CanDoItAll.canvasRuntime requires the browser ResizeObserver API.");
        }
    }

    function resolvePixelRatio(maxPixelRatio) {
        const devicePixelRatio = requirePositiveNumber(window.devicePixelRatio, "window.devicePixelRatio");
        return Math.min(devicePixelRatio, maxPixelRatio);
    }

    function readElementSize(element) {
        const rect = element.getBoundingClientRect();
        const rectWidth = requireNonNegativeNumber(rect.width, "resizeTarget bounding width");
        const rectHeight = requireNonNegativeNumber(rect.height, "resizeTarget bounding height");
        const clientWidth = requireNonNegativeNumber(element.clientWidth, "resizeTarget clientWidth");
        const clientHeight = requireNonNegativeNumber(element.clientHeight, "resizeTarget clientHeight");
        const inlineWidth = Number.parseFloat(element.style.width);
        const inlineHeight = Number.parseFloat(element.style.height);

        return Object.freeze({
            width: clientWidth > 0
                ? clientWidth
                : rectWidth > 0
                    ? rectWidth
                    : Number.isFinite(inlineWidth) && inlineWidth > 0
                        ? inlineWidth
                        : 0,
            height: clientHeight > 0
                ? clientHeight
                : rectHeight > 0
                    ? rectHeight
                    : Number.isFinite(inlineHeight) && inlineHeight > 0
                        ? inlineHeight
                        : 0
        });
    }

    function normalizePointerPoint(element, event) {
        requireElement(element, "coordinate element");
        requireObject(event, "pointer event");
        const clientX = requireFiniteNumber(event.clientX, "pointer event clientX");
        const clientY = requireFiniteNumber(event.clientY, "pointer event clientY");
        const rect = element.getBoundingClientRect();
        const width = requirePositiveNumber(rect.width, "coordinate element width");
        const height = requirePositiveNumber(rect.height, "coordinate element height");
        const x = clientX - requireFiniteNumber(rect.left, "coordinate element left");
        const y = clientY - requireFiniteNumber(rect.top, "coordinate element top");
        const normalizedX = x / width;
        const normalizedY = y / height;

        return Object.freeze({
            x,
            y,
            normalizedX,
            normalizedY,
            inside: normalizedX >= 0 && normalizedX <= 1 && normalizedY >= 0 && normalizedY <= 1
        });
    }

    function readRenderSize(settings) {
        const width = requirePositiveNumber(settings.width, "render width");
        const height = requirePositiveNumber(settings.height, "render height");
        const pixelRatio = settings.pixelRatio === undefined
            ? 1
            : requirePositiveNumber(settings.pixelRatio, "render pixelRatio");
        const backingWidth = Math.ceil(width * pixelRatio);
        const backingHeight = Math.ceil(height * pixelRatio);
        if (!Number.isSafeInteger(backingWidth) || !Number.isSafeInteger(backingHeight)) {
            throw new RangeError("The requested PNG dimensions exceed the supported integer canvas size.");
        }

        return Object.freeze({ width, height, pixelRatio, backingWidth, backingHeight });
    }

    function renderToPngDataUrl(options) {
        const settings = requireObject(options, "renderToPngDataUrl options");
        const draw = requireFunction(settings.draw, "renderToPngDataUrl options.draw");
        const size = readRenderSize(settings);
        if (settings.background !== undefined && (typeof settings.background !== "string" || settings.background.trim() === "")) {
            throw new TypeError("renderToPngDataUrl options.background must be a non-empty CSS color string when provided.");
        }

        if (!window.document || typeof window.document.createElement !== "function") {
            throw new Error("CanDoItAll.canvasRuntime requires document.createElement for PNG export.");
        }

        const canvas = requireCanvas(window.document.createElement("canvas"), "PNG export canvas");
        canvas.width = size.backingWidth;
        canvas.height = size.backingHeight;
        const context = requireCanvasContext(canvas, "PNG export canvas");
        context.setTransform(size.pixelRatio, 0, 0, size.pixelRatio, 0, 0);

        if (settings.background !== undefined) {
            context.fillStyle = settings.background;
            context.fillRect(0, 0, size.width, size.height);
        }

        const drawResult = draw(context, Object.freeze({
            canvas,
            width: size.width,
            height: size.height,
            pixelRatio: size.pixelRatio
        }));
        if (drawResult && typeof drawResult.then === "function") {
            throw new TypeError("renderToPngDataUrl options.draw must be synchronous.");
        }

        requireFunction(canvas.toDataURL, "PNG export canvas.toDataURL");
        const dataUrl = canvas.toDataURL("image/png");
        if (typeof dataUrl !== "string" || !dataUrl.startsWith("data:image/png;base64,")) {
            throw new Error("PNG export did not produce an image/png data URL.");
        }

        return dataUrl;
    }

    function downloadDataUrl(dataUrl, fileName) {
        requirePngDataUrl(dataUrl, "downloadDataUrl dataUrl");
        if (typeof fileName !== "string" || fileName.trim() === "") {
            throw new TypeError("downloadDataUrl fileName must be a non-empty string.");
        }

        if (fileName !== fileName.trim() || /[\\/\u0000-\u001f\u007f]/.test(fileName)) {
            throw new RangeError("downloadDataUrl fileName cannot contain surrounding whitespace, path separators, or control characters.");
        }

        if (!fileName.toLowerCase().endsWith(".png")) {
            throw new RangeError("downloadDataUrl fileName must use the .png extension.");
        }

        if (!window.document || !window.document.body || typeof window.document.createElement !== "function") {
            throw new Error("CanDoItAll.canvasRuntime requires document.body for image download.");
        }

        if (typeof window.HTMLAnchorElement !== "function") {
            throw new Error("CanDoItAll.canvasRuntime requires the browser HTMLAnchorElement API.");
        }

        const anchor = window.document.createElement("a");
        if (!(anchor instanceof window.HTMLAnchorElement)) {
            throw new Error("CanDoItAll.canvasRuntime could not create a download anchor.");
        }

        requireFunction(anchor.click, "download anchor.click");
        requireFunction(anchor.remove, "download anchor.remove");
        anchor.href = dataUrl;
        anchor.download = fileName;
        anchor.style.display = "none";
        window.document.body.appendChild(anchor);
        try {
            anchor.click();
        }
        finally {
            anchor.remove();
        }
    }

    class CanvasSurface {
        constructor(options) {
            const settings = requireObject(options, "CanvasSurface options");
            requireAnimationFrameApi();
            requireResizeObserverApi();

            this._canvas = requireCanvas(settings.canvas, "CanvasSurface options.canvas");
            this._context = requireCanvasContext(this._canvas, "CanvasSurface options.canvas");
            this._resizeTarget = settings.resizeTarget === undefined || settings.resizeTarget === null
                ? requireElement(this._canvas.parentElement || this._canvas, "CanvasSurface resize target")
                : requireElement(settings.resizeTarget, "CanvasSurface options.resizeTarget");
            this._onRender = requireFunction(settings.onRender, "CanvasSurface options.onRender");
            this._onResize = readOptionalFunction(settings.onResize, "CanvasSurface options.onResize");
            this._maxPixelRatio = settings.maxPixelRatio === undefined
                ? 3
                : requirePositiveNumber(settings.maxPixelRatio, "CanvasSurface options.maxPixelRatio");
            this._size = Object.freeze({
                width: 0,
                height: 0,
                pixelRatio: 1,
                backingWidth: 1,
                backingHeight: 1
            });
            this._frameRequestId = null;
            this._disposed = false;
            this._resizeObserver = new window.ResizeObserver(() => this.measure());
            this._resizeObserver.observe(this._resizeTarget);
            try {
                this.measure();
            }
            catch (error) {
                this._disposed = true;
                this._resizeObserver.disconnect();
                throw error;
            }
        }

        get canvas() {
            return this._canvas;
        }

        get context() {
            return this._context;
        }

        get size() {
            return this._size;
        }

        measure() {
            this._throwIfDisposed("measure");
            const logicalSize = readElementSize(this._resizeTarget);
            const pixelRatio = resolvePixelRatio(this._maxPixelRatio);
            const backingWidth = Math.max(1, Math.ceil(logicalSize.width * pixelRatio));
            const backingHeight = Math.max(1, Math.ceil(logicalSize.height * pixelRatio));
            const changed = logicalSize.width !== this._size.width ||
                logicalSize.height !== this._size.height ||
                pixelRatio !== this._size.pixelRatio ||
                backingWidth !== this._size.backingWidth ||
                backingHeight !== this._size.backingHeight;

            if (!changed) {
                return this._size;
            }

            if (this._canvas.width !== backingWidth) {
                this._canvas.width = backingWidth;
            }

            if (this._canvas.height !== backingHeight) {
                this._canvas.height = backingHeight;
            }

            this._canvas.style.width = `${logicalSize.width}px`;
            this._canvas.style.height = `${logicalSize.height}px`;
            this._context.setTransform(pixelRatio, 0, 0, pixelRatio, 0, 0);
            this._context.imageSmoothingEnabled = true;
            this._size = Object.freeze({
                width: logicalSize.width,
                height: logicalSize.height,
                pixelRatio,
                backingWidth,
                backingHeight
            });

            if (this._onResize) {
                invokeSynchronous(this._onResize, this._size, "CanvasSurface options.onResize");
            }

            this.requestRender();
            return this._size;
        }

        requestRender() {
            this._throwIfDisposed("requestRender");
            if (this._frameRequestId !== null) {
                return false;
            }

            this._frameRequestId = window.requestAnimationFrame(timestamp => {
                this._frameRequestId = null;
                if (this._disposed) {
                    return;
                }

                invokeSynchronous(this._onRender, Object.freeze({
                    canvas: this._canvas,
                    context: this._context,
                    size: this._size,
                    timestamp
                }), "CanvasSurface options.onRender");
            });
            return true;
        }

        pointFromEvent(event) {
            this._throwIfDisposed("pointFromEvent");
            const point = normalizePointerPoint(this._canvas, event);
            return Object.freeze({
                x: point.normalizedX * this._size.width,
                y: point.normalizedY * this._size.height,
                normalizedX: point.normalizedX,
                normalizedY: point.normalizedY,
                inside: point.inside
            });
        }

        toPngDataUrl(options) {
            this._throwIfDisposed("toPngDataUrl");
            const settings = options === undefined
                ? {}
                : requireObject(options, "CanvasSurface toPngDataUrl options");
            const width = settings.width === undefined ? this._size.width : settings.width;
            const height = settings.height === undefined ? this._size.height : settings.height;
            const pixelRatio = settings.pixelRatio === undefined ? this._size.pixelRatio : settings.pixelRatio;
            const customDraw = readOptionalFunction(settings.draw, "CanvasSurface toPngDataUrl options.draw");

            return renderToPngDataUrl({
                width,
                height,
                pixelRatio,
                background: settings.background,
                draw: customDraw
                    ? (context, exportSize) => customDraw(context, Object.freeze({
                        ...exportSize,
                        sourceCanvas: this._canvas,
                        sourceSize: this._size
                    }))
                    : (context, exportSize) => context.drawImage(
                        this._canvas,
                        0,
                        0,
                        exportSize.width,
                        exportSize.height)
            });
        }

        dispose() {
            if (this._disposed) {
                return;
            }

            this._disposed = true;
            this._resizeObserver.disconnect();
            if (this._frameRequestId !== null) {
                window.cancelAnimationFrame(this._frameRequestId);
                this._frameRequestId = null;
            }
        }

        _throwIfDisposed(operation) {
            if (this._disposed) {
                throw new Error(`CanvasSurface.${operation} cannot run after dispose().`);
            }
        }
    }

    class HitRegionRegistry {
        constructor() {
            this._regions = [];
        }

        get count() {
            return this._regions.length;
        }

        clear() {
            this._regions.length = 0;
        }

        add(bounds, metadata) {
            const safeBounds = requireObject(bounds, "HitRegionRegistry bounds");
            const safeMetadata = requireMetadata(metadata, "HitRegionRegistry metadata");
            const region = Object.freeze({
                bounds: Object.freeze({
                    x: requireFiniteNumber(safeBounds.x, "HitRegionRegistry bounds.x"),
                    y: requireFiniteNumber(safeBounds.y, "HitRegionRegistry bounds.y"),
                    width: requirePositiveNumber(safeBounds.width, "HitRegionRegistry bounds.width"),
                    height: requirePositiveNumber(safeBounds.height, "HitRegionRegistry bounds.height")
                }),
                metadata: safeMetadata
            });
            this._regions.push(region);
            return region;
        }

        find(x, y) {
            const pointX = requireFiniteNumber(x, "HitRegionRegistry x");
            const pointY = requireFiniteNumber(y, "HitRegionRegistry y");
            for (let index = this._regions.length - 1; index >= 0; index -= 1) {
                const region = this._regions[index];
                const bounds = region.bounds;
                if (pointX >= bounds.x &&
                    pointX <= bounds.x + bounds.width &&
                    pointY >= bounds.y &&
                    pointY <= bounds.y + bounds.height) {
                    return region.metadata;
                }
            }

            return null;
        }
    }

    class PointerRouter {
        constructor(options) {
            const settings = requireObject(options, "PointerRouter options");
            this._element = requireElement(settings.element, "PointerRouter options.element");
            this._coordinateElement = settings.coordinateElement === undefined || settings.coordinateElement === null
                ? this._element
                : requireElement(settings.coordinateElement, "PointerRouter options.coordinateElement");
            this._onPointerDown = requireFunction(settings.onPointerDown, "PointerRouter options.onPointerDown");
            this._onPointerMove = requireFunction(settings.onPointerMove, "PointerRouter options.onPointerMove");
            this._onPointerUp = requireFunction(settings.onPointerUp, "PointerRouter options.onPointerUp");
            this._onPointerCancel = requireFunction(settings.onPointerCancel, "PointerRouter options.onPointerCancel");
            requireFunction(this._element.setPointerCapture, "PointerRouter element.setPointerCapture");
            requireFunction(this._element.releasePointerCapture, "PointerRouter element.releasePointerCapture");
            requireFunction(this._element.hasPointerCapture, "PointerRouter element.hasPointerCapture");

            this._session = null;
            this._disposed = false;
            this._handlePointerDown = event => this._pointerDown(event);
            this._handlePointerMove = event => this._pointerMove(event);
            this._handlePointerUp = event => this._pointerUp(event);
            this._handlePointerCancel = event => this._pointerCancel(event, "pointercancel", true);
            this._handleLostPointerCapture = event => this._pointerCancel(event, "lostpointercapture", false);

            this._element.addEventListener("pointerdown", this._handlePointerDown);
            this._element.addEventListener("pointermove", this._handlePointerMove);
            this._element.addEventListener("pointerup", this._handlePointerUp);
            this._element.addEventListener("pointercancel", this._handlePointerCancel);
            this._element.addEventListener("lostpointercapture", this._handleLostPointerCapture);
        }

        get isActive() {
            return this._session !== null;
        }

        dispose() {
            if (this._disposed) {
                return;
            }

            let cancellationError = null;
            try {
                if (this._session) {
                    this._cancelSession(this._session.lastEvent, "dispose", true, this._session.lastPoint);
                }
            }
            catch (error) {
                cancellationError = error;
            }
            finally {
                this._disposed = true;
                this._element.removeEventListener("pointerdown", this._handlePointerDown);
                this._element.removeEventListener("pointermove", this._handlePointerMove);
                this._element.removeEventListener("pointerup", this._handlePointerUp);
                this._element.removeEventListener("pointercancel", this._handlePointerCancel);
                this._element.removeEventListener("lostpointercapture", this._handleLostPointerCapture);
            }

            if (cancellationError) {
                throw cancellationError;
            }
        }

        _pointerDown(event) {
            if (this._disposed || this._session || event.isPrimary === false || event.button !== 0) {
                return;
            }

            const pointerId = requireFiniteNumber(event.pointerId, "pointer event pointerId");
            const point = normalizePointerPoint(this._coordinateElement, event);
            this._element.setPointerCapture(pointerId);
            this._session = {
                pointerId,
                pointerType: String(event.pointerType || "unknown"),
                startPoint: point,
                previousPoint: point,
                lastPoint: point,
                lastEvent: event
            };

            this._preventDefault(event);
            try {
                const accepted = this._invokeCallback(
                    this._onPointerDown,
                    this._buildPayload("down", event, point),
                    "onPointerDown");
                if (accepted === false) {
                    this._releaseSession(true);
                }
            }
            catch (error) {
                this._releaseSession(true);
                throw error;
            }
        }

        _pointerMove(event) {
            if (!this._session || event.pointerId !== this._session.pointerId) {
                return;
            }

            const point = normalizePointerPoint(this._coordinateElement, event);
            this._preventDefault(event);
            const payload = this._buildPayload("move", event, point);
            this._invokeCallback(this._onPointerMove, payload, "onPointerMove");
            this._session.previousPoint = point;
            this._session.lastPoint = point;
            this._session.lastEvent = event;
        }

        _pointerUp(event) {
            if (!this._session || event.pointerId !== this._session.pointerId) {
                return;
            }

            const point = normalizePointerPoint(this._coordinateElement, event);
            this._preventDefault(event);
            try {
                this._invokeCallback(this._onPointerUp, this._buildPayload("up", event, point), "onPointerUp");
            }
            finally {
                this._releaseSession(true);
            }
        }

        _pointerCancel(event, reason, releaseCapture) {
            if (!this._session || event.pointerId !== this._session.pointerId) {
                return;
            }

            this._cancelSession(event, reason, releaseCapture);
        }

        _cancelSession(event, reason, releaseCapture, storedPoint = null) {
            const point = storedPoint || normalizePointerPoint(this._coordinateElement, event);
            try {
                this._invokeCallback(
                    this._onPointerCancel,
                    Object.freeze({ ...this._buildPayload("cancel", event, point), reason }),
                    "onPointerCancel");
            }
            finally {
                this._releaseSession(releaseCapture);
            }
        }

        _releaseSession(releaseCapture) {
            const session = this._session;
            this._session = null;
            if (releaseCapture && session && this._element.hasPointerCapture(session.pointerId)) {
                this._element.releasePointerCapture(session.pointerId);
            }
        }

        _buildPayload(phase, event, point) {
            const session = this._session;
            if (!session) {
                throw new Error(`PointerRouter cannot build a ${phase} payload without an active pointer session.`);
            }

            return Object.freeze({
                phase,
                event,
                pointerId: session.pointerId,
                pointerType: session.pointerType,
                point,
                startPoint: session.startPoint,
                delta: Object.freeze({
                    x: point.x - session.previousPoint.x,
                    y: point.y - session.previousPoint.y
                }),
                totalDelta: Object.freeze({
                    x: point.x - session.startPoint.x,
                    y: point.y - session.startPoint.y
                })
            });
        }

        _invokeCallback(callback, payload, name) {
            return invokeSynchronous(callback, payload, `PointerRouter ${name}`);
        }

        _preventDefault(event) {
            if (event.cancelable) {
                event.preventDefault();
            }
        }
    }

    root.canvasRuntime = Object.freeze({
        version: 1,
        CanvasSurface,
        HitRegionRegistry,
        PointerRouter,
        createSurface(options) {
            return new CanvasSurface(options);
        },
        createHitRegionRegistry() {
            return new HitRegionRegistry();
        },
        createPointerRouter(options) {
            return new PointerRouter(options);
        },
        renderToPngDataUrl,
        downloadDataUrl
    });
})();
