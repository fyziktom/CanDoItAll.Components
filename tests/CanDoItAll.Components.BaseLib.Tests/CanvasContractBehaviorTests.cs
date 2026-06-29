using CanDoItAll.Components.CanvasLib;
using CanDoItAll.Components.OverlayLib;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class CanvasContractBehaviorTests
{
    [Fact]
    public void CanvasWorkbenchUiStateParseFallsBackForMalformedJson()
    {
        var state = CanvasWorkbenchUiState.Parse("{ definitely not json");

        Assert.Equal(CanvasWorkbenchUiState.CurrentVersion, state.Version);
        Assert.Empty(state.SelectedNodeIds);
        Assert.Equal(1, state.Zoom);
        Assert.Equal(90, state.PanX);
        Assert.Equal(110, state.PanY);
        Assert.True(state.ShowMinimap);
    }

    [Fact]
    public void CanvasWorkbenchUiStateParseNormalizesSelectionListsAndWindowGeometry()
    {
        const string json = """
            {
              "selectedNodeIds": [" alpha ", "", "alpha", "beta"],
              "highlightedNodeIds": [" hot ", "hot", " "],
              "collapsedNodeIds": [" collapsed ", "collapsed"],
              "windowStates": {
                " inspector ": {
                  "isVisible": false,
                  "isMinimized": true,
                  "left": 0,
                  "top": 12.126,
                  "width": -5,
                  "height": 200.125
                },
                " ": {
                  "left": 10
                }
              }
            }
            """;

        var state = CanvasWorkbenchUiState.Parse(json);

        Assert.Equal(["alpha", "beta"], state.SelectedNodeIds);
        Assert.Equal(["hot"], state.HighlightedNodeIds);
        Assert.Equal(["collapsed"], state.CollapsedNodeIds);
        var window = Assert.Single(state.WindowStates);
        Assert.Equal("inspector", window.Key);
        Assert.False(window.Value.IsVisible);
        Assert.True(window.Value.IsMinimized);
        Assert.Null(window.Value.Left);
        Assert.Equal(12.13, window.Value.Top);
        Assert.Null(window.Value.Width);
        Assert.Equal(200.13, window.Value.Height);
    }

    [Fact]
    public void CanvasWorkbenchUiStateToJsonNormalizesBeforeSerialization()
    {
        var state = new CanvasWorkbenchUiState
        {
            SelectedNodeIds = [" a ", "a", "", "b"],
            HighlightedNodeIds = [" hot ", "hot"],
            WindowStates =
            {
                [" tools "] = new CanvasWorkbenchWindowState
                {
                    Left = 0,
                    Top = 7.125,
                    Width = 320.125,
                    Height = -1
                }
            }
        };

        var roundTripped = CanvasWorkbenchUiState.Parse(state.ToJson());

        Assert.Equal(["a", "b"], roundTripped.SelectedNodeIds);
        Assert.Equal(["hot"], roundTripped.HighlightedNodeIds);
        var window = Assert.Single(roundTripped.WindowStates);
        Assert.Equal("tools", window.Key);
        Assert.Null(window.Value.Left);
        Assert.Equal(7.13, window.Value.Top);
        Assert.Equal(320.13, window.Value.Width);
        Assert.Null(window.Value.Height);
    }

    [Fact]
    public void SelectionModelNormalizesPrimaryDuplicatesAndMissingNodes()
    {
        var selection = SelectionModel.From([" a ", "b", "a", "", "c"], primaryNodeId: " b ");

        Assert.Equal("b", selection.PrimaryNodeId);
        Assert.Equal(["b", "a", "c"], selection.SelectedNodeIds);

        var addedPrimary = selection.Add(" d ", makePrimary: true);
        Assert.Equal("d", addedPrimary.PrimaryNodeId);
        Assert.Equal(["d", "b", "a", "c"], addedPrimary.SelectedNodeIds);

        var removedMissing = addedPrimary.RemoveMissing(["a", "d"]);
        Assert.Equal(["d", "a"], removedMissing.SelectedNodeIds);
    }

    [Fact]
    public void CanvasWindowStateRoundTripsThroughOverlayState()
    {
        var canvas = new CanvasWorkbenchWindowState
        {
            IsVisible = false,
            IsMinimized = true,
            Left = 10.124,
            Top = -5,
            Width = 360.125,
            Height = 0
        };

        OverlayWindowState overlay = canvas.ToOverlayWindowState();
        CanvasWorkbenchWindowState restored = CanvasWorkbenchWindowState.FromOverlayWindowState(overlay);

        Assert.False(restored.IsVisible);
        Assert.True(restored.IsMinimized);
        Assert.Equal(10.12, restored.Left);
        Assert.Null(restored.Top);
        Assert.Equal(360.13, restored.Width);
        Assert.Null(restored.Height);
    }

    [Fact]
    public void SerializationPersistencePackDeserializeReturnsDefaultForBadPayload()
    {
        var restored = SerializationPersistencePack.Deserialize<CanvasWorkbenchSurface>("{ bad");

        Assert.Null(restored);
    }

    [Fact]
    public void CanvasLayoutCollisionResolverSeparatesOverlappingUnpinnedNodeFromPinnedNode()
    {
        var pinned = new CanvasLayoutNodeBox
        {
            NodeId = "node-a",
            X = 0,
            Y = 0,
            Width = 100,
            Height = 100,
            IsPinned = true
        };
        var movable = new CanvasLayoutNodeBox
        {
            NodeId = "node-b",
            X = 0,
            Y = 0,
            Width = 100,
            Height = 100
        };

        var resolved = CanvasLayoutCollisionResolver.Resolve(
            [pinned, movable],
            new CanvasLayoutCollisionOptions
            {
                AxisPreference = CanvasLayoutAxisPreference.Horizontal,
                MinimumGapX = 10,
                MinimumGapY = 10,
                MaxIterations = 3
            });

        var resolvedPinned = Assert.Single(resolved, node => node.NodeId == "node-a");
        var resolvedMovable = Assert.Single(resolved, node => node.NodeId == "node-b");
        Assert.Equal(0, resolvedPinned.X);
        Assert.True(resolvedMovable.Left >= resolvedPinned.Right + 10);
    }

    [Fact]
    public void CanvasCalendarContractsExposeStableDefaultsAndRequestContext()
    {
        var surface = new CanvasCalendarSurface();
        var calendarEvent = new CanvasCalendarEvent { EventId = "event-1", Title = "Planning" };
        var context = new CanvasCalendarOperationContext { SelectedDate = "2026-06-29" };
        var saveRequest = new CanvasCalendarSaveRequest(calendarEvent, context, "create");
        var exportRequest = new CanvasCalendarExportRequest("ics", [calendarEvent], context);

        Assert.Equal("week", surface.InitialView);
        Assert.Equal("UTC", surface.Timezone);
        Assert.Equal("en-US", surface.Locale);
        Assert.Equal(1, surface.WeekStartsOn);
        Assert.Equal(30, surface.SlotMinutes);
        Assert.True(surface.AllowCreate);
        Assert.True(surface.EnableListExport);
        Assert.Equal("create", saveRequest.Mode);
        Assert.Equal("2026-06-29", saveRequest.Context.SelectedDate);
        Assert.Equal("ics", exportRequest.Format);
        Assert.Same(calendarEvent, Assert.Single(exportRequest.VisibleEvents));
    }
}
