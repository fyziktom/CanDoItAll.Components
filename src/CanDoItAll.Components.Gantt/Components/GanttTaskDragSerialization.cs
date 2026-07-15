using System.Text.Json;

namespace CanDoItAll.Components.Gantt;

internal static class GanttTaskDragSerialization
{
    public const string DataFormat = "application/x-candoitall-gantt-task";
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static string Serialize(GanttTask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        var payload = new DragTaskPayload(
            task.Id.Value,
            task.Title,
            task.Start,
            task.End,
            task.Assignments.Select(static assignment => new DragAssignmentPayload(assignment.Kind, assignment.Name)).ToArray());
        return JsonSerializer.Serialize(payload, SerializerOptions);
    }

    public static GanttTask Deserialize(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new ArgumentException("A Gantt task drag payload is required.", nameof(payload));
        }

        var task = JsonSerializer.Deserialize<DragTaskPayload>(payload, SerializerOptions)
            ?? throw new JsonException("The Gantt task drag payload is empty.");
        return new GanttTask(
            new GanttTaskId(task.Id),
            task.Title,
            task.Start,
            task.End,
            task.Assignments.Select(static assignment => new GanttAssignment(assignment.Kind, assignment.Name)));
    }

    private sealed record DragTaskPayload(
        string Id,
        string Title,
        DateTimeOffset Start,
        DateTimeOffset End,
        IReadOnlyList<DragAssignmentPayload> Assignments);

    private sealed record DragAssignmentPayload(GanttAssignmentKind Kind, string Name);
}
