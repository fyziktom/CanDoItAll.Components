namespace CanDoItAll.Components.Gantt;

public sealed record GanttTimelineDoubleClickEventArgs
{
    public GanttTimelineDoubleClickEventArgs(GanttTaskId rowTaskId, DateTimeOffset clickedAtUtc)
    {
        GanttIdentifierGuard.Ensure(rowTaskId, nameof(rowTaskId));

        RowTaskId = rowTaskId;
        ClickedAtUtc = clickedAtUtc.ToUniversalTime();
    }

    public GanttTaskId RowTaskId { get; }

    public DateTimeOffset ClickedAtUtc { get; }
}

public sealed record GanttTaskOrderChangeRequest
{
    public GanttTaskOrderChangeRequest(
        GanttTaskId taskId,
        GanttTaskId anchorTaskId,
        GanttTaskOrderPlacement placement)
    {
        GanttIdentifierGuard.Ensure(taskId, nameof(taskId));
        GanttIdentifierGuard.Ensure(anchorTaskId, nameof(anchorTaskId));
        if (taskId == anchorTaskId)
        {
            throw new ArgumentException("A task cannot be ordered relative to itself.", nameof(anchorTaskId));
        }

        if (!Enum.IsDefined(placement))
        {
            throw new ArgumentOutOfRangeException(nameof(placement), placement, "The task order placement is not supported.");
        }

        TaskId = taskId;
        AnchorTaskId = anchorTaskId;
        Placement = placement;
    }

    public GanttTaskId TaskId { get; }

    public GanttTaskId AnchorTaskId { get; }

    public GanttTaskOrderPlacement Placement { get; }
}
