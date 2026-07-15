using CanDoItAll.Components.Gantt;

namespace CanDoItAll.Components.Gantt.Tests;

public sealed class GanttDagValidatorTests
{
    [Fact]
    public void Multiple_prerequisites_are_ordered_before_their_successor()
    {
        var tasks = new[]
        {
            Task("design", 0, 2),
            Task("security", 0, 3),
            Task("build", 3, 5)
        };
        var dependencies = new[]
        {
            Dependency("design-build", "design", "build"),
            Dependency("security-build", "security", "build")
        };

        var result = GanttDagValidator.Validate(tasks, dependencies);

        Assert.True(result.TopologicalOrder.IndexOf(TaskId("design")) < result.TopologicalOrder.IndexOf(TaskId("build")));
        Assert.True(result.TopologicalOrder.IndexOf(TaskId("security")) < result.TopologicalOrder.IndexOf(TaskId("build")));
    }

    [Fact]
    public void Cycle_is_rejected_with_a_typed_error()
    {
        var tasks = new[] { Task("a", 0, 1), Task("b", 1, 2), Task("c", 2, 3) };
        var dependencies = new[]
        {
            Dependency("a-b", "a", "b"),
            Dependency("b-c", "b", "c"),
            Dependency("c-a", "c", "a")
        };

        var exception = Assert.Throws<GanttScheduleException>(() => GanttDagValidator.Validate(tasks, dependencies));

        Assert.Equal(GanttScheduleErrorCode.CycleDetected, exception.Code);
    }

    [Fact]
    public void Missing_endpoint_is_rejected()
    {
        var tasks = new[] { Task("a", 0, 1) };
        var dependencies = new[] { Dependency("a-missing", "a", "missing") };

        var exception = Assert.Throws<GanttScheduleException>(() => GanttDagValidator.Validate(tasks, dependencies));

        Assert.Equal(GanttScheduleErrorCode.MissingSuccessor, exception.Code);
    }

    [Fact]
    public void Duplicate_edge_is_rejected_even_with_a_different_identifier()
    {
        var tasks = new[] { Task("a", 0, 1), Task("b", 1, 2) };
        var dependencies = new[]
        {
            Dependency("a-b-1", "a", "b"),
            Dependency("a-b-2", "a", "b")
        };

        var exception = Assert.Throws<GanttScheduleException>(() => GanttDagValidator.Validate(tasks, dependencies));

        Assert.Equal(GanttScheduleErrorCode.DuplicateDependency, exception.Code);
    }

    [Fact]
    public void Duplicate_task_identifier_is_rejected()
    {
        var tasks = new[] { Task("duplicate", 0, 1), Task("duplicate", 2, 3) };

        var exception = Assert.Throws<GanttScheduleException>(() => GanttDagValidator.Validate(tasks, []));

        Assert.Equal(GanttScheduleErrorCode.DuplicateTaskId, exception.Code);
    }

    [Fact]
    public void Duplicate_dependency_identifier_is_rejected()
    {
        var tasks = new[] { Task("a", 0, 1), Task("b", 1, 2), Task("c", 1, 2) };
        var dependencies = new[]
        {
            Dependency("duplicate", "a", "b"),
            Dependency("duplicate", "a", "c")
        };

        var exception = Assert.Throws<GanttScheduleException>(() => GanttDagValidator.Validate(tasks, dependencies));

        Assert.Equal(GanttScheduleErrorCode.DuplicateDependencyId, exception.Code);
    }

    [Fact]
    public void Invalid_task_interval_is_rejected_at_the_contract_boundary()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GanttTask(
            TaskId("invalid"),
            "Invalid",
            Start.AddHours(2),
            Start.AddHours(2)));
    }

    private static readonly DateTimeOffset Start = new(2026, 7, 14, 8, 0, 0, TimeSpan.Zero);

    private static GanttTask Task(string id, int startHour, int endHour)
        => new(TaskId(id), id, Start.AddHours(startHour), Start.AddHours(endHour));

    private static GanttDependency Dependency(string id, string predecessorId, string successorId)
        => new(new GanttDependencyId(id), TaskId(predecessorId), TaskId(successorId));

    private static GanttTaskId TaskId(string value)
        => new(value);
}

internal static class ReadOnlyListTestExtensions
{
    public static int IndexOf<T>(this IReadOnlyList<T> values, T value)
        => values.Select((candidate, index) => (candidate, index))
            .Single(pair => EqualityComparer<T>.Default.Equals(pair.candidate, value))
            .index;
}
