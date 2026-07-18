using CanDoItAll.Components.CanvasLib;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class CanvasWorkbenchClipboardContractTests
{
    [Fact]
    public async Task ClipboardRequestJsonDispatchesTypedWorkbenchEvent()
    {
        CanvasWorkbenchClipboardRequest? received = null;
        var component = new TestCanvasWorkbench(
            EventCallback.Factory.Create<CanvasWorkbenchClipboardRequest>(
                this,
                (CanvasWorkbenchClipboardRequest request) => received = request));

        await component.OnClipboardAction("""
            {
              "action": "Cut",
              "surfaceId": "project-structure",
              "primaryNodeId": "node-2",
              "selectedNodeIds": ["node-2", "node-3"],
              "anchorWorld": { "x": 125.5, "y": 240 }
            }
            """);

        Assert.NotNull(received);
        Assert.Equal(CanvasWorkbenchClipboardAction.Cut, received.Action);
        Assert.Equal("project-structure", received.SurfaceId);
        Assert.Equal("node-2", received.PrimaryNodeId);
        Assert.Equal(["node-2", "node-3"], received.SelectedNodeIds);
        Assert.Equal(125.5, received.AnchorWorld?.X);
        Assert.Equal(240, received.AnchorWorld?.Y);
    }

    [Fact]
    public async Task InvalidClipboardActionDoesNotDispatchEvent()
    {
        var dispatchCount = 0;
        var component = new TestCanvasWorkbench(
            EventCallback.Factory.Create<CanvasWorkbenchClipboardRequest>(
                this,
                (CanvasWorkbenchClipboardRequest _) => dispatchCount++));

        await component.OnClipboardAction("""
            {
              "action": "Unknown",
              "surfaceId": "project-structure",
              "primaryNodeId": null,
              "selectedNodeIds": []
            }
            """);

        Assert.Equal(0, dispatchCount);
    }

    [Fact]
    public void ClipboardActionSerializesAsAStringEnum()
    {
        var request = new CanvasWorkbenchClipboardRequest(
            CanvasWorkbenchClipboardAction.Copy,
            "project-structure",
            "node-1",
            ["node-1"]);

        var json = SerializationPersistencePack.Serialize(request);

        Assert.Contains("\"action\":\"Copy\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"action\":0", json, StringComparison.Ordinal);
    }

    [Fact]
    public void WorkbenchShortcutRoutersSuppressAcceptedRepeatedCommandsWithoutRedispatching()
    {
        string[] routerFileNames =
        [
            "05-viewport-and-events.js",
            "07a-runtime-interaction-router.js"
        ];

        foreach (var routerFileName in routerFileNames)
        {
            var router = ReadSource(
                "src",
                "CanDoItAll.Components.CanvasLib",
                "wwwroot",
                "js",
                "runtime",
                "workbench",
                routerFileName);
            var keyDown = ReadSection(router, "keyDown: event =>", "state.host.addEventListener(\"pointerdown\"");

            Assert.Contains("target === state.host || state.host.contains(target)", keyDown, StringComparison.Ordinal);
            Assert.DoesNotContain("state.document.body", keyDown, StringComparison.Ordinal);
            Assert.DoesNotContain("state.document.documentElement", keyDown, StringComparison.Ordinal);
            Assert.DoesNotContain("closest?.(\".cw-workbench-shell\")", keyDown, StringComparison.Ordinal);
            Assert.Contains("if (isEditable)", keyDown, StringComparison.Ordinal);
            Assert.Contains("const shouldDispatch = !event.repeat;", keyDown, StringComparison.Ordinal);

            AssertPreventsDefaultOnlyAfterAccepted(keyDown, "requestClipboardCut(state, shouldDispatch)");
            AssertPreventsDefaultOnlyAfterAccepted(keyDown, "copySelectionToClipboard(state, shouldDispatch)");
            AssertPreventsDefaultOnlyAfterAccepted(keyDown, "requestClipboardPaste(state, shouldDispatch)");
            AssertPreventsDefaultOnlyAfterAccepted(keyDown, "requestClipboardDuplicate(state, shouldDispatch)");
        }
    }

    [Fact]
    public void WorkbenchClipboardCommandsRequireHandlerAndNeverUseTheOperatingSystemClipboard()
    {
        var clipboardRuntime = ReadSource(
            "src",
            "CanDoItAll.Components.CanvasLib",
            "wwwroot",
            "js",
            "runtime",
            "workbench",
            "02-layout-and-legacy-render.js");
        var standardCommands = ReadSection(
            clipboardRuntime,
            "function buildClipboardPayload",
            "function toggleMinimap");
        var runtimeEntry = ReadSource(
            "src",
            "CanDoItAll.Components.CanvasLib",
            "wwwroot",
            "js",
            "runtime",
            "workbench",
            "07-runtime-entry.js");
        var component = ReadSource(
            "src",
            "CanDoItAll.Components.CanvasLib",
            "Components",
            "Workbench",
            "CanvasWorkbench.razor");

        Assert.Contains("!state.hasClipboardHandler", standardCommands, StringComparison.Ordinal);
        Assert.Contains("clipboard.allowCopy && hasSourceSelection", standardCommands, StringComparison.Ordinal);
        Assert.Contains("clipboard.allowCut && hasSourceSelection", standardCommands, StringComparison.Ordinal);
        Assert.Contains("clipboard.allowPaste", standardCommands, StringComparison.Ordinal);
        Assert.Contains("clipboard.allowDuplicate && hasSourceSelection", standardCommands, StringComparison.Ordinal);
        Assert.Contains("function requestClipboardAction(state, action, shouldDispatch = true)", standardCommands, StringComparison.Ordinal);
        Assert.Contains("if (shouldDispatch)", standardCommands, StringComparison.Ordinal);
        Assert.Contains("JSON.stringify(buildClipboardPayload(state, action))", standardCommands, StringComparison.Ordinal);
        Assert.DoesNotContain("writeClipboardText", standardCommands, StringComparison.Ordinal);
        Assert.DoesNotContain("readClipboardText", standardCommands, StringComparison.Ordinal);
        Assert.DoesNotContain("navigator.clipboard", standardCommands, StringComparison.Ordinal);
        Assert.DoesNotContain("localClipboard", standardCommands, StringComparison.Ordinal);
        Assert.DoesNotContain("selectedNodes", standardCommands, StringComparison.Ordinal);
        Assert.DoesNotContain("capturedAtUtc", standardCommands, StringComparison.Ordinal);

        Assert.Contains("state.hasClipboardHandler = options?.hasClipboardHandler === true", runtimeEntry, StringComparison.Ordinal);
        Assert.Contains("ClipboardRequested.HasDelegate", component, StringComparison.Ordinal);
    }

    private static void AssertPreventsDefaultOnlyAfterAccepted(string source, string requestExpression)
    {
        var requestIndex = source.IndexOf($"if ({requestExpression})", StringComparison.Ordinal);
        Assert.True(requestIndex >= 0, $"Could not find acceptance check for '{requestExpression}'.");

        var preventDefaultIndex = source.IndexOf("event.preventDefault();", requestIndex, StringComparison.Ordinal);
        Assert.True(preventDefaultIndex > requestIndex, $"'{requestExpression}' must be accepted before preventDefault.");
    }

    private static string ReadSource(params string[] segments)
        => File.ReadAllText(Path.Combine([FindRepoRoot(), .. segments]));

    private static string ReadSection(string source, string startMarker, string endMarker)
    {
        var start = source.IndexOf(startMarker, StringComparison.Ordinal);
        Assert.True(start >= 0, $"Could not find JavaScript section start '{startMarker}'.");
        var end = source.IndexOf(endMarker, start + startMarker.Length, StringComparison.Ordinal);
        Assert.True(end > start, $"Could not find JavaScript section end '{endMarker}'.");
        return source[start..end];
    }

    private static string FindRepoRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "CanDoItAll.Components.slnx")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException("Could not locate the CanDoItAll.Components repository root.");
    }

    private sealed class TestCanvasWorkbench : CanvasWorkbench
    {
        public TestCanvasWorkbench(EventCallback<CanvasWorkbenchClipboardRequest> clipboardRequested)
        {
            ClipboardRequested = clipboardRequested;
        }
    }
}
