namespace CanDoItAll.Components.Gantt;

public enum GanttAssignmentKind
{
    Process,
    Workflow,
    Agent,
    Person
}

public enum GanttScheduleGesture
{
    Move,
    ResizeStart,
    ResizeEnd
}

public enum GanttDependencyMutationKind
{
    Add,
    Remove,
    Reconnect
}

public enum GanttDependencyType
{
    FinishToStart
}

public enum GanttTimeScale
{
    Custom,
    QuarterHour,
    Hour,
    Day,
    Week
}
