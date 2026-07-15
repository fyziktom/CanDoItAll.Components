namespace CanDoItAll.Components.Gantt;

public enum GanttScheduleErrorCode
{
    DuplicateTaskId,
    DuplicateDependencyId,
    DuplicateDependency,
    MissingPredecessor,
    MissingSuccessor,
    CycleDetected,
    TaskNotFound,
    DependencyNotFound,
    DependencyConstraintViolation,
    InvalidSnapInterval,
    InsertionEdgeNotFound,
    InsertionTaskAlreadyExists,
    DependencyIdAlreadyExists
}

public sealed class GanttScheduleException : InvalidOperationException
{
    public GanttScheduleException(GanttScheduleErrorCode code, string message)
        : base(message)
    {
        Code = code;
    }

    public GanttScheduleErrorCode Code { get; }
}

public sealed record GanttDagValidationResult
{
    internal GanttDagValidationResult(IEnumerable<GanttTaskId> topologicalOrder)
    {
        TopologicalOrder = Array.AsReadOnly(topologicalOrder.ToArray());
    }

    public IReadOnlyList<GanttTaskId> TopologicalOrder { get; }
}

public static class GanttDagValidator
{
    public static GanttDagValidationResult Validate(
        IReadOnlyCollection<GanttTask> tasks,
        IReadOnlyCollection<GanttDependency> dependencies)
        => GanttScheduleGraph.Create(tasks, dependencies).Validation;
}

internal sealed class GanttScheduleGraph
{
    private GanttScheduleGraph(
        IReadOnlyList<GanttTask> tasks,
        IReadOnlyList<GanttDependency> dependencies,
        IReadOnlyDictionary<GanttTaskId, GanttTask> tasksById,
        IReadOnlyDictionary<GanttTaskId, IReadOnlyList<GanttTaskId>> predecessors,
        IReadOnlyDictionary<GanttTaskId, IReadOnlyList<GanttTaskId>> successors,
        GanttDagValidationResult validation)
    {
        Tasks = tasks;
        Dependencies = dependencies;
        TasksById = tasksById;
        Predecessors = predecessors;
        Successors = successors;
        Validation = validation;
    }

    public IReadOnlyList<GanttTask> Tasks { get; }

    public IReadOnlyList<GanttDependency> Dependencies { get; }

    public IReadOnlyDictionary<GanttTaskId, GanttTask> TasksById { get; }

    public IReadOnlyDictionary<GanttTaskId, IReadOnlyList<GanttTaskId>> Predecessors { get; }

    public IReadOnlyDictionary<GanttTaskId, IReadOnlyList<GanttTaskId>> Successors { get; }

    public GanttDagValidationResult Validation { get; }

    public static GanttScheduleGraph Create(
        IReadOnlyCollection<GanttTask> tasks,
        IReadOnlyCollection<GanttDependency> dependencies)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        ArgumentNullException.ThrowIfNull(dependencies);

        var taskArray = tasks.ToArray();
        var dependencyArray = dependencies.ToArray();
        if (taskArray.Any(static task => task is null))
        {
            throw new ArgumentException("Tasks cannot contain null values.", nameof(tasks));
        }

        if (dependencyArray.Any(static dependency => dependency is null))
        {
            throw new ArgumentException("Dependencies cannot contain null values.", nameof(dependencies));
        }

        var duplicateTask = taskArray
            .GroupBy(static task => task.Id)
            .FirstOrDefault(static group => group.Count() > 1);
        if (duplicateTask is not null)
        {
            throw new GanttScheduleException(
                GanttScheduleErrorCode.DuplicateTaskId,
                $"Task identifier '{duplicateTask.Key}' is duplicated.");
        }

        var duplicateDependencyId = dependencyArray
            .GroupBy(static dependency => dependency.Id)
            .FirstOrDefault(static group => group.Count() > 1);
        if (duplicateDependencyId is not null)
        {
            throw new GanttScheduleException(
                GanttScheduleErrorCode.DuplicateDependencyId,
                $"Dependency identifier '{duplicateDependencyId.Key}' is duplicated.");
        }

        var duplicateDependency = dependencyArray
            .GroupBy(static dependency => new DependencyKey(dependency.PredecessorId, dependency.SuccessorId, dependency.Type))
            .FirstOrDefault(static group => group.Count() > 1);
        if (duplicateDependency is not null)
        {
            throw new GanttScheduleException(
                GanttScheduleErrorCode.DuplicateDependency,
                $"Dependency '{duplicateDependency.Key.PredecessorId}' -> '{duplicateDependency.Key.SuccessorId}' is duplicated.");
        }

        var tasksById = taskArray.ToDictionary(static task => task.Id);
        var predecessorLists = taskArray.ToDictionary(static task => task.Id, static _ => new List<GanttTaskId>());
        var successorLists = taskArray.ToDictionary(static task => task.Id, static _ => new List<GanttTaskId>());
        foreach (var dependency in dependencyArray)
        {
            if (!tasksById.ContainsKey(dependency.PredecessorId))
            {
                throw new GanttScheduleException(
                    GanttScheduleErrorCode.MissingPredecessor,
                    $"Dependency '{dependency.Id}' references missing predecessor '{dependency.PredecessorId}'.");
            }

            if (!tasksById.ContainsKey(dependency.SuccessorId))
            {
                throw new GanttScheduleException(
                    GanttScheduleErrorCode.MissingSuccessor,
                    $"Dependency '{dependency.Id}' references missing successor '{dependency.SuccessorId}'.");
            }

            predecessorLists[dependency.SuccessorId].Add(dependency.PredecessorId);
            successorLists[dependency.PredecessorId].Add(dependency.SuccessorId);
        }

        var remainingPredecessors = predecessorLists.ToDictionary(static pair => pair.Key, static pair => pair.Value.Count);
        var ready = new Queue<GanttTaskId>(taskArray.Where(task => remainingPredecessors[task.Id] == 0).Select(static task => task.Id));
        var topologicalOrder = new List<GanttTaskId>(taskArray.Length);
        while (ready.TryDequeue(out var taskId))
        {
            topologicalOrder.Add(taskId);
            foreach (var successorId in successorLists[taskId])
            {
                remainingPredecessors[successorId]--;
                if (remainingPredecessors[successorId] == 0)
                {
                    ready.Enqueue(successorId);
                }
            }
        }

        if (topologicalOrder.Count != taskArray.Length)
        {
            throw new GanttScheduleException(
                GanttScheduleErrorCode.CycleDetected,
                "The Gantt dependency graph contains a cycle.");
        }

        var predecessors = predecessorLists.ToDictionary(
            static pair => pair.Key,
            static pair => (IReadOnlyList<GanttTaskId>)Array.AsReadOnly(pair.Value.ToArray()));
        var successors = successorLists.ToDictionary(
            static pair => pair.Key,
            static pair => (IReadOnlyList<GanttTaskId>)Array.AsReadOnly(pair.Value.ToArray()));
        var validation = new GanttDagValidationResult(topologicalOrder);

        return new GanttScheduleGraph(
            Array.AsReadOnly(taskArray),
            Array.AsReadOnly(dependencyArray),
            tasksById,
            predecessors,
            successors,
            validation);
    }

    private readonly record struct DependencyKey(
        GanttTaskId PredecessorId,
        GanttTaskId SuccessorId,
        GanttDependencyType Type);
}
