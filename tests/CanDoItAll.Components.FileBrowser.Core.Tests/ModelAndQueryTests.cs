namespace CanDoItAll.Components.FileBrowser.Core.Tests;

public sealed class ModelAndQueryTests
{
    [Fact]
    public void OccurrenceKeysRemainDistinctWhenContentIdentityIsShared()
    {
        var content = new FileBrowserContentIdentity(" CID ", "bafy-shared");
        var first = TestFileBrowserFactory.File(
            "projects/alpha/readme",
            TestFileBrowserFactory.Key("projects/alpha"),
            "README.md",
            contentIdentity: content);
        var second = TestFileBrowserFactory.File(
            "projects/beta/readme",
            TestFileBrowserFactory.Key("projects/beta"),
            "README.md",
            contentIdentity: new FileBrowserContentIdentity("cid", "bafy-shared"));

        Assert.NotEqual(first.Key, second.Key);
        Assert.Equal(first.ContentIdentity, second.ContentIdentity);
        Assert.Equal("cid:bafy-shared", content.ToString());
        Assert.Equal("source:projects/alpha/readme", first.Key.ToString());
    }

    [Fact]
    public void IdentityValuesTrimAndRevisionParticipatesInOccurrenceIdentity()
    {
        var source = new FileBrowserSourceId(" project-files ");
        var current = new FileBrowserItemKey(source, " /docs/readme ", " rev-2 ");
        var previous = new FileBrowserItemKey(source, "/docs/readme", "rev-1");

        Assert.Equal("project-files", source.Value);
        Assert.Equal("/docs/readme", current.Value);
        Assert.Equal("rev-2", current.Revision);
        Assert.NotEqual(previous, current);
        Assert.Equal("project-files:/docs/readme@rev-2", current.ToString());
    }

    [Fact]
    public void IdentityConstructorsRejectMissingSourceOccurrenceAndContentValues()
    {
        Assert.Throws<ArgumentException>(() => new FileBrowserSourceId(" "));
        Assert.Throws<ArgumentException>(() => new FileBrowserItemKey(default, "item"));
        Assert.Throws<ArgumentException>(() => new FileBrowserItemKey(TestFileBrowserFactory.Source(), ""));
        Assert.Throws<ArgumentException>(() => new FileBrowserContentIdentity(" ", "hash"));
        Assert.Throws<ArgumentException>(() => new FileBrowserContentIdentity("cid", " "));
    }

    [Fact]
    public void ItemNormalizesDisplayValuesAndCopiesCustomMetadata()
    {
        var metadata = new Dictionary<string, string> { ["provider"] = "project" };
        var item = new FileBrowserItem(
            TestFileBrowserFactory.Key("docs/spec"),
            TestFileBrowserFactory.Key("docs"),
            "  Specification.md  ",
            FileBrowserItemKind.File,
            FileBrowserItemCategory.Document,
            displayPath: "/docs/Specification.md",
            childState: FileBrowserChildState.Empty,
            size: 42,
            mediaType: " text/markdown ",
            owner: " team-a ",
            capabilities: FileBrowserItemCapabilities.Select | FileBrowserItemCapabilities.CopyPath,
            openUri: " /viewer/spec ",
            metadata: metadata);

        metadata["provider"] = "mutated";

        Assert.Equal("Specification.md", item.Name);
        Assert.Equal(" text/markdown ", item.MediaType);
        Assert.Equal(" team-a ", item.Owner);
        Assert.Equal("/viewer/spec", item.OpenUri);
        Assert.Equal("project", item.Metadata["provider"]);
        Assert.True(item.Supports(FileBrowserItemCapabilities.CopyPath));
        Assert.False(item.Supports(FileBrowserItemCapabilities.DownloadFile));
    }

    [Fact]
    public void ItemRejectsInvalidHierarchyCountsSizeChildStateAndUri()
    {
        var key = TestFileBrowserFactory.Key("item");

        Assert.Throws<ArgumentException>(() => new FileBrowserItem(
            default,
            null,
            "item",
            FileBrowserItemKind.File,
            FileBrowserItemCategory.Document));
        Assert.Throws<ArgumentException>(() => new FileBrowserItem(
            key,
            TestFileBrowserFactory.Key("parent", "other"),
            "item",
            FileBrowserItemKind.File,
            FileBrowserItemCategory.Document));
        Assert.Throws<ArgumentOutOfRangeException>(() => new FileBrowserItem(
            key,
            null,
            "item",
            FileBrowserItemKind.Container,
            FileBrowserItemCategory.Folder,
            childCount: -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new FileBrowserItem(
            key,
            null,
            "item",
            FileBrowserItemKind.File,
            FileBrowserItemCategory.Document,
            size: -1));
        Assert.Throws<ArgumentException>(() => new FileBrowserItem(
            key,
            null,
            "item",
            FileBrowserItemKind.Link,
            FileBrowserItemCategory.Link,
            childState: FileBrowserChildState.HasChildren));
        Assert.Throws<ArgumentException>(() => new FileBrowserItem(
            key,
            null,
            "item",
            FileBrowserItemKind.File,
            FileBrowserItemCategory.Document,
            openUri: "http://[invalid"));
    }

    [Fact]
    public void SourceDescriptorRequiresNativeSearchScopeAndValidPageBounds()
    {
        var source = TestFileBrowserFactory.Source();

        Assert.Throws<ArgumentOutOfRangeException>(() => new FileBrowserSourceDescriptor(
            source,
            "Source",
            recommendedPageSize: 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new FileBrowserSourceDescriptor(
            source,
            "Source",
            recommendedPageSize: 100,
            maximumPageSize: 99));
        Assert.Throws<ArgumentException>(() => new FileBrowserSourceDescriptor(
            source,
            "Source",
            capabilities: FileBrowserSourceCapabilities.PagedBrowse | FileBrowserSourceCapabilities.NativeSearch,
            supportedSearchScopes: [FileBrowserSearchScope.LoadedFolder]));

        var descriptor = new FileBrowserSourceDescriptor(
            source,
            " Project files ",
            icon: " ",
            capabilities: FileBrowserSourceCapabilities.PagedBrowse | FileBrowserSourceCapabilities.NativeSearch,
            supportedSearchScopes: [FileBrowserSearchScope.LoadedFolder, FileBrowserSearchScope.Provider]);

        Assert.Equal("Project files", descriptor.DisplayName);
        Assert.Equal("folder", descriptor.Icon);
        Assert.True(descriptor.Supports(FileBrowserSourceCapabilities.NativeSearch));
        Assert.Contains(FileBrowserSearchScope.Provider, descriptor.SupportedSearchScopes);
    }

    [Fact]
    public void FilterIntersectsKindCategoryExtensionAndMediaType()
    {
        var parent = TestFileBrowserFactory.Key("root");
        var filter = new FileBrowserFilter(
            kinds: [FileBrowserItemKind.File],
            categories: [FileBrowserItemCategory.Image],
            extensions: [" PNG ", ".png"],
            mediaTypePrefix: " IMAGE/ ");
        var matching = TestFileBrowserFactory.File(
            "hero",
            parent,
            "Hero.PNG",
            mediaType: "image/png",
            category: FileBrowserItemCategory.Image);
        var wrongCategory = TestFileBrowserFactory.File(
            "data",
            parent,
            "data.png",
            mediaType: "image/png",
            category: FileBrowserItemCategory.Data);
        var wrongExtension = TestFileBrowserFactory.File(
            "photo",
            parent,
            "photo.jpg",
            mediaType: "image/jpeg",
            category: FileBrowserItemCategory.Image);
        var wrongMedia = TestFileBrowserFactory.File(
            "fake",
            parent,
            "fake.png",
            mediaType: "text/plain",
            category: FileBrowserItemCategory.Image);
        var folder = TestFileBrowserFactory.Container("assets", parent);

        Assert.Equal([".png"], filter.Extensions);
        Assert.True(filter.Matches(matching));
        Assert.False(filter.Matches(wrongCategory));
        Assert.False(filter.Matches(wrongExtension));
        Assert.False(filter.Matches(wrongMedia));
        Assert.False(filter.Matches(folder));
    }

    [Fact]
    public void ExtensionOnlyFilterDoesNotExcludeContainers()
    {
        var filter = new FileBrowserFilter(extensions: [".cs"]);
        var parent = TestFileBrowserFactory.Key("root");

        Assert.True(filter.Matches(TestFileBrowserFactory.Container("src", parent)));
        Assert.True(filter.Matches(TestFileBrowserFactory.File("code", parent, "Program.CS")));
        Assert.False(filter.Matches(TestFileBrowserFactory.File("doc", parent, "README.md")));
    }

    [Fact]
    public void OrderingKeepsFoldersFirstThenAppliesDirectionAndStableTies()
    {
        var parent = TestFileBrowserFactory.Key("root");
        var items = new[]
        {
            TestFileBrowserFactory.File("b", parent, "same.txt", size: 10),
            TestFileBrowserFactory.File("a", parent, "same.txt", size: 10),
            TestFileBrowserFactory.File("large", parent, "large.txt", size: 30),
            TestFileBrowserFactory.Container("folder", parent, "z-folder")
        };

        var ordered = FileBrowserItemOrdering.Apply(
            items,
            new FileBrowserSortDescriptor(
                FileBrowserSortField.Size,
                FileBrowserSortDirection.Descending,
                FoldersFirst: true));

        Assert.Equal(["folder", "large", "a", "b"], ordered.Select(item => item.Key.Value));
    }

    [Fact]
    public void OrderingUsesNameAsAscendingTieBreakerEvenForDescendingField()
    {
        var parent = TestFileBrowserFactory.Key("root");
        var items = new[]
        {
            TestFileBrowserFactory.File("z", parent, "Zulu", owner: "same"),
            TestFileBrowserFactory.File("a", parent, "Alpha", owner: "same"),
            TestFileBrowserFactory.File("m", parent, "Middle", owner: "top")
        };

        var ordered = FileBrowserItemOrdering.Apply(
            items,
            new FileBrowserSortDescriptor(
                FileBrowserSortField.Owner,
                FileBrowserSortDirection.Descending,
                FoldersFirst: false));

        Assert.Equal(["m", "a", "z"], ordered.Select(item => item.Key.Value));
    }

    [Fact]
    public void QueryFingerprintIgnoresPageCursorsAndNormalizesFilterSetOrder()
    {
        var parent = TestFileBrowserFactory.Key("root", revision: "revision-7");
        var first = TestFileBrowserFactory.BrowseRequest(
            parent,
            pageSize: 75,
            sort: new FileBrowserSortDescriptor(FileBrowserSortField.Type, FileBrowserSortDirection.Descending, false),
            filter: new FileBrowserFilter(
                kinds: [FileBrowserItemKind.Link, FileBrowserItemKind.File],
                categories: [FileBrowserItemCategory.Code, FileBrowserItemCategory.Document],
                extensions: [".JSON", "cs"],
                mediaTypePrefix: "text/"),
            includeDescendants: true,
            consistencyToken: "cursor-revision-a",
            metadata: new FileBrowserMetadataRequest(FileBrowserMetadataFields.All, IncludeExpensive: true));
        var continued = new FileBrowserBrowseRequest(
            parent,
            75,
            continuationToken: "page-2",
            sort: first.Sort,
            filter: new FileBrowserFilter(
                kinds: [FileBrowserItemKind.File, FileBrowserItemKind.Link],
                categories: [FileBrowserItemCategory.Document, FileBrowserItemCategory.Code],
                extensions: ["CS", ".json"],
                mediaTypePrefix: "text/"),
            includeDescendants: true,
            consistencyToken: "cursor-revision-b",
            metadata: first.Metadata);

        Assert.Equal(FileBrowserQueryFingerprint.From(first), FileBrowserQueryFingerprint.From(continued));
        Assert.Equal(64, FileBrowserQueryFingerprint.From(first).Value.Length);
    }

    [Fact]
    public void QueryFingerprintChangesForEverySemanticQueryDimension()
    {
        var parent = TestFileBrowserFactory.Key("root");
        var baseline = TestFileBrowserFactory.BrowseRequest(parent);
        FileBrowserBrowseRequest[] variants =
        [
            TestFileBrowserFactory.BrowseRequest(TestFileBrowserFactory.Key("other")),
            TestFileBrowserFactory.BrowseRequest(parent, pageSize: 3),
            TestFileBrowserFactory.BrowseRequest(parent, sort: new FileBrowserSortDescriptor(FileBrowserSortField.Size)),
            TestFileBrowserFactory.BrowseRequest(parent, sort: new FileBrowserSortDescriptor(Direction: FileBrowserSortDirection.Descending)),
            TestFileBrowserFactory.BrowseRequest(parent, sort: new FileBrowserSortDescriptor(FoldersFirst: false)),
            TestFileBrowserFactory.BrowseRequest(parent, filter: new FileBrowserFilter(kinds: [FileBrowserItemKind.File])),
            TestFileBrowserFactory.BrowseRequest(parent, filter: new FileBrowserFilter(categories: [FileBrowserItemCategory.Code])),
            TestFileBrowserFactory.BrowseRequest(parent, filter: new FileBrowserFilter(extensions: [".cs"])),
            TestFileBrowserFactory.BrowseRequest(parent, filter: new FileBrowserFilter(mediaTypePrefix: "text/")),
            TestFileBrowserFactory.BrowseRequest(parent, includeDescendants: true),
            TestFileBrowserFactory.BrowseRequest(parent, metadata: new FileBrowserMetadataRequest(FileBrowserMetadataFields.Name)),
            TestFileBrowserFactory.BrowseRequest(parent, metadata: new FileBrowserMetadataRequest(FileBrowserMetadataFields.Standard, true))
        ];

        var fingerprints = variants
            .Prepend(baseline)
            .Select(FileBrowserQueryFingerprint.From)
            .ToArray();

        Assert.Equal(fingerprints.Length, fingerprints.Distinct().Count());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1001)]
    public void BrowseAndSearchRequestsRejectOutOfRangePageSizes(int pageSize)
    {
        var parent = TestFileBrowserFactory.Key("root");

        Assert.Throws<ArgumentOutOfRangeException>(() => new FileBrowserBrowseRequest(parent, pageSize));
        Assert.Throws<ArgumentOutOfRangeException>(() => new FileBrowserSearchRequest(
            parent,
            "query",
            FileBrowserSearchScope.LoadedFolder,
            pageSize));
    }

    [Fact]
    public void SearchRequestRequiresValidContainerAndNonBlankQuery()
    {
        Assert.Throws<ArgumentException>(() => new FileBrowserSearchRequest(
            default,
            "query",
            FileBrowserSearchScope.LoadedFolder));
        Assert.Throws<ArgumentException>(() => new FileBrowserSearchRequest(
            TestFileBrowserFactory.Key("root"),
            " ",
            FileBrowserSearchScope.LoadedFolder));
    }

    [Fact]
    public void ActionReadErrorAndPageContractsRejectInvalidBounds()
    {
        var key = TestFileBrowserFactory.Key("file");

        Assert.Throws<ArgumentException>(() => new FileBrowserActionDescriptor(" ", "Open", "open"));
        Assert.Throws<ArgumentException>(() => new FileBrowserActionRequest(default, FileBrowserActionIds.Open));
        Assert.Throws<ArgumentException>(() => new FileBrowserActionRequest(key, " "));
        Assert.Throws<ArgumentException>(() => new FileBrowserReadRequest(default));
        Assert.Throws<ArgumentOutOfRangeException>(() => new FileBrowserReadRequest(key, Offset: -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new FileBrowserReadRequest(key, Length: 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new FileBrowserError(
            FileBrowserErrorCode.RateLimited,
            "Retry later",
            retryAfter: TimeSpan.FromSeconds(-1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new FileBrowserPage([], totalCount: -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new FileBrowserSearchPage(
            [],
            "loaded",
            scannedItems: -1));
    }

    [Fact]
    public async Task ContentLeaseNormalizesMetadataAndHonorsStreamOwnership()
    {
        var borrowedStream = new MemoryStream([1, 2, 3]);
        var borrowed = new FileBrowserContentLease(
            borrowedStream,
            mediaType: " application/octet-stream ",
            length: 3,
            ownsStream: false);

        await borrowed.DisposeAsync();

        Assert.True(borrowedStream.CanRead);
        Assert.Equal("application/octet-stream", borrowed.MediaType);
        Assert.Equal(3, borrowed.Length);

        var ownedStream = new MemoryStream([4, 5]);
        var owned = new FileBrowserContentLease(ownedStream, ownsStream: true);
        await owned.DisposeAsync();

        Assert.False(ownedStream.CanRead);
    }
}
