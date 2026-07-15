(function () {
    "use strict";

    const root = window.CanDoItAll = window.CanDoItAll || {};
    const chartStates = new WeakMap();
    const dragSources = new WeakMap();
    const hourMs = 60 * 60 * 1000;
    const dayMs = 24 * hourMs;
    const minimumMovementPx = 3;
    const maximumBackingDimension = 16_000;
    const maximumBackingPixels = 40_000_000;
    const maximumExportDimension = 12_000;
    const maximumExportPixels = 32_000_000;
    const dependencyArrowLength = 7;
    const routeComparisonTolerance = 0.01;

    const HitKind = Object.freeze({
        TaskBody: "task-body",
        ResizeStart: "resize-start",
        ResizeEnd: "resize-end",
        InputPort: "input-port",
        OutputPort: "output-port",
        DependencySource: "dependency-source",
        DependencyTarget: "dependency-target",
        Assignment: "assignment"
    });

    const InteractionKind = Object.freeze({
        Pan: "pan",
        Move: "move",
        ResizeStart: "resize-start",
        ResizeEnd: "resize-end",
        AddDependency: "add-dependency",
        ReconnectSource: "reconnect-source",
        ReconnectTarget: "reconnect-target"
    });

    const DependencyMutation = Object.freeze({
        Add: "add",
        Reconnect: "reconnect"
    });

    const Cursor = Object.freeze({
        Default: "default",
        Grab: "grab",
        Grabbing: "grabbing",
        Move: "move",
        ResizeHorizontal: "ew-resize",
        Connect: "crosshair",
        Help: "help"
    });

    function requireCanvasRuntime() {
        if (!root.canvasRuntime) {
            throw new Error("CanDoItAll.canvasRuntime is required before the Gantt chart runtime. Add <CanvasLibBodyAssets IncludeRuntimeAssets=\"true\" /> before <GanttChartBodyAssets />.");
        }

        return root.canvasRuntime;
    }

    function requireElement(value, name) {
        if (!(value instanceof HTMLElement)) {
            throw new Error(`Gantt chart ${name} must be an HTML element.`);
        }

        return value;
    }

    function requireCanvas(value) {
        if (!(value instanceof HTMLCanvasElement)) {
            throw new Error("Gantt chart canvas must be an HTMLCanvasElement.");
        }

        return value;
    }

    function requireFinite(value, name) {
        if (!Number.isFinite(value)) {
            throw new Error(`Gantt chart ${name} must be a finite number.`);
        }

        return value;
    }

    function requirePositive(value, name) {
        const resolved = requireFinite(value, name);
        if (resolved <= 0) {
            throw new Error(`Gantt chart ${name} must be greater than zero.`);
        }

        return resolved;
    }

    function requireText(value, name) {
        if (typeof value !== "string" || !value.trim()) {
            throw new Error(`Gantt chart ${name} is required.`);
        }

        return value.trim();
    }

    function resolvePixelRatioLimit(width, height) {
        const resolvedWidth = requirePositive(width, "canvas width");
        const resolvedHeight = requirePositive(height, "canvas height");
        return Math.min(
            1.5,
            maximumBackingDimension / resolvedWidth,
            maximumBackingDimension / resolvedHeight,
            Math.sqrt(maximumBackingPixels / (resolvedWidth * resolvedHeight)));
    }

    function createSurface(runtime, state) {
        state.surfacePixelRatioLimit = resolvePixelRatioLimit(
            state.model.options.timelineWidth,
            state.model.options.canvasHeight);
        return runtime.createSurface({
            canvas: state.canvas,
            resizeTarget: state.canvas,
            maxPixelRatio: state.surfacePixelRatioLimit,
            onRender: () => renderStateSafely(state)
        });
    }

    function renderStateSafely(state) {
        if (state.renderErrorReported) {
            return;
        }

        try {
            renderState(state);
        }
        catch (error) {
            if (!state.renderErrorReported) {
                state.renderErrorReported = true;
                reportRenderError(state, error);
            }
        }
    }

    function normalizeAssignment(value) {
        return {
            id: requireText(value?.id, "assignment id"),
            label: requireText(value?.label, "assignment label"),
            kind: requireText(value?.kind, "assignment kind")
        };
    }

    function normalizeTask(value, index) {
        const startMs = requireFinite(value?.startMs, `task ${index} start`);
        const endMs = requireFinite(value?.endMs, `task ${index} end`);
        if (endMs <= startMs) {
            throw new Error(`Gantt task '${value?.id || index}' must end after it starts.`);
        }

        return {
            id: requireText(value?.id, `task ${index} id`),
            title: requireText(value?.title, `task ${index} title`),
            startMs,
            endMs,
            order: Number.isInteger(value?.order) ? value.order : index,
            accentColor: typeof value?.accentColor === "string" ? value.accentColor.trim() : "",
            isReadOnly: value?.isReadOnly === true,
            isScheduleReadOnly: value?.isReadOnly === true || value?.isScheduleReadOnly === true,
            isTitleReadOnly: value?.isReadOnly === true || value?.isTitleReadOnly === true,
            isDependencyReadOnly: value?.isReadOnly === true || value?.isDependencyReadOnly === true,
            isProjectionOnly: value?.isProjectionOnly === true,
            isCritical: value?.isCritical === true,
            assignments: Array.isArray(value?.assignments) ? value.assignments.map(normalizeAssignment) : []
        };
    }

    function normalizeDependency(value, index) {
        return {
            id: requireText(value?.id, `dependency ${index} id`),
            predecessorId: requireText(value?.predecessorId, `dependency ${index} predecessor`),
            successorId: requireText(value?.successorId, `dependency ${index} successor`),
            lagMs: Number.isFinite(value?.lagMs) ? value.lagMs : 0,
            isCritical: value?.isCritical === true
        };
    }

    function calculateRequiredDependencyGutter(options, dependencies) {
        const outgoingCounts = new Map();
        const incomingCounts = new Map();
        for (const dependency of dependencies) {
            outgoingCounts.set(dependency.predecessorId, (outgoingCounts.get(dependency.predecessorId) || 0) + 1);
            incomingCounts.set(dependency.successorId, (incomingCounts.get(dependency.successorId) || 0) + 1);
        }

        let maximumFanSize = 0;
        for (const count of outgoingCounts.values()) {
            maximumFanSize = Math.max(maximumFanSize, count);
        }
        for (const count of incomingCounts.values()) {
            maximumFanSize = Math.max(maximumFanSize, count);
        }
        const availableSlotHeight = Math.max(0, options.barHeight - (2 * options.dependencyEndpointRadius));
        const slotsPerLane = Math.max(
            1,
            Math.floor(availableSlotHeight / options.dependencyEndpointVerticalSpacing) + 1);
        const laneCount = Math.max(1, Math.ceil(maximumFanSize / slotsPerLane));
        return options.dependencyEndpointEdgeOffset +
            ((laneCount - 1) * options.dependencyEndpointLaneSpacing) +
            (options.dependencyEndpointHitSize / 2) +
            1;
    }

    function normalizeModel(value) {
        if (!value || typeof value !== "object") {
            throw new Error("Gantt chart model is required.");
        }

        const tasks = Array.isArray(value.tasks)
            ? value.tasks.map(normalizeTask).sort((left, right) => left.order - right.order || left.id.localeCompare(right.id))
            : [];
        const taskIds = new Set();
        for (const task of tasks) {
            if (taskIds.has(task.id)) {
                throw new Error(`Gantt task id '${task.id}' is duplicated.`);
            }
            taskIds.add(task.id);
        }

        const dependencies = Array.isArray(value.dependencies)
            ? value.dependencies.map(normalizeDependency)
            : [];
        const dependencyIds = new Set();
        for (const dependency of dependencies) {
            if (dependencyIds.has(dependency.id)) {
                throw new Error(`Gantt dependency id '${dependency.id}' is duplicated.`);
            }
            if (!taskIds.has(dependency.predecessorId) || !taskIds.has(dependency.successorId)) {
                throw new Error(`Gantt dependency '${dependency.id}' references a task that is not in the chart.`);
            }
            dependencyIds.add(dependency.id);
        }

        const options = value.options || {};
        const normalizedOptions = {
            rowHeight: requirePositive(options.rowHeight, "row height"),
            headerHeight: requirePositive(options.headerHeight, "header height"),
            barHeight: requirePositive(options.barHeight, "bar height"),
            pixelsPerHour: requirePositive(options.pixelsPerHour, "pixels per hour"),
            tickIntervalMs: Number.isFinite(options.tickIntervalMs) && options.tickIntervalMs > 0
                ? options.tickIntervalMs
                : 0,
            snapMs: requirePositive(options.snapMs, "snap duration"),
            snapOriginMs: requireFinite(options.snapOriginMs, "snap origin"),
            minimumTaskDurationMs: requirePositive(options.minimumTaskDurationMs, "minimum task duration"),
            timelineStartMs: requireFinite(options.timelineStartMs, "timeline start"),
            timelineEndMs: requireFinite(options.timelineEndMs, "timeline end"),
            timelineWidth: requirePositive(options.timelineWidth, "timeline width"),
            timelineGutter: requirePositive(options.timelineGutter, "timeline gutter"),
            dependencyEndpointRadius: requirePositive(options.dependencyEndpointRadius, "dependency endpoint radius"),
            dependencyEndpointHitSize: requirePositive(options.dependencyEndpointHitSize, "dependency endpoint hit size"),
            dependencyEndpointVerticalSpacing: requirePositive(options.dependencyEndpointVerticalSpacing, "dependency endpoint vertical spacing"),
            dependencyEndpointLaneSpacing: requirePositive(options.dependencyEndpointLaneSpacing, "dependency endpoint lane spacing"),
            dependencyEndpointEdgeOffset: requirePositive(options.dependencyEndpointEdgeOffset, "dependency endpoint edge offset"),
            dependencyRouteClearance: requirePositive(options.dependencyRouteClearance, "dependency route clearance"),
            canvasHeight: requirePositive(options.canvasHeight, "canvas height"),
            taskTableWidth: requirePositive(options.taskTableWidth, "task table width"),
            hoursPerManDay: requirePositive(options.hoursPerManDay, "hours per man-day"),
            showTaskTable: options.showTaskTable !== false,
            allowTaskEditing: options.allowTaskEditing !== false,
            allowDependencyEditing: options.allowDependencyEditing !== false,
            allowTaskInsertion: options.allowTaskInsertion === true,
            dragDataFormat: requireText(options.dragDataFormat, "drag data format")
        };
        if (normalizedOptions.timelineWidth <= normalizedOptions.timelineGutter * 2) {
            throw new Error("Gantt chart timeline gutter must leave a positive timeline content width.");
        }
        if (normalizedOptions.dependencyEndpointHitSize < normalizedOptions.dependencyEndpointRadius * 2) {
            throw new Error("Gantt chart dependency endpoint hit size must cover the visible endpoint.");
        }
        if (normalizedOptions.dependencyEndpointVerticalSpacing <= normalizedOptions.dependencyEndpointHitSize ||
            normalizedOptions.dependencyEndpointLaneSpacing <= normalizedOptions.dependencyEndpointHitSize) {
            throw new Error("Gantt chart dependency endpoint spacing must be larger than its hit size.");
        }
        if (normalizedOptions.timelineGutter <
            normalizedOptions.dependencyEndpointEdgeOffset + (normalizedOptions.dependencyEndpointHitSize / 2)) {
            throw new Error("Gantt chart timeline gutter is too small for dependency endpoint handles.");
        }
        const requiredDependencyGutter = calculateRequiredDependencyGutter(normalizedOptions, dependencies);
        if (normalizedOptions.timelineGutter < requiredDependencyGutter) {
            throw new Error(
                `Gantt chart timeline gutter is ${normalizedOptions.timelineGutter}px but the dependency fan requires ${requiredDependencyGutter}px.`);
        }

        return {
            tasks,
            dependencies,
            taskLookup: new Map(tasks.map(task => [task.id, task])),
            options: normalizedOptions
        };
    }

    function resolveColors(host) {
        const style = window.getComputedStyle(host);
        const read = (name, fallback) => style.getPropertyValue(name).trim() || fallback;
        return {
            surface: read("--gantt-surface", "#ffffff"),
            surfaceMuted: read("--gantt-surface-muted", "#f7f9fc"),
            grid: read("--gantt-grid", "#e7edf5"),
            border: read("--gantt-border", "#dbe3ef"),
            text: read("--gantt-text", "#172033"),
            textMuted: read("--gantt-text-muted", "#657087"),
            accent: read("--gantt-accent", "#236887"),
            critical: read("--gantt-critical", "#d97706"),
            handle: read("--gantt-handle", "#4d9f38"),
            connector: read("--gantt-connector", "#0ea5e9")
        };
    }

    function roundTo(value, interval) {
        const intervals = value / interval;
        return Math.sign(intervals) * Math.floor(Math.abs(intervals) + 0.5) * interval;
    }

    function snapTime(model, value) {
        return model.options.snapOriginMs + roundTo(
            value - model.options.snapOriginMs,
            model.options.snapMs);
    }

    function timeToX(model, timeMs, originX) {
        return originX + model.options.timelineGutter +
            ((timeMs - model.options.timelineStartMs) / hourMs) * model.options.pixelsPerHour;
    }

    function xToTime(model, x) {
        return model.options.timelineStartMs +
            ((x - model.options.timelineGutter) / model.options.pixelsPerHour) * hourMs;
    }

    function taskY(model, index) {
        return model.options.headerHeight + (index * model.options.rowHeight) + ((model.options.rowHeight - model.options.barHeight) / 2);
    }

    function taskCenterY(model, index) {
        return model.options.headerHeight + (index * model.options.rowHeight) + (model.options.rowHeight / 2);
    }

    function taskRect(model, task, index, originX) {
        const x = timeToX(model, task.startMs, originX);
        return {
            x,
            y: taskY(model, index),
            width: Math.max(8, timeToX(model, task.endMs, originX) - x),
            height: model.options.barHeight
        };
    }

    function addHit(registry, bounds, metadata) {
        registry.add(bounds, metadata);
    }

    function fillRoundedRect(context, x, y, width, height, radius) {
        const safeRadius = Math.min(radius, width / 2, height / 2);
        context.beginPath();
        context.roundRect(x, y, width, height, safeRadius);
        context.fill();
    }

    function strokeRoundedRect(context, x, y, width, height, radius) {
        const safeRadius = Math.min(radius, width / 2, height / 2);
        context.beginPath();
        context.roundRect(x, y, width, height, safeRadius);
        context.stroke();
    }

    function fitText(context, text, maxWidth) {
        if (maxWidth <= 8 || context.measureText(text).width <= maxWidth) {
            return maxWidth <= 8 ? "" : text;
        }

        let lower = 1;
        let upper = text.length - 1;
        let fittedLength = 1;
        while (lower <= upper) {
            const candidateLength = Math.floor((lower + upper) / 2);
            if (context.measureText(`${text.slice(0, candidateLength)}…`).width <= maxWidth) {
                fittedLength = candidateLength;
                lower = candidateLength + 1;
            }
            else {
                upper = candidateLength - 1;
            }
        }

        return `${text.slice(0, fittedLength)}…`;
    }

    function resolveCursor(state) {
        switch (state.interaction?.kind) {
            case InteractionKind.Pan:
            case InteractionKind.Move:
                return Cursor.Grabbing;
            case InteractionKind.ResizeStart:
            case InteractionKind.ResizeEnd:
                return Cursor.ResizeHorizontal;
            case InteractionKind.AddDependency:
            case InteractionKind.ReconnectSource:
            case InteractionKind.ReconnectTarget:
                return Cursor.Connect;
            default:
                break;
        }

        switch (state.hoverHit?.kind) {
            case HitKind.ResizeStart:
            case HitKind.ResizeEnd:
                return Cursor.ResizeHorizontal;
            case HitKind.InputPort:
            case HitKind.OutputPort:
            case HitKind.DependencySource:
            case HitKind.DependencyTarget:
                return Cursor.Connect;
            case HitKind.Assignment:
                return Cursor.Help;
            case HitKind.TaskBody:
                return state.model.options.allowTaskEditing && !state.hoverHit.task.isScheduleReadOnly
                    ? Cursor.Move
                    : Cursor.Default;
            default:
                return Cursor.Grab;
        }
    }

    function updateCursor(state) {
        const cursor = resolveCursor(state);
        if (cursor === state.cursor) {
            return;
        }

        state.cursor = cursor;
        state.canvas.style.cursor = cursor;
    }

    function hoverAffectsDrawing(hit) {
        return hit?.kind === HitKind.Assignment ||
            hit?.kind === HitKind.DependencySource ||
            hit?.kind === HitKind.DependencyTarget;
    }

    function resolveTickInterval(model) {
        if (model.options.tickIntervalMs > 0) {
            return model.options.tickIntervalMs;
        }

        const pixelsPerHour = model.options.pixelsPerHour;
        if (pixelsPerHour >= 48) {
            return hourMs;
        }
        if (pixelsPerHour >= 14) {
            return 6 * hourMs;
        }
        if (pixelsPerHour >= 4) {
            return dayMs;
        }
        return 7 * dayMs;
    }

    function resolveTickLabelInterval(model, tickInterval, measureLabelWidth) {
        let labelInterval = tickInterval;
        while (true) {
            const label = formatTick(model.options.timelineStartMs, labelInterval);
            const labelSpacing = measureLabelWidth(label) + 14;
            const availableSpacing = model.options.pixelsPerHour * (labelInterval / hourMs);
            if (availableSpacing >= labelSpacing) {
                return labelInterval;
            }

            labelInterval *= 2;
        }
    }

    function formatTick(timestamp, interval) {
        const date = new Date(timestamp);
        const year = date.getUTCFullYear();
        const month = String(date.getUTCMonth() + 1).padStart(2, "0");
        const day = String(date.getUTCDate()).padStart(2, "0");
        if (interval < dayMs) {
            const hour = String(date.getUTCHours()).padStart(2, "0");
            const minute = String(date.getUTCMinutes()).padStart(2, "0");
            return `${month}-${day} ${hour}:${minute} UTC`;
        }
        return `${year}-${month}-${day} UTC`;
    }

    function formatUtcDateTime(timestamp) {
        const date = new Date(timestamp);
        const year = date.getUTCFullYear();
        const month = String(date.getUTCMonth() + 1).padStart(2, "0");
        const day = String(date.getUTCDate()).padStart(2, "0");
        const hour = String(date.getUTCHours()).padStart(2, "0");
        const minute = String(date.getUTCMinutes()).padStart(2, "0");
        return `${year}-${month}-${day} ${hour}:${minute} UTC`;
    }

    function drawGrid(context, model, colors, originX, width) {
        const options = model.options;
        context.fillStyle = colors.surface;
        context.fillRect(originX, 0, width, options.canvasHeight);
        context.fillStyle = colors.surfaceMuted;
        context.fillRect(originX, 0, width, options.headerHeight);

        context.strokeStyle = colors.grid;
        context.lineWidth = 1;
        for (let index = 0; index <= model.tasks.length; index += 1) {
            const y = options.headerHeight + (index * options.rowHeight) + 0.5;
            context.beginPath();
            context.moveTo(originX, y);
            context.lineTo(originX + width, y);
            context.stroke();
        }

        context.font = '600 11px "Segoe UI", sans-serif';
        context.textBaseline = "middle";
        const tickInterval = resolveTickInterval(model);
        const firstTick = Math.floor(options.timelineStartMs / tickInterval) * tickInterval;
        for (let tick = firstTick; tick <= options.timelineEndMs; tick += tickInterval) {
            const x = Math.round(timeToX(model, tick, originX)) + 0.5;
            if (x < originX) {
                continue;
            }
            context.strokeStyle = tick % dayMs === 0 ? colors.border : colors.grid;
            context.beginPath();
            context.moveTo(x, 0);
            context.lineTo(x, options.canvasHeight);
            context.stroke();
        }

        const labelInterval = resolveTickLabelInterval(
            model,
            tickInterval,
            label => context.measureText(label).width);
        const firstLabel = Math.floor(options.timelineStartMs / labelInterval) * labelInterval;
        for (let tick = firstLabel; tick <= options.timelineEndMs; tick += labelInterval) {
            const x = Math.round(timeToX(model, tick, originX)) + 0.5;
            if (x < originX) {
                continue;
            }

            context.fillStyle = colors.textMuted;
            context.fillText(formatTick(tick, labelInterval), x + 7, options.headerHeight / 2);
        }

        const nowX = timeToX(model, Date.now(), originX);
        if (nowX >= originX && nowX <= originX + width) {
            context.strokeStyle = colors.critical;
            context.lineWidth = 1.5;
            context.beginPath();
            context.moveTo(nowX, 0);
            context.lineTo(nowX, options.canvasHeight);
            context.stroke();
        }
    }

    function drawArrow(context, x, y, color) {
        context.fillStyle = color;
        context.beginPath();
        context.moveTo(x, y);
        context.lineTo(x - dependencyArrowLength, y - 4);
        context.lineTo(x - dependencyArrowLength, y + 4);
        context.closePath();
        context.fill();
    }

    function drawDependencies(context, state, model, colors, geometries) {
        for (const geometry of geometries) {
            const { dependency } = geometry;
            const color = dependency.isCritical ? colors.critical : colors.accent;

            context.strokeStyle = color;
            context.lineWidth = dependency.isCritical ? 2.25 : 1.6;
            context.beginPath();
            context.moveTo(geometry.routePoints[0].x, geometry.routePoints[0].y);
            for (let pointIndex = 1; pointIndex < geometry.routePoints.length; pointIndex += 1) {
                const point = geometry.routePoints[pointIndex];
                context.lineTo(point.x, point.y);
            }
            context.stroke();
            drawArrow(context, geometry.targetAttachmentX, geometry.targetY, color);

            const predecessor = model.taskLookup.get(dependency.predecessorId);
            const successor = model.taskLookup.get(dependency.successorId);
            if (model.options.allowDependencyEditing &&
                !predecessor.isDependencyReadOnly &&
                !successor.isDependencyReadOnly) {
                drawDependencyEndpoint(
                    context,
                    state,
                    dependency,
                    HitKind.DependencySource,
                    geometry.sourceHandleX,
                    geometry.sourceY,
                    colors);
                drawDependencyEndpoint(
                    context,
                    state,
                    dependency,
                    HitKind.DependencyTarget,
                    geometry.targetHandleX,
                    geometry.targetY,
                    colors);
            }
        }
    }

    function drawDependencyEndpoint(context, state, dependency, kind, x, y, colors) {
        const isHovered = state.hoverHit?.kind === kind && state.hoverHit?.dependency?.id === dependency.id;
        const radius = state.model.options.dependencyEndpointRadius + (isHovered ? 2 : 0);
        context.fillStyle = colors.connector;
        context.beginPath();
        context.arc(x, y, radius, 0, Math.PI * 2);
        context.fill();
        if (isHovered) {
            context.strokeStyle = colors.text;
            context.lineWidth = 1.5;
            context.stroke();
        }
    }

    function invalidateDependencyGeometry(state) {
        state.dependencyGeometry = null;
    }

    function clearTaskPreview(state) {
        state.previewTasks.clear();
        state.previewOwnerToken = null;
        invalidateDependencyGeometry(state);
    }

    function resolveDependencyGeometry(state, model, originX, validateRoutes = true) {
        const taskIndexes = new Map(model.tasks.map((task, index) => [task.id, index]));
        const taskRects = model.tasks.map((task, index) => ({
            taskId: task.id,
            ...taskRect(
                model,
                state.previewTasks.get(task.id) || task,
                index,
                originX)
        }));
        const outgoingCounts = new Map();
        const incomingCounts = new Map();
        for (const dependency of model.dependencies) {
            outgoingCounts.set(dependency.predecessorId, (outgoingCounts.get(dependency.predecessorId) || 0) + 1);
            incomingCounts.set(dependency.successorId, (incomingCounts.get(dependency.successorId) || 0) + 1);
        }

        const outgoingIndexes = new Map();
        const incomingIndexes = new Map();
        return model.dependencies.map(dependency => {
            const predecessor = state.previewTasks.get(dependency.predecessorId) || model.taskLookup.get(dependency.predecessorId);
            const successor = state.previewTasks.get(dependency.successorId) || model.taskLookup.get(dependency.successorId);
            const predecessorIndex = taskIndexes.get(dependency.predecessorId);
            const successorIndex = taskIndexes.get(dependency.successorId);
            const predecessorRect = taskRects[predecessorIndex];
            const successorRect = taskRects[successorIndex];
            const outgoingIndex = outgoingIndexes.get(dependency.predecessorId) || 0;
            const incomingIndex = incomingIndexes.get(dependency.successorId) || 0;
            outgoingIndexes.set(dependency.predecessorId, outgoingIndex + 1);
            incomingIndexes.set(dependency.successorId, incomingIndex + 1);

            const source = resolveDependencyEndpoint(
                model,
                predecessorRect,
                outgoingIndex,
                outgoingCounts.get(dependency.predecessorId),
                true);
            const target = resolveDependencyEndpoint(
                model,
                successorRect,
                incomingIndex,
                incomingCounts.get(dependency.successorId),
                false);

            const geometry = {
                dependency,
                sourceAttachmentX: source.attachmentX,
                sourceHandleX: source.handleX,
                sourceY: source.y,
                targetHandleX: target.handleX,
                targetAttachmentX: target.attachmentX,
                targetY: target.y,
                routePoints: resolveObstacleSafeRoute(
                    model.options,
                    taskRects,
                    predecessorIndex,
                    successorIndex,
                    source,
                    target,
                    originX)
            };
            if (validateRoutes) {
                assertDependencyRouteAvoidsUnrelatedTasks(
                    geometry,
                    taskRects,
                    dependency.predecessorId,
                    dependency.successorId);
            }
            return geometry;
        });
    }

    function getDependencyGeometry(state, model, originX) {
        if (originX !== 0) {
            return resolveDependencyGeometry(state, model, originX, state.previewTasks.size === 0);
        }

        state.dependencyGeometry ??= resolveDependencyGeometry(
            state,
            model,
            originX,
            state.previewTasks.size === 0);
        return state.dependencyGeometry;
    }

    function resolveObstacleSafeRoute(
        options,
        taskRects,
        predecessorIndex,
        successorIndex,
        source,
        target,
        originX = 0) {
        const arrowBase = {
            x: target.attachmentX - dependencyArrowLength,
            y: target.y
        };
        if (Math.abs(successorIndex - predecessorIndex) === 1) {
            const boundaryY = successorIndex > predecessorIndex
                ? options.headerHeight + ((predecessorIndex + 1) * options.rowHeight)
                : options.headerHeight + (predecessorIndex * options.rowHeight);
            return compactRoutePoints([
                { x: source.attachmentX, y: source.y },
                { x: source.handleX, y: source.y },
                { x: source.handleX, y: boundaryY },
                { x: target.handleX, y: boundaryY },
                { x: target.handleX, y: target.y },
                arrowBase
            ]);
        }

        const directLaneX = resolveDirectDependencyLaneX(
            options,
            taskRects,
            predecessorIndex,
            successorIndex,
            source,
            target);
        if (directLaneX !== null) {
            return compactRoutePoints([
                { x: source.attachmentX, y: source.y },
                { x: source.handleX, y: source.y },
                { x: directLaneX, y: source.y },
                { x: directLaneX, y: target.y },
                { x: target.handleX, y: target.y },
                arrowBase
            ]);
        }

        const firstIndex = Math.min(predecessorIndex, successorIndex);
        const lastIndex = Math.max(predecessorIndex, successorIndex);
        const traversedRects = taskRects.slice(firstIndex, lastIndex + 1);
        const minimumLeft = Math.min(...traversedRects.map(rect => rect.x));
        const maximumRight = Math.max(...traversedRects.map(rect => rect.x + rect.width));
        const clearance = options.dependencyRouteClearance;
        const minimumSpineX = originX + clearance;
        const maximumSpineX = originX + options.timelineWidth - clearance;
        const leftSpineX = Math.max(
            minimumSpineX,
            Math.min(minimumLeft - clearance, source.handleX - clearance, target.handleX - clearance));
        const rightSpineX = Math.min(
            maximumSpineX,
            Math.max(maximumRight + clearance, source.handleX + clearance, target.handleX + clearance));
        const sourceBoundaryY = successorIndex > predecessorIndex
            ? options.headerHeight + ((predecessorIndex + 1) * options.rowHeight)
            : options.headerHeight + (predecessorIndex * options.rowHeight);
        const targetBoundaryY = successorIndex > predecessorIndex
            ? options.headerHeight + (successorIndex * options.rowHeight)
            : options.headerHeight + ((successorIndex + 1) * options.rowHeight);
        const leftLength = Math.abs(source.handleX - leftSpineX) + Math.abs(target.handleX - leftSpineX);
        const rightLength = Math.abs(source.handleX - rightSpineX) + Math.abs(target.handleX - rightSpineX);
        const spineX = leftLength <= rightLength ? leftSpineX : rightSpineX;

        return compactRoutePoints([
            { x: source.attachmentX, y: source.y },
            { x: source.handleX, y: source.y },
            { x: source.handleX, y: sourceBoundaryY },
            { x: spineX, y: sourceBoundaryY },
            { x: spineX, y: targetBoundaryY },
            { x: target.handleX, y: targetBoundaryY },
            { x: target.handleX, y: target.y },
            arrowBase
        ]);
    }

    function resolveDirectDependencyLaneX(options, taskRects, predecessorIndex, successorIndex, source, target) {
        if (source.handleX > target.handleX) {
            return null;
        }

        const clearance = options.dependencyRouteClearance;
        const firstIndex = Math.min(predecessorIndex, successorIndex) + 1;
        const lastIndex = Math.max(predecessorIndex, successorIndex) - 1;
        const desiredX = source.handleX + Math.max(18, (target.handleX - source.handleX) * 0.45);
        const blockedIntervals = [];
        for (let index = firstIndex; index <= lastIndex; index += 1) {
            const rect = taskRects[index];
            const left = rect.x - clearance;
            const right = rect.x + rect.width + clearance;
            if (right > source.handleX && left < target.handleX) {
                blockedIntervals.push({ left, right });
            }
        }

        blockedIntervals.sort((left, right) => left.left - right.left || left.right - right.right);
        const mergedIntervals = [];
        for (const interval of blockedIntervals) {
            const previous = mergedIntervals[mergedIntervals.length - 1];
            if (previous && interval.left < previous.right) {
                previous.right = Math.max(previous.right, interval.right);
            }
            else {
                mergedIntervals.push({ ...interval });
            }
        }

        const boundedDesiredX = Math.max(source.handleX, Math.min(target.handleX, desiredX));
        const blockingInterval = mergedIntervals.find(interval =>
            boundedDesiredX > interval.left && boundedDesiredX < interval.right);
        if (!blockingInterval) {
            return boundedDesiredX;
        }

        const candidates = [blockingInterval.left, blockingInterval.right]
            .filter(candidate => candidate >= source.handleX && candidate <= target.handleX);
        return candidates.sort((left, right) =>
            Math.abs(left - boundedDesiredX) - Math.abs(right - boundedDesiredX))[0] ?? null;
    }

    function compactRoutePoints(points) {
        return points.filter((point, index) => index === 0 ||
            Math.abs(point.x - points[index - 1].x) > routeComparisonTolerance ||
            Math.abs(point.y - points[index - 1].y) > routeComparisonTolerance);
    }

    function assertDependencyRouteAvoidsUnrelatedTasks(
        geometry,
        taskRects,
        predecessorId,
        successorId) {
        const terminalPoint = geometry.routePoints[geometry.routePoints.length - 1];
        if (Math.abs(terminalPoint.x - (geometry.targetAttachmentX - dependencyArrowLength)) > routeComparisonTolerance ||
            Math.abs(terminalPoint.y - geometry.targetY) > routeComparisonTolerance) {
            throw new Error(`Gantt dependency '${geometry.dependency.id}' does not terminate at its successor edge.`);
        }

        for (const rect of taskRects) {
            if (rect.taskId === predecessorId || rect.taskId === successorId) {
                continue;
            }

            for (let pointIndex = 1; pointIndex < geometry.routePoints.length; pointIndex += 1) {
                if (routeSegmentIntersectsTaskRectangle(
                    geometry.routePoints[pointIndex - 1],
                    geometry.routePoints[pointIndex],
                    rect)) {
                    throw new Error(
                        `Gantt dependency '${geometry.dependency.id}' intersects unrelated task '${rect.taskId}'.`);
                }
            }
        }
    }

    function routeSegmentIntersectsTaskRectangle(start, end, rect) {
        const rectRight = rect.x + rect.width;
        const rectBottom = rect.y + rect.height;
        if (Math.abs(start.x - end.x) <= routeComparisonTolerance) {
            const segmentTop = Math.min(start.y, end.y);
            const segmentBottom = Math.max(start.y, end.y);
            return start.x > rect.x + routeComparisonTolerance &&
                start.x < rectRight - routeComparisonTolerance &&
                segmentBottom > rect.y + routeComparisonTolerance &&
                segmentTop < rectBottom - routeComparisonTolerance;
        }

        if (Math.abs(start.y - end.y) <= routeComparisonTolerance) {
            const segmentLeft = Math.min(start.x, end.x);
            const segmentRight = Math.max(start.x, end.x);
            return start.y > rect.y + routeComparisonTolerance &&
                start.y < rectBottom - routeComparisonTolerance &&
                segmentRight > rect.x + routeComparisonTolerance &&
                segmentLeft < rectRight - routeComparisonTolerance;
        }

        throw new Error("Gantt dependency routes must contain only orthogonal segments.");
    }

    function resolveDependencyEndpoint(model, rect, index, count, isSource) {
        const options = model.options;
        const availableSlotHeight = Math.max(0, rect.height - (2 * options.dependencyEndpointRadius));
        const slotsPerLane = Math.max(
            1,
            Math.floor(availableSlotHeight / options.dependencyEndpointVerticalSpacing) + 1);
        const lane = Math.floor(index / slotsPerLane);
        const firstLaneIndex = lane * slotsPerLane;
        const slot = index - firstLaneIndex;
        const laneItemCount = Math.min(slotsPerLane, count - firstLaneIndex);
        const attachmentX = isSource ? rect.x + rect.width : rect.x;
        const direction = isSource ? 1 : -1;
        return {
            attachmentX,
            handleX: attachmentX + direction *
                (options.dependencyEndpointEdgeOffset + (lane * options.dependencyEndpointLaneSpacing)),
            y: rect.y + (rect.height / 2) +
                (slot - ((laneItemCount - 1) / 2)) * options.dependencyEndpointVerticalSpacing
        };
    }

    function drawResizeHandle(context, x, y, height, isStart, colors) {
        const direction = isStart ? 1 : -1;
        context.fillStyle = colors.handle;
        context.strokeStyle = "rgba(20, 67, 36, 0.9)";
        context.lineWidth = 1;
        context.beginPath();
        context.moveTo(x, y + 2);
        context.lineTo(x + (direction * 11), y + 8);
        context.lineTo(x, y + 14);
        context.closePath();
        context.fill();
        context.stroke();
    }

    function assignmentGlyph(kind) {
        switch (kind.toLowerCase()) {
            case "process": return "P";
            case "workflow": return "W";
            case "agent": return "A";
            case "person": return "@";
            default: return "•";
        }
    }

    function drawTasks(context, state, model, colors, originX, registerHits) {
        context.textBaseline = "middle";
        for (let index = 0; index < model.tasks.length; index += 1) {
            const canonicalTask = model.tasks[index];
            const task = state.previewTasks.get(canonicalTask.id) || canonicalTask;
            const rect = taskRect(model, task, index, originX);
            const fill = task.accentColor || colors.accent;

            context.fillStyle = fill;
            fillRoundedRect(context, rect.x, rect.y, rect.width, rect.height, 6);
            context.strokeStyle = task.isCritical ? colors.critical : "rgba(15, 23, 42, 0.72)";
            context.lineWidth = task.isCritical ? 2.5 : 1.2;
            if (task.isProjectionOnly) {
                context.setLineDash([5, 4]);
            }
            strokeRoundedRect(context, rect.x, rect.y, rect.width, rect.height, 6);
            context.setLineDash([]);

            if (registerHits) {
                addHit(state.hitRegistry, rect, { kind: HitKind.TaskBody, task });
            }

            const assignmentSlots = Math.max(1, Math.min(3, Math.floor(Math.max(0, rect.width - 20) / 19)));
            const renderedAssignments = task.assignments.slice(0, assignmentSlots);
            if (task.assignments.length > assignmentSlots) {
                const hiddenAssignments = task.assignments.slice(assignmentSlots - 1);
                renderedAssignments[assignmentSlots - 1] = {
                    id: `${task.id}:assignment-summary`,
                    label: hiddenAssignments.map(assignment => `${assignment.kind}: ${assignment.label}`).join(" · "),
                    kind: `${hiddenAssignments.length} assignments`,
                    glyph: `+${hiddenAssignments.length}`
                };
            }
            const assignmentCount = renderedAssignments.length;
            const assignmentWidth = assignmentCount * 19;
            context.font = '700 12px "Segoe UI", sans-serif';
            context.fillStyle = "#ffffff";
            const title = fitText(context, task.title, Math.max(0, rect.width - assignmentWidth - 28));
            context.fillText(title, rect.x + 14, rect.y + rect.height / 2);

            for (let assignmentIndex = 0; assignmentIndex < assignmentCount; assignmentIndex += 1) {
                const assignment = renderedAssignments[assignmentIndex];
                const centerX = rect.width < 40
                    ? rect.x + rect.width / 2
                    : rect.x + rect.width - 24 - (assignmentIndex * 19);
                const centerY = rect.y + rect.height / 2;
                context.fillStyle = "rgba(255, 255, 255, 0.2)";
                context.beginPath();
                context.arc(centerX, centerY, 7.5, 0, Math.PI * 2);
                context.fill();
                context.fillStyle = "#ffffff";
                context.font = '800 8px "Segoe UI", sans-serif';
                context.textAlign = "center";
                context.fillText(assignment.glyph || assignmentGlyph(assignment.kind), centerX, centerY + 0.5);
                context.textAlign = "left";
                if (registerHits) {
                    addHit(state.hitRegistry, { x: centerX - 9, y: centerY - 9, width: 18, height: 18 }, {
                        kind: HitKind.Assignment,
                        task,
                        assignment,
                        bounds: { x: centerX - 9, y: centerY - 9, width: 18, height: 18 }
                    });
                }
            }

            const portY = rect.y + rect.height - 2;
            if (model.options.allowDependencyEditing && !task.isDependencyReadOnly) {
                context.fillStyle = colors.connector;
                context.strokeStyle = "rgba(3, 78, 112, 0.92)";
                context.lineWidth = 1.4;
                for (const portX of [rect.x + 7, rect.x + rect.width - 7]) {
                    context.beginPath();
                    context.arc(portX, portY, 5.5, 0, Math.PI * 2);
                    context.fill();
                    context.stroke();
                }
            }

            if (!task.isScheduleReadOnly && model.options.allowTaskEditing) {
                drawResizeHandle(context, rect.x, rect.y, rect.height, true, colors);
                drawResizeHandle(context, rect.x + rect.width, rect.y, rect.height, false, colors);
            }

            if (registerHits) {
                if (!task.isScheduleReadOnly && model.options.allowTaskEditing) {
                    addHit(state.hitRegistry, { x: rect.x - 4, y: rect.y, width: 15, height: rect.height }, { kind: HitKind.ResizeStart, task });
                    addHit(state.hitRegistry, { x: rect.x + rect.width - 11, y: rect.y, width: 15, height: rect.height }, { kind: HitKind.ResizeEnd, task });
                }
                if (model.options.allowDependencyEditing && !task.isDependencyReadOnly) {
                    addHit(state.hitRegistry, { x: rect.x - 3, y: portY - 9, width: 20, height: 18 }, { kind: HitKind.InputPort, task });
                    addHit(state.hitRegistry, { x: rect.x + rect.width - 17, y: portY - 9, width: 20, height: 18 }, { kind: HitKind.OutputPort, task });
                }
            }
        }
    }

    function drawAssignmentPopover(context, state, colors) {
        const hit = state.hoverHit;
        if (hit?.kind !== HitKind.Assignment) {
            return;
        }

        const padding = 10;
        context.font = '700 12px "Segoe UI", sans-serif';
        const labelWidth = context.measureText(hit.assignment.label).width;
        context.font = '600 10px "Segoe UI", sans-serif';
        const kindLabel = hit.assignment.kind;
        const width = Math.min(300, Math.max(150, labelWidth + padding * 2));
        const height = 52;
        const x = Math.min(state.model.options.timelineWidth - width - 8, hit.bounds.x + 14);
        const y = Math.max(8, hit.bounds.y - height - 8);

        context.fillStyle = "rgba(15, 23, 42, 0.96)";
        fillRoundedRect(context, x, y, width, height, 8);
        context.fillStyle = "#ffffff";
        context.font = '700 12px "Segoe UI", sans-serif';
        context.fillText(fitText(context, hit.assignment.label, width - padding * 2), x + padding, y + 19);
        context.fillStyle = "#b8c5d8";
        context.font = '600 10px "Segoe UI", sans-serif';
        context.fillText(kindLabel, x + padding, y + 37);
    }

    function drawInteraction(context, state, colors) {
        const interaction = state.interaction;
        if (!interaction || !interaction.currentPoint || !interaction.originPoint) {
            return;
        }

        if (![InteractionKind.AddDependency, InteractionKind.ReconnectSource, InteractionKind.ReconnectTarget].includes(interaction.kind)) {
            return;
        }

        context.strokeStyle = colors.connector;
        context.lineWidth = 2;
        context.setLineDash([6, 4]);
        context.beginPath();
        context.moveTo(interaction.originPoint.x, interaction.originPoint.y);
        context.lineTo(interaction.currentPoint.x, interaction.currentPoint.y);
        context.stroke();
        context.setLineDash([]);
    }

    function drawTableForExport(context, state, model, colors) {
        const width = model.options.taskTableWidth;
        context.fillStyle = colors.surface;
        context.fillRect(0, 0, width, model.options.canvasHeight);
        context.fillStyle = colors.surfaceMuted;
        context.fillRect(0, 0, width, model.options.headerHeight);
        context.strokeStyle = colors.border;
        context.strokeRect(0.5, 0.5, width - 1, model.options.canvasHeight - 1);

        const titleWidth = width * 0.42;
        const startWidth = width * 0.22;
        const endWidth = width * 0.22;
        const durationX = titleWidth + startWidth + endWidth;
        const columns = [titleWidth, titleWidth + startWidth, durationX];
        for (const x of columns) {
            context.beginPath();
            context.moveTo(x + 0.5, 0);
            context.lineTo(x + 0.5, model.options.canvasHeight);
            context.stroke();
        }

        context.font = '800 10px "Segoe UI", sans-serif';
        context.fillStyle = colors.textMuted;
        context.fillText("TASK", 10, model.options.headerHeight / 2);
        context.fillText("START UTC", titleWidth + 10, model.options.headerHeight / 2);
        context.fillText("END UTC", titleWidth + startWidth + 10, model.options.headerHeight / 2);
        context.fillText("H / MD", durationX + 10, model.options.headerHeight / 2);

        context.font = '600 11px "Segoe UI", sans-serif';
        for (let index = 0; index < model.tasks.length; index += 1) {
            const task = state.previewTasks.get(model.tasks[index].id) || model.tasks[index];
            const y = model.options.headerHeight + index * model.options.rowHeight;
            const centerY = y + model.options.rowHeight / 2;
            context.strokeStyle = colors.grid;
            context.beginPath();
            context.moveTo(0, y + model.options.rowHeight + 0.5);
            context.lineTo(width, y + model.options.rowHeight + 0.5);
            context.stroke();
            context.fillStyle = colors.text;
            context.fillText(fitText(context, task.title, titleWidth - 20), 10, centerY);
            context.fillStyle = colors.textMuted;
            context.fillText(formatUtcDateTime(task.startMs), titleWidth + 10, centerY);
            context.fillText(formatUtcDateTime(task.endMs), titleWidth + startWidth + 10, centerY);
            const hours = (task.endMs - task.startMs) / hourMs;
            const manDays = hours / model.options.hoursPerManDay;
            context.fillText(`${hours.toFixed(hours % 1 === 0 ? 0 : 1)} / ${manDays.toFixed(manDays % 1 === 0 ? 0 : 1)}`, durationX + 10, centerY);
        }
    }

    function renderState(state) {
        if (state.disposed) {
            return;
        }

        const context = state.surface.context;
        const model = state.model;
        const dependencyGeometry = getDependencyGeometry(state, model, 0);
        state.hitRegistry.clear();
        drawGrid(context, model, state.colors, 0, model.options.timelineWidth);
        drawDependencies(context, state, model, state.colors, dependencyGeometry);
        drawTasks(context, state, model, state.colors, 0, true);
        registerDependencyEndpointHits(state, model, dependencyGeometry);
        drawInteraction(context, state, state.colors);
        drawAssignmentPopover(context, state, state.colors);
    }

    function registerDependencyEndpointHits(state, model, geometries) {
        if (!model.options.allowDependencyEditing) {
            return;
        }

        for (const geometry of geometries) {
            const { dependency } = geometry;
            const predecessor = model.taskLookup.get(dependency.predecessorId);
            const successor = model.taskLookup.get(dependency.successorId);
            if (predecessor.isDependencyReadOnly || successor.isDependencyReadOnly) {
                continue;
            }

            const hitSize = model.options.dependencyEndpointHitSize;
            const hitOffset = hitSize / 2;
            addHit(state.hitRegistry, {
                x: geometry.sourceHandleX - hitOffset,
                y: geometry.sourceY - hitOffset,
                width: hitSize,
                height: hitSize
            }, {
                kind: HitKind.DependencySource,
                dependency
            });
            addHit(state.hitRegistry, {
                x: geometry.targetHandleX - hitOffset,
                y: geometry.targetY - hitOffset,
                width: hitSize,
                height: hitSize
            }, {
                kind: HitKind.DependencyTarget,
                dependency
            });
        }
    }

    function resolveDependencyBridgeAtPoint(state, point) {
        let closest = null;
        for (const geometry of getDependencyGeometry(state, state.model, 0)) {
            const predecessor = state.model.taskLookup.get(geometry.dependency.predecessorId);
            const successor = state.model.taskLookup.get(geometry.dependency.successorId);
            if (predecessor.isDependencyReadOnly || successor.isDependencyReadOnly) {
                continue;
            }

            const distance = Math.min(...geometry.routePoints
                .slice(1)
                .map((routePoint, pointIndex) => {
                    const previousPoint = geometry.routePoints[pointIndex];
                    return distanceToSegment(
                        point,
                        previousPoint.x,
                        previousPoint.y,
                        routePoint.x,
                        routePoint.y);
                }));
            if (distance <= 7 && (!closest || distance < closest.distance)) {
                closest = { dependency: geometry.dependency, distance };
            }
        }

        return closest?.dependency || null;
    }

    function distanceToSegment(point, startX, startY, endX, endY) {
        const deltaX = endX - startX;
        const deltaY = endY - startY;
        if (deltaX === 0 && deltaY === 0) {
            return Math.hypot(point.x - startX, point.y - startY);
        }

        const projection = Math.max(0, Math.min(1,
            ((point.x - startX) * deltaX + (point.y - startY) * deltaY) /
            (deltaX * deltaX + deltaY * deltaY)));
        return Math.hypot(
            point.x - (startX + projection * deltaX),
            point.y - (startY + projection * deltaY));
    }

    function beginTaskInteraction(state, hit, point) {
        if (state.commitInFlightToken !== null ||
            !state.model.options.allowTaskEditing ||
            hit.task.isScheduleReadOnly) {
            return false;
        }

        const kind = hit.kind === HitKind.ResizeStart
            ? InteractionKind.ResizeStart
            : hit.kind === HitKind.ResizeEnd
                ? InteractionKind.ResizeEnd
                : InteractionKind.Move;
        state.selectedTaskId = hit.task.id;
        state.interaction = {
            operationToken: ++state.nextOperationToken,
            kind,
            task: hit.task,
            originPoint: point,
            currentPoint: point,
            originalStartMs: hit.task.startMs,
            originalEndMs: hit.task.endMs,
            hasMoved: false
        };
        return true;
    }

    function beginDependencyInteraction(state, hit, point) {
        if (state.commitInFlightToken !== null || !state.model.options.allowDependencyEditing) {
            return false;
        }

        if (hit.kind === HitKind.OutputPort || hit.kind === HitKind.InputPort) {
            if (hit.task.isDependencyReadOnly) {
                return false;
            }

            state.interaction = {
                operationToken: ++state.nextOperationToken,
                kind: InteractionKind.AddDependency,
                startPortKind: hit.kind,
                startTaskId: hit.task.id,
                originPoint: point,
                currentPoint: point,
                hasMoved: false
            };
            return true;
        }

        if (hit.kind === HitKind.DependencySource || hit.kind === HitKind.DependencyTarget) {
            const predecessor = state.model.taskLookup.get(hit.dependency.predecessorId);
            const successor = state.model.taskLookup.get(hit.dependency.successorId);
            if (predecessor.isDependencyReadOnly || successor.isDependencyReadOnly) {
                return false;
            }

            state.interaction = {
                operationToken: ++state.nextOperationToken,
                kind: hit.kind === HitKind.DependencySource ? InteractionKind.ReconnectSource : InteractionKind.ReconnectTarget,
                dependency: hit.dependency,
                originPoint: point,
                currentPoint: point,
                hasMoved: false
            };
            return true;
        }

        return false;
    }

    function handlePointerDown(state, pointer, event = null) {
        if (state.renderErrorReported || state.commitInFlightToken !== null) {
            return false;
        }

        const hit = state.hitRegistry.find(pointer.x, pointer.y);
        state.hoverHit = hit;
        if (!hit) {
            state.selectedTaskId = null;
            state.interaction = {
                operationToken: ++state.nextOperationToken,
                kind: InteractionKind.Pan,
                originPoint: pointer,
                currentPoint: pointer,
                originClientX: Number.isFinite(event?.clientX) ? event.clientX : 0,
                originScrollLeft: state.viewport.scrollLeft,
                hasMoved: false
            };
            state.canvas.classList.add("cda-gantt__canvas--panning");
            updateCursor(state);
            return true;
        }

        let handled;
        if ([HitKind.TaskBody, HitKind.ResizeStart, HitKind.ResizeEnd].includes(hit.kind)) {
            handled = beginTaskInteraction(state, hit, pointer);
        }
        else {
            handled = beginDependencyInteraction(state, hit, pointer);
        }
        updateCursor(state);
        return handled;
    }

    function resolveRequiredStart(model, taskId) {
        let requiredStartMs = Number.NEGATIVE_INFINITY;
        for (const dependency of model.dependencies) {
            if (dependency.successorId !== taskId) {
                continue;
            }

            const predecessor = model.taskLookup.get(dependency.predecessorId);
            requiredStartMs = Math.max(requiredStartMs, predecessor.endMs);
        }

        return requiredStartMs;
    }

    function updateTaskPreview(state, interaction, point) {
        const deltaMs = xToTime(state.model, point.x) - xToTime(state.model, interaction.originPoint.x);
        const minimum = state.model.options.minimumTaskDurationMs;
        const requiredStartMs = resolveRequiredStart(state.model, interaction.task.id);
        let startMs = interaction.originalStartMs;
        let endMs = interaction.originalEndMs;
        if (interaction.kind === InteractionKind.Move) {
            startMs = Math.max(requiredStartMs, snapTime(state.model, startMs + deltaMs));
            endMs = startMs + (interaction.originalEndMs - interaction.originalStartMs);
        }
        else if (interaction.kind === InteractionKind.ResizeStart) {
            const latestStartMs = endMs - minimum;
            startMs = requiredStartMs > latestStartMs
                ? interaction.originalStartMs
                : Math.max(
                    requiredStartMs,
                    Math.min(latestStartMs, snapTime(state.model, startMs + deltaMs)));
        }
        else if (interaction.kind === InteractionKind.ResizeEnd) {
            endMs = Math.max(startMs + minimum, snapTime(state.model, endMs + deltaMs));
        }

        state.previewTasks.set(interaction.task.id, {
            ...interaction.task,
            startMs,
            endMs
        });
        state.previewOwnerToken = interaction.operationToken;
        invalidateDependencyGeometry(state);
    }

    function handlePointerMove(state, pointer, event = null) {
        if (state.renderErrorReported || state.commitInFlightToken !== null) {
            return;
        }

        if (!state.interaction) {
            const previousHover = state.hoverHit;
            const nextHover = state.hitRegistry.find(pointer.x, pointer.y);
            const changed = nextHover?.kind !== state.hoverHit?.kind ||
                nextHover?.assignment?.id !== state.hoverHit?.assignment?.id ||
                nextHover?.dependency?.id !== state.hoverHit?.dependency?.id;
            state.hoverHit = nextHover;
            updateCursor(state);
            if (changed) {
                if (hoverAffectsDrawing(previousHover) || hoverAffectsDrawing(nextHover)) {
                    state.surface.requestRender();
                }
            }
            return;
        }

        const interaction = state.interaction;
        interaction.currentPoint = pointer;
        interaction.hasMoved = interaction.hasMoved ||
            Math.hypot(pointer.x - interaction.originPoint.x, pointer.y - interaction.originPoint.y) >= minimumMovementPx;
        if (interaction.kind === InteractionKind.Pan) {
            const clientX = Number.isFinite(event?.clientX) ? event.clientX : interaction.originClientX;
            state.viewport.scrollLeft = interaction.originScrollLeft - (clientX - interaction.originClientX);
            return;
        }
        if ([InteractionKind.Move, InteractionKind.ResizeStart, InteractionKind.ResizeEnd].includes(interaction.kind)) {
            updateTaskPreview(state, interaction, pointer);
        }
        state.surface.requestRender();
    }

    async function commitTaskInteraction(state, interaction) {
        const preview = state.previewTasks.get(interaction.task.id);
        if (!preview ||
            !interaction.hasMoved ||
            (preview.startMs === interaction.originalStartMs &&
                preview.endMs === interaction.originalEndMs)) {
            return;
        }

        await state.dotNetRef.invokeMethodAsync(
            "CommitScheduleChangeAsync",
            interaction.task.id,
            preview.startMs,
            preview.endMs,
            interaction.kind);
    }

    function resolveDependencyDrop(state, interaction, point) {
        const hit = state.hitRegistry.find(point.x, point.y);
        if (!hit) {
            return null;
        }

        if (interaction.kind === InteractionKind.AddDependency) {
            if (interaction.startPortKind === HitKind.OutputPort && hit.kind === HitKind.InputPort) {
                return {
                    mutation: DependencyMutation.Add,
                    dependencyId: null,
                    predecessorId: interaction.startTaskId,
                    successorId: hit.task.id
                };
            }
            if (interaction.startPortKind === HitKind.InputPort && hit.kind === HitKind.OutputPort) {
                return {
                    mutation: DependencyMutation.Add,
                    dependencyId: null,
                    predecessorId: hit.task.id,
                    successorId: interaction.startTaskId
                };
            }
            return null;
        }

        if (interaction.kind === InteractionKind.ReconnectSource && hit.kind === HitKind.OutputPort) {
            return {
                mutation: DependencyMutation.Reconnect,
                dependencyId: interaction.dependency.id,
                predecessorId: hit.task.id,
                successorId: interaction.dependency.successorId
            };
        }

        if (interaction.kind === InteractionKind.ReconnectTarget && hit.kind === HitKind.InputPort) {
            return {
                mutation: DependencyMutation.Reconnect,
                dependencyId: interaction.dependency.id,
                predecessorId: interaction.dependency.predecessorId,
                successorId: hit.task.id
            };
        }

        return null;
    }

    async function commitDependencyInteraction(state, interaction, point) {
        const request = resolveDependencyDrop(state, interaction, point);
        if (!request || request.predecessorId === request.successorId) {
            return;
        }

        await state.dotNetRef.invokeMethodAsync(
            "CommitDependencyChangeAsync",
            request.mutation,
            request.dependencyId,
            request.predecessorId,
            request.successorId);
    }

    async function handlePointerUp(state, pointer) {
        const interaction = state.interaction;
        state.interaction = null;
        state.canvas.classList.remove("cda-gantt__canvas--panning");
        state.hoverHit = state.hitRegistry.find(pointer.x, pointer.y);
        updateCursor(state);
        if (state.renderErrorReported) {
            clearTaskPreview(state);
            return;
        }

        if (!interaction) {
            return;
        }

        if (state.commitInFlightToken !== null) {
            if (state.previewOwnerToken === interaction.operationToken) {
                clearTaskPreview(state);
            }
            return;
        }

        const commitToken = interaction.operationToken;
        state.commitInFlightToken = commitToken;
        const isTaskGesture = [InteractionKind.Move, InteractionKind.ResizeStart, InteractionKind.ResizeEnd]
            .includes(interaction.kind);

        try {
            await Promise.resolve();
            if (state.disposed || state.commitInFlightToken !== commitToken) {
                return;
            }

            if (isTaskGesture) {
                await commitTaskInteraction(state, interaction);
                if (!state.disposed &&
                    state.commitInFlightToken === commitToken &&
                    state.model.taskLookup.has(interaction.task.id)) {
                    await state.dotNetRef.invokeMethodAsync("NotifyTaskSelectedAsync", interaction.task.id);
                }
            }
            else if (interaction.kind !== InteractionKind.Pan) {
                await commitDependencyInteraction(state, interaction, pointer);
            }
        }
        catch (error) {
            await reportInteropError(state, error);
        }
        finally {
            if (state.previewOwnerToken === commitToken) {
                clearTaskPreview(state);
            }
            if (state.commitInFlightToken === commitToken) {
                state.commitInFlightToken = null;
            }
            if (!state.disposed) {
                state.surface.requestRender();
            }
        }
    }

    function cancelInteraction(state) {
        state.interaction = null;
        clearTaskPreview(state);
        state.canvas.classList.remove("cda-gantt__canvas--panning");
        updateCursor(state);
        state.surface.requestRender();
    }

    function reportInteropError(state, error) {
        const message = error instanceof Error ? error.message : String(error);
        return state.dotNetRef.invokeMethodAsync("ReportRuntimeErrorAsync", message).catch(() => {
            window.console.error("Gantt chart runtime error", error);
        });
    }

    function reportRenderError(state, error) {
        const message = error instanceof Error ? error.message : String(error);
        return state.dotNetRef.invokeMethodAsync("ReportRenderErrorAsync", message).catch(() => {
            window.console.error("Gantt chart render error", error);
        });
    }

    function handleDoubleClick(state, event) {
        if (state.renderErrorReported || state.commitInFlightToken !== null) {
            return;
        }

        const point = state.surface.pointFromEvent(event);
        const hit = state.hitRegistry.find(point.x, point.y);
        if (hit?.task && !hit.task.isTitleReadOnly) {
            state.dotNetRef.invokeMethodAsync("BeginTitleEditAsync", hit.task.id).catch(error => reportInteropError(state, error));
        }
    }

    function handleKeyDown(state, event) {
        if (state.renderErrorReported) {
            return;
        }

        if (event.key === "Escape") {
            cancelInteraction(state);
            return;
        }

        if (event.key === "Enter" && state.selectedTaskId) {
            event.preventDefault();
            state.dotNetRef.invokeMethodAsync("BeginTitleEditAsync", state.selectedTaskId).catch(error => reportInteropError(state, error));
        }
    }

    function handleDragOver(state, event) {
        if (state.renderErrorReported ||
            state.interaction !== null ||
            state.commitInFlightToken !== null ||
            !state.model.options.allowTaskInsertion ||
            !event.dataTransfer?.types.includes(state.model.options.dragDataFormat)) {
            return;
        }

        const point = state.surface.pointFromEvent(event);
        if (!resolveDependencyBridgeAtPoint(state, point)) {
            return;
        }

        event.preventDefault();
        event.dataTransfer.dropEffect = "copy";
    }

    async function commitInsertion(state, payload, dependencyId, commitToken) {
        try {
            await state.dotNetRef.invokeMethodAsync("CommitInsertionAsync", payload, dependencyId);
        }
        catch (error) {
            await reportInteropError(state, error);
        }
        finally {
            if (state.commitInFlightToken === commitToken) {
                state.commitInFlightToken = null;
            }
        }
    }

    function handleDrop(state, event) {
        if (state.renderErrorReported ||
            state.interaction !== null ||
            state.commitInFlightToken !== null ||
            !state.model.options.allowTaskInsertion) {
            return;
        }

        const payload = event.dataTransfer?.getData(state.model.options.dragDataFormat);
        if (!payload) {
            return;
        }

        event.preventDefault();
        const point = state.surface.pointFromEvent(event);
        const dependency = resolveDependencyBridgeAtPoint(state, point);
        if (!dependency) {
            return;
        }

        const commitToken = ++state.nextOperationToken;
        state.commitInFlightToken = commitToken;
        commitInsertion(state, payload, dependency.id, commitToken);
    }

    function attachDomEvents(state) {
        state.doubleClickHandler = event => handleDoubleClick(state, event);
        state.keyDownHandler = event => handleKeyDown(state, event);
        state.hoverMoveHandler = event => {
            if (!state.interaction) {
                handlePointerMove(state, state.surface.pointFromEvent(event), event);
            }
        };
        state.pointerLeaveHandler = () => {
            if (!state.interaction && state.hoverHit) {
                const redrawRequired = hoverAffectsDrawing(state.hoverHit);
                state.hoverHit = null;
                updateCursor(state);
                if (redrawRequired) {
                    state.surface.requestRender();
                }
            }
        };
        state.dragOverHandler = event => handleDragOver(state, event);
        state.dropHandler = event => handleDrop(state, event);
        state.canvas.addEventListener("dblclick", state.doubleClickHandler);
        state.canvas.addEventListener("keydown", state.keyDownHandler);
        state.canvas.addEventListener("pointermove", state.hoverMoveHandler);
        state.canvas.addEventListener("pointerleave", state.pointerLeaveHandler);
        state.canvas.addEventListener("dragover", state.dragOverHandler);
        state.canvas.addEventListener("drop", state.dropHandler);
    }

    function detachDomEvents(state) {
        state.canvas.removeEventListener("dblclick", state.doubleClickHandler);
        state.canvas.removeEventListener("keydown", state.keyDownHandler);
        state.canvas.removeEventListener("pointermove", state.hoverMoveHandler);
        state.canvas.removeEventListener("pointerleave", state.pointerLeaveHandler);
        state.canvas.removeEventListener("dragover", state.dragOverHandler);
        state.canvas.removeEventListener("drop", state.dropHandler);
    }

    function disposeState(state) {
        if (!state || state.disposed) {
            return;
        }

        state.disposed = true;
        state.canvas.classList.remove("cda-gantt__canvas--panning");
        state.canvas.style.removeProperty("cursor");
        try {
            detachDomEvents(state);
        }
        finally {
            try {
                state.pointerRouter.dispose();
            }
            finally {
                state.interaction = null;
                clearTaskPreview(state);
                state.commitInFlightToken = null;
                try {
                    state.surface.dispose();
                }
                finally {
                    chartStates.delete(state.host);
                }
            }
        }
    }

    function resolveState(host) {
        const state = chartStates.get(host);
        if (!state || state.disposed) {
            throw new Error("The Gantt chart runtime is not initialized for this host.");
        }

        return state;
    }

    root.ganttChart = {
        create(hostValue, canvasValue, dotNetRef, modelValue) {
            const runtime = requireCanvasRuntime();
            const host = requireElement(hostValue, "host");
            const canvas = requireCanvas(canvasValue);
            const viewport = requireElement(host.querySelector("[data-gantt-viewport]"), "viewport");
            if (!dotNetRef) {
                throw new Error("Gantt chart .NET callback reference is required.");
            }

            const existing = chartStates.get(host);
            if (existing) {
                disposeState(existing);
            }

            const model = normalizeModel(modelValue);
            canvas.style.width = `${model.options.timelineWidth}px`;
            canvas.style.height = `${model.options.canvasHeight}px`;
            const state = {
                host,
                canvas,
                viewport,
                dotNetRef,
                model,
                colors: resolveColors(host),
                previewTasks: new Map(),
                previewOwnerToken: null,
                dependencyGeometry: null,
                hitRegistry: runtime.createHitRegionRegistry(),
                interaction: null,
                nextOperationToken: 0,
                commitInFlightToken: null,
                hoverHit: null,
                cursor: null,
                renderErrorReported: false,
                selectedTaskId: null,
                disposed: false
            };
            state.surface = createSurface(runtime, state);
            state.pointerRouter = runtime.createPointerRouter({
                element: canvas,
                coordinateElement: canvas,
                onPointerDown: payload => handlePointerDown(state, payload.point, payload.event),
                onPointerMove: payload => handlePointerMove(state, payload.point, payload.event),
                onPointerUp: payload => {
                    handlePointerUp(state, payload.point).catch(error => reportInteropError(state, error));
                },
                onPointerCancel: () => cancelInteraction(state)
            });
            attachDomEvents(state);
            chartStates.set(host, state);
            updateCursor(state);
            state.surface.measure();
            state.surface.requestRender();
        },

        update(hostValue, modelValue) {
            const host = requireElement(hostValue, "host");
            const state = resolveState(host);
            state.model = normalizeModel(modelValue);
            state.colors = resolveColors(state.host);
            state.interaction = null;
            clearTaskPreview(state);
            state.hoverHit = null;
            updateCursor(state);
            state.canvas.style.width = `${state.model.options.timelineWidth}px`;
            state.canvas.style.height = `${state.model.options.canvasHeight}px`;
            const nextPixelRatioLimit = resolvePixelRatioLimit(
                state.model.options.timelineWidth,
                state.model.options.canvasHeight);
            if (nextPixelRatioLimit !== state.surfacePixelRatioLimit) {
                state.surface.dispose();
                state.surface = createSurface(root.canvasRuntime, state);
            }
            else {
                state.surface.measure();
            }
            state.surface.requestRender();
        },

        async exportPngDataUrl(hostValue, includeTaskTable) {
            const runtime = requireCanvasRuntime();
            const host = requireElement(hostValue, "host");
            const state = resolveState(host);
            const tableWidth = includeTaskTable && state.model.options.showTaskTable
                ? state.model.options.taskTableWidth
                : 0;
            const sourceWidth = tableWidth + state.model.options.timelineWidth;
            const sourceHeight = state.model.options.canvasHeight;
            const scale = Math.min(
                1,
                maximumExportDimension / sourceWidth,
                maximumExportDimension / sourceHeight,
                Math.sqrt(maximumExportPixels / (sourceWidth * sourceHeight)));
            const width = Math.max(1, Math.floor(sourceWidth * scale));
            const height = Math.max(1, Math.floor(sourceHeight * scale));
            return await runtime.renderToPngDataUrl({
                width,
                height,
                background: state.colors.surface,
                draw: context => {
                    context.save();
                    context.scale(scale, scale);
                    if (tableWidth > 0) {
                        drawTableForExport(context, state, state.model, state.colors);
                    }
                    drawGrid(context, state.model, state.colors, tableWidth, state.model.options.timelineWidth);
                    const dependencyGeometry = getDependencyGeometry(state, state.model, tableWidth);
                    drawDependencies(context, state, state.model, state.colors, dependencyGeometry);
                    drawTasks(context, state, state.model, state.colors, tableWidth, false);
                    context.restore();
                }
            });
        },

        async downloadPng(hostValue, fileName, includeTaskTable) {
            const runtime = requireCanvasRuntime();
            const dataUrl = await this.exportPngDataUrl(hostValue, includeTaskTable);
            runtime.downloadDataUrl(dataUrl, requireText(fileName, "export file name"));
        },

        registerDragSource(elementValue, payloadValue, dataFormat) {
            const element = requireElement(elementValue, "drag source");
            const format = requireText(dataFormat, "drag data format");
            const payload = typeof payloadValue === "string" ? payloadValue : JSON.stringify(payloadValue);
            if (!payload) {
                throw new Error("Gantt task drag payload is required.");
            }

            const existing = dragSources.get(element);
            existing?.dispose();
            element.draggable = true;
            const handler = event => {
                event.dataTransfer.setData(format, payload);
                event.dataTransfer.effectAllowed = "copy";
            };
            element.addEventListener("dragstart", handler);
            const registration = {
                dispose() {
                    element.removeEventListener("dragstart", handler);
                    element.draggable = false;
                    dragSources.delete(element);
                }
            };
            dragSources.set(element, registration);
            return registration;
        },

        dispose(hostValue) {
            const host = requireElement(hostValue, "host");
            disposeState(chartStates.get(host));
        }
    };

    if (typeof module === "object" && module.exports) {
        module.exports = Object.freeze({
            dependencyArrowLength,
            resolveObstacleSafeRoute,
            routeSegmentIntersectsTaskRectangle,
            assertDependencyRouteAvoidsUnrelatedTasks,
            resolveTickLabelInterval
        });
    }
})();
