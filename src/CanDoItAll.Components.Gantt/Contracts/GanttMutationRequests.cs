namespace CanDoItAll.Components.Gantt;

public sealed record GanttTaskTitleChangeRequest
{
    public GanttTaskTitleChangeRequest(GanttTaskId taskId, string currentTitle, string proposedTitle)
    {
        GanttIdentifierGuard.Ensure(taskId, nameof(taskId));
        if (string.IsNullOrWhiteSpace(currentTitle))
        {
            throw new ArgumentException("The current title is required.", nameof(currentTitle));
        }

        if (string.IsNullOrWhiteSpace(proposedTitle))
        {
            throw new ArgumentException("The proposed title is required.", nameof(proposedTitle));
        }

        TaskId = taskId;
        CurrentTitle = currentTitle;
        ProposedTitle = proposedTitle.Trim();
    }

    public GanttTaskId TaskId { get; }

    public string CurrentTitle { get; }

    public string ProposedTitle { get; }
}

public sealed record GanttTaskDateChange
{
    public GanttTaskDateChange(
        GanttTaskId taskId,
        DateTimeOffset previousStart,
        DateTimeOffset previousEnd,
        DateTimeOffset proposedStart,
        DateTimeOffset proposedEnd,
        bool isCritical)
    {
        GanttIdentifierGuard.Ensure(taskId, nameof(taskId));
        if (previousEnd <= previousStart)
        {
            throw new ArgumentOutOfRangeException(nameof(previousEnd), previousEnd, "The previous task interval is invalid.");
        }

        if (proposedEnd <= proposedStart)
        {
            throw new ArgumentOutOfRangeException(nameof(proposedEnd), proposedEnd, "The proposed task interval is invalid.");
        }

        TaskId = taskId;
        PreviousStart = previousStart;
        PreviousEnd = previousEnd;
        ProposedStart = proposedStart;
        ProposedEnd = proposedEnd;
        IsCritical = isCritical;
    }

    public GanttTaskId TaskId { get; }

    public DateTimeOffset PreviousStart { get; }

    public DateTimeOffset PreviousEnd { get; }

    public DateTimeOffset ProposedStart { get; }

    public DateTimeOffset ProposedEnd { get; }

    public bool IsCritical { get; }
}

public sealed record GanttTaskScheduleChangeRequest
{
    public GanttTaskScheduleChangeRequest(
        GanttTaskId taskId,
        GanttScheduleGesture gesture,
        IEnumerable<GanttTaskDateChange> affectedTasks,
        IEnumerable<GanttTaskId> criticalTaskIds)
    {
        GanttIdentifierGuard.Ensure(taskId, nameof(taskId));
        if (!Enum.IsDefined(gesture))
        {
            throw new ArgumentOutOfRangeException(nameof(gesture), gesture, "The schedule gesture is not supported.");
        }

        AffectedTasks = CopyRequired(affectedTasks, nameof(affectedTasks));
        CriticalTaskIds = CopyRequired(criticalTaskIds, nameof(criticalTaskIds));
        TaskId = taskId;
        Gesture = gesture;
    }

    public GanttTaskId TaskId { get; }

    public GanttScheduleGesture Gesture { get; }

    public IReadOnlyList<GanttTaskDateChange> AffectedTasks { get; }

    public IReadOnlyList<GanttTaskId> CriticalTaskIds { get; }

    private static IReadOnlyList<T> CopyRequired<T>(IEnumerable<T> values, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(values, parameterName);
        return Array.AsReadOnly(values.ToArray());
    }
}

public sealed record GanttDependencyMutationRequest
{
    public GanttDependencyMutationRequest(
        GanttDependencyMutationKind mutation,
        GanttDependency? previousDependency,
        GanttDependency? proposedDependency,
        IEnumerable<GanttTaskDateChange>? affectedTasks = null,
        IEnumerable<GanttTaskId>? criticalTaskIds = null)
    {
        if (!Enum.IsDefined(mutation))
        {
            throw new ArgumentOutOfRangeException(nameof(mutation), mutation, "The dependency mutation is not supported.");
        }

        var valid = mutation switch
        {
            GanttDependencyMutationKind.Add => previousDependency is null && proposedDependency is not null,
            GanttDependencyMutationKind.Remove => previousDependency is not null && proposedDependency is null,
            GanttDependencyMutationKind.Reconnect => previousDependency is not null &&
                                                      proposedDependency is not null &&
                                                      previousDependency.Id == proposedDependency.Id,
            _ => false
        };
        if (!valid)
        {
            throw new ArgumentException($"The dependency values do not match mutation {mutation}.");
        }

        Mutation = mutation;
        PreviousDependency = previousDependency;
        ProposedDependency = proposedDependency;
        AffectedTasks = Array.AsReadOnly((affectedTasks ?? []).ToArray());
        CriticalTaskIds = Array.AsReadOnly((criticalTaskIds ?? []).ToArray());
    }

    public GanttDependencyMutationKind Mutation { get; }

    public GanttDependency? PreviousDependency { get; }

    public GanttDependency? ProposedDependency { get; }

    public IReadOnlyList<GanttTaskDateChange> AffectedTasks { get; }

    public IReadOnlyList<GanttTaskId> CriticalTaskIds { get; }
}

public sealed record GanttTaskInsertionRequest
{
    public GanttTaskInsertionRequest(
        GanttTask insertedTask,
        GanttTaskId predecessorId,
        GanttTaskId successorId,
        IEnumerable<GanttDependencyMutationRequest> dependencyChanges,
        IEnumerable<GanttTaskDateChange> affectedTasks,
        IEnumerable<GanttTaskId> criticalTaskIds)
    {
        ArgumentNullException.ThrowIfNull(insertedTask);
        GanttIdentifierGuard.Ensure(predecessorId, nameof(predecessorId));
        GanttIdentifierGuard.Ensure(successorId, nameof(successorId));
        ArgumentNullException.ThrowIfNull(dependencyChanges);
        ArgumentNullException.ThrowIfNull(affectedTasks);
        ArgumentNullException.ThrowIfNull(criticalTaskIds);

        InsertedTask = insertedTask;
        PredecessorId = predecessorId;
        SuccessorId = successorId;
        DependencyChanges = Array.AsReadOnly(dependencyChanges.ToArray());
        AffectedTasks = Array.AsReadOnly(affectedTasks.ToArray());
        CriticalTaskIds = Array.AsReadOnly(criticalTaskIds.ToArray());
    }

    public GanttTask InsertedTask { get; }

    public GanttTaskId PredecessorId { get; }

    public GanttTaskId SuccessorId { get; }

    public IReadOnlyList<GanttDependencyMutationRequest> DependencyChanges { get; }

    public IReadOnlyList<GanttTaskDateChange> AffectedTasks { get; }

    public IReadOnlyList<GanttTaskId> CriticalTaskIds { get; }
}
