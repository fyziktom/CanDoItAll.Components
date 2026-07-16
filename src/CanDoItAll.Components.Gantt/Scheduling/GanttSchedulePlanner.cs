namespace CanDoItAll.Components.Gantt;

public readonly record struct GanttSnapGrid
{
    public GanttSnapGrid(DateTimeOffset origin, TimeSpan interval)
    {
        if (interval <= TimeSpan.Zero)
        {
            throw new GanttScheduleException(
                GanttScheduleErrorCode.InvalidSnapInterval,
                "The snap interval must be greater than zero.");
        }

        Origin = origin;
        Interval = interval;
    }

    public DateTimeOffset Origin { get; }

    public TimeSpan Interval { get; }
}

public static class GanttSnapper
{
    public static DateTimeOffset Snap(DateTimeOffset value, GanttSnapGrid grid)
    {
        if (grid.Interval <= TimeSpan.Zero)
        {
            throw new GanttScheduleException(
                GanttScheduleErrorCode.InvalidSnapInterval,
                "The snap interval must be greater than zero.");
        }

        var elapsedTicks = value.UtcTicks - grid.Origin.UtcTicks;
        var intervals = (decimal)elapsedTicks / grid.Interval.Ticks;
        var snappedIntervals = decimal.Round(intervals, 0, MidpointRounding.AwayFromZero);
        var snappedUtcTicks = checked(grid.Origin.UtcTicks + (long)snappedIntervals * grid.Interval.Ticks);
        return new DateTimeOffset(snappedUtcTicks, TimeSpan.Zero).ToOffset(value.Offset);
    }
}

public static class GanttSchedulePlanner
{
    public static GanttTaskScheduleChangeRequest Plan(
        IReadOnlyCollection<GanttTask> tasks,
        IReadOnlyCollection<GanttDependency> dependencies,
        GanttTaskId taskId,
        GanttScheduleGesture gesture,
        DateTimeOffset proposedDate,
        GanttSnapGrid? snapGrid = null,
        TimeSpan? minimumTaskDuration = null)
    {
        GanttIdentifierGuard.Ensure(taskId, nameof(taskId));
        if (!Enum.IsDefined(gesture))
        {
            throw new ArgumentOutOfRangeException(nameof(gesture), gesture, "The schedule gesture is not supported.");
        }

        var graph = GanttScheduleGraph.Create(tasks, dependencies);
        GanttSchedulePropagation.ValidateConstraints(graph, graph.TasksById);
        if (!graph.TasksById.TryGetValue(taskId, out var task))
        {
            throw new GanttScheduleException(
                GanttScheduleErrorCode.TaskNotFound,
                $"Task '{taskId}' does not exist.");
        }

        var adjustedDate = snapGrid is { } grid ? GanttSnapper.Snap(proposedDate, grid) : proposedDate;
        var minimumDuration = ResolveMinimumTaskDuration(minimumTaskDuration);

        var proposedTask = gesture switch
        {
            GanttScheduleGesture.Move => CopyWithDates(task, adjustedDate, adjustedDate + task.Duration),
            GanttScheduleGesture.ResizeStart => CopyWithDates(
                task,
                Min(adjustedDate, task.End - minimumDuration),
                task.End),
            GanttScheduleGesture.ResizeEnd => CopyWithDates(
                task,
                task.Start,
                Max(adjustedDate, task.Start + minimumDuration)),
            _ => throw new ArgumentOutOfRangeException(nameof(gesture), gesture, "The schedule gesture is not supported.")
        };

        GanttSchedulePropagation.ValidateTaskConstraint(graph, proposedTask, graph.TasksById);
        var updatedTasks = graph.TasksById.ToDictionary(static pair => pair.Key, static pair => pair.Value);
        updatedTasks[taskId] = proposedTask;
        GanttSchedulePropagation.Propagate(graph, graph, updatedTasks, taskId);

        return BuildRequest(graph, updatedTasks, taskId, gesture);
    }

    public static GanttTaskScheduleChangeRequest PlanInterval(
        IReadOnlyCollection<GanttTask> tasks,
        IReadOnlyCollection<GanttDependency> dependencies,
        GanttTaskId taskId,
        DateTimeOffset proposedStart,
        DateTimeOffset proposedEnd,
        GanttSnapGrid? snapGrid = null,
        TimeSpan? minimumTaskDuration = null)
    {
        GanttIdentifierGuard.Ensure(taskId, nameof(taskId));
        var graph = GanttScheduleGraph.Create(tasks, dependencies);
        GanttSchedulePropagation.ValidateConstraints(graph, graph.TasksById);
        if (!graph.TasksById.TryGetValue(taskId, out var task))
        {
            throw new GanttScheduleException(
                GanttScheduleErrorCode.TaskNotFound,
                $"Task '{taskId}' does not exist.");
        }

        var minimumDuration = ResolveMinimumTaskDuration(minimumTaskDuration);

        var adjustedStart = snapGrid is { } startGrid
            ? GanttSnapper.Snap(proposedStart, startGrid)
            : proposedStart;
        var adjustedEnd = snapGrid is { } endGrid
            ? GanttSnapper.Snap(proposedEnd, endGrid)
            : proposedEnd;
        if (adjustedEnd <= adjustedStart || adjustedEnd - adjustedStart < minimumDuration)
        {
            throw new ArgumentOutOfRangeException(
                nameof(proposedEnd),
                proposedEnd,
                $"The proposed task interval must be at least {minimumDuration} and end after it starts.");
        }

        var proposedTask = CopyWithDates(task, adjustedStart, adjustedEnd);
        GanttSchedulePropagation.ValidateTaskConstraint(graph, proposedTask, graph.TasksById);
        var updatedTasks = graph.TasksById.ToDictionary(static pair => pair.Key, static pair => pair.Value);
        updatedTasks[taskId] = proposedTask;
        GanttSchedulePropagation.Propagate(graph, graph, updatedTasks, taskId);

        return BuildRequest(graph, updatedTasks, taskId, GanttScheduleGesture.SetInterval);
    }

    internal static GanttTask CopyWithDates(GanttTask task, DateTimeOffset start, DateTimeOffset end)
        => new(task.Id, task.Title, start, end, task.Assignments)
        {
            ProgressPercent = task.ProgressPercent,
            ExpectedEffort = task.ExpectedEffort
        };

    private static DateTimeOffset Min(DateTimeOffset left, DateTimeOffset right)
        => left <= right ? left : right;

    private static DateTimeOffset Max(DateTimeOffset left, DateTimeOffset right)
        => left >= right ? left : right;

    private static TimeSpan ResolveMinimumTaskDuration(TimeSpan? minimumTaskDuration)
    {
        var resolved = minimumTaskDuration ?? TimeSpan.Zero;
        if (resolved < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumTaskDuration), minimumTaskDuration, "The minimum task duration cannot be negative.");
        }

        return resolved;
    }

    internal static GanttTaskScheduleChangeRequest BuildRequest(
        GanttScheduleGraph originalGraph,
        IReadOnlyDictionary<GanttTaskId, GanttTask> updatedTasks,
        GanttTaskId taskId,
        GanttScheduleGesture gesture)
    {
        var criticalTaskIds = GanttCriticalPathCalculator.Calculate(
            originalGraph.Tasks.Select(task => updatedTasks[task.Id]).ToArray(),
            originalGraph.Dependencies);
        var criticalSet = criticalTaskIds.ToHashSet();
        var changes = originalGraph.Tasks
            .Where(task => task.Start != updatedTasks[task.Id].Start || task.End != updatedTasks[task.Id].End)
            .Select(task => new GanttTaskDateChange(
                task.Id,
                task.Start,
                task.End,
                updatedTasks[task.Id].Start,
                updatedTasks[task.Id].End,
                criticalSet.Contains(task.Id)))
            .ToArray();

        return new GanttTaskScheduleChangeRequest(taskId, gesture, changes, criticalTaskIds);
    }
}

internal static class GanttSchedulePropagation
{
    public static void ValidateConstraints(
        GanttScheduleGraph graph,
        IReadOnlyDictionary<GanttTaskId, GanttTask> tasks)
    {
        foreach (var task in graph.Tasks)
        {
            ValidateTaskConstraint(graph, tasks[task.Id], tasks);
        }
    }

    public static void ValidateTaskConstraint(
        GanttScheduleGraph graph,
        GanttTask task,
        IReadOnlyDictionary<GanttTaskId, GanttTask> tasks)
    {
        var predecessors = graph.Predecessors[task.Id];
        if (predecessors.Count == 0)
        {
            return;
        }

        var requiredStart = predecessors.Max(predecessorId => tasks[predecessorId].End);
        if (task.Start < requiredStart)
        {
            throw new GanttScheduleException(
                GanttScheduleErrorCode.DependencyConstraintViolation,
                $"Task '{task.Id}' starts before its finish-to-start prerequisites complete.");
        }
    }

    public static void Propagate(
        GanttScheduleGraph originalGraph,
        GanttScheduleGraph updatedGraph,
        IDictionary<GanttTaskId, GanttTask> updatedTasks,
        GanttTaskId excludedTaskId)
    {
        foreach (var taskId in updatedGraph.Validation.TopologicalOrder)
        {
            if (taskId == excludedTaskId || !originalGraph.TasksById.TryGetValue(taskId, out var originalTask))
            {
                continue;
            }

            var originalPredecessors = originalGraph.Predecessors[taskId];
            var updatedPredecessors = updatedGraph.Predecessors[taskId];
            if (originalPredecessors.Count == 0 || updatedPredecessors.Count == 0)
            {
                continue;
            }

            var originalRequiredStart = originalPredecessors.Max(predecessorId => originalGraph.TasksById[predecessorId].End);
            var slack = originalTask.Start - originalRequiredStart;
            if (slack < TimeSpan.Zero)
            {
                throw new GanttScheduleException(
                    GanttScheduleErrorCode.DependencyConstraintViolation,
                    $"Task '{taskId}' starts before its finish-to-start prerequisites complete.");
            }

            var updatedRequiredStart = updatedPredecessors.Max(predecessorId => updatedTasks[predecessorId].End);
            var updatedStart = updatedRequiredStart.ToOffset(originalTask.Start.Offset) + slack;
            if (updatedStart != originalTask.Start)
            {
                updatedTasks[taskId] = GanttSchedulePlanner.CopyWithDates(
                    originalTask,
                    updatedStart,
                    updatedStart + originalTask.Duration);
            }
        }
    }
}
