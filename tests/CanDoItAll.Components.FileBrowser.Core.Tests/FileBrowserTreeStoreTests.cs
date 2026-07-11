namespace CanDoItAll.Components.FileBrowser.Core.Tests;

public sealed class FileBrowserTreeStoreTests
{
    [Fact]
    public void ReplacePageRemovesPreviousQueryItemsAndResetsPageState()
    {
        var store = new FileBrowserTreeStore();
        var root = TestFileBrowserFactory.Key("root");
        var request = TestFileBrowserFactory.BrowseRequest(root);
        store.ApplyPage(
            request,
            new FileBrowserPage(
                [
                    TestFileBrowserFactory.File("old-a", root),
                    TestFileBrowserFactory.File("old-b", root)
                ],
                nextContinuationToken: "page-2",
                totalCount: 3,
                consistencyToken: "revision-1",
                completeness: FileBrowserCompleteness.Partial,
                warnings: [new FileBrowserPageWarning("partial", "More data remains.")]),
            FileBrowserPageApplyMode.Replace);

        var replaced = store.ApplyPage(
            request,
            new FileBrowserPage(
                [TestFileBrowserFactory.File("new", root)],
                totalCount: 1,
                consistencyToken: "revision-2"),
            FileBrowserPageApplyMode.Replace);

        Assert.Equal(["new"], replaced.Items.Select(item => item.Key.Value));
        Assert.Equal(1, replaced.LoadedPageCount);
        Assert.Equal(1, replaced.TotalCount);
        Assert.Equal("revision-2", replaced.ConsistencyToken);
        Assert.Null(replaced.NextContinuationToken);
        Assert.Empty(replaced.Warnings);
        Assert.True(replaced.IsComplete);
        Assert.Equal(["new"], store.GetLoadedChildren(root).Select(item => item.Key.Value));
    }

    [Fact]
    public void ContainerIsCompleteRequiresProviderCompletenessEvenWithoutCursorOrError()
    {
        var store = new FileBrowserTreeStore();
        var root = TestFileBrowserFactory.Key("root");
        var request = TestFileBrowserFactory.BrowseRequest(root);

        var partial = store.ApplyPage(
            request,
            new FileBrowserPage(
                [TestFileBrowserFactory.File("known", root)],
                completeness: FileBrowserCompleteness.Partial),
            FileBrowserPageApplyMode.Replace);

        Assert.False(partial.HasMore);
        Assert.Null(partial.Error);
        Assert.False(partial.IsComplete);

        var complete = store.ApplyPage(
            request,
            new FileBrowserPage([TestFileBrowserFactory.File("known", root)]),
            FileBrowserPageApplyMode.Replace);

        Assert.True(complete.IsComplete);
    }

    [Fact]
    public void AppendPageDeduplicatesOverlappingOccurrencesAndUsesNewestMetadata()
    {
        var store = new FileBrowserTreeStore();
        var root = TestFileBrowserFactory.Key("root");
        var firstRequest = TestFileBrowserFactory.BrowseRequest(root, consistencyToken: "revision-1");
        store.ApplyPage(
            firstRequest,
            new FileBrowserPage(
                [
                    TestFileBrowserFactory.File("a", root, "Alpha"),
                    TestFileBrowserFactory.File("b", root, "Beta old", size: 10)
                ],
                nextContinuationToken: "page-2",
                totalCount: 3,
                consistencyToken: "revision-1"),
            FileBrowserPageApplyMode.Replace);

        var appended = store.ApplyPage(
            firstRequest.Next("page-2", "revision-1"),
            new FileBrowserPage(
                [
                    TestFileBrowserFactory.File("b", root, "Beta current", size: 20),
                    TestFileBrowserFactory.File("c", root, "Charlie")
                ],
                totalCount: 3,
                consistencyToken: "revision-1"),
            FileBrowserPageApplyMode.Append);

        Assert.Equal(["a", "b", "c"], appended.Items.Select(item => item.Key.Value));
        var beta = Assert.Single(appended.Items, item => item.Key.Value == "b");
        Assert.Equal("Beta current", beta.Name);
        Assert.Equal(20, beta.Size);
        Assert.Equal(2, appended.LoadedPageCount);
        Assert.Equal(3, appended.TotalCount);
        Assert.Null(appended.NextContinuationToken);
        Assert.True(appended.IsComplete);
    }

    [Fact]
    public void AppendRejectsMissingOrMismatchedContinuationCursor()
    {
        var store = new FileBrowserTreeStore();
        var root = TestFileBrowserFactory.Key("root");
        var request = TestFileBrowserFactory.BrowseRequest(root);
        store.ApplyPage(
            request,
            new FileBrowserPage([], nextContinuationToken: "expected"),
            FileBrowserPageApplyMode.Replace);

        Assert.Throws<InvalidOperationException>(() => store.ApplyPage(
            request,
            new FileBrowserPage([]),
            FileBrowserPageApplyMode.Append));

        var stale = Assert.Throws<FileBrowserProviderException>(() => store.ApplyPage(
            request.Next("wrong"),
            new FileBrowserPage([]),
            FileBrowserPageApplyMode.Append));

        Assert.Equal(FileBrowserErrorCode.StaleCursor, stale.Error.Code);
        Assert.True(stale.Error.IsRetryable);
        Assert.True(store.TryGetContainer(request, out var unchanged));
        Assert.Equal("expected", unchanged!.NextContinuationToken);
        Assert.Equal(1, unchanged.LoadedPageCount);
    }

    [Fact]
    public void AppendRejectsContinuationWhenTheFirstPageIsNotLoaded()
    {
        var store = new FileBrowserTreeStore();
        var request = TestFileBrowserFactory.BrowseRequest(
            TestFileBrowserFactory.Key("root"),
            continuationToken: "orphaned-page-2");

        var exception = Assert.Throws<FileBrowserProviderException>(() => store.ApplyPage(
            request,
            new FileBrowserPage([TestFileBrowserFactory.File("item", request.ParentKey)]),
            FileBrowserPageApplyMode.Append));

        Assert.Equal(FileBrowserErrorCode.StaleCursor, exception.Error.Code);
        Assert.True(exception.Error.IsRetryable);
        Assert.False(store.TryGetContainer(request.FirstPage(), out _));
    }

    [Fact]
    public void ApplyPageRejectsConsistencyChangesAndItemsFromAnotherSource()
    {
        var store = new FileBrowserTreeStore();
        var root = TestFileBrowserFactory.Key("root");
        var request = TestFileBrowserFactory.BrowseRequest(root, consistencyToken: "revision-1");

        var consistency = Assert.Throws<FileBrowserProviderException>(() => store.ApplyPage(
            request,
            new FileBrowserPage([], consistencyToken: "revision-2"),
            FileBrowserPageApplyMode.Replace));
        var foreignItem = Assert.Throws<FileBrowserProviderException>(() => store.ApplyPage(
            request,
            new FileBrowserPage([TestFileBrowserFactory.File("foreign", source: "other")]),
            FileBrowserPageApplyMode.Replace));

        Assert.Equal(FileBrowserErrorCode.StaleCursor, consistency.Error.Code);
        Assert.True(consistency.Error.IsRetryable);
        Assert.Equal(FileBrowserErrorCode.CorruptProviderResponse, foreignItem.Error.Code);
        Assert.False(store.TryGetContainer(request, out _));
    }

    [Fact]
    public void ShallowPageRejectsMissingDifferentAndSelfParents()
    {
        var store = new FileBrowserTreeStore();
        var root = TestFileBrowserFactory.Key("root");
        var request = TestFileBrowserFactory.BrowseRequest(root);

        var missingParent = Assert.Throws<FileBrowserProviderException>(() => store.ApplyPage(
            request,
            new FileBrowserPage([TestFileBrowserFactory.File("missing-parent")]),
            FileBrowserPageApplyMode.Replace));
        var differentParent = Assert.Throws<FileBrowserProviderException>(() => store.ApplyPage(
            request,
            new FileBrowserPage([
                TestFileBrowserFactory.File("wrong-parent", TestFileBrowserFactory.Key("other-folder"))
            ]),
            FileBrowserPageApplyMode.Replace));
        var selfParent = Assert.Throws<FileBrowserProviderException>(() => store.ApplyPage(
            request,
            new FileBrowserPage([TestFileBrowserFactory.Container("root", root)]),
            FileBrowserPageApplyMode.Replace));

        Assert.Equal(FileBrowserErrorCode.CorruptProviderResponse, missingParent.Error.Code);
        Assert.Equal(FileBrowserErrorCode.CorruptProviderResponse, differentParent.Error.Code);
        Assert.Equal(FileBrowserErrorCode.CorruptProviderResponse, selfParent.Error.Code);
        Assert.False(store.TryGetContainer(request, out _));
    }

    [Fact]
    public void PageAllowsEquivalentDuplicateDescriptorsButRejectsConflicts()
    {
        var store = new FileBrowserTreeStore();
        var root = TestFileBrowserFactory.Key("root");
        var request = TestFileBrowserFactory.BrowseRequest(root);
        var first = TestFileBrowserFactory.File(
            "same",
            root,
            "Same",
            size: 42,
            metadata: new Dictionary<string, string> { ["kind"] = "proof" });
        var equivalent = TestFileBrowserFactory.File(
            "same",
            root,
            "Same",
            size: 42,
            metadata: new Dictionary<string, string> { ["kind"] = "proof" });

        var accepted = store.ApplyPage(
            request,
            new FileBrowserPage([first, equivalent]),
            FileBrowserPageApplyMode.Replace);

        Assert.Single(accepted.Items);

        var conflict = Assert.Throws<FileBrowserProviderException>(() => store.ApplyPage(
            request,
            new FileBrowserPage([first, TestFileBrowserFactory.File("same", root, "Changed")]),
            FileBrowserPageApplyMode.Replace));

        Assert.Equal(FileBrowserErrorCode.CorruptProviderResponse, conflict.Error.Code);
        Assert.True(store.TryGetContainer(request, out var unchanged));
        Assert.Equal("Same", Assert.Single(unchanged!.Items).Name);
    }

    [Fact]
    public void RecursivePageAllowsRepresentedDepthAndOmittedIntermediateAncestors()
    {
        var store = new FileBrowserTreeStore();
        var root = TestFileBrowserFactory.Key("root");
        var folder = TestFileBrowserFactory.Container("folder", root);
        var nested = TestFileBrowserFactory.File("nested", folder.Key);
        var omittedParent = TestFileBrowserFactory.Key("omitted-parent");
        var pagedDescendant = TestFileBrowserFactory.File("paged-descendant", omittedParent);
        var request = TestFileBrowserFactory.BrowseRequest(root, pageSize: 3, includeDescendants: true);

        var snapshot = store.ApplyPage(
            request,
            new FileBrowserPage([nested, pagedDescendant, folder]),
            FileBrowserPageApplyMode.Replace);

        Assert.Equal(3, snapshot.Items.Count);
        Assert.Contains(snapshot.Items, item => item.Key == nested.Key);
        Assert.Contains(snapshot.Items, item => item.Key == pagedDescendant.Key);
    }

    [Fact]
    public void RecursivePageRejectsVisibleCyclesAndFullyRepresentedUnrootedChains()
    {
        var store = new FileBrowserTreeStore();
        var root = TestFileBrowserFactory.Key("root");
        var firstKey = TestFileBrowserFactory.Key("first");
        var secondKey = TestFileBrowserFactory.Key("second");
        var request = TestFileBrowserFactory.BrowseRequest(root, includeDescendants: true);

        var cycle = Assert.Throws<FileBrowserProviderException>(() => store.ApplyPage(
            request,
            new FileBrowserPage([
                TestFileBrowserFactory.Container("first", secondKey),
                TestFileBrowserFactory.Container("second", firstKey)
            ]),
            FileBrowserPageApplyMode.Replace));

        var unrooted = Assert.Throws<FileBrowserProviderException>(() => store.ApplyPage(
            request,
            new FileBrowserPage([
                TestFileBrowserFactory.File("leaf", firstKey),
                TestFileBrowserFactory.Container("first")
            ]),
            FileBrowserPageApplyMode.Replace));

        Assert.Equal(FileBrowserErrorCode.CorruptProviderResponse, cycle.Error.Code);
        Assert.Equal(FileBrowserErrorCode.CorruptProviderResponse, unrooted.Error.Code);
        Assert.False(store.TryGetContainer(request, out _));
    }

    [Fact]
    public void AppendRejectsRevisionChangeFromPreviouslyLoadedPage()
    {
        var store = new FileBrowserTreeStore();
        var root = TestFileBrowserFactory.Key("root");
        var first = TestFileBrowserFactory.BrowseRequest(root);
        store.ApplyPage(
            first,
            new FileBrowserPage([], nextContinuationToken: "page-2", consistencyToken: "revision-1"),
            FileBrowserPageApplyMode.Replace);

        var exception = Assert.Throws<FileBrowserProviderException>(() => store.ApplyPage(
            first.Next("page-2"),
            new FileBrowserPage([], consistencyToken: "revision-2"),
            FileBrowserPageApplyMode.Append));

        Assert.Equal(FileBrowserErrorCode.StaleCursor, exception.Error.Code);
        Assert.True(exception.Error.IsRetryable);
    }

    [Fact]
    public void AppendPreservesPartialCompletenessAndAccumulatesWarnings()
    {
        var store = new FileBrowserTreeStore();
        var root = TestFileBrowserFactory.Key("root");
        var request = TestFileBrowserFactory.BrowseRequest(root);
        var firstWarning = new FileBrowserPageWarning("first-partial", "First page is partial.");
        var secondWarning = new FileBrowserPageWarning("second-page", "Second page warning.");
        store.ApplyPage(
            request,
            new FileBrowserPage(
                [TestFileBrowserFactory.File("first", root)],
                nextContinuationToken: "cursor-2",
                totalCount: 2,
                completeness: FileBrowserCompleteness.Partial,
                warnings: [firstWarning]),
            FileBrowserPageApplyMode.Replace);

        FileBrowserContainerSnapshot appended = store.ApplyPage(
            request.Next("cursor-2"),
            new FileBrowserPage(
                [TestFileBrowserFactory.File("second", root)],
                totalCount: 2,
                completeness: FileBrowserCompleteness.Complete,
                warnings: [secondWarning]),
            FileBrowserPageApplyMode.Append);

        Assert.Equal(FileBrowserCompleteness.Partial, appended.Completeness);
        Assert.False(appended.IsComplete);
        Assert.Equal([firstWarning, secondWarning], appended.Warnings);
    }

    [Fact]
    public void BrowsePagesRejectBoundsCountChangesAndCursorCyclesBeforeMutation()
    {
        var store = new FileBrowserTreeStore();
        var root = TestFileBrowserFactory.Key("root");
        var request = TestFileBrowserFactory.BrowseRequest(root, pageSize: 1);

        FileBrowserProviderException oversized = Assert.Throws<FileBrowserProviderException>(() =>
            store.ApplyPage(
                request,
                new FileBrowserPage([
                    TestFileBrowserFactory.File("one", root),
                    TestFileBrowserFactory.File("two", root)
                ]),
                FileBrowserPageApplyMode.Replace));
        Assert.Equal(FileBrowserErrorCode.CorruptProviderResponse, oversized.Error.Code);
        Assert.False(store.TryGetContainer(request, out _));

        store.ApplyPage(
            request,
            new FileBrowserPage(
                [TestFileBrowserFactory.File("one", root)],
                nextContinuationToken: "cursor-a",
                totalCount: 2),
            FileBrowserPageApplyMode.Replace);
        store.ApplyPage(
            request.Next("cursor-a"),
            new FileBrowserPage(
                [TestFileBrowserFactory.File("two", root)],
                nextContinuationToken: "cursor-b",
                totalCount: 2),
            FileBrowserPageApplyMode.Append);

        FileBrowserProviderException cycle = Assert.Throws<FileBrowserProviderException>(() =>
            store.ApplyPage(
                request.Next("cursor-b"),
                new FileBrowserPage([], nextContinuationToken: "cursor-a", totalCount: 2),
                FileBrowserPageApplyMode.Append));
        FileBrowserProviderException changedCount = Assert.Throws<FileBrowserProviderException>(() =>
            store.ApplyPage(
                request.Next("cursor-b"),
                new FileBrowserPage([], totalCount: 3),
                FileBrowserPageApplyMode.Append));

        Assert.Equal(FileBrowserErrorCode.CorruptProviderResponse, cycle.Error.Code);
        Assert.Equal(FileBrowserErrorCode.CorruptProviderResponse, changedCount.Error.Code);
        Assert.True(store.TryGetContainer(request, out var unchanged));
        Assert.Equal("cursor-b", unchanged!.NextContinuationToken);
        Assert.Equal(2, unchanged.LoadedPageCount);
        Assert.Equal(2, unchanged.TotalCount);
    }

    [Fact]
    public void ContainerQueriesUseLeastRecentlyUsedEviction()
    {
        var store = new FileBrowserTreeStore(new FileBrowserTreeStoreOptions(maximumContainers: 2, maximumItems: 100));
        var requestA = TestFileBrowserFactory.BrowseRequest(TestFileBrowserFactory.Key("a"));
        var requestB = TestFileBrowserFactory.BrowseRequest(TestFileBrowserFactory.Key("b"));
        var requestC = TestFileBrowserFactory.BrowseRequest(TestFileBrowserFactory.Key("c"));
        store.ApplyPage(requestA, new FileBrowserPage([]), FileBrowserPageApplyMode.Replace);
        store.ApplyPage(requestB, new FileBrowserPage([]), FileBrowserPageApplyMode.Replace);
        Assert.True(store.TryGetContainer(requestA, out _));

        store.ApplyPage(requestC, new FileBrowserPage([]), FileBrowserPageApplyMode.Replace);

        Assert.True(store.TryGetContainer(requestA, out _));
        Assert.False(store.TryGetContainer(requestB, out _));
        Assert.True(store.TryGetContainer(requestC, out _));
        Assert.Equal(new FileBrowserTreeDiagnostics(0, 2, 0, 1), store.GetDiagnostics());
    }

    [Fact]
    public void ProtectedContainerSurvivesRetentionPressure()
    {
        var store = new FileBrowserTreeStore(new FileBrowserTreeStoreOptions(maximumContainers: 1, maximumItems: 100));
        var protectedRequest = TestFileBrowserFactory.BrowseRequest(TestFileBrowserFactory.Key("protected"));
        var disposableRequest = TestFileBrowserFactory.BrowseRequest(TestFileBrowserFactory.Key("disposable"));
        store.SetProtectedPath([protectedRequest.ParentKey]);
        store.ApplyPage(protectedRequest, new FileBrowserPage([]), FileBrowserPageApplyMode.Replace);

        store.ApplyPage(disposableRequest, new FileBrowserPage([]), FileBrowserPageApplyMode.Replace);

        Assert.True(store.TryGetContainer(protectedRequest, out _));
        Assert.False(store.TryGetContainer(disposableRequest, out _));
        Assert.Equal(1, store.GetDiagnostics().ProtectedItemCount);
        Assert.Equal(1, store.GetDiagnostics().EvictedContainerQueryCount);
    }

    [Fact]
    public void UnattachedItemsUseLeastRecentlyUsedEviction()
    {
        var store = new FileBrowserTreeStore(new FileBrowserTreeStoreOptions(maximumContainers: 2, maximumItems: 2));
        var first = TestFileBrowserFactory.File("first");
        var second = TestFileBrowserFactory.File("second");
        var third = TestFileBrowserFactory.File("third");
        store.Upsert(first);
        store.Upsert(second);
        Assert.True(store.TryGetItem(first.Key, out _));

        store.Upsert(third);

        Assert.True(store.TryGetItem(first.Key, out var retainedFirst));
        Assert.Same(first, retainedFirst);
        Assert.False(store.TryGetItem(second.Key, out _));
        Assert.True(store.TryGetItem(third.Key, out var retainedThird));
        Assert.Same(third, retainedThird);
        Assert.Equal(2, store.GetDiagnostics().CachedItemCount);
    }

    [Fact]
    public void ItemBudgetEvictsLeastRecentlyUsedUnprotectedContainerQuery()
    {
        var store = new FileBrowserTreeStore(new FileBrowserTreeStoreOptions(
            maximumContainers: 10,
            maximumItems: 3));
        var firstRoot = TestFileBrowserFactory.Key("first-root");
        var secondRoot = TestFileBrowserFactory.Key("second-root");
        var firstRequest = TestFileBrowserFactory.BrowseRequest(firstRoot);
        var secondRequest = TestFileBrowserFactory.BrowseRequest(secondRoot);
        store.ApplyPage(
            firstRequest,
            new FileBrowserPage([
                TestFileBrowserFactory.File("first-a", firstRoot),
                TestFileBrowserFactory.File("first-b", firstRoot)
            ]),
            FileBrowserPageApplyMode.Replace);

        store.ApplyPage(
            secondRequest,
            new FileBrowserPage([
                TestFileBrowserFactory.File("second-a", secondRoot),
                TestFileBrowserFactory.File("second-b", secondRoot)
            ]),
            FileBrowserPageApplyMode.Replace);

        Assert.False(store.TryGetContainer(firstRequest, out _));
        Assert.True(store.TryGetContainer(secondRequest, out var retained));
        Assert.Equal(["second-a", "second-b"], retained!.Items.Select(item => item.Key.Value));
        Assert.False(store.TryGetItem(TestFileBrowserFactory.Key("first-a"), out _));
        Assert.True(store.TryGetItem(TestFileBrowserFactory.Key("first-b"), out _));
        Assert.Equal(3, store.GetDiagnostics().CachedItemCount);
        Assert.Equal(1, store.GetDiagnostics().CachedContainerQueryCount);
        Assert.Equal(1, store.GetDiagnostics().EvictedContainerQueryCount);
    }

    [Fact]
    public void LoadedDescendantsAreBreadthFirstDeduplicatedAndCycleSafe()
    {
        var store = new FileBrowserTreeStore();
        var rootKey = TestFileBrowserFactory.Key("root");
        var folder = TestFileBrowserFactory.Container("folder", rootKey);
        var rootLeaf = TestFileBrowserFactory.File("root-leaf", rootKey);
        store.ApplyPage(
            TestFileBrowserFactory.BrowseRequest(rootKey),
            new FileBrowserPage([folder, rootLeaf]),
            FileBrowserPageApplyMode.Replace);

        var cycleToRoot = TestFileBrowserFactory.Container("root", folder.Key, childState: FileBrowserChildState.HasChildren);
        var nestedLeaf = TestFileBrowserFactory.File("nested-leaf", folder.Key);
        store.ApplyPage(
            TestFileBrowserFactory.BrowseRequest(folder.Key, pageSize: 3),
            new FileBrowserPage([cycleToRoot, nestedLeaf, nestedLeaf]),
            FileBrowserPageApplyMode.Replace);

        var descendants = store.GetLoadedDescendants(rootKey);

        Assert.Equal(["folder", "root-leaf", "nested-leaf"], descendants.Select(item => item.Key.Value));
        Assert.Equal(descendants.Count, descendants.Select(item => item.Key).Distinct().Count());
    }

    [Fact]
    public void QueryVariantsRemainIndependentButLoadedChildrenAreDeduplicated()
    {
        var store = new FileBrowserTreeStore();
        var root = TestFileBrowserFactory.Key("root");
        var byName = TestFileBrowserFactory.BrowseRequest(root);
        var bySize = TestFileBrowserFactory.BrowseRequest(
            root,
            sort: new FileBrowserSortDescriptor(FileBrowserSortField.Size));
        var shared = TestFileBrowserFactory.File("shared", root, "Shared", size: 20);
        store.ApplyPage(byName, new FileBrowserPage([shared]), FileBrowserPageApplyMode.Replace);
        store.ApplyPage(
            bySize,
            new FileBrowserPage([shared, TestFileBrowserFactory.File("size-only", root, size: 10)]),
            FileBrowserPageApplyMode.Replace);

        Assert.True(store.TryGetContainer(byName, out var nameSnapshot));
        Assert.True(store.TryGetContainer(bySize, out var sizeSnapshot));
        Assert.Equal(["shared"], nameSnapshot!.Items.Select(item => item.Key.Value));
        Assert.Equal(["shared", "size-only"], sizeSnapshot!.Items.Select(item => item.Key.Value));
        Assert.Equal(["shared", "size-only"], store.GetLoadedChildren(root).Select(item => item.Key.Value));
    }

    [Fact]
    public void RecordErrorIsClearedBySuccessfulReplacementAndInvalidateRemovesQuery()
    {
        var store = new FileBrowserTreeStore();
        var root = TestFileBrowserFactory.Key("root");
        var request = TestFileBrowserFactory.BrowseRequest(root);
        var error = new FileBrowserError(FileBrowserErrorCode.Offline, "Source offline", isRetryable: true);

        var failed = store.RecordError(request, error);

        Assert.Same(error, failed.Error);
        Assert.False(failed.IsComplete);
        Assert.Equal(0, failed.LoadedPageCount);

        var recovered = store.ApplyPage(
            request,
            new FileBrowserPage([TestFileBrowserFactory.File("available", root)]),
            FileBrowserPageApplyMode.Replace);
        Assert.Null(recovered.Error);
        Assert.True(recovered.IsComplete);

        store.Invalidate(root);
        Assert.False(store.TryGetContainer(request, out _));
        Assert.Empty(store.GetLoadedChildren(root));
    }

    [Fact]
    public void ClearSourceRemovesOnlyThatSourcesItemsQueriesAndProtection()
    {
        var store = new FileBrowserTreeStore();
        var firstRoot = TestFileBrowserFactory.Key("root", "first");
        var secondRoot = TestFileBrowserFactory.Key("root", "second");
        var firstItem = TestFileBrowserFactory.File("first-file", firstRoot, source: "first");
        var secondItem = TestFileBrowserFactory.File("second-file", secondRoot, source: "second");
        var firstRequest = TestFileBrowserFactory.BrowseRequest(firstRoot);
        var secondRequest = TestFileBrowserFactory.BrowseRequest(secondRoot);
        store.ApplyPage(firstRequest, new FileBrowserPage([firstItem]), FileBrowserPageApplyMode.Replace);
        store.ApplyPage(secondRequest, new FileBrowserPage([secondItem]), FileBrowserPageApplyMode.Replace);
        store.SetProtectedPath([firstRoot, secondRoot]);

        store.ClearSource(TestFileBrowserFactory.Source("first"));

        Assert.False(store.TryGetContainer(firstRequest, out _));
        Assert.False(store.TryGetItem(firstItem.Key, out _));
        Assert.True(store.TryGetContainer(secondRequest, out _));
        Assert.True(store.TryGetItem(secondItem.Key, out _));
        Assert.Equal(1, store.GetDiagnostics().ProtectedItemCount);
    }
}
