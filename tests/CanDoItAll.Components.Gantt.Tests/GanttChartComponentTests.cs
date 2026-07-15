using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Bunit;

namespace CanDoItAll.Components.Gantt.Tests;

public sealed class GanttChartComponentTests
{
    private static readonly DateTimeOffset Start = new(2026, 7, 14, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Task_table_rows_share_the_canvas_row_contract_and_render_man_days()
    {
        using var context = CreateContext();
        var tasks = new[]
        {
            Task("analysis", "Analysis", 0, 16),
            Task("delivery", "Delivery", 16, 20)
        };

        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, tasks)
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.RowHeight, 54)
            .Add(component => component.HeaderHeight, 42)
            .Add(component => component.HoursPerManDay, 8));

        var taskRows = cut.FindAll(".cda-gantt__table-row")
            .Skip(1)
            .ToArray();

        Assert.Equal(tasks.Length, taskRows.Length);
        Assert.All(taskRows, row => Assert.Contains("height: 54px", row.GetAttribute("style"), StringComparison.Ordinal));
        Assert.Contains("height: 150px", cut.Find(".cda-gantt__content").GetAttribute("style"), StringComparison.Ordinal);
        Assert.Contains("16 h · 2 md", cut.Markup, StringComparison.Ordinal);
        Assert.Contains($"4 h · {0.5.ToString("0.##", CultureInfo.CurrentCulture)} md", cut.Markup, StringComparison.Ordinal);

        var model = GetCreateModel(context);
        var options = GetProperty(model, "Options");
        Assert.Equal(54, GetProperty<double>(options, "RowHeight"));
        Assert.Equal(42, GetProperty<double>(options, "HeaderHeight"));
        Assert.Equal(150, GetProperty<double>(options, "CanvasHeight"));
    }

    [Fact]
    public void Interop_create_and_update_preserve_assignment_kinds_and_task_flags()
    {
        using var context = CreateContext();
        var firstId = TaskId("design");
        var secondId = TaskId("build");
        var tasks = new[]
        {
            new GanttTask(
                firstId,
                "Design",
                Start,
                Start.AddHours(4),
                [
                    new GanttAssignment(GanttAssignmentKind.Process, "Discovery"),
                    new GanttAssignment(GanttAssignmentKind.Workflow, "Approval"),
                    new GanttAssignment(GanttAssignmentKind.Agent, "Planner"),
                    new GanttAssignment(GanttAssignmentKind.Person, "Alex")
                ]),
            new GanttTask(secondId, "Build", Start.AddHours(4), Start.AddHours(10))
        };
        var dependencies = new[]
        {
            new GanttDependency(new GanttDependencyId("design-build"), firstId, secondId)
        };

        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, tasks)
            .Add(component => component.Dependencies, dependencies)
            .Add(component => component.TaskReadOnlySelector, task => task.Id == firstId)
            .Add(component => component.ProjectionOnlySelector, task => task.Id == firstId)
            .Add(component => component.TaskAccentColorSelector, task => task.Id == firstId ? "#2457c5" : null)
            .Add(component => component.TaskScheduleChangeRequested, _ => { })
            .Add(component => component.DependencyMutationRequested, _ => { })
            .Add(component => component.TaskInsertionRequested, _ => { }));

        var createModel = GetCreateModel(context);
        var createTasks = GetItems(GetProperty(createModel, "Tasks"));
        var design = Assert.Single(createTasks, task => GetProperty<string>(task, "Id") == firstId.Value);
        var assignments = GetItems(GetProperty(design, "Assignments"));

        Assert.Equal(
            ["process", "workflow", "agent", "person"],
            assignments.Select(assignment => GetProperty<string>(assignment, "Kind")));
        Assert.True(GetProperty<bool>(design, "IsReadOnly"));
        Assert.True(GetProperty<bool>(design, "IsProjectionOnly"));
        Assert.True(GetProperty<bool>(design, "IsCritical"));
        Assert.Equal("#2457c5", GetProperty<string>(design, "AccentColor"));

        var assignmentIndicators = cut.FindAll(".cda-gantt__assignment-indicator");
        Assert.Equal(4, assignmentIndicators.Count);
        Assert.Equal(
            ["Process assignment", "Workflow assignment", "Agent assignment", "Person assignment"],
            assignmentIndicators.Select(indicator => indicator.GetAttribute("aria-label")));
        Assert.All(assignmentIndicators, indicator =>
        {
            Assert.Equal("0", indicator.GetAttribute("tabindex"));
            Assert.Equal("img", indicator.GetAttribute("role"));
            var tooltipId = Assert.IsType<string>(indicator.GetAttribute("aria-describedby"));
            Assert.Equal("tooltip", cut.Find($"#{tooltipId}").GetAttribute("role"));
        });

        var createOptions = GetProperty(createModel, "Options");
        Assert.True(GetProperty<bool>(createOptions, "AllowTaskEditing"));
        Assert.True(GetProperty<bool>(createOptions, "AllowDependencyEditing"));
        Assert.True(GetProperty<bool>(createOptions, "AllowTaskInsertion"));

        var updatedTasks = new[]
        {
            new GanttTask(firstId, "Design reviewed", Start, Start.AddHours(4), tasks[0].Assignments),
            tasks[1]
        };
        cut.SetParametersAndRender(parameters => parameters.Add(component => component.Tasks, updatedTasks));

        var updateInvocation = Assert.Single(
            context.JSInterop.Invocations,
            invocation => invocation.Identifier == "CanDoItAll.ganttChart.update");
        var updateModel = Assert.IsAssignableFrom<object>(updateInvocation.Arguments[1]);
        var updatedDesign = Assert.Single(
            GetItems(GetProperty(updateModel, "Tasks")),
            task => GetProperty<string>(task, "Id") == firstId.Value);

        Assert.Equal("Design reviewed", GetProperty<string>(updatedDesign, "Title"));
        Assert.Equal(
            ["process", "workflow", "agent", "person"],
            GetItems(GetProperty(updatedDesign, "Assignments"))
                .Select(assignment => GetProperty<string>(assignment, "Kind")));
        Assert.True(GetProperty<bool>(updatedDesign, "IsReadOnly"));
        Assert.True(GetProperty<bool>(updatedDesign, "IsProjectionOnly"));
        Assert.True(GetProperty<bool>(updatedDesign, "IsCritical"));
    }

    [Fact]
    public void Title_edit_emits_a_typed_request_without_mutating_the_controlled_task()
    {
        using var context = CreateContext();
        var task = Task("analysis", "Original title", 0, 8);
        GanttTaskTitleChangeRequest? requestedChange = null;
        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, new[] { task })
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.TaskTitleChangeRequested, request => requestedChange = request));

        cut.Find(".cda-gantt__task-button").Click();
        var editor = cut.Find(".cda-gantt__title-editor");
        editor.Input("Renamed task");
        Assert.DoesNotContain(
            context.JSInterop.Invocations,
            invocation => invocation.Identifier == "CanDoItAll.ganttChart.update");
        editor.Blur();

        Assert.NotNull(requestedChange);
        Assert.Equal(task.Id, requestedChange.TaskId);
        Assert.Equal("Original title", requestedChange.CurrentTitle);
        Assert.Equal("Renamed task", requestedChange.ProposedTitle);
        Assert.Equal("Original title", task.Title);
        Assert.Equal("Original title", cut.Find(".cda-gantt__task-button").TextContent.Trim());
        Assert.DoesNotContain("Renamed task", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void Parameter_change_that_revokes_title_permission_cancels_the_open_editor_without_a_callback()
    {
        using var context = CreateContext();
        var callbackCount = 0;
        var task = Task("analysis", "Analysis", 0, 8);
        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, new[] { task })
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.TaskTitleChangeRequested, _ => callbackCount++));

        cut.Find(".cda-gantt__task-button").Click();
        cut.Find(".cda-gantt__title-editor").Input("Renamed task");

        cut.SetParametersAndRender(parameters => parameters
            .Add(component => component.AllowTaskEditing, false));

        Assert.Equal(0, callbackCount);
        Assert.Empty(cut.FindAll(".cda-gantt__title-editor"));
        Assert.True(cut.Find(".cda-gantt__task-button").HasAttribute("disabled"));
        Assert.DoesNotContain("Renamed task", cut.Markup, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("resize-start", 160, 240, 90, 150)]
    [InlineData("resize-end", 0, 40, 20, 80)]
    public async Task Schedule_commit_passes_minimum_duration_to_the_planner(
        string gesture,
        int proposedStartMinutes,
        int proposedEndMinutes,
        int expectedStartMinutes,
        int expectedEndMinutes)
    {
        using var context = CreateContext();
        var selectedTask = new GanttTask(
            TaskId("selected"),
            "Selected",
            Start.AddMinutes(20),
            Start.AddMinutes(150));
        GanttTaskScheduleChangeRequest? callbackRequest = null;
        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, new[]
            {
                Task("anchor", "Anchor", 0, 1),
                selectedTask
            })
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.SnapInterval, TimeSpan.FromHours(1))
            .Add(component => component.MinimumTaskDuration, TimeSpan.FromHours(1))
            .Add(component => component.TaskScheduleChangeRequested, request => callbackRequest = request));

        await cut.Instance.CommitScheduleChangeAsync(
            selectedTask.Id.Value,
            Start.AddMinutes(proposedStartMinutes).ToUnixTimeMilliseconds(),
            Start.AddMinutes(proposedEndMinutes).ToUnixTimeMilliseconds(),
            gesture);

        Assert.NotNull(callbackRequest);
        var change = Assert.Single(callbackRequest.AffectedTasks, change => change.TaskId == selectedTask.Id);
        Assert.Equal(Start.AddMinutes(expectedStartMinutes), change.ProposedStart);
        Assert.Equal(Start.AddMinutes(expectedEndMinutes), change.ProposedEnd);
        Assert.Equal(TimeSpan.FromHours(1), change.ProposedEnd - change.ProposedStart);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    public async Task Direct_schedule_call_does_not_invoke_the_callback_when_permissions_block_it(
        bool allowTaskEditing,
        bool taskReadOnly,
        bool scheduleReadOnly)
    {
        using var context = CreateContext();
        var callbackCount = 0;
        var task = Task("analysis", "Analysis", 0, 8);
        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, new[] { task })
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.AllowTaskEditing, allowTaskEditing)
            .Add(component => component.TaskReadOnlySelector, candidate => taskReadOnly && candidate.Id == task.Id)
            .Add(component => component.TaskScheduleReadOnlySelector, candidate => scheduleReadOnly && candidate.Id == task.Id)
            .Add(component => component.TaskScheduleChangeRequested, _ => callbackCount++));

        await Assert.ThrowsAsync<InvalidOperationException>(() => cut.Instance.CommitScheduleChangeAsync(
            task.Id.Value,
            task.Start.AddHours(1).ToUnixTimeMilliseconds(),
            task.End.AddHours(1).ToUnixTimeMilliseconds(),
            "move"));

        Assert.Equal(0, callbackCount);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    public async Task Direct_dependency_call_does_not_invoke_the_callback_when_permissions_block_it(
        bool allowDependencyEditing,
        bool taskReadOnly,
        bool dependencyReadOnly)
    {
        using var context = CreateContext();
        var callbackCount = 0;
        var predecessor = Task("predecessor", "Predecessor", 0, 2);
        var successor = Task("successor", "Successor", 2, 4);
        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, new[] { predecessor, successor })
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.AllowDependencyEditing, allowDependencyEditing)
            .Add(component => component.TaskReadOnlySelector, task => taskReadOnly && task.Id == predecessor.Id)
            .Add(component => component.TaskDependencyReadOnlySelector, task => dependencyReadOnly && task.Id == predecessor.Id)
            .Add(component => component.DependencyMutationRequested, _ => callbackCount++));

        await Assert.ThrowsAsync<InvalidOperationException>(() => cut.Instance.CommitDependencyChangeAsync(
            "add",
            null,
            predecessor.Id.Value,
            successor.Id.Value));

        Assert.Equal(0, callbackCount);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    public async Task Direct_insertion_call_does_not_invoke_the_callback_when_permissions_block_it(
        bool allowTaskInsertion,
        bool taskReadOnly,
        bool dependencyReadOnly)
    {
        using var context = CreateContext();
        var callbackCount = 0;
        var predecessor = Task("predecessor", "Predecessor", 0, 2);
        var successor = Task("successor", "Successor", 2, 4);
        var bridge = new GanttDependency(
            new GanttDependencyId("predecessor-successor"),
            predecessor.Id,
            successor.Id);
        var payload = SerializeDragTask(Task("inserted", "Inserted", -2, 0));
        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, new[] { predecessor, successor })
            .Add(component => component.Dependencies, new[] { bridge })
            .Add(component => component.AllowTaskInsertion, allowTaskInsertion)
            .Add(component => component.TaskReadOnlySelector, task => taskReadOnly && task.Id == predecessor.Id)
            .Add(component => component.TaskDependencyReadOnlySelector, task => dependencyReadOnly && task.Id == predecessor.Id)
            .Add(component => component.TaskInsertionRequested, _ => callbackCount++));

        await Assert.ThrowsAsync<InvalidOperationException>(() => cut.Instance.CommitInsertionAsync(
            payload,
            bridge.Id.Value));

        Assert.Equal(0, callbackCount);
    }

    [Fact]
    public void Task_table_toggle_emits_the_requested_visibility()
    {
        using var context = CreateContext();
        bool? requestedVisibility = null;
        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, new[] { Task("analysis", "Analysis", 0, 8) })
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.ShowTaskTableChanged, visible => requestedVisibility = visible));

        cut.FindAll("button")
            .Single(button => button.TextContent.Contains("Hide task table", StringComparison.Ordinal))
            .Click();

        Assert.False(requestedVisibility);
        Assert.Empty(cut.FindAll(".cda-gantt__table"));
        Assert.Contains("Show task table", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void Invalid_dependency_cycle_renders_an_explicit_error_without_starting_interop()
    {
        using var context = CreateContext();
        var firstId = TaskId("first");
        var secondId = TaskId("second");
        var tasks = new[]
        {
            new GanttTask(firstId, "First", Start, Start.AddHours(2)),
            new GanttTask(secondId, "Second", Start.AddHours(2), Start.AddHours(4))
        };
        var dependencies = new[]
        {
            new GanttDependency(new GanttDependencyId("first-second"), firstId, secondId),
            new GanttDependency(new GanttDependencyId("second-first"), secondId, firstId)
        };

        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, tasks)
            .Add(component => component.Dependencies, dependencies));

        var alert = cut.Find(".cda-gantt__error");
        Assert.Contains("The chart model is invalid", alert.TextContent, StringComparison.Ordinal);
        Assert.Contains("contains a cycle", alert.TextContent, StringComparison.Ordinal);
        Assert.True(cut.Find(".cda-gantt__viewport").HasAttribute("hidden"));
        Assert.DoesNotContain(
            context.JSInterop.Invocations,
            invocation => invocation.Identifier == "CanDoItAll.ganttChart.create");
    }

    [Fact]
    public void Finish_to_start_violation_renders_an_explicit_error_without_starting_interop()
    {
        using var context = CreateContext();
        var predecessorId = TaskId("predecessor");
        var successorId = TaskId("successor");
        var tasks = new[]
        {
            new GanttTask(predecessorId, "Predecessor", Start, Start.AddHours(8)),
            new GanttTask(successorId, "Successor", Start.AddHours(4), Start.AddHours(12))
        };
        var dependencies = new[]
        {
            new GanttDependency(new GanttDependencyId("predecessor-successor"), predecessorId, successorId)
        };

        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, tasks)
            .Add(component => component.Dependencies, dependencies));

        var alert = cut.Find(".cda-gantt__error");
        Assert.Contains("The chart model is invalid", alert.TextContent, StringComparison.Ordinal);
        Assert.Contains("starts before its finish-to-start prerequisites complete", alert.TextContent, StringComparison.Ordinal);
        Assert.True(cut.Find(".cda-gantt__viewport").HasAttribute("hidden"));
        Assert.DoesNotContain(
            context.JSInterop.Invocations,
            invocation => invocation.Identifier == "CanDoItAll.ganttChart.create");
    }

    [Fact]
    public void Blank_title_keeps_the_editor_open_and_reports_the_validation_error_without_a_callback()
    {
        using var context = CreateContext();
        var callbackCount = 0;
        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, new[] { Task("analysis", "Analysis", 0, 8) })
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.TaskTitleChangeRequested, _ => callbackCount++));

        cut.Find(".cda-gantt__task-button").Click();
        var editor = cut.Find(".cda-gantt__title-editor");
        editor.Input("   ");
        editor.Blur();

        Assert.Equal(0, callbackCount);
        Assert.Single(cut.FindAll(".cda-gantt__title-editor"));
        Assert.Contains("A task title is required.", cut.Find(".cda-gantt__error").TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public void Dependency_read_only_selector_is_independent_and_task_read_only_cascades_to_every_interaction()
    {
        using var context = CreateContext();
        var dependencyReadOnlyId = TaskId("dependency-read-only");
        var taskReadOnlyId = TaskId("task-read-only");
        var tasks = new[]
        {
            new GanttTask(dependencyReadOnlyId, "Dependency read only", Start, Start.AddHours(4)),
            new GanttTask(taskReadOnlyId, "Task read only", Start.AddHours(4), Start.AddHours(8))
        };

        context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, tasks)
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.TaskDependencyReadOnlySelector, task => task.Id == dependencyReadOnlyId)
            .Add(component => component.TaskReadOnlySelector, task => task.Id == taskReadOnlyId));

        var interopTasks = GetItems(GetProperty(GetCreateModel(context), "Tasks"));
        var dependencyReadOnlyTask = Assert.Single(
            interopTasks,
            task => GetProperty<string>(task, "Id") == dependencyReadOnlyId.Value);
        var taskReadOnlyTask = Assert.Single(
            interopTasks,
            task => GetProperty<string>(task, "Id") == taskReadOnlyId.Value);

        Assert.False(GetProperty<bool>(dependencyReadOnlyTask, "IsReadOnly"));
        Assert.False(GetProperty<bool>(dependencyReadOnlyTask, "IsScheduleReadOnly"));
        Assert.False(GetProperty<bool>(dependencyReadOnlyTask, "IsTitleReadOnly"));
        Assert.True(GetProperty<bool>(dependencyReadOnlyTask, "IsDependencyReadOnly"));

        Assert.True(GetProperty<bool>(taskReadOnlyTask, "IsReadOnly"));
        Assert.True(GetProperty<bool>(taskReadOnlyTask, "IsScheduleReadOnly"));
        Assert.True(GetProperty<bool>(taskReadOnlyTask, "IsTitleReadOnly"));
        Assert.True(GetProperty<bool>(taskReadOnlyTask, "IsDependencyReadOnly"));
    }

    [Fact]
    public void Snap_origin_remains_stable_when_the_earliest_task_changes()
    {
        using var context = CreateContext();
        var snapOrigin = new DateTimeOffset(2026, 7, 1, 5, 0, 0, TimeSpan.Zero);
        var tasks = new[]
        {
            Task("first", "First", 0, 4),
            Task("second", "Second", 8, 12)
        };

        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, tasks)
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.SnapInterval, TimeSpan.FromHours(3))
            .Add(component => component.SnapOrigin, snapOrigin));

        var createOptions = GetProperty(GetCreateModel(context), "Options");
        var initialTimelineStart = GetProperty<long>(createOptions, "TimelineStartMs");

        cut.SetParametersAndRender(parameters => parameters.Add(
            component => component.Tasks,
            new[]
            {
                Task("first", "First", -5, 4),
                tasks[1]
            }));

        var updateInvocation = Assert.Single(
            context.JSInterop.Invocations,
            invocation => invocation.Identifier == "CanDoItAll.ganttChart.update");
        var updateModel = Assert.IsAssignableFrom<object>(updateInvocation.Arguments[1]);
        var updateOptions = GetProperty(updateModel, "Options");

        Assert.Equal(snapOrigin.ToUnixTimeMilliseconds(), GetProperty<long>(createOptions, "SnapOriginMs"));
        Assert.Equal(snapOrigin.ToUnixTimeMilliseconds(), GetProperty<long>(updateOptions, "SnapOriginMs"));
        Assert.NotEqual(initialTimelineStart, GetProperty<long>(updateOptions, "TimelineStartMs"));
    }

    [Fact]
    public void Eight_edge_fans_use_non_overlapping_geometry_and_a_fixed_pixel_gutter()
    {
        using var context = CreateContext();
        var hub = Task("hub", "Hub", 1, 2);
        var predecessors = Enumerable.Range(0, 8)
            .Select(index => Task($"predecessor-{index}", $"Predecessor {index}", 0, 1))
            .ToArray();
        var successors = Enumerable.Range(0, 8)
            .Select(index => Task($"successor-{index}", $"Successor {index}", 2, 3))
            .ToArray();
        var dependencies = predecessors
            .Select((task, index) => new GanttDependency(
                new GanttDependencyId($"incoming-{index}"),
                task.Id,
                hub.Id))
            .Concat(successors.Select((task, index) => new GanttDependency(
                new GanttDependencyId($"outgoing-{index}"),
                hub.Id,
                task.Id)))
            .ToArray();

        context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, predecessors.Append(hub).Concat(successors).ToArray())
            .Add(component => component.Dependencies, dependencies));

        var options = GetProperty(GetCreateModel(context), "Options");
        Assert.Equal(4.5, GetProperty<double>(options, "DependencyEndpointRadius"));
        Assert.Equal(10, GetProperty<double>(options, "DependencyEndpointHitSize"));
        Assert.Equal(11, GetProperty<double>(options, "DependencyEndpointVerticalSpacing"));
        Assert.Equal(12, GetProperty<double>(options, "DependencyEndpointLaneSpacing"));
        Assert.Equal(6, GetProperty<double>(options, "DependencyEndpointEdgeOffset"));
        Assert.Equal(48, GetProperty<double>(options, "TimelineGutter"));
        Assert.Equal(896, GetProperty<double>(options, "TimelineWidth"));
    }

    [Theory]
    [InlineData(GanttTimeScale.QuarterHour, 96, 900_000)]
    [InlineData(GanttTimeScale.Hour, 32, 3_600_000)]
    [InlineData(GanttTimeScale.Day, 4, 86_400_000)]
    [InlineData(GanttTimeScale.Week, 0.75, 604_800_000)]
    public void Typed_time_scale_presets_change_timeline_geometry_and_tick_interval(
        GanttTimeScale scale,
        double expectedPixelsPerHour,
        double expectedTickIntervalMs)
    {
        using var context = CreateContext();
        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, new[] { Task("schedule", "Schedule", 0, 48) })
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.TimeScale, scale));

        var options = GetProperty(GetCreateModel(context), "Options");
        Assert.Equal(expectedPixelsPerHour, GetProperty<double>(options, "PixelsPerHour"));
        Assert.Equal(expectedTickIntervalMs, GetProperty<double>(options, "TickIntervalMs"));
        Assert.True(cut.Find(".cda-gantt__viewport").HasAttribute("data-gantt-viewport"));
        Assert.Equal(
            ["0.25 h", "1 h", "1 d", "1 w", "Custom"],
            cut.FindAll(".cda-gantt__time-scale option").Select(option => option.TextContent.Trim()));
    }

    [Fact]
    public void Fine_time_scale_creates_real_horizontal_overflow_without_changing_row_geometry()
    {
        using var context = CreateContext();
        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, new[]
            {
                Task("first", "First", 0, 8),
                Task("second", "Second", 8, 16)
            })
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.TimeScale, GanttTimeScale.QuarterHour)
            .Add(component => component.RowHeight, 52)
            .Add(component => component.HeaderHeight, 42));

        var options = GetProperty(GetCreateModel(context), "Options");
        Assert.True(GetProperty<double>(options, "TimelineWidth") > 3_000);
        Assert.Equal(146, GetProperty<double>(options, "CanvasHeight"));
        Assert.All(
            cut.FindAll(".cda-gantt__table-row").Skip(1),
            row => Assert.Contains("height: 52px", row.GetAttribute("style"), StringComparison.Ordinal));
    }

    [Fact]
    public void Dense_schedule_stays_valid_and_exposes_internal_two_axis_overflow_geometry()
    {
        using var context = CreateContext();
        var tasks = Enumerable.Range(0, 64)
            .Select(index => Task($"dense-{index}", $"Dense task {index + 1}", index * 2, (index * 2) + 2))
            .ToArray();
        var dependencies = tasks
            .Skip(1)
            .Select((task, index) => new GanttDependency(
                new GanttDependencyId($"dense-edge-{index}"),
                tasks[index].Id,
                task.Id))
            .ToArray();

        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, tasks)
            .Add(component => component.Dependencies, dependencies)
            .Add(component => component.TimeScale, GanttTimeScale.Hour)
            .Add(component => component.RowHeight, 52)
            .Add(component => component.HeaderHeight, 42)
            .Add(component => component.MaxHeight, "42rem"));

        var options = GetProperty(GetCreateModel(context), "Options");
        Assert.Equal(3_370, GetProperty<double>(options, "CanvasHeight"));
        Assert.True(GetProperty<double>(options, "TimelineWidth") > 4_000);
        Assert.DoesNotContain("chart model is invalid", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--cda-gantt-max-height: 42rem", cut.Find(".cda-gantt").GetAttribute("style"), StringComparison.Ordinal);
    }

    [Fact]
    public void Time_scale_selection_and_zoom_raise_typed_controlled_view_state()
    {
        using var context = CreateContext();
        GanttTimeScale? requestedScale = null;
        double? requestedPixelsPerHour = null;
        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, new[] { Task("schedule", "Schedule", 0, 24) })
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.TimeScaleChanged, scale => requestedScale = scale)
            .Add(component => component.PixelsPerHourChanged, value => requestedPixelsPerHour = value));

        cut.Find(".cda-gantt__time-scale").Change("1");

        Assert.Equal(GanttTimeScale.Hour, requestedScale);
        var scaleUpdate = context.JSInterop.Invocations.Last(
            invocation => invocation.Identifier == "CanDoItAll.ganttChart.update");
        Assert.Equal(
            32,
            GetProperty<double>(GetProperty(Assert.IsAssignableFrom<object>(scaleUpdate.Arguments[1]), "Options"), "PixelsPerHour"));

        cut.FindAll("button")
            .Single(button => button.TextContent.Contains("Zoom in", StringComparison.Ordinal))
            .Click();

        Assert.Equal(GanttTimeScale.Custom, requestedScale);
        Assert.NotNull(requestedPixelsPerHour);
        Assert.True(requestedPixelsPerHour > 32);
    }

    [Fact]
    public void Dependency_fan_that_cannot_fit_the_bounded_canvas_fails_explicitly()
    {
        using var context = CreateContext();
        var hub = Task("hub", "Hub", 0, 1);
        var successors = Enumerable.Range(0, 467)
            .Select(index => Task($"successor-{index}", $"Successor {index}", 1, 2))
            .ToArray();
        var dependencies = successors
            .Select((task, index) => new GanttDependency(
                new GanttDependencyId($"dependency-{index}"),
                hub.Id,
                task.Id))
            .ToArray();

        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, successors.Prepend(hub).ToArray())
            .Add(component => component.Dependencies, dependencies)
            .Add(component => component.RowHeight, 10)
            .Add(component => component.BarHeight, 9)
            .Add(component => component.ShowTaskTable, false)
            .Add(component => component.ShowToolbar, false));

        Assert.Contains("dependency fan requires a 5604 px gutter", cut.Find(".cda-gantt__error").TextContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(
            context.JSInterop.Invocations,
            invocation => invocation.Identifier == "CanDoItAll.ganttChart.create");
    }

    [Fact]
    public void Long_timeline_reports_fitted_scale_and_caps_the_interop_width()
    {
        using var context = CreateContext();
        var tasks = new[]
        {
            new GanttTask(TaskId("long-running"), "Long running", Start, Start.AddDays(365))
        };

        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, tasks)
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.PixelsPerHour, 96));

        Assert.Contains("fitted scale", cut.Find(".cda-gantt__status").TextContent, StringComparison.Ordinal);
        var options = GetProperty(GetCreateModel(context), "Options");
        Assert.Equal(12_000, GetProperty<double>(options, "TimelineWidth"));
        Assert.True(GetProperty<double>(options, "PixelsPerHour") < 96);
    }

    [Fact]
    public void Fitted_timeline_allows_zooming_out_when_the_rendered_scale_can_change()
    {
        using var context = CreateContext();
        var tasks = new[]
        {
            new GanttTask(TaskId("long-running"), "Long running", Start, Start.AddDays(120))
        };

        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, tasks)
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>())
            .Add(component => component.PixelsPerHour, 12));

        var zoomOut = cut.FindAll("button")
            .Single(button => button.TextContent.Contains("Zoom out", StringComparison.Ordinal));
        var zoomIn = cut.FindAll("button")
            .Single(button => button.TextContent.Contains("Zoom in", StringComparison.Ordinal));

        Assert.Contains("fitted scale", cut.Find(".cda-gantt__status").TextContent, StringComparison.Ordinal);
        Assert.False(zoomOut.HasAttribute("disabled"));
        Assert.True(zoomIn.HasAttribute("disabled"));

        zoomOut.Click();

        Assert.DoesNotContain("fitted scale", cut.Find(".cda-gantt__status").TextContent, StringComparison.Ordinal);
        Assert.False(cut.FindAll("button")
            .Single(button => button.TextContent.Contains("Zoom in", StringComparison.Ordinal))
            .HasAttribute("disabled"));
        var update = context.JSInterop.Invocations.Last(
            invocation => invocation.Identifier == "CanDoItAll.ganttChart.update");
        var options = GetProperty(Assert.IsAssignableFrom<object>(update.Arguments[1]), "Options");
        Assert.True(GetProperty<double>(options, "TimelineWidth") < 12_000);
    }

    [Fact]
    public void Equal_duplicate_assignments_render_with_unique_interop_keys()
    {
        using var context = CreateContext();
        var task = new GanttTask(
            TaskId("paired-review"),
            "Paired review",
            Start,
            Start.AddHours(4),
            [
                new GanttAssignment(GanttAssignmentKind.Person, "Alex"),
                new GanttAssignment(GanttAssignmentKind.Person, "Alex")
            ]);

        var cut = context.RenderComponent<GanttChart>(parameters => parameters
            .Add(component => component.Tasks, new[] { task })
            .Add(component => component.Dependencies, Array.Empty<GanttDependency>()));

        Assert.Equal(2, cut.FindAll("ul[aria-label='Task assignments'] li").Count);
        var interopTask = Assert.Single(GetItems(GetProperty(GetCreateModel(context), "Tasks")));
        var assignments = GetItems(GetProperty(interopTask, "Assignments"));
        Assert.Equal(2, assignments.Count);
        Assert.Equal(2, assignments.Select(assignment => GetProperty<string>(assignment, "Id")).Distinct().Count());
    }

    [Fact]
    public void Drag_source_registers_the_typed_task_payload_and_data_format()
    {
        using var context = CreateContext();
        var task = new GanttTask(
            TaskId("queued-task"),
            "Queued task",
            Start,
            Start.AddHours(3),
            [new GanttAssignment(GanttAssignmentKind.Agent, "Planner")]);

        context.RenderComponent<GanttTaskDragSource>(parameters => parameters
            .Add(component => component.Task, task)
            .Add(component => component.DragDataFormat, "application/x-test-gantt-task"));

        var registration = Assert.Single(
            context.JSInterop.Invocations,
            invocation => invocation.Identifier == "CanDoItAll.ganttChart.registerDragSource");
        Assert.Equal("application/x-test-gantt-task", registration.Arguments[2]);

        using var payload = JsonDocument.Parse(Assert.IsType<string>(registration.Arguments[1]));
        Assert.Equal(task.Id.Value, payload.RootElement.GetProperty("id").GetString());
        Assert.Equal(task.Title, payload.RootElement.GetProperty("title").GetString());
        Assert.Single(payload.RootElement.GetProperty("assignments").EnumerateArray());
    }

    private static TestContext CreateContext()
    {
        var context = new TestContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    private static object GetCreateModel(TestContext context)
    {
        var invocation = Assert.Single(
            context.JSInterop.Invocations,
            candidate => candidate.Identifier == "CanDoItAll.ganttChart.create");
        return Assert.IsAssignableFrom<object>(invocation.Arguments[3]);
    }

    private static object GetProperty(object target, string propertyName)
        => target.GetType()
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)!
            .GetValue(target)!;

    private static T GetProperty<T>(object target, string propertyName)
        => Assert.IsType<T>(GetProperty(target, propertyName));

    private static IReadOnlyList<object> GetItems(object collection)
        => Assert.IsAssignableFrom<IEnumerable>(collection)
            .Cast<object>()
            .ToArray();

    private static string SerializeDragTask(GanttTask task)
        => JsonSerializer.Serialize(
            new
            {
                Id = task.Id.Value,
                task.Title,
                task.Start,
                task.End,
                Assignments = Array.Empty<object>()
            },
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

    private static GanttTask Task(string id, string title, int startHour, int endHour)
        => new(TaskId(id), title, Start.AddHours(startHour), Start.AddHours(endHour));

    private static GanttTaskId TaskId(string value)
        => new(value);
}
