namespace CanDoItAll.Components.Gantt;

public readonly record struct GanttTaskId
{
    public GanttTaskId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A Gantt task identifier is required.", nameof(value));
        }

        Value = value.Trim();
    }

    public string Value { get; }

    public override string ToString()
        => Value ?? string.Empty;
}

public readonly record struct GanttDependencyId
{
    public GanttDependencyId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A Gantt dependency identifier is required.", nameof(value));
        }

        Value = value.Trim();
    }

    public string Value { get; }

    public override string ToString()
        => Value ?? string.Empty;
}

internal static class GanttIdentifierGuard
{
    public static void Ensure(GanttTaskId taskId, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(taskId.Value))
        {
            throw new ArgumentException("A Gantt task identifier is required.", parameterName);
        }
    }

    public static void Ensure(GanttDependencyId dependencyId, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(dependencyId.Value))
        {
            throw new ArgumentException("A Gantt dependency identifier is required.", parameterName);
        }
    }
}
