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
