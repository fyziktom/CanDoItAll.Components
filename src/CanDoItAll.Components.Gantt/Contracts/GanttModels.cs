namespace CanDoItAll.Components.Gantt;

public sealed record GanttAssignment
{
    public GanttAssignment(GanttAssignmentKind kind, string name)
    {
        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(nameof(kind), kind, "The assignment kind is not supported.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("An assignment name is required.", nameof(name));
        }

        Kind = kind;
        Name = name.Trim();
    }

    public GanttAssignmentKind Kind { get; }

    public string Name { get; }
}

public sealed record GanttTask
{
    private int? progressPercent;
    private TimeSpan? expectedEffort;

    public GanttTask(GanttTaskId id, string title, DateTimeOffset start, DateTimeOffset end)
        : this(id, title, start, end, [])
    {
    }

    public GanttTask(
        GanttTaskId id,
        string title,
        DateTimeOffset start,
        DateTimeOffset end,
        IEnumerable<GanttAssignment> assignments)
    {
        GanttIdentifierGuard.Ensure(id, nameof(id));
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("A Gantt task title is required.", nameof(title));
        }

        if (end <= start)
        {
            throw new ArgumentOutOfRangeException(nameof(end), end, "A Gantt task must end after it starts.");
        }

        ArgumentNullException.ThrowIfNull(assignments);
        var assignmentArray = assignments.ToArray();
        if (assignmentArray.Any(static assignment => assignment is null))
        {
            throw new ArgumentException("Assignments cannot contain null values.", nameof(assignments));
        }

        Id = id;
        Title = title.Trim();
        Start = start;
        End = end;
        Assignments = Array.AsReadOnly(assignmentArray);
    }

    public GanttTaskId Id { get; }

    public string Title { get; }

    public DateTimeOffset Start { get; }

    public DateTimeOffset End { get; }

    public TimeSpan Duration => End - Start;

    public IReadOnlyList<GanttAssignment> Assignments { get; }

    public int? ProgressPercent
    {
        get => progressPercent;
        init
        {
            if (value is < 0 or > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(ProgressPercent), value, "Task progress must be between 0 and 100 percent.");
            }

            progressPercent = value;
        }
    }

    public TimeSpan? ExpectedEffort
    {
        get => expectedEffort;
        init
        {
            if (value is { } effort && effort <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(ExpectedEffort), value, "Expected task effort must be greater than zero.");
            }

            expectedEffort = value;
        }
    }
}

public sealed record GanttDependency
{
    public GanttDependency(
        GanttDependencyId id,
        GanttTaskId predecessorId,
        GanttTaskId successorId,
        GanttDependencyType type = GanttDependencyType.FinishToStart)
    {
        GanttIdentifierGuard.Ensure(id, nameof(id));
        GanttIdentifierGuard.Ensure(predecessorId, nameof(predecessorId));
        GanttIdentifierGuard.Ensure(successorId, nameof(successorId));
        if (predecessorId == successorId)
        {
            throw new ArgumentException("A task cannot depend on itself.", nameof(successorId));
        }

        if (type is not GanttDependencyType.FinishToStart)
        {
            throw new ArgumentOutOfRangeException(nameof(type), type, "The dependency type is not supported.");
        }

        Id = id;
        PredecessorId = predecessorId;
        SuccessorId = successorId;
        Type = type;
    }

    public GanttDependencyId Id { get; }

    public GanttTaskId PredecessorId { get; }

    public GanttTaskId SuccessorId { get; }

    public GanttDependencyType Type { get; }
}
