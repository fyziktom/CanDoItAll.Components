using System.Reflection;
using CanDoItAll.Components.FileBrowser.Core;

namespace CanDoItAll.Components.FileBrowser.BaseLib.Tests;

public sealed class FileBrowserInteractionGuardTests
{
    [Fact]
    public void ReplacingSessionInvalidatesQueuedSnapshotAndMenuWork()
    {
        var guard = new FileBrowserInteractionGuard();
        var oldContext = guard.ChangeSession();

        guard.ChangeSession();

        Assert.False(guard.IsCurrent(oldContext));
        Assert.False(guard.IsCurrentSession(oldContext.SessionVersion));
    }

    [Fact]
    public void NewerSearchEditCannotBeCompletedOrOverwrittenByOlderSearch()
    {
        var guard = new FileBrowserInteractionGuard();
        guard.ChangeSession();
        var canceledQuery = guard.BeginSearchEdit();
        var latestQuery = guard.BeginSearchEdit();

        Assert.False(guard.TryCompleteSearchEdit(canceledQuery));
        Assert.False(guard.CanSynchronizeCommittedSearch);
        Assert.True(guard.TryCompleteSearchEdit(latestQuery));
        Assert.True(guard.CanSynchronizeCommittedSearch);
    }

    [Fact]
    public void SnapshotChangeInvalidatesOpenMenuWithoutEndingActiveSearchEdit()
    {
        var guard = new FileBrowserInteractionGuard();
        guard.ChangeSession();
        var menuContext = guard.Capture();
        var searchEdit = guard.BeginSearchEdit();

        guard.AcceptSnapshot();

        Assert.False(guard.IsCurrent(menuContext));
        Assert.False(guard.CanSynchronizeCommittedSearch);
        Assert.True(guard.TryCompleteSearchEdit(searchEdit));
    }

    [Fact]
    public void MenuSelectionRevalidatesBuiltInAndCustomCapabilities()
    {
        var method = typeof(FileBrowser).GetMethod(
            "IsActionStillSupported",
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);
        var opaque = TestFileBrowserItemFactory.File(
            "opaque",
            capabilities: FileBrowserItemCapabilities.Select);
        var extensible = TestFileBrowserItemFactory.File(
            "extensible",
            capabilities: FileBrowserItemCapabilities.CustomActions);

        Assert.False(InvokeSupportCheck(method, opaque, FileBrowserActionIds.Open));
        Assert.False(InvokeSupportCheck(method, opaque, "provider.pin"));
        Assert.True(InvokeSupportCheck(method, extensible, "provider.pin"));
    }

    private static bool InvokeSupportCheck(MethodInfo method, FileBrowserItem item, string actionId)
        => Assert.IsType<bool>(method.Invoke(null, [item, actionId]));
}
