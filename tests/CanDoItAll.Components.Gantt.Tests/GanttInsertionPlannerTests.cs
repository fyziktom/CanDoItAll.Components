using CanDoItAll.Components.Gantt;

namespace CanDoItAll.Components.Gantt.Tests;

public sealed class GanttInsertionPlannerTests
{
    [Fact]
    public void Insertion_rewires_the_selected_edge_and_preserves_unrelated_prerequisites()
    {
        var tasks = new[]
        {
            Task("predecessor", 0, 2),
            Task("unrelated", 0, 3),
            Task("successor", 3, 5),
            Task("downstream", 5, 6)
        };
        var bridge = Dependency("bridge", "predecessor", "successor");
        var unrelated = Dependency("unrelated-successor", "unrelated", "successor");
        var downstream = Dependency("successor-downstream", "successor", "downstream");
        var inserted = Task("inserted", -4, -2);

        var request = GanttInsertionPlanner.Plan(
            tasks,
            [bridge, unrelated, downstream],
            inserted,
            TaskId("predecessor"),
            TaskId("successor"),
            new GanttDependencyId("predecessor-inserted"),
            new GanttDependencyId("inserted-successor"));

        Assert.Equal(Start.AddHours(2), request.InsertedTask.Start);
        Assert.Equal(Start.AddHours(4), request.InsertedTask.End);
        Assert.Collection(
            request.DependencyChanges,
            change =>
            {
                Assert.Equal(GanttDependencyMutationKind.Remove, change.Mutation);
                Assert.Equal(bridge, change.PreviousDependency);
            },
            change => AssertDependency(change, "predecessor", "inserted"),
            change => AssertDependency(change, "inserted", "successor"));
        Assert.DoesNotContain(
            request.DependencyChanges,
            change => change.PreviousDependency?.Id == unrelated.Id || change.ProposedDependency?.Id == unrelated.Id);
        AssertChange(request, "successor", 4, 6);
        AssertChange(request, "downstream", 6, 7);
    }

    [Fact]
    public void Reconnecting_an_edge_into_a_cycle_is_rejected()
    {
        var tasks = new[] { Task("a", 0, 1), Task("b", 1, 2), Task("c", 2, 3) };
        var dependencies = new[]
        {
            Dependency("a-b", "a", "b"),
            Dependency("b-c", "b", "c")
        };

        var exception = Assert.Throws<GanttScheduleException>(() => GanttDependencyPlanner.PlanReconnect(
            tasks,
            dependencies,
            new GanttDependencyId("a-b"),
            TaskId("c"),
            TaskId("b")));

        Assert.Equal(GanttScheduleErrorCode.CycleDetected, exception.Code);
    }

    private static void AssertDependency(
        GanttDependencyMutationRequest change,
        string predecessorId,
        string successorId)
    {
        Assert.Equal(GanttDependencyMutationKind.Add, change.Mutation);
        Assert.NotNull(change.ProposedDependency);
        Assert.Equal(TaskId(predecessorId), change.ProposedDependency.PredecessorId);
        Assert.Equal(TaskId(successorId), change.ProposedDependency.SuccessorId);
    }

    private static void AssertChange(
        GanttTaskInsertionRequest request,
        string taskId,
        int proposedStartHour,
        int proposedEndHour)
    {
        var change = Assert.Single(request.AffectedTasks, change => change.TaskId == TaskId(taskId));
        Assert.Equal(Start.AddHours(proposedStartHour), change.ProposedStart);
        Assert.Equal(Start.AddHours(proposedEndHour), change.ProposedEnd);
    }

    private static readonly DateTimeOffset Start = new(2026, 7, 14, 8, 0, 0, TimeSpan.Zero);

    private static GanttTask Task(string id, int startHour, int endHour)
        => new(TaskId(id), id, Start.AddHours(startHour), Start.AddHours(endHour));

    private static GanttDependency Dependency(string id, string predecessorId, string successorId)
        => new(new GanttDependencyId(id), TaskId(predecessorId), TaskId(successorId));

    private static GanttTaskId TaskId(string value)
        => new(value);
}
