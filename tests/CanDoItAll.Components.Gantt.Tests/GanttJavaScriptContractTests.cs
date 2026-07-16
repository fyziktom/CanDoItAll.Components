namespace CanDoItAll.Components.Gantt.Tests;

public sealed class GanttJavaScriptContractTests
{
    [Fact]
    public void Pointer_router_disposal_reuses_the_last_normalized_point()
    {
        var runtime = ReadSource(
            "src",
            "CanDoItAll.Components.CanvasLib",
            "wwwroot",
            "js",
            "runtime",
            "canvas-runtime.js");

        Assert.Contains(
            "this._cancelSession(this._session.lastEvent, \"dispose\", true, this._session.lastPoint);",
            runtime,
            StringComparison.Ordinal);
        Assert.Contains(
            "const point = storedPoint || normalizePointerPoint(this._coordinateElement, event);",
            runtime,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Gantt_mutation_commits_use_an_input_latch_and_preview_owner_token()
    {
        var runtime = ReadSource(
            "src",
            "CanDoItAll.Components.Gantt",
            "wwwroot",
            "js",
            "gantt-chart.js");

        Assert.Contains("state.commitInFlightToken !== null", runtime, StringComparison.Ordinal);
        Assert.Contains("state.commitInFlightToken = commitToken;", runtime, StringComparison.Ordinal);
        Assert.Contains("state.previewOwnerToken = interaction.operationToken;", runtime, StringComparison.Ordinal);
        Assert.Contains("state.previewOwnerToken === commitToken", runtime, StringComparison.Ordinal);
        Assert.Contains("state.commitInFlightToken === commitToken", runtime, StringComparison.Ordinal);
    }

    [Fact]
    public void Gantt_task_selection_is_deferred_until_pointer_up()
    {
        var runtime = ReadSource(
            "src",
            "CanDoItAll.Components.Gantt",
            "wwwroot",
            "js",
            "gantt-chart.js");
        var beginTaskInteraction = ReadSection(
            runtime,
            "function beginTaskInteraction",
            "function beginDependencyInteraction");
        var handlePointerUp = ReadSection(
            runtime,
            "async function handlePointerUp",
            "function cancelInteraction");

        Assert.DoesNotContain("invokeMethodAsync", beginTaskInteraction, StringComparison.Ordinal);
        Assert.DoesNotContain("NotifyTaskSelectedAsync", beginTaskInteraction, StringComparison.Ordinal);
        Assert.Contains("await Promise.resolve();", handlePointerUp, StringComparison.Ordinal);
        Assert.Contains("await commitTaskInteraction(state, interaction);", handlePointerUp, StringComparison.Ordinal);
        Assert.Contains("NotifyTaskSelectedAsync", handlePointerUp, StringComparison.Ordinal);
        Assert.Contains("if (isTaskGesture && !interaction.hasMoved)", handlePointerUp, StringComparison.Ordinal);
        Assert.True(
            handlePointerUp.IndexOf("if (isTaskGesture && !interaction.hasMoved)", StringComparison.Ordinal) <
            handlePointerUp.IndexOf("state.commitInFlightToken = commitToken;", StringComparison.Ordinal),
            "A stationary task click must not take the mutation latch and block the browser dblclick event.");
    }

    [Fact]
    public void Gantt_task_preview_respects_finish_to_start_boundaries()
    {
        var runtime = ReadSource(
            "src",
            "CanDoItAll.Components.Gantt",
            "wwwroot",
            "js",
            "gantt-chart.js");
        var requiredStart = ReadSection(
            runtime,
            "function resolveRequiredStart",
            "function updateTaskPreview");
        var updateTaskPreview = ReadSection(
            runtime,
            "function updateTaskPreview",
            "function handlePointerMove");

        Assert.Contains("dependency.successorId !== taskId", requiredStart, StringComparison.Ordinal);
        Assert.Contains("predecessor.endMs", requiredStart, StringComparison.Ordinal);
        Assert.Contains("Math.max(requiredStartMs, snapTime", updateTaskPreview, StringComparison.Ordinal);
        Assert.Contains("requiredStartMs,", updateTaskPreview, StringComparison.Ordinal);
        Assert.Contains("requiredStartMs > latestStartMs", updateTaskPreview, StringComparison.Ordinal);
        Assert.Contains("? interaction.originalStartMs", updateTaskPreview, StringComparison.Ordinal);
    }

    [Fact]
    public void Gantt_task_commit_ignores_a_clamped_no_op()
    {
        var runtime = ReadSource(
            "src",
            "CanDoItAll.Components.Gantt",
            "wwwroot",
            "js",
            "gantt-chart.js");
        var commitTaskInteraction = ReadSection(
            runtime,
            "async function commitTaskInteraction",
            "function resolveDependencyDrop");

        Assert.Contains("preview.startMs === interaction.originalStartMs", commitTaskInteraction, StringComparison.Ordinal);
        Assert.Contains("preview.endMs === interaction.originalEndMs", commitTaskInteraction, StringComparison.Ordinal);
    }

    [Fact]
    public void Gantt_task_commit_retains_its_preview_until_the_controlled_model_reconciles()
    {
        var runtime = ReadSource(
            "src",
            "CanDoItAll.Components.Gantt",
            "wwwroot",
            "js",
            "gantt-chart.js");
        var commitTaskInteraction = ReadSection(
            runtime,
            "async function commitTaskInteraction",
            "function resolveDependencyDrop");
        var handlePointerUp = ReadSection(
            runtime,
            "async function handlePointerUp",
            "function cancelInteraction");
        var update = ReadSection(
            runtime,
            "update(hostValue, modelValue)",
            "async exportPngDataUrl");

        Assert.Contains("return false;", commitTaskInteraction, StringComparison.Ordinal);
        Assert.Contains("return true;", commitTaskInteraction, StringComparison.Ordinal);
        Assert.Contains("retainTaskPreview = await commitTaskInteraction", handlePointerUp, StringComparison.Ordinal);
        Assert.Contains("state.previewOwnerToken === commitToken && !retainTaskPreview", handlePointerUp, StringComparison.Ordinal);
        Assert.Contains("clearTaskPreview(state);", update, StringComparison.Ordinal);
    }

    [Fact]
    public void Gantt_dependency_endpoints_use_bounded_multi_lane_geometry()
    {
        var runtime = ReadSource(
            "src",
            "CanDoItAll.Components.Gantt",
            "wwwroot",
            "js",
            "gantt-chart.js");

        Assert.Contains("options.dependencyEndpointVerticalSpacing", runtime, StringComparison.Ordinal);
        Assert.Contains("lane * options.dependencyEndpointLaneSpacing", runtime, StringComparison.Ordinal);
        Assert.Contains("source.attachmentX", runtime, StringComparison.Ordinal);
        Assert.Contains("target.attachmentX", runtime, StringComparison.Ordinal);
        Assert.Contains("state.hoverHit?.kind === kind", runtime, StringComparison.Ordinal);
        Assert.Contains("model.options.timelineGutter", runtime, StringComparison.Ordinal);
        Assert.Contains("calculateRequiredDependencyGutter", runtime, StringComparison.Ordinal);
        Assert.DoesNotContain("32 / (count - 1)", runtime, StringComparison.Ordinal);
        Assert.DoesNotContain("spreadDependencyEndpoint", runtime, StringComparison.Ordinal);
    }

    [Fact]
    public void Gantt_dependency_routes_enforce_obstacle_clearance_and_share_final_segments_with_bridge_hits()
    {
        var runtime = ReadSource(
            "src",
            "CanDoItAll.Components.Gantt",
            "wwwroot",
            "js",
            "gantt-chart.js");
        var geometry = ReadSection(
            runtime,
            "function resolveDependencyGeometry",
            "function resolveDependencyEndpoint");
        var validator = ReadSection(
            runtime,
            "function assertDependencyRouteAvoidsUnrelatedTasks",
            "function routeSegmentIntersectsTaskRectangle");
        var bridgeHitTest = ReadSection(
            runtime,
            "function resolveDependencyBridgeAtPoint",
            "function distanceToSegment");

        Assert.Contains("resolveObstacleSafeRoute", geometry, StringComparison.Ordinal);
        Assert.Contains("assertDependencyRouteAvoidsUnrelatedTasks", geometry, StringComparison.Ordinal);
        Assert.Contains("for (const rect of taskRects)", validator, StringComparison.Ordinal);
        Assert.Contains("rect.taskId === predecessorId || rect.taskId === successorId", validator, StringComparison.Ordinal);
        Assert.Contains("pointIndex < geometry.routePoints.length", validator, StringComparison.Ordinal);
        Assert.Contains("routeSegmentIntersectsTaskRectangle", validator, StringComparison.Ordinal);
        Assert.Contains("does not terminate at its successor edge", validator, StringComparison.Ordinal);
        Assert.Contains("geometry.routePoints", bridgeHitTest, StringComparison.Ordinal);
        Assert.DoesNotContain("resolveDependencyElbowX", bridgeHitTest, StringComparison.Ordinal);
    }

    [Fact]
    public void Gantt_empty_timeline_drag_pans_the_horizontal_scroll_owner()
    {
        var runtime = ReadSource(
            "src",
            "CanDoItAll.Components.Gantt",
            "wwwroot",
            "js",
            "gantt-chart.js");
        var pointerDown = ReadSection(
            runtime,
            "function handlePointerDown",
            "function resolveRequiredStart");
        var pointerMove = ReadSection(
            runtime,
            "function handlePointerMove",
            "async function commitTaskInteraction");

        Assert.Contains("kind: InteractionKind.Pan", pointerDown, StringComparison.Ordinal);
        Assert.Contains("originScrollLeft: state.viewport.scrollLeft", pointerDown, StringComparison.Ordinal);
        Assert.Contains("state.viewport.scrollLeft =", pointerMove, StringComparison.Ordinal);
        Assert.Contains("interaction.originClientX", pointerMove, StringComparison.Ordinal);
    }

    [Fact]
    public void Gantt_empty_timeline_double_click_ignores_action_hits_and_reports_a_snapped_row_datetime()
    {
        var runtime = ReadSource(
            "src",
            "CanDoItAll.Components.Gantt",
            "wwwroot",
            "js",
            "gantt-chart.js");
        var resolver = ReadSection(
            runtime,
            "function resolveTimelineDoubleClick",
            "function taskY");
        var titleDoubleClick = ReadSection(
            runtime,
            "function handleDoubleClick",
            "function handleTimelineTaskCreationDoubleClick");
        var creationDoubleClick = ReadSection(
            runtime,
            "function handleTimelineTaskCreationDoubleClick",
            "function handleKeyDown");

        Assert.Contains("point.y < options.headerHeight", resolver, StringComparison.Ordinal);
        Assert.Contains("point.y >= options.canvasHeight", resolver, StringComparison.Ordinal);
        Assert.Contains("rowTaskId: rowTask.id", resolver, StringComparison.Ordinal);
        Assert.Contains("snapTime(model, xToTime(model, point.x))", resolver, StringComparison.Ordinal);
        Assert.Contains("hit?.kind === HitKind.TaskBody", titleDoubleClick, StringComparison.Ordinal);
        Assert.Contains("NotifyTaskDoubleClickedAsync", titleDoubleClick, StringComparison.Ordinal);
        Assert.DoesNotContain("BeginTitleEditAsync", titleDoubleClick, StringComparison.Ordinal);
        Assert.Contains("!state.model.options.allowTimelineTaskCreation", creationDoubleClick, StringComparison.Ordinal);
        Assert.Contains("resolveTimelineTaskCreation(state, point)", creationDoubleClick, StringComparison.Ordinal);
        Assert.Contains("NotifyTimelineDoubleClickedAsync", creationDoubleClick, StringComparison.Ordinal);
        Assert.DoesNotContain("CommitScheduleChangeAsync", creationDoubleClick, StringComparison.Ordinal);
    }

    [Fact]
    public void Gantt_task_metrics_are_validated_and_rendered_independently_from_delivery_width()
    {
        var runtime = ReadSource(
            "src",
            "CanDoItAll.Components.Gantt",
            "wwwroot",
            "js",
            "gantt-chart.js");
        var normalization = ReadSection(
            runtime,
            "function normalizeTask",
            "function normalizeDependency");
        var metrics = ReadSection(
            runtime,
            "function drawTaskMetrics",
            "function drawTasks");
        var taskDrawing = ReadSection(
            runtime,
            "function drawTasks",
            "function drawAssignmentPopover");

        Assert.Contains("normalizeProgressPercent", normalization, StringComparison.Ordinal);
        Assert.Contains("normalizeExpectedEffort", normalization, StringComparison.Ordinal);
        Assert.Contains("task.progressPercent / 100", metrics, StringComparison.Ordinal);
        Assert.Contains("task.expectedEffortMs / deliveryDurationMs", metrics, StringComparison.Ordinal);
        Assert.Contains("Math.min(", metrics, StringComparison.Ordinal);
        Assert.Contains("colors.progressComplete", metrics, StringComparison.Ordinal);
        Assert.Contains("colors.progressRemaining", metrics, StringComparison.Ordinal);
        Assert.Contains("colors.effort", metrics, StringComparison.Ordinal);
        Assert.Contains("drawTaskMetrics(context, model, task, rect, colors);", taskDrawing, StringComparison.Ordinal);
    }

    [Fact]
    public void Gantt_timeline_task_creation_capability_controls_its_cursor_and_dom_listener()
    {
        var runtime = ReadSource(
            "src",
            "CanDoItAll.Components.Gantt",
            "wwwroot",
            "js",
            "gantt-chart.js");
        var cursor = ReadSection(
            runtime,
            "function resolveCursor",
            "function updateCursor");
        var listener = ReadSection(
            runtime,
            "function syncTimelineTaskCreationListener",
            "function attachDomEvents");

        Assert.Contains("resolveTimelineTaskCreation(state, state.hoverPoint)", cursor, StringComparison.Ordinal);
        Assert.Contains("return Cursor.Create", cursor, StringComparison.Ordinal);
        Assert.Contains("state.model.options.allowTimelineTaskCreation", listener, StringComparison.Ordinal);
        Assert.Contains("addEventListener(\"dblclick\", state.timelineTaskCreationDoubleClickHandler)", listener, StringComparison.Ordinal);
        Assert.Contains("removeEventListener(\"dblclick\", state.timelineTaskCreationDoubleClickHandler)", listener, StringComparison.Ordinal);
    }

    [Fact]
    public void Gantt_action_hits_publish_specific_cached_cursors()
    {
        var runtime = ReadSource(
            "src",
            "CanDoItAll.Components.Gantt",
            "wwwroot",
            "js",
            "gantt-chart.js");
        var cursorResolver = ReadSection(
            runtime,
            "function resolveCursor",
            "function updateCursor");
        var hoverHandler = ReadSection(
            runtime,
            "function handlePointerMove",
            "async function commitTaskInteraction");

        Assert.Contains("Cursor.ResizeHorizontal", cursorResolver, StringComparison.Ordinal);
        Assert.Contains("Cursor.Connect", cursorResolver, StringComparison.Ordinal);
        Assert.Contains("Cursor.Move", cursorResolver, StringComparison.Ordinal);
        Assert.Contains("Cursor.Help", cursorResolver, StringComparison.Ordinal);
        Assert.Contains("cursor === state.cursor", runtime, StringComparison.Ordinal);
        Assert.Contains("hoverAffectsDrawing(previousHover)", hoverHandler, StringComparison.Ordinal);
        Assert.Contains("updateCursor(state);", hoverHandler, StringComparison.Ordinal);
    }

    [Fact]
    public void Gantt_render_reuses_one_dependency_geometry_for_drawing_and_hits()
    {
        var runtime = ReadSource(
            "src",
            "CanDoItAll.Components.Gantt",
            "wwwroot",
            "js",
            "gantt-chart.js");
        var renderState = ReadSection(
            runtime,
            "function renderState(state)",
            "function registerDependencyEndpointHits");
        var routeResolver = ReadSection(
            runtime,
            "function resolveDirectDependencyLaneX",
            "function compactRoutePoints");

        Assert.Equal(1, CountOccurrences(renderState, "getDependencyGeometry("));
        Assert.Contains("drawDependencies(context, state, model, state.colors, dependencyGeometry);", renderState, StringComparison.Ordinal);
        Assert.Contains("registerDependencyEndpointHits(state, model, dependencyGeometry);", renderState, StringComparison.Ordinal);
        Assert.Contains("blockedIntervals.sort", routeResolver, StringComparison.Ordinal);
        Assert.Contains("mergedIntervals", routeResolver, StringComparison.Ordinal);
        Assert.DoesNotContain("isVerticalRouteLaneClear", routeResolver, StringComparison.Ordinal);
    }

    private static string ReadSource(params string[] segments)
        => File.ReadAllText(Path.Combine([FindRepoRoot(), .. segments]));

    private static string ReadSection(string source, string startMarker, string endMarker)
    {
        var start = source.IndexOf(startMarker, StringComparison.Ordinal);
        Assert.True(start >= 0, $"Could not find JavaScript section start '{startMarker}'.");
        var end = source.IndexOf(endMarker, start + startMarker.Length, StringComparison.Ordinal);
        Assert.True(end > start, $"Could not find JavaScript section end '{endMarker}'.");
        return source[start..end];
    }

    private static int CountOccurrences(string source, string value)
        => (source.Length - source.Replace(value, string.Empty, StringComparison.Ordinal).Length) / value.Length;

    private static string FindRepoRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "CanDoItAll.Components.slnx")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException("Could not locate the CanDoItAll.Components repository root.");
    }
}
