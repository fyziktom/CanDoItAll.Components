# CanDoItAll.Components.Gantt

Reusable controlled Gantt chart for Blazor, rendered through the generic CanvasLib runtime.

The host owns persistence and supplies immutable tasks and dependencies. The chart never writes data or accepts an edit internally. Title, schedule, dependency, and insertion gestures are returned as typed requests; the host must commit or reject each request and then replace the input model.

## Assets

Load the shared assets once in the host document:

```razor
<head>
    <GanttChartHeadAssets />
</head>
<body>
    <Routes />
    <GanttChartBodyAssets />
</body>
```

The body wrapper loads only CanvasLib's generic canvas runtime, not its Workbench CSS or runtime. If the host already loads `CanvasRuntimeBodyAssets` or the full `CanvasLibBodyAssets`, set `IncludeCanvasRuntimeAssets="false"` on `GanttChartBodyAssets`. If the host already loads BaseLib's `css/output.css`, set `IncludeBaseLibStyles="false"` on `GanttChartHeadAssets`.

## Controlled chart

```razor
<GanttChart Tasks="tasks"
            Dependencies="dependencies"
            TaskTitleChangeRequested="CommitTitleAsync"
            TaskScheduleChangeRequested="CommitScheduleAsync"
            DependencyMutationRequested="CommitDependencyAsync"
            TaskInsertionRequested="CommitInsertionAsync" />
```

`GanttSchedulePlanner`, `GanttDependencyPlanner`, and `GanttInsertionPlanner` validate the same typed request contracts without depending on Blazor. Finish-to-start dependencies use `PredecessorId -> SuccessorId`. Multiple dependencies are first-class records rather than fields encoded into a task. Dependency add/reconnect requests include the downstream schedule changes required to keep the proposed graph valid; the host still reloads and validates authoritative state before persistence. Snapping uses the stable `SnapOrigin` parameter (UTC Unix epoch by default), never a task-derived timeline boundary.

Use `GanttTaskDragSource` to stage a caller-owned task, then drop it on the exact dependency line to replace. Insertion removes only that bridge, adds the two replacement dependencies, and returns all downstream date changes for host-side atomic persistence.

The chart includes an aligned hideable task table, compact process/workflow/agent/person indicators, keyboard-accessible task titles, critical-path emphasis, dependency add/reconnect gestures, and PNG export. Dates, canvas ticks, and PNG labels use an explicit UTC display contract. Synthetic or read-only data remains a host concern and can be marked through `ProjectionOnlySelector`, `TaskScheduleReadOnlySelector`, `TaskTitleReadOnlySelector`, `TaskDependencyReadOnlySelector`, or `TaskReadOnlySelector`. Wide timelines use an explicit fitted scale, and large PNG exports are proportionally downsampled to a bounded backing store.

## Validation

```powershell
dotnet build src/CanDoItAll.Components.Gantt/CanDoItAll.Components.Gantt.csproj --configuration Release
dotnet test tests/CanDoItAll.Components.Gantt.Tests/CanDoItAll.Components.Gantt.Tests.csproj --configuration Release
```
