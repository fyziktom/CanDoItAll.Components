using System.Reflection;
using CanDoItAll.Components.BaseLib;
using CanDoItAll.Components.FileBrowser.Core;

namespace CanDoItAll.Components.FileBrowser.BaseLib.Tests;

public sealed class FileBrowserMenuProjectionTests
{
    [Fact]
    public void BuildMenuItems_ProjectsAllSupportedBuiltInActionsInStableOrder()
    {
        var item = TestFileBrowserItemFactory.File(
            "spec.pdf",
            FileBrowserItemCategory.Document,
            displayPath: "/projects/alpha/spec.pdf",
            capabilities: FileBrowserItemCapabilities.Open
                | FileBrowserItemCapabilities.OpenInNewTab
                | FileBrowserItemCapabilities.CopyPath
                | FileBrowserItemCapabilities.CopyContentIdentity
                | FileBrowserItemCapabilities.DownloadFile,
            contentIdentity: new FileBrowserContentIdentity("cid", "bafy-spec"));

        var menu = BuildMenuItems(item);

        Assert.Collection(
            menu,
            entry => AssertMenuEntry(entry, FileBrowserActionIds.Open, "Open", "open_in_new", separatorBefore: false),
            entry => AssertMenuEntry(entry, FileBrowserActionIds.OpenInNewTab, "Open in new tab", "open_in_new", separatorBefore: false),
            entry => AssertMenuEntry(entry, FileBrowserActionIds.CopyPath, "Copy path", "content_copy", separatorBefore: true),
            entry => AssertMenuEntry(entry, FileBrowserActionIds.CopyContentIdentity, "Copy content ID", "fingerprint", separatorBefore: false),
            entry => AssertMenuEntry(entry, FileBrowserActionIds.Download, "Download", "download", separatorBefore: true));
    }

    [Fact]
    public void BuildMenuItems_ContainerOmitsNavigationWhenCapabilityIsUnavailable()
    {
        var item = TestFileBrowserItemFactory.Folder(
            capabilities: FileBrowserItemCapabilities.Select | FileBrowserItemCapabilities.DownloadDirectory);

        var menu = BuildMenuItems(item);

        Assert.Collection(
            menu,
            entry => AssertMenuEntry(entry, FileBrowserActionIds.Download, "Download", "download", separatorBefore: false));
    }

    [Fact]
    public void BuildMenuItems_OmitsActionsWhoseRequiredValuesAreUnavailable()
    {
        var item = TestFileBrowserItemFactory.File(
            "opaque",
            capabilities: FileBrowserItemCapabilities.CopyPath
                | FileBrowserItemCapabilities.CopyContentIdentity);

        Assert.Empty(BuildMenuItems(item));
    }

    [Fact]
    public void BuildMenuItems_CollapsesFileAndDirectoryDownloadCapabilitiesIntoOneAction()
    {
        var item = TestFileBrowserItemFactory.File(
            "bundle.car",
            FileBrowserItemCategory.Data,
            capabilities: FileBrowserItemCapabilities.DownloadFile | FileBrowserItemCapabilities.DownloadDirectory);

        var entry = Assert.Single(BuildMenuItems(item));

        AssertMenuEntry(entry, FileBrowserActionIds.Download, "Download", "download", separatorBefore: false);
    }

    [Fact]
    public void PublicActionEventsKeepOccurrenceAndActionIdentityIntact()
    {
        var item = TestFileBrowserItemFactory.File("readme.md", FileBrowserItemCategory.Document);

        var action = new FileBrowserItemActionEventArgs(item, FileBrowserActionIds.CopyPath);
        var invocation = new FileBrowserItemInvokedEventArgs(item, FileBrowserInvocationKind.Keyboard);
        var menu = new FileBrowserActionMenuRequest(item, 12.5, 42.25);

        Assert.Same(item, action.Item);
        Assert.Equal(FileBrowserActionIds.CopyPath, action.ActionId);
        Assert.Same(item, invocation.Item);
        Assert.Equal(FileBrowserInvocationKind.Keyboard, invocation.Kind);
        Assert.Same(item, menu.Item);
        Assert.Equal(12.5, menu.X);
        Assert.Equal(42.25, menu.Y);
    }

    private static IReadOnlyList<ContextMenuItem> BuildMenuItems(FileBrowserItem item)
    {
        var method = typeof(FileBrowser).GetMethod(
            "BuildMenuItems",
            BindingFlags.Static | BindingFlags.NonPublic);

        Assert.NotNull(method);
        var result = method.Invoke(null, [FileBrowserBuiltInActions.GetFor(item)]);
        return Assert.IsAssignableFrom<IReadOnlyList<ContextMenuItem>>(result);
    }

    private static void AssertMenuEntry(
        ContextMenuItem entry,
        string id,
        string text,
        string icon,
        bool separatorBefore)
    {
        Assert.Equal(id, entry.Id);
        Assert.Equal(text, entry.Text);
        Assert.Equal(icon, entry.Icon);
        Assert.Equal(separatorBefore, entry.SeparatorBefore);
        Assert.False(entry.Disabled);
        Assert.False(entry.Danger);
    }
}
