namespace CanDoItAll.Components.Gantt;

public static class GanttCriticalPathCalculator
{
    public static IReadOnlyList<GanttTaskId> Calculate(
        IReadOnlyCollection<GanttTask> tasks,
        IReadOnlyCollection<GanttDependency> dependencies)
        => Calculate(GanttScheduleGraph.Create(tasks, dependencies));

    internal static IReadOnlyList<GanttTaskId> Calculate(GanttScheduleGraph graph)
    {
        if (graph.Tasks.Count == 0)
        {
            return Array.Empty<GanttTaskId>();
        }

        var earliestStart = new Dictionary<GanttTaskId, TimeSpan>(graph.Tasks.Count);
        var earliestFinish = new Dictionary<GanttTaskId, TimeSpan>(graph.Tasks.Count);
        foreach (var taskId in graph.Validation.TopologicalOrder)
        {
            var start = graph.Predecessors[taskId].Count == 0
                ? TimeSpan.Zero
                : graph.Predecessors[taskId].Max(predecessorId => earliestFinish[predecessorId]);
            earliestStart[taskId] = start;
            earliestFinish[taskId] = start + graph.TasksById[taskId].Duration;
        }

        var projectDuration = earliestFinish.Values.Max();
        var latestStart = new Dictionary<GanttTaskId, TimeSpan>(graph.Tasks.Count);
        var latestFinish = new Dictionary<GanttTaskId, TimeSpan>(graph.Tasks.Count);
        foreach (var taskId in graph.Validation.TopologicalOrder.Reverse())
        {
            var finish = graph.Successors[taskId].Count == 0
                ? projectDuration
                : graph.Successors[taskId].Min(successorId => latestStart[successorId]);
            latestFinish[taskId] = finish;
            latestStart[taskId] = finish - graph.TasksById[taskId].Duration;
        }

        return Array.AsReadOnly(
            graph.Validation.TopologicalOrder
                .Where(taskId => latestStart[taskId] == earliestStart[taskId])
                .ToArray());
    }
}
