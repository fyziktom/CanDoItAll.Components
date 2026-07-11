using CanDoItAll.Components.FileBrowser.Core;

namespace CanDoItAll.Components.FileBrowser.BaseLib.Tests;

internal static class TestFileBrowserItemFactory
{
    private static readonly FileBrowserSourceId SourceId = new("test-source");

    public static FileBrowserItem File(
        string name,
        FileBrowserItemCategory category = FileBrowserItemCategory.Other,
        string? displayPath = null,
        string? mediaType = null,
        FileBrowserItemCapabilities capabilities = FileBrowserItemCapabilities.Select,
        FileBrowserContentIdentity? contentIdentity = null,
        IReadOnlyDictionary<string, string>? metadata = null)
        => new(
            new FileBrowserItemKey(SourceId, $"files/{name}"),
            new FileBrowserItemKey(SourceId, "root"),
            name,
            category == FileBrowserItemCategory.Link ? FileBrowserItemKind.Link : FileBrowserItemKind.File,
            category,
            displayPath: displayPath,
            mediaType: mediaType,
            contentIdentity: contentIdentity,
            capabilities: capabilities,
            metadata: metadata);

    public static FileBrowserItem Folder(
        string name = "Folder",
        string? displayPath = null,
        FileBrowserItemCapabilities capabilities = FileBrowserItemCapabilities.Select | FileBrowserItemCapabilities.Navigate,
        IReadOnlyDictionary<string, string>? metadata = null)
        => new(
            new FileBrowserItemKey(SourceId, $"folders/{name}"),
            new FileBrowserItemKey(SourceId, "root"),
            name,
            FileBrowserItemKind.Container,
            FileBrowserItemCategory.Folder,
            displayPath: displayPath,
            childState: FileBrowserChildState.Unknown,
            capabilities: capabilities,
            metadata: metadata);
}
