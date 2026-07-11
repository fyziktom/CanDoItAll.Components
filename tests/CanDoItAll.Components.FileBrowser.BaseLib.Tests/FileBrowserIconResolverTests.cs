using CanDoItAll.Components.FileBrowser.Core;

namespace CanDoItAll.Components.FileBrowser.BaseLib.Tests;

public sealed class FileBrowserIconResolverTests
{
    [Theory]
    [InlineData("photo.jpg", FileBrowserItemCategory.Image, "image")]
    [InlineData("clip.mp4", FileBrowserItemCategory.Video, "video_file")]
    [InlineData("recording.wav", FileBrowserItemCategory.Audio, "audio_file")]
    [InlineData("bundle.zip", FileBrowserItemCategory.Archive, "folder_zip")]
    [InlineData("Program.cs", FileBrowserItemCategory.Code, "code")]
    [InlineData("index.car", FileBrowserItemCategory.Data, "dataset")]
    [InlineData("gateway", FileBrowserItemCategory.Link, "link")]
    [InlineData("contract.PDF", FileBrowserItemCategory.Document, "picture_as_pdf")]
    [InlineData("contract.docx", FileBrowserItemCategory.Document, "description")]
    [InlineData("unknown.bin", FileBrowserItemCategory.Other, "draft")]
    public void Resolve_MapsCategoriesAndDocumentExtensionsToMaterialIcons(
        string name,
        FileBrowserItemCategory category,
        string expected)
    {
        var item = TestFileBrowserItemFactory.File(name, category);

        Assert.Equal(expected, FileBrowserIconResolver.Resolve(item));
    }

    [Fact]
    public void Resolve_FolderUsesProviderSuppliedCustomIcon()
    {
        var item = TestFileBrowserItemFactory.Folder(
            metadata: new Dictionary<string, string> { ["icon"] = "account_tree" });

        Assert.Equal("account_tree", FileBrowserIconResolver.Resolve(item));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Resolve_FolderWithoutUsableCustomIconUsesFolderFallback(string? icon)
    {
        IReadOnlyDictionary<string, string>? metadata = icon is null
            ? null
            : new Dictionary<string, string> { ["icon"] = icon };
        var item = TestFileBrowserItemFactory.Folder(metadata: metadata);

        Assert.Equal("folder", FileBrowserIconResolver.Resolve(item));
    }

    [Theory]
    [InlineData(FileBrowserItemCategory.Data, "snapshot.car", "dataset")]
    [InlineData(FileBrowserItemCategory.Document, "spec.pdf", "picture_as_pdf")]
    [InlineData(FileBrowserItemCategory.Other, "raw-block", "draft")]
    public void Resolve_CidBackedItemsRemainCategoryDriven(
        FileBrowserItemCategory category,
        string name,
        string expected)
    {
        var item = TestFileBrowserItemFactory.File(
            name,
            category,
            contentIdentity: new FileBrowserContentIdentity("cid", "bafy-test-content"));

        Assert.Equal(expected, FileBrowserIconResolver.Resolve(item));
    }

    [Fact]
    public void Resolve_NullItemThrows()
    {
        Assert.Throws<ArgumentNullException>(() => FileBrowserIconResolver.Resolve(null!));
    }
}
