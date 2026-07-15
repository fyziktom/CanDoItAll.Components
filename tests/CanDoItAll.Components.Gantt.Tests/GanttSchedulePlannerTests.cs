using CanDoItAll.Components.Gantt;

namespace CanDoItAll.Components.Gantt.Tests;

public sealed class GanttSchedulePlannerTests
{
    [Fact]
    public void Move_propagates_forward_using_the_latest_of_multiple_prerequisites()
    {
        var (tasks, dependencies) = CreateBranchedSchedule();

        var request = GanttSchedulePlanner.Plan(
            tasks,
            dependencies,
            TaskId("security"),
            GanttScheduleGesture.Move,
            Start.AddHours(2));

        AssertChange(request, "security", 2, 5);
        AssertChange(request, "build", 6, 8);
        AssertChange(request, "ship", 8, 9);
        Assert.DoesNotContain(request.AffectedTasks, change => change.TaskId == TaskId("design"));
    }

    [Fact]
    public void Move_propagates_backward_without_crossing_another_prerequisite()
    {
        var (tasks, dependencies) = CreateBranchedSchedule();

        var request = GanttSchedulePlanner.Plan(
            tasks,
            dependencies,
            TaskId("security"),
            GanttScheduleGesture.Move,
            Start.AddHours(-1));

        AssertChange(request, "security", -1, 2);
        AssertChange(request, "build", 3, 5);
        AssertChange(request, "ship", 5, 6);
    }

    [Fact]
    public void Resize_end_changes_duration_and_propagates_dependents()
    {
        var (tasks, dependencies) = CreateBranchedSchedule();

        var request = GanttSchedulePlanner.Plan(
            tasks,
            dependencies,
            TaskId("security"),
            GanttScheduleGesture.ResizeEnd,
            Start.AddHours(4));

        AssertChange(request, "security", 0, 4);
        AssertChange(request, "build", 5, 7);
        AssertChange(request, "ship", 7, 8);
    }

    [Fact]
    public void Resize_start_changes_duration_without_moving_successors_when_the_end_is_unchanged()
    {
        var (tasks, dependencies) = CreateBranchedSchedule();

        var request = GanttSchedulePlanner.Plan(
            tasks,
            dependencies,
            TaskId("security"),
            GanttScheduleGesture.ResizeStart,
            Start.AddHours(1));

        AssertChange(request, "security", 1, 3);
        Assert.Single(request.AffectedTasks);
    }

    [Fact]
    public void Proposed_dates_snap_to_the_explicit_grid()
    {
        GanttTask[] tasks = [Task("task", 0, 2)];
        var grid = new GanttSnapGrid(Start, TimeSpan.FromHours(1));

        var request = GanttSchedulePlanner.Plan(
            tasks,
            [],
            TaskId("task"),
            GanttScheduleGesture.Move,
            Start.AddHours(3).AddMinutes(31),
            grid);

        AssertChange(request, "task", 4, 6);
    }

    [Fact]
    public void Resize_start_enforces_the_minimum_duration_against_an_off_grid_end_after_snapping()
    {
        var task = new GanttTask(
            TaskId("task"),
            "task",
            Start.AddMinutes(20),
            Start.AddHours(2).AddMinutes(30));
        var grid = new GanttSnapGrid(Start, TimeSpan.FromHours(1));

        var request = GanttSchedulePlanner.Plan(
            [task],
            [],
            task.Id,
            GanttScheduleGesture.ResizeStart,
            Start.AddHours(2).AddMinutes(40),
            grid,
            TimeSpan.FromHours(1));

        var change = Assert.Single(request.AffectedTasks);
        Assert.Equal(Start.AddHours(1).AddMinutes(30), change.ProposedStart);
        Assert.Equal(task.End, change.ProposedEnd);
        Assert.Equal(TimeSpan.FromHours(1), change.ProposedEnd - change.ProposedStart);
    }

    [Fact]
    public void Resize_end_enforces_the_minimum_duration_against_an_off_grid_start_after_snapping()
    {
        var task = new GanttTask(
            TaskId("task"),
            "task",
            Start.AddMinutes(20),
            Start.AddHours(2).AddMinutes(30));
        var grid = new GanttSnapGrid(Start, TimeSpan.FromHours(1));

        var request = GanttSchedulePlanner.Plan(
            [task],
            [],
            task.Id,
            GanttScheduleGesture.ResizeEnd,
            Start.AddMinutes(40),
            grid,
            TimeSpan.FromHours(1));

        var change = Assert.Single(request.AffectedTasks);
        Assert.Equal(task.Start, change.ProposedStart);
        Assert.Equal(Start.AddHours(1).AddMinutes(20), change.ProposedEnd);
        Assert.Equal(TimeSpan.FromHours(1), change.ProposedEnd - change.ProposedStart);
    }

    [Fact]
    public void Critical_path_marks_the_longest_network_path()
    {
        var tasks = new[]
        {
            Task("long-root", 0, 3),
            Task("short-root", 0, 1),
            Task("long-end", 3, 5),
            Task("short-end", 1, 2)
        };
        var dependencies = new[]
        {
            Dependency("long", "long-root", "long-end"),
            Dependency("short", "short-root", "short-end")
        };

        var critical = GanttCriticalPathCalculator.Calculate(tasks, dependencies);

        Assert.Equal([TaskId("long-root"), TaskId("long-end")], critical);
    }

    private static (GanttTask[] Tasks, GanttDependency[] Dependencies) CreateBranchedSchedule()
    {
        GanttTask[] tasks =
        [
            Task("design", 0, 2),
            Task("security", 0, 3),
            Task("build", 4, 6),
            Task("ship", 6, 7)
        ];
        GanttDependency[] dependencies =
        [
            Dependency("design-build", "design", "build"),
            Dependency("security-build", "security", "build"),
            Dependency("build-ship", "build", "ship")
        ];
        return (tasks, dependencies);
    }

    private static void AssertChange(
        GanttTaskScheduleChangeRequest request,
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
