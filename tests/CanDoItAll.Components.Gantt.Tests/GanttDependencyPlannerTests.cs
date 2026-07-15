using CanDoItAll.Components.Gantt;

namespace CanDoItAll.Components.Gantt.Tests;

public sealed class GanttDependencyPlannerTests
{
    private static readonly DateTimeOffset Start = new(2026, 7, 14, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Adding_a_temporally_constrained_edge_shifts_the_successor_chain_and_returns_a_schedulable_plan()
    {
        var tasks = new[]
        {
            Task("root", 0, 4),
            Task("successor", 2, 4),
            Task("downstream", 4, 5)
        };
        var existingDependency = Dependency("successor-downstream", "successor", "downstream");
        var addedDependency = Dependency("root-successor", "root", "successor");

        var request = GanttDependencyPlanner.PlanAdd(tasks, [existingDependency], addedDependency);

        Assert.Equal(GanttDependencyMutationKind.Add, request.Mutation);
        Assert.Equal(addedDependency, request.ProposedDependency);
        AssertChange(request, "successor", 4, 6);
        AssertChange(request, "downstream", 6, 7);

        var proposedTasks = ApplyChanges(tasks, request.AffectedTasks);
        AssertSchedulable(proposedTasks, [existingDependency, addedDependency]);
    }

    [Fact]
    public void Reconnecting_to_a_later_predecessor_shifts_the_successor_chain_and_returns_a_schedulable_plan()
    {
        var tasks = new[]
        {
            Task("root", 0, 2),
            Task("later-root", 0, 6),
            Task("successor", 2, 4),
            Task("downstream", 4, 6)
        };
        var reconnectedDependency = Dependency("root-successor", "root", "successor");
        var downstreamDependency = Dependency("successor-downstream", "successor", "downstream");
        var dependencies = new[] { reconnectedDependency, downstreamDependency };

        var request = GanttDependencyPlanner.PlanReconnect(
            tasks,
            dependencies,
            reconnectedDependency.Id,
            TaskId("later-root"),
            TaskId("successor"));

        Assert.Equal(GanttDependencyMutationKind.Reconnect, request.Mutation);
        Assert.Equal(reconnectedDependency, request.PreviousDependency);
        Assert.NotNull(request.ProposedDependency);
        Assert.Equal(TaskId("later-root"), request.ProposedDependency.PredecessorId);
        Assert.Equal(TaskId("successor"), request.ProposedDependency.SuccessorId);
        AssertChange(request, "successor", 6, 8);
        AssertChange(request, "downstream", 8, 10);

        var proposedTasks = ApplyChanges(tasks, request.AffectedTasks);
        AssertSchedulable(proposedTasks, [request.ProposedDependency, downstreamDependency]);
    }

    private static void AssertChange(
        GanttDependencyMutationRequest request,
        string taskId,
        int proposedStartHour,
        int proposedEndHour)
    {
        var change = Assert.Single(request.AffectedTasks, change => change.TaskId == TaskId(taskId));
        Assert.Equal(Start.AddHours(proposedStartHour), change.ProposedStart);
        Assert.Equal(Start.AddHours(proposedEndHour), change.ProposedEnd);
    }

    private static GanttTask[] ApplyChanges(
        IEnumerable<GanttTask> tasks,
        IReadOnlyCollection<GanttTaskDateChange> changes)
    {
        var changesByTaskId = changes.ToDictionary(static change => change.TaskId);
        return tasks
            .Select(task => changesByTaskId.TryGetValue(task.Id, out var change)
                ? new GanttTask(task.Id, task.Title, change.ProposedStart, change.ProposedEnd, task.Assignments)
                : task)
            .ToArray();
    }

    private static void AssertSchedulable(
        IReadOnlyCollection<GanttTask> tasks,
        IReadOnlyCollection<GanttDependency> dependencies)
    {
        var root = tasks.First(task => dependencies.All(dependency => dependency.SuccessorId != task.Id));
        var exception = Record.Exception(() => GanttSchedulePlanner.Plan(
            tasks,
            dependencies,
            root.Id,
            GanttScheduleGesture.Move,
            root.Start));

        Assert.Null(exception);
    }

    private static GanttTask Task(string id, int startHour, int endHour)
        => new(TaskId(id), id, Start.AddHours(startHour), Start.AddHours(endHour));

    private static GanttDependency Dependency(string id, string predecessorId, string successorId)
        => new(new GanttDependencyId(id), TaskId(predecessorId), TaskId(successorId));

    private static GanttTaskId TaskId(string value)
        => new(value);
}
