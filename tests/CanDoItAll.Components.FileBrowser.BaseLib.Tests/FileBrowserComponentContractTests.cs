using System.Reflection;
using CanDoItAll.Components.FileBrowser.Core;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.FileBrowser.BaseLib.Tests;

public sealed class FileBrowserComponentContractTests
{
    [Fact]
    public void FileBrowser_ExposesTheHostIntegrationContract()
    {
        AssertParameterContract<FileBrowser>(new Dictionary<string, Type>
        {
            [nameof(FileBrowser.Session)] = typeof(IFileBrowserSession),
            [nameof(FileBrowser.AriaLabel)] = typeof(string),
            [nameof(FileBrowser.Class)] = typeof(string),
            [nameof(FileBrowser.InitializeOnFirstRender)] = typeof(bool),
            [nameof(FileBrowser.InitialSourceId)] = typeof(FileBrowserSourceId?),
            [nameof(FileBrowser.InitialItemKey)] = typeof(FileBrowserItemKey?),
            [nameof(FileBrowser.InitialViewMode)] = typeof(FileBrowserViewMode),
            [nameof(FileBrowser.SearchDebounceMilliseconds)] = typeof(int),
            [nameof(FileBrowser.SnapshotChanged)] = typeof(EventCallback<FileBrowserSnapshot>),
            [nameof(FileBrowser.ViewModeChanged)] = typeof(EventCallback<FileBrowserViewMode>),
            [nameof(FileBrowser.ItemInvoked)] = typeof(EventCallback<FileBrowserItemInvokedEventArgs>),
            [nameof(FileBrowser.ActionRequested)] = typeof(EventCallback<FileBrowserItemActionEventArgs>)
        });

        AssertEditorRequired<FileBrowser>(nameof(FileBrowser.Session));
        Assert.True(typeof(IAsyncDisposable).IsAssignableFrom(typeof(FileBrowser)));
    }

    [Fact]
    public void ItemViews_ExposeSelectionInvocationAndActionCallbacks()
    {
        AssertParameterContract<FileBrowserListView>(new Dictionary<string, Type>
        {
            [nameof(FileBrowserListView.Snapshot)] = typeof(FileBrowserSnapshot),
            [nameof(FileBrowserListView.SelectRequested)] = typeof(EventCallback<FileBrowserItem>),
            [nameof(FileBrowserListView.ToggleSelectionRequested)] = typeof(EventCallback<FileBrowserItem>),
            [nameof(FileBrowserListView.InvokeRequested)] = typeof(EventCallback<FileBrowserItemInvokedEventArgs>),
            [nameof(FileBrowserListView.SortRequested)] = typeof(EventCallback<FileBrowserSortField>),
            [nameof(FileBrowserListView.ActionRequested)] = typeof(EventCallback<FileBrowserItemActionEventArgs>),
            [nameof(FileBrowserListView.MenuRequested)] = typeof(EventCallback<FileBrowserActionMenuRequest>),
            [nameof(FileBrowserListView.OpenMenuItemKey)] = typeof(FileBrowserItemKey?)
        });
        AssertParameterContract<FileBrowserCardView>(new Dictionary<string, Type>
        {
            [nameof(FileBrowserCardView.Snapshot)] = typeof(FileBrowserSnapshot),
            [nameof(FileBrowserCardView.SelectRequested)] = typeof(EventCallback<FileBrowserItem>),
            [nameof(FileBrowserCardView.ToggleSelectionRequested)] = typeof(EventCallback<FileBrowserItem>),
            [nameof(FileBrowserCardView.InvokeRequested)] = typeof(EventCallback<FileBrowserItemInvokedEventArgs>),
            [nameof(FileBrowserCardView.ActionRequested)] = typeof(EventCallback<FileBrowserItemActionEventArgs>),
            [nameof(FileBrowserCardView.MenuRequested)] = typeof(EventCallback<FileBrowserActionMenuRequest>),
            [nameof(FileBrowserCardView.OpenMenuItemKey)] = typeof(FileBrowserItemKey?)
        });

        AssertEditorRequired<FileBrowserListView>(nameof(FileBrowserListView.Snapshot));
        AssertEditorRequired<FileBrowserCardView>(nameof(FileBrowserCardView.Snapshot));
    }

    [Fact]
    public void SupportingComponents_ExposeExplicitProviderNeutralParameters()
    {
        AssertParameterContract<FileBrowserItemActions>(new Dictionary<string, Type>
        {
            [nameof(FileBrowserItemActions.Item)] = typeof(FileBrowserItem),
            [nameof(FileBrowserItemActions.IsMenuOpen)] = typeof(bool),
            [nameof(FileBrowserItemActions.ActionRequested)] = typeof(EventCallback<FileBrowserItemActionEventArgs>),
            [nameof(FileBrowserItemActions.MenuRequested)] = typeof(EventCallback<FileBrowserActionMenuRequest>)
        });
        AssertParameterContract<FileBrowserSourceNavigation>(new Dictionary<string, Type>
        {
            [nameof(FileBrowserSourceNavigation.Snapshot)] = typeof(FileBrowserSnapshot),
            [nameof(FileBrowserSourceNavigation.Title)] = typeof(string),
            [nameof(FileBrowserSourceNavigation.SourceChanged)] = typeof(EventCallback<FileBrowserSourceId>)
        });
        AssertParameterContract<FileBrowserBreadcrumbs>(new Dictionary<string, Type>
        {
            [nameof(FileBrowserBreadcrumbs.Snapshot)] = typeof(FileBrowserSnapshot),
            [nameof(FileBrowserBreadcrumbs.BackRequested)] = typeof(EventCallback),
            [nameof(FileBrowserBreadcrumbs.ForwardRequested)] = typeof(EventCallback),
            [nameof(FileBrowserBreadcrumbs.UpRequested)] = typeof(EventCallback),
            [nameof(FileBrowserBreadcrumbs.NavigateRequested)] = typeof(EventCallback<FileBrowserItemKey>)
        });
        AssertParameterContract<FileBrowserStatusBar>(new Dictionary<string, Type>
        {
            [nameof(FileBrowserStatusBar.Snapshot)] = typeof(FileBrowserSnapshot)
        });

        AssertEditorRequired<FileBrowserItemActions>(nameof(FileBrowserItemActions.Item));
        AssertEditorRequired<FileBrowserSourceNavigation>(nameof(FileBrowserSourceNavigation.Snapshot));
        AssertEditorRequired<FileBrowserBreadcrumbs>(nameof(FileBrowserBreadcrumbs.Snapshot));
        AssertEditorRequired<FileBrowserStatusBar>(nameof(FileBrowserStatusBar.Snapshot));
    }

    [Fact]
    public void Toolbar_ExposesControlledStateAndEvents()
    {
        AssertParameterContract<FileBrowserToolbar>(new Dictionary<string, Type>
        {
            [nameof(FileBrowserToolbar.Snapshot)] = typeof(FileBrowserSnapshot),
            [nameof(FileBrowserToolbar.SearchText)] = typeof(string),
            [nameof(FileBrowserToolbar.SearchTextChanged)] = typeof(EventCallback<string>),
            [nameof(FileBrowserToolbar.SearchScope)] = typeof(FileBrowserSearchScope),
            [nameof(FileBrowserToolbar.SearchScopeChanged)] = typeof(EventCallback<FileBrowserSearchScope>),
            [nameof(FileBrowserToolbar.ViewMode)] = typeof(FileBrowserViewMode),
            [nameof(FileBrowserToolbar.ViewModeChanged)] = typeof(EventCallback<FileBrowserViewMode>),
            [nameof(FileBrowserToolbar.CategoryChanged)] = typeof(EventCallback<FileBrowserItemCategory?>),
            [nameof(FileBrowserToolbar.SortChanged)] = typeof(EventCallback<FileBrowserSortField>),
            [nameof(FileBrowserToolbar.SortDirectionReversed)] = typeof(EventCallback),
            [nameof(FileBrowserToolbar.IncludeDescendantsChanged)] = typeof(EventCallback<bool>),
            [nameof(FileBrowserToolbar.RefreshRequested)] = typeof(EventCallback)
        });

        AssertEditorRequired<FileBrowserToolbar>(nameof(FileBrowserToolbar.Snapshot));
    }

    [Fact]
    public void FileBrowser_DefaultsAreSafeForEnterpriseEmbedding()
    {
        var browser = new FileBrowser();
        var sourceNavigation = new FileBrowserSourceNavigation();

        Assert.Equal("File browser", browser.AriaLabel);
        Assert.True(browser.InitializeOnFirstRender);
        Assert.Equal(FileBrowserViewMode.List, browser.InitialViewMode);
        Assert.Equal(280, browser.SearchDebounceMilliseconds);
        Assert.Equal("Sources", sourceNavigation.Title);
    }

    private static void AssertParameterContract<TComponent>(IReadOnlyDictionary<string, Type> expected)
    {
        var actual = typeof(TComponent)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .ToDictionary(property => property.Name, property => property.PropertyType, StringComparer.Ordinal);

        Assert.Equal(expected.OrderBy(pair => pair.Key), actual.OrderBy(pair => pair.Key));
    }

    private static void AssertEditorRequired<TComponent>(string propertyName)
    {
        var property = typeof(TComponent).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

        Assert.NotNull(property);
        Assert.NotNull(property.GetCustomAttribute<EditorRequiredAttribute>());
    }
}
