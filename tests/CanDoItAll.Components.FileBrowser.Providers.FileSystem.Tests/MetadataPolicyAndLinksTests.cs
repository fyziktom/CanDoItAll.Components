namespace CanDoItAll.Components.FileBrowser.Providers.FileSystem.Tests;

public sealed class MetadataPolicyAndLinksTests
{
    [Fact]
    public async Task HiddenPolicyExcludesAndIncludesDotPrefixedEntries()
    {
        using var fileSystem = new TemporaryFileSystem();
        fileSystem.CreateDirectory(".private-folder");
        fileSystem.CreateFile(".secret.txt");
        fileSystem.CreateFile("visible.txt");

        var excluding = FileSystemTestFactory.CreateProvider(fileSystem, includeHidden: false);
        var including = FileSystemTestFactory.CreateProvider(fileSystem, includeHidden: true);

        var excludedPage = await excluding.BrowseAsync(FileSystemTestFactory.Browse());
        var includedPage = await including.BrowseAsync(FileSystemTestFactory.Browse());

        Assert.Equal(["visible.txt"], excludedPage.Items.Select(item => item.Name));
        Assert.Equal(
            [".private-folder", ".secret.txt", "visible.txt"],
            includedPage.Items.Select(item => item.Name));

        var hiddenPathError = await FileSystemTestFactory.PathErrorAsync(
            excluding,
            FileSystemTestFactory.Key(".private-folder"));
        Assert.Equal(FileBrowserErrorCode.NotFound, hiddenPathError.Error.Code);
    }

    [Fact]
    public async Task WindowsHiddenAttributeRespectsConfiguredPolicy()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        using var fileSystem = new TemporaryFileSystem();
        var hiddenPath = fileSystem.CreateFile("hidden.txt");
        File.SetAttributes(hiddenPath, File.GetAttributes(hiddenPath) | FileAttributes.Hidden);
        fileSystem.CreateFile("visible.txt");

        var excluding = FileSystemTestFactory.CreateProvider(fileSystem, includeHidden: false);
        var including = FileSystemTestFactory.CreateProvider(fileSystem, includeHidden: true);

        var excludedPage = await excluding.BrowseAsync(FileSystemTestFactory.Browse());
        var includedPage = await including.BrowseAsync(FileSystemTestFactory.Browse());

        Assert.Equal(["visible.txt"], excludedPage.Items.Select(item => item.Name));
        Assert.Equal(["hidden.txt", "visible.txt"], includedPage.Items.Select(item => item.Name));
    }

    [Fact]
    public async Task RequestedMetadataAndCapabilitiesReflectAvailableFilesystemData()
    {
        using var fileSystem = new TemporaryFileSystem();
        var filePath = fileSystem.CreateFile("report.pdf", new byte[23]);
        var expectedModified = new DateTime(2024, 5, 4, 12, 30, 0, DateTimeKind.Utc);
        File.SetLastWriteTimeUtc(filePath, expectedModified);
        var provider = FileSystemTestFactory.CreateProvider(fileSystem);

        var page = await provider.BrowseAsync(FileSystemTestFactory.Browse(
            metadata: new FileBrowserMetadataRequest(FileBrowserMetadataFields.All)));
        var item = Assert.Single(page.Items);

        Assert.Equal(FileSystemTestFactory.Key("report.pdf"), item.Key);
        Assert.Equal(FileSystemTestFactory.Key("."), item.ParentKey);
        Assert.Equal(FileBrowserItemKind.File, item.Kind);
        Assert.Equal(FileBrowserItemCategory.Document, item.Category);
        Assert.Equal(FileBrowserChildState.Empty, item.ChildState);
        Assert.Equal(23, item.Size);
        Assert.Equal("application/pdf", item.MediaType);
        Assert.Equal(filePath, item.DisplayPath);
        Assert.NotNull(item.CreatedAt);
        Assert.NotNull(item.ModifiedAt);
        Assert.Equal(expectedModified, item.ModifiedAt!.Value.UtcDateTime);
        Assert.Null(item.Owner);
        Assert.Null(item.ContentIdentity);
        Assert.Null(item.OpenUri);
        Assert.Null(item.DownloadUri);
        Assert.Equal(FileBrowserCompleteness.Partial, item.MetadataState.Completeness);
        Assert.True(item.MetadataState.ExactFields.HasFlag(FileBrowserMetadataFields.Size));
        Assert.True(item.MetadataState.ExactFields.HasFlag(FileBrowserMetadataFields.Timestamps));
        Assert.True(item.MetadataState.ApproximateFields.HasFlag(FileBrowserMetadataFields.MediaType));
        Assert.True(item.MetadataState.ExactFields.HasFlag(FileBrowserMetadataFields.Custom));
        Assert.Equal("report.pdf", item.Metadata["relative-path"]);
        Assert.Equal("false", item.Metadata["is-reparse-point"]);
        Assert.True(item.Supports(FileBrowserItemCapabilities.Select));
        Assert.True(item.Supports(FileBrowserItemCapabilities.CopyPath));
        Assert.False(item.Supports(FileBrowserItemCapabilities.Navigate));
        Assert.False(item.Supports(FileBrowserItemCapabilities.Open));
        Assert.False(item.Supports(FileBrowserItemCapabilities.DownloadFile));
    }

    [Fact]
    public async Task MinimalMetadataRequestDoesNotPopulateUnrequestedFields()
    {
        using var fileSystem = new TemporaryFileSystem();
        fileSystem.CreateFile("data.json", new byte[12]);
        var provider = FileSystemTestFactory.CreateProvider(fileSystem);
        var metadata = new FileBrowserMetadataRequest(
            FileBrowserMetadataFields.Name | FileBrowserMetadataFields.Kind);

        var page = await provider.BrowseAsync(FileSystemTestFactory.Browse(metadata: metadata));
        var item = Assert.Single(page.Items);

        Assert.Null(item.DisplayPath);
        Assert.Null(item.Size);
        Assert.Null(item.MediaType);
        Assert.Null(item.CreatedAt);
        Assert.Null(item.ModifiedAt);
        Assert.Empty(item.Metadata);
        Assert.Equal(FileBrowserCompleteness.Complete, item.MetadataState.Completeness);
        Assert.False(item.Supports(FileBrowserItemCapabilities.CopyPath));
    }

    [Theory]
    [InlineData("source.cs", FileBrowserItemCategory.Code, "text/x-csharp")]
    [InlineData("image.webp", FileBrowserItemCategory.Image, "image/webp")]
    [InlineData("archive.zip", FileBrowserItemCategory.Archive, "application/zip")]
    [InlineData("table.csv", FileBrowserItemCategory.Data, "text/csv")]
    [InlineData("movie.mp4", FileBrowserItemCategory.Video, "video/mp4")]
    [InlineData("sound.mp3", FileBrowserItemCategory.Audio, "audio/mpeg")]
    [InlineData("unknown.bin", FileBrowserItemCategory.Other, null)]
    public async Task ExtensionMappingProjectsCategoryAndApproximateMediaType(
        string fileName,
        FileBrowserItemCategory category,
        string? mediaType)
    {
        using var fileSystem = new TemporaryFileSystem();
        fileSystem.CreateFile(fileName);
        var provider = FileSystemTestFactory.CreateProvider(fileSystem);

        var item = Assert.Single((await provider.BrowseAsync(FileSystemTestFactory.Browse())).Items);

        Assert.Equal(category, item.Category);
        Assert.Equal(mediaType, item.MediaType);
        Assert.Equal(
            mediaType is null,
            !item.MetadataState.ApproximateFields.HasFlag(FileBrowserMetadataFields.MediaType));
    }

    [Fact]
    public async Task SortingCanRequestRequiredMetadataEvenWhenCallerRequestsMinimalFields()
    {
        using var fileSystem = new TemporaryFileSystem();
        fileSystem.CreateFile("large.txt", new byte[20]);
        fileSystem.CreateFile("small.txt", new byte[2]);
        var provider = FileSystemTestFactory.CreateProvider(fileSystem);
        var request = FileSystemTestFactory.Browse(
            sort: new FileBrowserSortDescriptor(FileBrowserSortField.Size),
            metadata: new FileBrowserMetadataRequest(
                FileBrowserMetadataFields.Name | FileBrowserMetadataFields.Kind));

        var page = await provider.BrowseAsync(request);

        Assert.Equal(["small.txt", "large.txt"], page.Items.Select(item => item.Name));
        Assert.Equal([2L, 20L], page.Items.Select(item => item.Size));
        Assert.All(page.Items, item => Assert.True(
            item.MetadataState.ExactFields.HasFlag(FileBrowserMetadataFields.Size)));
    }

    [Fact]
    public async Task DisabledDirectorySymlinkIsANonNavigableLinkOccurrence()
    {
        using var fileSystem = new TemporaryFileSystem();
        var target = fileSystem.CreateDirectory("target");
        fileSystem.CreateFile("target/inside.txt");
        if (!fileSystem.TryCreateDirectorySymbolicLink("alias", target, out _))
        {
            return;
        }

        var provider = FileSystemTestFactory.CreateProvider(
            fileSystem,
            followDirectoryReparsePoints: false);
        var page = await provider.BrowseAsync(FileSystemTestFactory.Browse(
            metadata: new FileBrowserMetadataRequest(FileBrowserMetadataFields.All)));
        var alias = Assert.Single(page.Items, item => item.Name == "alias");

        Assert.Equal(FileBrowserItemKind.Link, alias.Kind);
        Assert.Equal(FileBrowserItemCategory.Link, alias.Category);
        Assert.Equal(FileBrowserChildState.Empty, alias.ChildState);
        Assert.False(alias.Supports(FileBrowserItemCapabilities.Navigate));
        Assert.Equal("true", alias.Metadata["is-reparse-point"]);

        var browseError = await FileSystemTestFactory.BrowseErrorAsync(
            provider,
            FileSystemTestFactory.Browse("alias"));
        var pathError = await FileSystemTestFactory.PathErrorAsync(
            provider,
            FileSystemTestFactory.Key("alias"));
        Assert.Equal(FileBrowserErrorCode.Unsupported, browseError.Error.Code);
        Assert.Equal(FileBrowserErrorCode.Unsupported, pathError.Error.Code);
    }

    [Fact]
    public async Task EnabledInRootDirectorySymlinkPreservesAliasOccurrenceAndCanBrowseTarget()
    {
        using var fileSystem = new TemporaryFileSystem();
        var target = fileSystem.CreateDirectory("target");
        fileSystem.CreateFile("target/inside.txt");
        if (!fileSystem.TryCreateDirectorySymbolicLink("alias", target, out var aliasPath))
        {
            return;
        }

        var provider = FileSystemTestFactory.CreateProvider(
            fileSystem,
            followDirectoryReparsePoints: true);
        var rootPage = await provider.BrowseAsync(FileSystemTestFactory.Browse());
        var alias = Assert.Single(rootPage.Items, item => item.Name == "alias");
        var aliasPage = await provider.BrowseAsync(FileSystemTestFactory.Browse("alias"));
        var aliasPathItems = await provider.GetPathAsync(
            FileSystemTestFactory.Key("alias"),
            FileBrowserMetadataRequest.Standard);

        Assert.Equal(FileBrowserItemKind.Container, alias.Kind);
        Assert.True(alias.Supports(FileBrowserItemCapabilities.Navigate));
        Assert.Equal(FileSystemTestFactory.Key("alias/inside.txt"), Assert.Single(aliasPage.Items).Key);
        Assert.Equal([".", "alias"], aliasPathItems.Select(item => item.Key.Value));
        Assert.Equal(aliasPath, aliasPathItems[^1].DisplayPath);
    }

    [Fact]
    public async Task EnabledOutOfRootDirectorySymlinkIsForbidden()
    {
        using var fileSystem = new TemporaryFileSystem();
        using var outside = new TemporaryFileSystem();
        outside.CreateFile("outside.txt");
        if (!fileSystem.TryCreateDirectorySymbolicLink("escape", outside.RootPath, out _))
        {
            return;
        }

        var provider = FileSystemTestFactory.CreateProvider(
            fileSystem,
            followDirectoryReparsePoints: true);

        var error = await FileSystemTestFactory.BrowseErrorAsync(
            provider,
            FileSystemTestFactory.Browse());

        Assert.Equal(FileBrowserErrorCode.Forbidden, error.Error.Code);
        Assert.False(error.Error.IsRetryable);
    }
}
