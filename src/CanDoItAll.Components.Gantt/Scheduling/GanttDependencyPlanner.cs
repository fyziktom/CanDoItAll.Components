namespace CanDoItAll.Components.Gantt;

public static class GanttDependencyPlanner
{
    public static GanttDependencyMutationRequest PlanAdd(
        IReadOnlyCollection<GanttTask> tasks,
        IReadOnlyCollection<GanttDependency> dependencies,
        GanttDependency dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        var graph = GanttScheduleGraph.Create(tasks, dependencies);
        GanttSchedulePropagation.ValidateConstraints(graph, graph.TasksById);
        var candidate = graph.Dependencies.Append(dependency).ToArray();
        return BuildRequest(graph, candidate, GanttDependencyMutationKind.Add, null, dependency);
    }

    public static GanttDependencyMutationRequest PlanRemove(
        IReadOnlyCollection<GanttTask> tasks,
        IReadOnlyCollection<GanttDependency> dependencies,
        GanttDependencyId dependencyId)
    {
        GanttIdentifierGuard.Ensure(dependencyId, nameof(dependencyId));
        var graph = GanttScheduleGraph.Create(tasks, dependencies);
        GanttSchedulePropagation.ValidateConstraints(graph, graph.TasksById);
        var dependency = graph.Dependencies.FirstOrDefault(candidate => candidate.Id == dependencyId)
            ?? throw new GanttScheduleException(
                GanttScheduleErrorCode.DependencyNotFound,
                $"Dependency '{dependencyId}' does not exist.");
        var candidate = graph.Dependencies.Where(candidate => candidate.Id != dependencyId).ToArray();
        var criticalTaskIds = GanttCriticalPathCalculator.Calculate(graph.Tasks, candidate);
        return new GanttDependencyMutationRequest(
            GanttDependencyMutationKind.Remove,
            dependency,
            null,
            criticalTaskIds: criticalTaskIds);
    }

    public static GanttDependencyMutationRequest PlanReconnect(
        IReadOnlyCollection<GanttTask> tasks,
        IReadOnlyCollection<GanttDependency> dependencies,
        GanttDependencyId dependencyId,
        GanttTaskId predecessorId,
        GanttTaskId successorId)
    {
        GanttIdentifierGuard.Ensure(dependencyId, nameof(dependencyId));
        var graph = GanttScheduleGraph.Create(tasks, dependencies);
        GanttSchedulePropagation.ValidateConstraints(graph, graph.TasksById);
        var previous = graph.Dependencies.FirstOrDefault(candidate => candidate.Id == dependencyId)
            ?? throw new GanttScheduleException(
                GanttScheduleErrorCode.DependencyNotFound,
                $"Dependency '{dependencyId}' does not exist.");
        var proposed = new GanttDependency(previous.Id, predecessorId, successorId, previous.Type);
        var candidate = graph.Dependencies
            .Select(dependency => dependency.Id == dependencyId ? proposed : dependency)
            .ToArray();
        return BuildRequest(graph, candidate, GanttDependencyMutationKind.Reconnect, previous, proposed);
    }

    private static GanttDependencyMutationRequest BuildRequest(
        GanttScheduleGraph originalGraph,
        IReadOnlyCollection<GanttDependency> candidateDependencies,
        GanttDependencyMutationKind mutation,
        GanttDependency? previousDependency,
        GanttDependency? proposedDependency)
    {
        var candidateGraph = GanttScheduleGraph.Create(originalGraph.Tasks, candidateDependencies);
        var updatedTasks = originalGraph.TasksById.ToDictionary(static pair => pair.Key, static pair => pair.Value);
        foreach (var taskId in candidateGraph.Validation.TopologicalOrder)
        {
            var predecessorIds = candidateGraph.Predecessors[taskId];
            if (predecessorIds.Count == 0)
            {
                continue;
            }

            var task = updatedTasks[taskId];
            var requiredStart = predecessorIds.Max(predecessorId => updatedTasks[predecessorId].End);
            if (task.Start >= requiredStart)
            {
                continue;
            }

            var shiftedStart = requiredStart.ToOffset(task.Start.Offset);
            updatedTasks[taskId] = GanttSchedulePlanner.CopyWithDates(
                task,
                shiftedStart,
                shiftedStart + task.Duration);
        }

        GanttSchedulePropagation.ValidateConstraints(candidateGraph, updatedTasks);
        var criticalTaskIds = GanttCriticalPathCalculator.Calculate(
            originalGraph.Tasks.Select(task => updatedTasks[task.Id]).ToArray(),
            candidateDependencies);
        var criticalSet = criticalTaskIds.ToHashSet();
        var affectedTasks = originalGraph.Tasks
            .Where(task => task.Start != updatedTasks[task.Id].Start || task.End != updatedTasks[task.Id].End)
            .Select(task => new GanttTaskDateChange(
                task.Id,
                task.Start,
                task.End,
                updatedTasks[task.Id].Start,
                updatedTasks[task.Id].End,
                criticalSet.Contains(task.Id)))
            .ToArray();
        return new GanttDependencyMutationRequest(
            mutation,
            previousDependency,
            proposedDependency,
            affectedTasks,
            criticalTaskIds);
    }
}
