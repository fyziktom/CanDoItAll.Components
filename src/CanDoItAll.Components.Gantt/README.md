# CanDoItAll.Components.Gantt

Reusable controlled Gantt chart for Blazor, rendered through the generic CanvasLib runtime.

The host owns persistence and supplies immutable tasks and dependencies. The chart never writes data or accepts an edit internally. Title, schedule, dependency, insertion, and row-order gestures are returned as typed requests; task and empty-row double-clicks are returned as typed events. The host must handle the intent and then replace the input model when data changes.

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
            TimeScale="GanttTimeScale.Hour"
            TaskTitleChangeRequested="CommitTitleAsync"
            TaskScheduleChangeRequested="CommitScheduleAsync"
            DependencyMutationRequested="CommitDependencyAsync"
            TaskInsertionRequested="CommitInsertionAsync"
            TimelineDoubleClicked="OpenTaskDialogAsync"
            TaskDoubleClicked="OpenTaskDetailsAsync"
            AllowTimelineTaskCreation="true"
            TaskOrderChangeRequested="CommitTaskOrderAsync" />
```

`GanttSchedulePlanner`, `GanttDependencyPlanner`, and `GanttInsertionPlanner` validate the same typed request contracts without depending on Blazor. `GanttSchedulePlanner.PlanInterval` lets a host validate an edited start/end pair and calculate its dependent path as one `SetInterval` request. Finish-to-start dependencies use `PredecessorId -> SuccessorId`. Multiple dependencies are first-class records rather than fields encoded into a task. Dependency add/reconnect requests include the downstream schedule changes required to keep the proposed graph valid; the host still reloads and validates authoritative state before persistence. Snapping uses the stable `SnapOrigin` parameter (UTC Unix epoch by default), never a task-derived timeline boundary.

`GanttTask.Start` and `End` define elapsed delivery time and therefore the rendered bar width. Optional `ExpectedEffort` is independent pure work time; the chart renders it as a purple bottom indicator on the delivery scale and caps only its drawing at the delivery width. Optional `ProgressPercent` is an integer from 0 through 100 and renders as a green completed/red remaining indicator above the task. Missing metrics are rendered as unknown rather than inferred. The aligned table labels delivery and pure effort separately.

Use `GanttTaskDragSource` to stage a caller-owned task, then drop it on the exact dependency line to replace. Insertion removes only that bridge, adds the two replacement dependencies, and returns all downstream date changes for host-side atomic persistence.

When `TimelineDoubleClicked` is handled, double-clicking empty timeline content in a task row returns `GanttTimelineDoubleClickEventArgs` with the stable row task id and the clicked UTC time snapped to the configured `SnapOrigin` and `SnapInterval`. The chart does not choose a title, duration, parent, resource, or persistence operation. A host can use the row id as an insertion anchor and open its own creation dialog. Set `AllowTimelineTaskCreation="true"` to enable this independently from schedule/title editing, or `false` to suppress its cursor, browser listener, and event. Leaving the nullable capability unset preserves compatibility by inheriting `AllowTaskEditing`.

When `TaskDoubleClicked` is handled, double-clicking only the task body returns its `GanttTaskId`; resize handles, dependency ports, assignment indicators, and dependency endpoints do not trigger it. The host can open its authoritative detail editor without coupling that dialog to the reusable chart. If no task-double-click handler is supplied, the existing inline title-edit behavior remains the fallback.

When `TaskOrderChangeRequested` is handled, compact up/down actions appear beside task titles. They return `GanttTaskOrderChangeRequest` with the moving task id, an adjacent anchor task id, and a `Before` or `After` placement. Boundary and read-only moves are disabled, and the component does not reorder its controlled `Tasks` input. Set `AllowTaskReordering="false"` to disable the actions while preserving the supplied row order.

The chart includes an aligned hideable task table, compact process/workflow/agent/person indicators, keyboard-accessible task titles, critical-path emphasis, dependency add/reconnect gestures, and PNG export. Dependency paths use obstacle-safe orthogonal routes, terminate directly at the successor edge, and fail explicitly if a safe route cannot be produced. Dates, canvas ticks, and PNG labels use an explicit UTC display contract. Synthetic or read-only data remains a host concern and can be marked through `ProjectionOnlySelector`, `TaskScheduleReadOnlySelector`, `TaskTitleReadOnlySelector`, `TaskDependencyReadOnlySelector`, or `TaskReadOnlySelector`.

`GanttTimeScale` exposes `QuarterHour`, `Hour`, `Day`, and `Week` presets with toolbar labels `0.25 h`, `1 h`, `1 d`, and `1 w`. `Custom` preserves the existing `PixelsPerHour` contract. Fine scales create real horizontal overflow; use the scrollbar or drag empty timeline space left/right to pan. `TimeScaleChanged` and `PixelsPerHourChanged` let a host control that view state. Extremely wide timelines use an explicit fitted scale, and large PNG exports are proportionally downsampled to a bounded backing store.

## Validation

```powershell
dotnet build src/CanDoItAll.Components.Gantt/CanDoItAll.Components.Gantt.csproj --configuration Release
dotnet test tests/CanDoItAll.Components.Gantt.Tests/CanDoItAll.Components.Gantt.Tests.csproj --configuration Release
npm run gantt:test-routing
```
