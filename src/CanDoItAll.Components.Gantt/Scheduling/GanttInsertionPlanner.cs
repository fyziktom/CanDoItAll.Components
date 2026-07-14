namespace CanDoItAll.Components.Gantt;

public static class GanttInsertionPlanner
{
    public static GanttTaskInsertionRequest Plan(
        IReadOnlyCollection<GanttTask> tasks,
        IReadOnlyCollection<GanttDependency> dependencies,
        GanttTask insertedTask,
        GanttTaskId predecessorId,
        GanttTaskId successorId,
        GanttDependencyId predecessorToInsertedDependencyId,
        GanttDependencyId insertedToSuccessorDependencyId)
    {
        ArgumentNullException.ThrowIfNull(insertedTask);
        var originalGraph = GanttScheduleGraph.Create(tasks, dependencies);
        GanttSchedulePropagation.ValidateConstraints(originalGraph, originalGraph.TasksById);
        if (originalGraph.TasksById.ContainsKey(insertedTask.Id))
        {
            throw new GanttScheduleException(
                GanttScheduleErrorCode.InsertionTaskAlreadyExists,
                $"Task '{insertedTask.Id}' already exists.");
        }

        if (!originalGraph.TasksById.TryGetValue(predecessorId, out var predecessor))
        {
            throw new GanttScheduleException(
                GanttScheduleErrorCode.TaskNotFound,
                $"Predecessor task '{predecessorId}' does not exist.");
        }

        if (!originalGraph.TasksById.ContainsKey(successorId))
        {
            throw new GanttScheduleException(
                GanttScheduleErrorCode.TaskNotFound,
                $"Successor task '{successorId}' does not exist.");
        }

        var bridge = originalGraph.Dependencies.FirstOrDefault(dependency =>
            dependency.PredecessorId == predecessorId &&
            dependency.SuccessorId == successorId &&
            dependency.Type == GanttDependencyType.FinishToStart)
            ?? throw new GanttScheduleException(
                GanttScheduleErrorCode.InsertionEdgeNotFound,
                $"No finish-to-start dependency exists from '{predecessorId}' to '{successorId}'.");

        RejectExistingDependencyId(originalGraph, predecessorToInsertedDependencyId);
        RejectExistingDependencyId(originalGraph, insertedToSuccessorDependencyId);
        if (predecessorToInsertedDependencyId == insertedToSuccessorDependencyId)
        {
            throw new GanttScheduleException(
                GanttScheduleErrorCode.DuplicateDependencyId,
                "Insertion dependencies require distinct identifiers.");
        }

        var insertedStart = predecessor.End.ToOffset(insertedTask.Start.Offset);
        var normalizedInsertedTask = GanttSchedulePlanner.CopyWithDates(
            insertedTask,
            insertedStart,
            insertedStart + insertedTask.Duration);
        var predecessorToInserted = new GanttDependency(
            predecessorToInsertedDependencyId,
            predecessorId,
            normalizedInsertedTask.Id);
        var insertedToSuccessor = new GanttDependency(
            insertedToSuccessorDependencyId,
            normalizedInsertedTask.Id,
            successorId);
        var updatedDependencies = originalGraph.Dependencies
            .Where(dependency => dependency.Id != bridge.Id)
            .Append(predecessorToInserted)
            .Append(insertedToSuccessor)
            .ToArray();
        var updatedTaskList = originalGraph.Tasks.Append(normalizedInsertedTask).ToArray();
        var updatedGraph = GanttScheduleGraph.Create(updatedTaskList, updatedDependencies);
        var updatedTasks = updatedTaskList.ToDictionary(static task => task.Id);

        GanttSchedulePropagation.Propagate(originalGraph, updatedGraph, updatedTasks, normalizedInsertedTask.Id);
        var criticalTaskIds = GanttCriticalPathCalculator.Calculate(
            updatedTaskList.Select(task => updatedTasks[task.Id]).ToArray(),
            updatedDependencies);
        var criticalSet = criticalTaskIds.ToHashSet();
        var affectedTasks = new List<GanttTaskDateChange>
        {
            new(
                normalizedInsertedTask.Id,
                insertedTask.Start,
                insertedTask.End,
                normalizedInsertedTask.Start,
                normalizedInsertedTask.End,
                criticalSet.Contains(normalizedInsertedTask.Id))
        };
        affectedTasks.AddRange(
            originalGraph.Tasks
                .Where(task => task.Start != updatedTasks[task.Id].Start || task.End != updatedTasks[task.Id].End)
                .Select(task => new GanttTaskDateChange(
                    task.Id,
                    task.Start,
                    task.End,
                    updatedTasks[task.Id].Start,
                    updatedTasks[task.Id].End,
                    criticalSet.Contains(task.Id))));

        GanttDependencyMutationRequest[] dependencyChanges =
        [
            new(GanttDependencyMutationKind.Remove, bridge, null),
            new(GanttDependencyMutationKind.Add, null, predecessorToInserted),
            new(GanttDependencyMutationKind.Add, null, insertedToSuccessor)
        ];

        return new GanttTaskInsertionRequest(
            normalizedInsertedTask,
            predecessorId,
            successorId,
            dependencyChanges,
            affectedTasks,
            criticalTaskIds);
    }

    private static void RejectExistingDependencyId(GanttScheduleGraph graph, GanttDependencyId dependencyId)
    {
        GanttIdentifierGuard.Ensure(dependencyId, nameof(dependencyId));
        if (graph.Dependencies.Any(dependency => dependency.Id == dependencyId))
        {
            throw new GanttScheduleException(
                GanttScheduleErrorCode.DependencyIdAlreadyExists,
                $"Dependency identifier '{dependencyId}' already exists.");
        }
    }
}
