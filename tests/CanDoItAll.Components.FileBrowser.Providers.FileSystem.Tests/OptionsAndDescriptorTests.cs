namespace CanDoItAll.Components.FileBrowser.Providers.FileSystem.Tests;

public sealed class OptionsAndDescriptorTests
{
    [Fact]
    public void OptionsNormalizeAnExistingRootAndConfiguredPolicies()
    {
        using var fileSystem = new TemporaryFileSystem();
        var rootWithSeparator = fileSystem.RootPath + Path.DirectorySeparatorChar;

        var options = new FileSystemFileBrowserOptions(
            FileSystemTestFactory.SourceId,
            rootWithSeparator,
            displayName: "  Workspace files  ",
            includeHidden: true,
            followDirectoryReparsePoints: true,
            recommendedPageSize: 20,
            maximumPageSize: 80);

        Assert.Equal(Path.TrimEndingDirectorySeparator(fileSystem.RootPath), options.RootPath);
        Assert.Equal("Workspace files", options.DisplayName);
        Assert.Equal(FileSystemHiddenItemPolicy.Include, options.HiddenItemPolicy);
        Assert.True(options.IncludeHidden);
        Assert.True(options.FollowDirectoryReparsePoints);
        Assert.Equal(20, options.RecommendedPageSize);
        Assert.Equal(80, options.MaximumPageSize);
    }

    [Fact]
    public void OptionsUseRootDirectoryNameAsDefaultDisplayName()
    {
        using var fileSystem = new TemporaryFileSystem();

        var options = new FileSystemFileBrowserOptions(
            FileSystemTestFactory.SourceId,
            fileSystem.RootPath);

        Assert.Equal(new DirectoryInfo(fileSystem.RootPath).Name, options.DisplayName);
        Assert.Equal(FileSystemHiddenItemPolicy.Exclude, options.HiddenItemPolicy);
        Assert.False(options.IncludeHidden);
    }

    [Fact]
    public void OptionsRejectRelativeAndMissingRoots()
    {
        Assert.Throws<ArgumentException>(() => new FileSystemFileBrowserOptions(
            FileSystemTestFactory.SourceId,
            "relative-root"));

        var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Assert.Throws<DirectoryNotFoundException>(() => new FileSystemFileBrowserOptions(
            FileSystemTestFactory.SourceId,
            missing));
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1001, 1001)]
    [InlineData(10, 0)]
    [InlineData(10, 1001)]
    [InlineData(20, 10)]
    public void OptionsRejectInvalidPageLimits(int recommended, int maximum)
    {
        using var fileSystem = new TemporaryFileSystem();

        Assert.Throws<ArgumentOutOfRangeException>(() => new FileSystemFileBrowserOptions(
            FileSystemTestFactory.SourceId,
            fileSystem.RootPath,
            recommendedPageSize: recommended,
            maximumPageSize: maximum));
    }

    [Fact]
    public void DescriptorAccuratelyAdvertisesTheShallowFilesystemContract()
    {
        using var fileSystem = new TemporaryFileSystem();
        var provider = FileSystemTestFactory.CreateProvider(
            fileSystem,
            includeHidden: true,
            followDirectoryReparsePoints: false,
            recommendedPageSize: 7,
            maximumPageSize: 19);

        var descriptor = provider.Descriptor;
        Assert.Equal(FileSystemTestFactory.SourceId, descriptor.Id);
        Assert.Equal("Test files", descriptor.DisplayName);
        Assert.Equal("folder", descriptor.Icon);
        Assert.Equal(FileBrowserSourceCapabilities.PagedBrowse, descriptor.Capabilities);
        Assert.Equal(7, descriptor.RecommendedPageSize);
        Assert.Equal(19, descriptor.MaximumPageSize);
        Assert.Equal(
            [
                FileBrowserSortField.Name,
                FileBrowserSortField.ModifiedAt,
                FileBrowserSortField.Size,
                FileBrowserSortField.Type,
                FileBrowserSortField.Path
            ],
            descriptor.SupportedSortFields.Order());
        Assert.Equal(
            [
                FileBrowserSearchScope.LoadedFolder,
                FileBrowserSearchScope.LoadedDescendants,
                FileBrowserSearchScope.Progressive
            ],
            descriptor.SupportedSearchScopes.Order());
        Assert.Equal("filesystem", descriptor.Metadata["provider"]);
        Assert.Equal(provider.Options.RootPath, descriptor.Metadata["root-path"]);
        Assert.Equal("included", descriptor.Metadata["hidden-items"]);
        Assert.Equal("false", descriptor.Metadata["follows-directory-reparse-points"]);
        Assert.False(descriptor.Supports(FileBrowserSourceCapabilities.NativeSearch));
        Assert.False(descriptor.Supports(FileBrowserSourceCapabilities.ContentRead));
    }

    [Fact]
    public void ProviderRequiresOptions()
    {
        Assert.Throws<ArgumentNullException>(() => new FileSystemFileBrowserProvider(null!));
    }
}

