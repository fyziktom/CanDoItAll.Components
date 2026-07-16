using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.Gantt;

public partial class GanttChart : ComponentBase, IAsyncDisposable
{
    private const double ZoomFactor = 1.25;
    private const double MinimumPixelsPerHour = 0.25;
    private const double MaximumPixelsPerHour = 96;
    private const double MinimumTimelineContentWidth = 800;
    private const double MaximumTimelineWidth = 12_000;
    private const double MaximumCanvasHeight = 16_000;
    private const double ScaleComparisonTolerance = 0.000_001;
    private const double DependencyEndpointRadius = 4.5;
    private const double DependencyEndpointHitSize = 10;
    private const double DependencyEndpointVerticalSpacing = 11;
    private const double DependencyEndpointLaneSpacing = 12;
    private const double DependencyEndpointEdgeOffset = 6;
    private const double DependencyRouteClearance = 4;
    private const double QuarterHourPixelsPerHour = 96;
    private const double HourPixelsPerHour = 32;
    private const double DayPixelsPerHour = 4;
    private const double WeekPixelsPerHour = 0.75;
    private static readonly IReadOnlyList<GanttTimeScaleOption> TimeScaleOptions =
    [
        new(GanttTimeScale.QuarterHour, "0.25 h"),
        new(GanttTimeScale.Hour, "1 h"),
        new(GanttTimeScale.Day, "1 d"),
        new(GanttTimeScale.Week, "1 w"),
        new(GanttTimeScale.Custom, "Custom")
    ];
    private readonly string instructionsId = $"cda-gantt-instructions-{Guid.NewGuid():N}";
    private readonly HashSet<GanttTaskId> criticalTaskIds = [];
    private ElementReference hostElement;
    private ElementReference canvasElement;
    private ElementReference titleEditor;
    private DotNetObjectReference<GanttChart>? dotNetReference;
    private GanttTaskId? editingTaskId;
    private string editingTitle = string.Empty;
    private string? validationError;
    private string? runtimeError;
    private bool tableVisible;
    private bool tableVisibilityInitialized;
    private bool lastTableVisibilityParameter;
    private bool interopInitialized;
    private bool interopFaulted;
    private bool mutationInFlight;
    private bool disposed;
    private bool exportInFlight;
    private bool titleCommitInFlight;
    private bool timeScaleInitialized;
    private bool interopUpdateRequired = true;
    private double zoomedPixelsPerHour;
    private double lastPixelsPerHourParameter;
    private double dependencyEndpointGutter;
    private DateTimeOffset timelineStart;
    private DateTimeOffset timelineEnd;
    private GanttTimeline? cachedTimeline;
    private GanttTimeScale selectedTimeScale;
    private GanttTimeScale lastTimeScaleParameter;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = default!;

    [Inject]
    private ILogger<GanttChart> Logger { get; set; } = default!;

    [Parameter, EditorRequired]
    public IReadOnlyList<GanttTask> Tasks { get; set; } = [];

    [Parameter, EditorRequired]
    public IReadOnlyList<GanttDependency> Dependencies { get; set; } = [];

    [Parameter]
    public EventCallback<GanttTaskTitleChangeRequest> TaskTitleChangeRequested { get; set; }

    [Parameter]
    public EventCallback<GanttTaskScheduleChangeRequest> TaskScheduleChangeRequested { get; set; }

    [Parameter]
    public EventCallback<GanttDependencyMutationRequest> DependencyMutationRequested { get; set; }

    [Parameter]
    public EventCallback<GanttTaskInsertionRequest> TaskInsertionRequested { get; set; }

    [Parameter]
    public EventCallback<GanttTimelineDoubleClickEventArgs> TimelineDoubleClicked { get; set; }

    [Parameter]
    public EventCallback<GanttTaskId> TaskDoubleClicked { get; set; }

    [Parameter]
    public EventCallback<GanttTaskOrderChangeRequest> TaskOrderChangeRequested { get; set; }

    [Parameter]
    public EventCallback<GanttTaskId> TaskSelected { get; set; }

    [Parameter]
    public EventCallback<bool> ShowTaskTableChanged { get; set; }

    [Parameter]
    public EventCallback<GanttTimeScale> TimeScaleChanged { get; set; }

    [Parameter]
    public EventCallback<double> PixelsPerHourChanged { get; set; }

    [Parameter]
    public Func<GanttTaskId, GanttTaskId, GanttDependencyId>? DependencyIdFactory { get; set; }

    [Parameter]
    public Func<GanttTask, bool>? TaskReadOnlySelector { get; set; }

    [Parameter]
    public Func<GanttTask, bool>? TaskScheduleReadOnlySelector { get; set; }

    [Parameter]
    public Func<GanttTask, bool>? TaskTitleReadOnlySelector { get; set; }

    [Parameter]
    public Func<GanttTask, bool>? TaskDependencyReadOnlySelector { get; set; }

    [Parameter]
    public Func<GanttTask, bool>? ProjectionOnlySelector { get; set; }

    [Parameter]
    public Func<GanttTask, string?>? TaskAccentColorSelector { get; set; }

    [Parameter]
    public bool AllowTaskEditing { get; set; } = true;

    [Parameter]
    public bool? AllowTimelineTaskCreation { get; set; }

    [Parameter]
    public bool AllowDependencyEditing { get; set; } = true;

    [Parameter]
    public bool AllowTaskInsertion { get; set; } = true;

    [Parameter]
    public bool AllowTaskReordering { get; set; } = true;

    [Parameter]
    public bool ShowTaskTable { get; set; } = true;

    [Parameter]
    public bool ShowToolbar { get; set; } = true;

    [Parameter]
    public GanttTimeScale TimeScale { get; set; } = GanttTimeScale.Custom;

    [Parameter]
    public double PixelsPerHour { get; set; } = 12;

    [Parameter]
    public double HoursPerManDay { get; set; } = 8;

    [Parameter]
    public double RowHeight { get; set; } = 48;

    [Parameter]
    public double HeaderHeight { get; set; } = 40;

    [Parameter]
    public double BarHeight { get; set; } = 28;

    [Parameter]
    public double TaskTableWidth { get; set; } = 600;

    [Parameter]
    public TimeSpan SnapInterval { get; set; } = TimeSpan.FromHours(1);

    [Parameter]
    public DateTimeOffset SnapOrigin { get; set; } = DateTimeOffset.UnixEpoch;

    [Parameter]
    public TimeSpan MinimumTaskDuration { get; set; } = TimeSpan.FromHours(1);

    [Parameter]
    public TimeSpan TimelinePadding { get; set; } = TimeSpan.FromHours(8);

    [Parameter]
    public string DragDataFormat { get; set; } = GanttTaskDragSerialization.DataFormat;

    [Parameter]
    public string ExportFileName { get; set; } = "gantt-chart.png";

    [Parameter]
    public string AriaLabel { get; set; } = "Interactive Gantt chart";

    [Parameter]
    public string MaxHeight { get; set; } = "44rem";

    [Parameter]
    public string? Class { get; set; }

    private bool IsInteractionDisabled => mutationInFlight || validationError is not null || interopFaulted;

    private bool IsZoomOutDisabled => IsInteractionDisabled || !WouldChangeRenderedScale(CalculateZoomOutScale());

    private bool IsZoomInDisabled => IsInteractionDisabled || !WouldChangeRenderedScale(CalculateZoomInScale());

    private string TaskTableButtonText => tableVisible ? "Hide task table" : "Show task table";

    private string ScheduleSummary
    {
        get
        {
            var scaleStatus = validationError is null && CalculateTimeline().IsCompressed
                ? " · fitted scale"
                : string.Empty;
            return $"{Tasks.Count} tasks · {Dependencies.Count} dependencies · {FormatTimeScale(selectedTimeScale)} · UTC · {HoursPerManDay:0.#} h/man-day{scaleStatus}";
        }
    }

    private string RowHeightCss => ToPixels(RowHeight);

    private string HeaderHeightCss => ToPixels(HeaderHeight);

    private string TaskTableWidthCss => ToPixels(TaskTableWidth);

    private string CanvasLeftCss => tableVisible ? TaskTableWidthCss : "0px";

    private string CanvasAriaLabel => IsTimelineTaskCreationEnabled
        ? "Interactive Gantt timeline. Drag a bar to move it, drag green ends to resize it, drag blue ports to change dependencies, or double-click empty row space to request a task at that time."
        : "Interactive Gantt timeline. Drag a bar to move it, drag green ends to resize it, or drag blue ports to change dependencies.";

    private bool IsTimelineTaskCreationEnabled =>
        (AllowTimelineTaskCreation ?? AllowTaskEditing) && TimelineDoubleClicked.HasDelegate;

    private string CanvasHeightCss => ToPixels(HeaderHeight + Tasks.Count * RowHeight);

    private string ContentWidthCss => validationError is null
        ? ToPixels((tableVisible ? TaskTableWidth : 0) + CalculateTimeline().Width)
        : "0px";

    protected override void OnParametersSet()
    {
        ArgumentNullException.ThrowIfNull(Tasks);
        ArgumentNullException.ThrowIfNull(Dependencies);

        interopUpdateRequired = true;
        cachedTimeline = null;
        criticalTaskIds.Clear();
        try
        {
            SynchronizeViewParameters();
            ValidateOptions();
            CacheTimelineBounds();
            var graph = GanttScheduleGraph.Create(Tasks, Dependencies);
            GanttSchedulePropagation.ValidateConstraints(graph, graph.TasksById);
            criticalTaskIds.UnionWith(GanttCriticalPathCalculator.Calculate(graph));
            validationError = null;
        }
        catch (Exception exception) when (exception is ArgumentException or GanttScheduleException or OverflowException)
        {
            validationError = exception.Message;
            Logger.LogWarning(exception, "Rejected invalid Gantt model with {TaskCount} tasks and {DependencyCount} dependencies.", Tasks.Count, Dependencies.Count);
        }

        if (editingTaskId is { } editingId)
        {
            var editingTask = Tasks.FirstOrDefault(task => task.Id == editingId);
            if (editingTask is null || !CanEditTitle(editingTask))
            {
                editingTaskId = null;
                editingTitle = string.Empty;
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (disposed || mutationInFlight)
        {
            return;
        }

        if (validationError is not null)
        {
            await DisposeInteropAsync();
            return;
        }

        if (interopFaulted)
        {
            return;
        }

        if (interopInitialized && !interopUpdateRequired)
        {
            return;
        }

        try
        {
            var model = BuildInteropModel();
            if (!interopInitialized)
            {
                dotNetReference ??= DotNetObjectReference.Create(this);
                await JsRuntime.InvokeVoidAsync(
                    "CanDoItAll.ganttChart.create",
                    hostElement,
                    canvasElement,
                    dotNetReference,
                    model);
                interopInitialized = true;
                interopUpdateRequired = false;
                return;
            }

            await JsRuntime.InvokeVoidAsync("CanDoItAll.ganttChart.update", hostElement, model);
            interopUpdateRequired = false;
        }
        catch (JSException exception)
        {
            interopFaulted = true;
            runtimeError = $"The Gantt canvas could not start: {exception.Message}";
            Logger.LogError(exception, "Gantt canvas interop failed for {TaskCount} tasks.", Tasks.Count);
            await InvokeAsync(StateHasChanged);
        }
    }

    [JSInvokable]
    public async Task NotifyTaskSelectedAsync(string taskId)
    {
        var id = new GanttTaskId(taskId);
        if (!Tasks.Any(task => task.Id == id))
        {
            throw new InvalidOperationException($"Task '{id}' is not present in the controlled chart model.");
        }

        await TaskSelected.InvokeAsync(id);
    }

    [JSInvokable]
    public async Task NotifyTimelineDoubleClickedAsync(string rowTaskId, double clickedAtMs)
    {
        if (!IsTimelineTaskCreationEnabled || IsInteractionDisabled)
        {
            return;
        }

        var id = new GanttTaskId(rowTaskId);
        if (!Tasks.Any(task => task.Id == id))
        {
            throw new InvalidOperationException($"Task '{id}' is not present in the controlled chart model.");
        }

        var request = new GanttTimelineDoubleClickEventArgs(id, FromUnixMilliseconds(clickedAtMs));
        await TimelineDoubleClicked.InvokeAsync(request);
    }

    [JSInvokable]
    public async Task NotifyTaskDoubleClickedAsync(string taskId)
    {
        var id = new GanttTaskId(taskId);
        if (!Tasks.Any(task => task.Id == id))
        {
            throw new InvalidOperationException($"Task '{id}' is not present in the controlled chart model.");
        }

        if (IsInteractionDisabled)
        {
            return;
        }

        if (TaskDoubleClicked.HasDelegate)
        {
            await TaskDoubleClicked.InvokeAsync(id);
            return;
        }

        await BeginTitleEditAsync(taskId);
    }

    [JSInvokable]
    public async Task CommitScheduleChangeAsync(string taskId, double proposedStartMs, double proposedEndMs, string gesture)
    {
        var id = new GanttTaskId(taskId);
        var task = Tasks.FirstOrDefault(candidate => candidate.Id == id)
            ?? throw new InvalidOperationException($"Task '{id}' is not present in the controlled chart model.");
        if (!CanEditSchedule(task))
        {
            throw new InvalidOperationException($"Task '{id}' is not currently editable from the Gantt schedule.");
        }

        var parsedGesture = ParseGesture(gesture);
        var proposedStart = FromUnixMilliseconds(proposedStartMs);
        var proposedEnd = FromUnixMilliseconds(proposedEndMs);
        if (proposedEnd <= proposedStart)
        {
            throw new InvalidOperationException($"Task '{id}' received an invalid proposed interval.");
        }

        var proposedDate = parsedGesture == GanttScheduleGesture.ResizeEnd ? proposedEnd : proposedStart;
        var request = GanttSchedulePlanner.Plan(
            Tasks,
            Dependencies,
            id,
            parsedGesture,
            proposedDate,
            new GanttSnapGrid(SnapOrigin, SnapInterval),
            MinimumTaskDuration);
        await DispatchMutationAsync(
            "schedule",
            id.Value,
            TaskScheduleChangeRequested,
            request);
    }

    [JSInvokable]
    public async Task CommitDependencyChangeAsync(
        string mutation,
        string? dependencyId,
        string predecessorId,
        string successorId)
    {
        if (!CanEditDependencies())
        {
            throw new InvalidOperationException("Dependencies are not currently editable from the Gantt chart.");
        }

        var predecessor = new GanttTaskId(predecessorId);
        var successor = new GanttTaskId(successorId);
        RequireDependencyEditableTasks(predecessor, successor);
        GanttDependencyMutationRequest request = mutation switch
        {
            "add" => GanttDependencyPlanner.PlanAdd(
                Tasks,
                Dependencies,
                new GanttDependency(CreateDependencyId(predecessor, successor), predecessor, successor)),
            "reconnect" => GanttDependencyPlanner.PlanReconnect(
                Tasks,
                Dependencies,
                new GanttDependencyId(dependencyId ?? throw new InvalidOperationException("A dependency id is required for reconnection.")),
                predecessor,
                successor),
            _ => throw new InvalidOperationException($"Dependency mutation '{mutation}' is not supported.")
        };

        if (request.PreviousDependency is { } previousDependency)
        {
            RequireDependencyEditableTasks(previousDependency.PredecessorId, previousDependency.SuccessorId);
        }

        await DispatchMutationAsync(
            "dependency",
            request.ProposedDependency?.Id.Value ?? request.PreviousDependency?.Id.Value ?? "unknown",
            DependencyMutationRequested,
            request);
    }

    [JSInvokable]
    public async Task CommitInsertionAsync(string payload, string dependencyId)
    {
        if (!CanInsertTasks())
        {
            throw new InvalidOperationException("Task insertion is not currently enabled for the Gantt chart.");
        }

        var insertedTask = GanttTaskDragSerialization.Deserialize(payload);
        var bridgeId = new GanttDependencyId(dependencyId);
        var bridge = Dependencies.FirstOrDefault(candidate => candidate.Id == bridgeId)
            ?? throw new InvalidOperationException($"Dependency '{bridgeId}' is not present in the controlled chart model.");
        RequireDependencyEditableTasks(bridge.PredecessorId, bridge.SuccessorId);
        var request = GanttInsertionPlanner.Plan(
            Tasks,
            Dependencies,
            insertedTask,
            bridge.PredecessorId,
            bridge.SuccessorId,
            CreateDependencyId(bridge.PredecessorId, insertedTask.Id),
            CreateDependencyId(insertedTask.Id, bridge.SuccessorId));
        await DispatchMutationAsync(
            "insertion",
            insertedTask.Id.Value,
            TaskInsertionRequested,
            request);
    }

    [JSInvokable]
    public async Task BeginTitleEditAsync(string taskId)
    {
        var id = new GanttTaskId(taskId);
        var task = Tasks.FirstOrDefault(candidate => candidate.Id == id)
            ?? throw new InvalidOperationException($"Task '{id}' is not present in the controlled chart model.");
        if (!CanEditTitle(task))
        {
            return;
        }

        if (!tableVisible)
        {
            tableVisible = true;
            interopUpdateRequired = true;
            await ShowTaskTableChanged.InvokeAsync(true);
        }

        editingTaskId = id;
        editingTitle = task.Title;
        await InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public Task ReportRuntimeErrorAsync(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return Task.CompletedTask;
        }

        runtimeError = message;
        Logger.LogWarning("Gantt canvas reported an interaction error for a chart with {TaskCount} tasks.", Tasks.Count);
        return InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public Task ReportRenderErrorAsync(string message)
    {
        runtimeError = string.IsNullOrWhiteSpace(message)
            ? "The Gantt canvas failed while rendering."
            : message;
        interopFaulted = true;
        Logger.LogError("Gantt canvas rendering failed for a chart with {TaskCount} tasks.", Tasks.Count);
        return InvokeAsync(StateHasChanged);
    }

    public async Task DownloadPngAsync()
    {
        EnsureInteropAvailable();
        exportInFlight = true;
        runtimeError = null;
        try
        {
            await JsRuntime.InvokeVoidAsync(
                "CanDoItAll.ganttChart.downloadPng",
                hostElement,
                ExportFileName,
                tableVisible);
        }
        catch (JSException exception)
        {
            runtimeError = $"PNG export failed: {exception.Message}";
            Logger.LogError(exception, "Gantt PNG export failed for {TaskCount} tasks.", Tasks.Count);
        }
        finally
        {
            exportInFlight = false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        disposed = true;
        try
        {
            await DisposeInteropAsync();
        }
        finally
        {
            dotNetReference?.Dispose();
            dotNetReference = null;
            GC.SuppressFinalize(this);
        }
    }

    private async Task ToggleTaskTableAsync()
    {
        tableVisible = !tableVisible;
        interopUpdateRequired = true;
        await ShowTaskTableChanged.InvokeAsync(tableVisible);
    }

    private async Task RequestTaskOrderChangeAsync(
        GanttTaskId taskId,
        GanttTaskOrderPlacement placement)
    {
        var taskIndex = FindTaskIndex(taskId);
        var anchorIndex = placement switch
        {
            GanttTaskOrderPlacement.Before => taskIndex - 1,
            GanttTaskOrderPlacement.After => taskIndex + 1,
            _ => throw new ArgumentOutOfRangeException(nameof(placement), placement, "The task order placement is not supported.")
        };
        if (!CanChangeTaskOrder(taskIndex, anchorIndex))
        {
            throw new InvalidOperationException($"Task '{taskId}' cannot move {placement.ToString().ToLowerInvariant()} an adjacent row.");
        }

        var request = new GanttTaskOrderChangeRequest(taskId, Tasks[anchorIndex].Id, placement);
        await DispatchMutationAsync(
            "task order",
            taskId.Value,
            TaskOrderChangeRequested,
            request);
    }

    private async Task ZoomOutAsync()
    {
        var nextPixelsPerHour = CalculateZoomOutScale();
        selectedTimeScale = GanttTimeScale.Custom;
        zoomedPixelsPerHour = nextPixelsPerHour;
        InvalidateTimeline();
        await TimeScaleChanged.InvokeAsync(GanttTimeScale.Custom);
        await PixelsPerHourChanged.InvokeAsync(nextPixelsPerHour);
        await InvokeAsync(StateHasChanged);
    }

    private async Task ZoomInAsync()
    {
        var nextPixelsPerHour = CalculateZoomInScale();
        selectedTimeScale = GanttTimeScale.Custom;
        zoomedPixelsPerHour = nextPixelsPerHour;
        InvalidateTimeline();
        await TimeScaleChanged.InvokeAsync(GanttTimeScale.Custom);
        await PixelsPerHourChanged.InvokeAsync(nextPixelsPerHour);
        await InvokeAsync(StateHasChanged);
    }

    private async Task HandleTimeScaleChangedAsync(GanttTimeScale scale)
    {
        if (!Enum.IsDefined(scale))
        {
            throw new ArgumentOutOfRangeException(nameof(scale), scale, "The Gantt time scale is not supported.");
        }

        selectedTimeScale = scale;
        zoomedPixelsPerHour = ResolvePixelsPerHour(scale, PixelsPerHour);
        InvalidateTimeline();
        await TimeScaleChanged.InvokeAsync(scale);
        await InvokeAsync(StateHasChanged);
    }

    private double CalculateZoomOutScale()
    {
        var timeline = CalculateTimeline();
        var scale = Math.Min(timeline.PixelsPerHour, zoomedPixelsPerHour);
        return Math.Max(MinimumPixelsPerHour, scale / ZoomFactor);
    }

    private double CalculateZoomInScale()
    {
        var timeline = CalculateTimeline();
        var scale = Math.Max(timeline.PixelsPerHour, zoomedPixelsPerHour);
        return Math.Min(MaximumPixelsPerHour, scale * ZoomFactor);
    }

    private bool WouldChangeRenderedScale(double nextScale)
        => Math.Abs(CalculateTimeline(nextScale).PixelsPerHour - CalculateTimeline().PixelsPerHour) > ScaleComparisonTolerance;

    private void UpdateEditingTitle(ChangeEventArgs args)
        => editingTitle = args.Value?.ToString() ?? string.Empty;

    private async Task HandleTitleEditorKeyDownAsync(KeyboardEventArgs args)
    {
        if (args.Key == "Escape")
        {
            editingTaskId = null;
            editingTitle = string.Empty;
            return;
        }

        if (args.Key == "Enter")
        {
            await CommitTitleEditAsync();
        }
    }

    private async Task CommitTitleEditAsync()
    {
        if (titleCommitInFlight || editingTaskId is not { } taskId)
        {
            return;
        }

        var task = Tasks.FirstOrDefault(candidate => candidate.Id == taskId)
            ?? throw new InvalidOperationException($"Task '{taskId}' is not present in the controlled chart model.");
        if (!CanEditTitle(task))
        {
            editingTaskId = null;
            editingTitle = string.Empty;
            runtimeError = "Task title editing is no longer available.";
            await InvokeAsync(StateHasChanged);
            return;
        }

        var proposedTitle = editingTitle.Trim();
        if (proposedTitle.Length == 0)
        {
            runtimeError = "A task title is required.";
            await InvokeAsync(StateHasChanged);
            return;
        }

        if (proposedTitle == task.Title)
        {
            editingTaskId = null;
            editingTitle = string.Empty;
            return;
        }

        titleCommitInFlight = true;
        try
        {
            var request = new GanttTaskTitleChangeRequest(task.Id, task.Title, proposedTitle);
            await DispatchMutationAsync("title", task.Id.Value, TaskTitleChangeRequested, request);
            editingTaskId = null;
            editingTitle = string.Empty;
        }
        catch (Exception exception)
        {
            runtimeError = "The host rejected the title change.";
            Logger.LogError(exception, "Gantt title change was rejected for task {TaskId}.", task.Id.Value);
            await InvokeAsync(StateHasChanged);
        }
        finally
        {
            titleCommitInFlight = false;
        }
    }

    private async Task DispatchMutationAsync<TRequest>(
        string operation,
        string targetId,
        EventCallback<TRequest> callback,
        TRequest request)
    {
        if (!callback.HasDelegate)
        {
            throw new InvalidOperationException($"The host must handle the {operation} request before that interaction is enabled.");
        }

        if (mutationInFlight)
        {
            throw new InvalidOperationException("A Gantt mutation request is already awaiting its host.");
        }

        mutationInFlight = true;
        interopUpdateRequired = true;
        runtimeError = null;
        await InvokeAsync(StateHasChanged);
        try
        {
            await callback.InvokeAsync(request);
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Gantt {Operation} request failed for target {TargetId}.", operation, targetId);
            throw;
        }
        finally
        {
            mutationInFlight = false;
            interopUpdateRequired = true;
            if (!disposed)
            {
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private GanttDependencyId CreateDependencyId(GanttTaskId predecessorId, GanttTaskId successorId)
    {
        var id = DependencyIdFactory?.Invoke(predecessorId, successorId)
            ?? new GanttDependencyId($"gantt-dependency:{Guid.NewGuid():N}");
        GanttIdentifierGuard.Ensure(id, nameof(DependencyIdFactory));
        return id;
    }

    private GanttInteropModel BuildInteropModel()
    {
        var timeline = CalculateTimeline();
        var tasks = Tasks.Select((task, index) =>
        {
            var taskReadOnly = TaskReadOnlySelector?.Invoke(task) == true;
            return new GanttInteropTask(
                task.Id.Value,
                task.Title,
                task.Start.ToUnixTimeMilliseconds(),
                task.End.ToUnixTimeMilliseconds(),
                index,
                task.ProgressPercent,
                task.ExpectedEffort?.TotalMilliseconds,
                TaskAccentColorSelector?.Invoke(task),
                taskReadOnly,
                taskReadOnly || TaskScheduleReadOnlySelector?.Invoke(task) == true,
                taskReadOnly || TaskTitleReadOnlySelector?.Invoke(task) == true,
                taskReadOnly || TaskDependencyReadOnlySelector?.Invoke(task) == true,
                ProjectionOnlySelector?.Invoke(task) == true,
                criticalTaskIds.Contains(task.Id),
                task.Assignments.Select((assignment, assignmentIndex) => new GanttInteropAssignment(
                    $"{task.Id.Value}:{assignment.Kind}:{assignmentIndex}",
                    assignment.Name,
                    assignment.Kind.ToString().ToLowerInvariant())).ToArray());
        }).ToArray();
        var dependencies = Dependencies.Select(dependency => new GanttInteropDependency(
            dependency.Id.Value,
            dependency.PredecessorId.Value,
            dependency.SuccessorId.Value,
            0,
            criticalTaskIds.Contains(dependency.PredecessorId) && criticalTaskIds.Contains(dependency.SuccessorId))).ToArray();
        var options = new GanttInteropOptions(
            RowHeight,
            HeaderHeight,
            BarHeight,
            timeline.PixelsPerHour,
            ResolveTickIntervalMilliseconds(timeline.IsCompressed ? GanttTimeScale.Custom : selectedTimeScale),
            SnapInterval.TotalMilliseconds,
            SnapOrigin.ToUnixTimeMilliseconds(),
            MinimumTaskDuration.TotalMilliseconds,
            timeline.Start.ToUnixTimeMilliseconds(),
            timeline.End.ToUnixTimeMilliseconds(),
            timeline.Width,
            timeline.Gutter,
            DependencyEndpointRadius,
            DependencyEndpointHitSize,
            DependencyEndpointVerticalSpacing,
            DependencyEndpointLaneSpacing,
            DependencyEndpointEdgeOffset,
            DependencyRouteClearance,
            HeaderHeight + Tasks.Count * RowHeight,
            TaskTableWidth,
            HoursPerManDay,
            tableVisible,
            !mutationInFlight && AllowTaskEditing && TaskScheduleChangeRequested.HasDelegate,
            !mutationInFlight && AllowDependencyEditing && DependencyMutationRequested.HasDelegate,
            !mutationInFlight && AllowTaskInsertion && TaskInsertionRequested.HasDelegate,
            !mutationInFlight && IsTimelineTaskCreationEnabled,
            DragDataFormat);
        return new GanttInteropModel(tasks, dependencies, options);
    }

    private GanttTimeline CalculateTimeline()
        => cachedTimeline ??= CalculateTimeline(zoomedPixelsPerHour);

    private GanttTimeline CalculateTimeline(
        double requestedPixelsPerHour)
    {
        var timelineHours = (timelineEnd - timelineStart).TotalHours;
        var maximumContentWidth = MaximumTimelineWidth - (2 * dependencyEndpointGutter);
        var requestedContentWidth = timelineHours * requestedPixelsPerHour;
        var isCompressed = requestedContentWidth > maximumContentWidth;
        var renderedPixelsPerHour = isCompressed
            ? maximumContentWidth / timelineHours
            : requestedPixelsPerHour;
        var contentWidth = Math.Max(
            MinimumTimelineContentWidth,
            Math.Min(maximumContentWidth, requestedContentWidth));
        var renderedEnd = timelineStart.AddHours(contentWidth / renderedPixelsPerHour);
        return new GanttTimeline(
            timelineStart,
            renderedEnd,
            contentWidth + (2 * dependencyEndpointGutter),
            renderedPixelsPerHour,
            dependencyEndpointGutter,
            isCompressed);
    }

    private double CalculateDependencyEndpointGutter()
    {
        var outgoingCounts = new Dictionary<GanttTaskId, int>();
        var incomingCounts = new Dictionary<GanttTaskId, int>();
        var maximumFanSize = 0;
        foreach (var dependency in Dependencies)
        {
            maximumFanSize = Math.Max(
                maximumFanSize,
                IncrementCount(outgoingCounts, dependency.PredecessorId));
            maximumFanSize = Math.Max(
                maximumFanSize,
                IncrementCount(incomingCounts, dependency.SuccessorId));
        }

        var availableSlotHeight = Math.Max(0, BarHeight - (2 * DependencyEndpointRadius));
        var slotsPerLane = Math.Max(1, (int)Math.Floor(availableSlotHeight / DependencyEndpointVerticalSpacing) + 1);
        var laneCount = Math.Max(1, (int)Math.Ceiling((double)maximumFanSize / slotsPerLane));
        return DependencyEndpointEdgeOffset +
               ((laneCount - 1) * DependencyEndpointLaneSpacing) +
               (DependencyEndpointHitSize / 2) +
               1;
    }

    private static int IncrementCount(
        IDictionary<GanttTaskId, int> counts,
        GanttTaskId taskId)
    {
        counts.TryGetValue(taskId, out var count);
        count++;
        counts[taskId] = count;
        return count;
    }

    private void CacheTimelineBounds()
    {
        if (Tasks.Count == 0)
        {
            timelineStart = DateTimeOffset.UtcNow;
            timelineEnd = timelineStart.AddDays(1);
        }
        else
        {
            timelineStart = Tasks[0].Start;
            timelineEnd = Tasks[0].End;
            for (var index = 1; index < Tasks.Count; index++)
            {
                timelineStart = timelineStart <= Tasks[index].Start
                    ? timelineStart
                    : Tasks[index].Start;
                timelineEnd = timelineEnd >= Tasks[index].End
                    ? timelineEnd
                    : Tasks[index].End;
            }
        }

        timelineStart -= TimelinePadding;
        timelineEnd += TimelinePadding;
        if (timelineEnd - timelineStart < TimeSpan.FromHours(24))
        {
            timelineEnd = timelineStart.AddHours(24);
        }
    }

    private void InvalidateTimeline()
    {
        cachedTimeline = null;
        interopUpdateRequired = true;
    }

    private static double ResolvePixelsPerHour(GanttTimeScale scale, double customPixelsPerHour)
        => scale switch
        {
            GanttTimeScale.Custom => customPixelsPerHour,
            GanttTimeScale.QuarterHour => QuarterHourPixelsPerHour,
            GanttTimeScale.Hour => HourPixelsPerHour,
            GanttTimeScale.Day => DayPixelsPerHour,
            GanttTimeScale.Week => WeekPixelsPerHour,
            _ => throw new ArgumentOutOfRangeException(nameof(scale), scale, "The Gantt time scale is not supported.")
        };

    private static double ResolveTickIntervalMilliseconds(GanttTimeScale scale)
        => scale switch
        {
            GanttTimeScale.Custom => 0,
            GanttTimeScale.QuarterHour => TimeSpan.FromMinutes(15).TotalMilliseconds,
            GanttTimeScale.Hour => TimeSpan.FromHours(1).TotalMilliseconds,
            GanttTimeScale.Day => TimeSpan.FromDays(1).TotalMilliseconds,
            GanttTimeScale.Week => TimeSpan.FromDays(7).TotalMilliseconds,
            _ => throw new ArgumentOutOfRangeException(nameof(scale), scale, "The Gantt time scale is not supported.")
        };

    private static string FormatTimeScale(GanttTimeScale scale)
        => scale switch
        {
            GanttTimeScale.Custom => "custom scale",
            GanttTimeScale.QuarterHour => "0.25 h scale",
            GanttTimeScale.Hour => "1 h scale",
            GanttTimeScale.Day => "1 d scale",
            GanttTimeScale.Week => "1 w scale",
            _ => throw new ArgumentOutOfRangeException(nameof(scale), scale, "The Gantt time scale is not supported.")
        };

    private void SynchronizeViewParameters()
    {
        if (!tableVisibilityInitialized || ShowTaskTable != lastTableVisibilityParameter)
        {
            tableVisible = ShowTaskTable;
            lastTableVisibilityParameter = ShowTaskTable;
            tableVisibilityInitialized = true;
        }

        if (!timeScaleInitialized || TimeScale != lastTimeScaleParameter)
        {
            selectedTimeScale = TimeScale;
            lastTimeScaleParameter = TimeScale;
            timeScaleInitialized = true;
            zoomedPixelsPerHour = ResolvePixelsPerHour(selectedTimeScale, PixelsPerHour);
        }

        if (lastPixelsPerHourParameter != PixelsPerHour)
        {
            lastPixelsPerHourParameter = PixelsPerHour;
            if (selectedTimeScale == GanttTimeScale.Custom)
            {
                zoomedPixelsPerHour = PixelsPerHour;
            }
        }
    }

    private void ValidateOptions()
    {
        if (!Enum.IsDefined(TimeScale))
        {
            throw new ArgumentOutOfRangeException(nameof(TimeScale), TimeScale, "The Gantt time scale is not supported.");
        }

        if (!double.IsFinite(PixelsPerHour) ||
            !double.IsFinite(HoursPerManDay) ||
            !double.IsFinite(RowHeight) ||
            !double.IsFinite(HeaderHeight) ||
            !double.IsFinite(BarHeight) ||
            !double.IsFinite(TaskTableWidth))
        {
            throw new ArgumentOutOfRangeException(nameof(PixelsPerHour), "Gantt dimensions and scale values must be finite numbers.");
        }

        if (PixelsPerHour is < MinimumPixelsPerHour or > MaximumPixelsPerHour)
        {
            throw new ArgumentOutOfRangeException(nameof(PixelsPerHour), PixelsPerHour, $"Pixels per hour must be between {MinimumPixelsPerHour} and {MaximumPixelsPerHour}.");
        }

        if (HoursPerManDay <= 0 || RowHeight <= 0 || HeaderHeight <= 0 || BarHeight <= 0 || TaskTableWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(HoursPerManDay), "Gantt dimensions and hours per man-day must be greater than zero.");
        }

        if (BarHeight >= RowHeight)
        {
            throw new ArgumentOutOfRangeException(nameof(BarHeight), "The task bar height must be smaller than the row height.");
        }

        dependencyEndpointGutter = CalculateDependencyEndpointGutter();
        if (MaximumTimelineWidth - (2 * dependencyEndpointGutter) < MinimumTimelineContentWidth)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Dependencies),
                $"The dependency fan requires a {dependencyEndpointGutter:0} px gutter on each side, leaving less than the supported {MinimumTimelineContentWidth:0} px timeline inside the {MaximumTimelineWidth:0} px canvas limit. Reduce the visible dependency fan or provide a filtered task set.");
        }

        var canvasHeight = HeaderHeight + Tasks.Count * RowHeight;
        if (canvasHeight > MaximumCanvasHeight)
        {
            throw new ArgumentOutOfRangeException(
                nameof(RowHeight),
                $"The chart height is {canvasHeight:0} px, above the supported {MaximumCanvasHeight:0} px limit. Reduce the row height or provide a filtered task set.");
        }

        if (SnapInterval <= TimeSpan.Zero || MinimumTaskDuration <= TimeSpan.Zero || TimelinePadding < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(SnapInterval), "Gantt timing intervals are invalid.");
        }

        if (string.IsNullOrWhiteSpace(DragDataFormat) || string.IsNullOrWhiteSpace(ExportFileName) || !ExportFileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("The drag data format is required and the export file name must end with .png.");
        }
    }

    private bool CanEditTitle(GanttTask task)
        => AllowTaskEditing &&
           !mutationInFlight &&
           validationError is null &&
           !interopFaulted &&
           TaskTitleChangeRequested.HasDelegate &&
           TaskReadOnlySelector?.Invoke(task) != true &&
           TaskTitleReadOnlySelector?.Invoke(task) != true;

    private bool CanEditSchedule(GanttTask task)
        => AllowTaskEditing &&
           !mutationInFlight &&
           validationError is null &&
           !interopFaulted &&
           TaskScheduleChangeRequested.HasDelegate &&
           TaskReadOnlySelector?.Invoke(task) != true &&
           TaskScheduleReadOnlySelector?.Invoke(task) != true;

    private bool CanEditDependencies()
        => AllowDependencyEditing &&
           !mutationInFlight &&
           validationError is null &&
           !interopFaulted &&
           DependencyMutationRequested.HasDelegate;

    private bool CanInsertTasks()
        => AllowTaskInsertion &&
           !mutationInFlight &&
           validationError is null &&
           !interopFaulted &&
           TaskInsertionRequested.HasDelegate;

    private bool CanChangeTaskOrder(int taskIndex, int anchorIndex)
    {
        if (!AllowTaskReordering ||
            IsInteractionDisabled ||
            !TaskOrderChangeRequested.HasDelegate ||
            taskIndex < 0 ||
            taskIndex >= Tasks.Count ||
            anchorIndex < 0 ||
            anchorIndex >= Tasks.Count)
        {
            return false;
        }

        return TaskReadOnlySelector?.Invoke(Tasks[taskIndex]) != true &&
               TaskReadOnlySelector?.Invoke(Tasks[anchorIndex]) != true;
    }

    private int FindTaskIndex(GanttTaskId taskId)
    {
        for (var index = 0; index < Tasks.Count; index++)
        {
            if (Tasks[index].Id == taskId)
            {
                return index;
            }
        }

        return -1;
    }

    private void RequireDependencyEditableTasks(
        GanttTaskId predecessorId,
        GanttTaskId successorId)
    {
        var endpointIds = new[] { predecessorId, successorId };
        var endpointTasks = endpointIds
            .Select(endpointId => Tasks.FirstOrDefault(task => task.Id == endpointId)
                ?? throw new InvalidOperationException($"Task '{endpointId}' is not present in the controlled chart model."))
            .ToArray();
        if (endpointTasks.Any(task =>
                TaskReadOnlySelector?.Invoke(task) == true ||
                TaskDependencyReadOnlySelector?.Invoke(task) == true))
        {
            throw new InvalidOperationException("The dependency touches a task whose dependencies are read-only.");
        }

    }

    private string ResolveTaskTitle(GanttTaskId taskId)
        => Tasks.FirstOrDefault(task => task.Id == taskId)?.Title ?? taskId.Value;

    private string FormatDuration(TimeSpan duration)
        => $"{duration.TotalHours:0.#} h · {duration.TotalHours / HoursPerManDay:0.##} md";

    private string FormatExpectedEffort(TimeSpan? effort)
        => effort is { } value
            ? FormatDuration(value)
            : "—";

    private string FormatExpectedEffortDescription(TimeSpan? effort)
        => effort is { } value
            ? FormatDuration(value)
            : "Not estimated";

    private static string FormatProgress(int? progressPercent)
        => progressPercent is { } value
            ? $"{value}%"
            : "Not reported";

    private static string FormatAssignmentAriaLabel(GanttAssignmentKind kind)
        => $"{FormatAssignmentKind(kind)} assignment";

    private static string FormatAssignmentKind(GanttAssignmentKind kind)
        => kind switch
        {
            GanttAssignmentKind.Process => "Process",
            GanttAssignmentKind.Workflow => "Workflow",
            GanttAssignmentKind.Agent => "Agent",
            GanttAssignmentKind.Person => "Person",
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "The assignment kind is not supported.")
        };

    private static string FormatAssignmentAbbreviation(GanttAssignmentKind kind)
        => kind switch
        {
            GanttAssignmentKind.Process => "PR",
            GanttAssignmentKind.Workflow => "WF",
            GanttAssignmentKind.Agent => "AI",
            GanttAssignmentKind.Person => "PE",
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "The assignment kind is not supported.")
        };

    private static string FormatDate(DateTimeOffset value)
        => value.UtcDateTime.ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture);

    private static string ToPixels(double value)
        => string.Create(CultureInfo.InvariantCulture, $"{value:0.##}px");

    private static DateTimeOffset FromUnixMilliseconds(double value)
    {
        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "A finite Unix timestamp is required.");
        }

        return DateTimeOffset.FromUnixTimeMilliseconds(checked((long)Math.Round(value, MidpointRounding.AwayFromZero)));
    }

    private static GanttScheduleGesture ParseGesture(string gesture)
        => gesture switch
        {
            "move" => GanttScheduleGesture.Move,
            "resize-start" => GanttScheduleGesture.ResizeStart,
            "resize-end" => GanttScheduleGesture.ResizeEnd,
            _ => throw new InvalidOperationException($"Schedule gesture '{gesture}' is not supported.")
        };

    private void EnsureInteropAvailable()
    {
        if (!interopInitialized || interopFaulted)
        {
            throw new InvalidOperationException("The Gantt canvas is not available for export.");
        }
    }

    private async ValueTask DisposeInteropAsync()
    {
        if (!interopInitialized)
        {
            return;
        }

        try
        {
            await JsRuntime.InvokeVoidAsync("CanDoItAll.ganttChart.dispose", hostElement);
        }
        catch (JSDisconnectedException)
        {
        }
        finally
        {
            interopInitialized = false;
        }
    }

    private sealed record GanttInteropModel(
        IReadOnlyList<GanttInteropTask> Tasks,
        IReadOnlyList<GanttInteropDependency> Dependencies,
        GanttInteropOptions Options);

    private sealed record GanttInteropTask(
        string Id,
        string Title,
        long StartMs,
        long EndMs,
        int Order,
        int? ProgressPercent,
        double? ExpectedEffortMs,
        string? AccentColor,
        bool IsReadOnly,
        bool IsScheduleReadOnly,
        bool IsTitleReadOnly,
        bool IsDependencyReadOnly,
        bool IsProjectionOnly,
        bool IsCritical,
        IReadOnlyList<GanttInteropAssignment> Assignments);

    private sealed record GanttInteropDependency(
        string Id,
        string PredecessorId,
        string SuccessorId,
        double LagMs,
        bool IsCritical);

    private sealed record GanttInteropAssignment(string Id, string Label, string Kind);

    private sealed record GanttTimeScaleOption(GanttTimeScale Value, string Label);

    private sealed record GanttInteropOptions(
        double RowHeight,
        double HeaderHeight,
        double BarHeight,
        double PixelsPerHour,
        double TickIntervalMs,
        double SnapMs,
        long SnapOriginMs,
        double MinimumTaskDurationMs,
        long TimelineStartMs,
        long TimelineEndMs,
        double TimelineWidth,
        double TimelineGutter,
        double DependencyEndpointRadius,
        double DependencyEndpointHitSize,
        double DependencyEndpointVerticalSpacing,
        double DependencyEndpointLaneSpacing,
        double DependencyEndpointEdgeOffset,
        double DependencyRouteClearance,
        double CanvasHeight,
        double TaskTableWidth,
        double HoursPerManDay,
        bool ShowTaskTable,
        bool AllowTaskEditing,
        bool AllowDependencyEditing,
        bool AllowTaskInsertion,
        bool AllowTimelineTaskCreation,
        string DragDataFormat);

    private readonly record struct GanttTimeline(
        DateTimeOffset Start,
        DateTimeOffset End,
        double Width,
        double PixelsPerHour,
        double Gutter,
        bool IsCompressed);
}
