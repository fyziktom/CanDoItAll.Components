using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class DialogNavigationOwnershipTests {

    [Fact]
    public async Task Fragment_dialogs_keep_their_reference_and_rendered_content_until_explicit_close_all() {
        using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var navigation = new TestNavigation();
        using var service = new DialogService(navigation);
        context.Services.AddSingleton(service);
        using var lease = service.PreserveDialogsOnSamePageNavigation();
        using var cancellation = new CancellationTokenSource();
        var first = service.OpenAsync("First", reference => builder => builder.AddContent(0, "First " + reference.Id), cancellationToken: cancellation.Token);
        var second = service.OpenAsync("Second", reference => builder => builder.AddContent(0, "Second " + reference.Id));
        var references = service.Dialogs.ToArray();
        var host = context.Render<DialogHost>();
        Assert.Equal(2, host.FindAll(".cda-dialog").Count);
        Assert.Contains("First " + references[0].Id, host.Markup);
        Assert.Contains("Second " + references[1].Id, host.Markup);
        await host.InvokeAsync(() => navigation.NavigateTo("/agents?tab=providers#details"));
        Assert.Equal(references, service.Dialogs);
        Assert.Equal(2, host.FindAll(".cda-dialog").Count);
        await host.InvokeAsync(() => service.CloseAll("closed"));
        Assert.Equal("closed", await first);
        Assert.Equal("closed", await second);
        Assert.Empty(host.FindAll(".cda-dialog"));
        var changes = 0;
        service.Changed += () => changes++;
        cancellation.Cancel();
        Assert.Equal(0, changes);
    }

    [Fact]
    public async Task Nested_owner_cancellation_removes_only_its_fragment_and_keeps_outer_dialog_active() {
        using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var navigation = new TestNavigation();
        using var service = new DialogService(navigation);
        context.Services.AddSingleton(service);
        using var outerLease = service.PreserveDialogsOnSamePageNavigation();
        var outerResult = service.OpenAsync("Outer", _ => builder => builder.AddContent(0, "Outer content"));
        using var innerLease = service.PreserveDialogsOnSamePageNavigation();
        using var cancellation = new CancellationTokenSource();
        var innerResult = service.OpenAsync("Inner", _ => builder => builder.AddContent(0, "Inner content"), cancellationToken: cancellation.Token);
        var host = context.Render<DialogHost>();
        Assert.Equal(2, host.FindAll(".cda-dialog").Count);
        await host.InvokeAsync(cancellation.Cancel);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => innerResult);
        innerLease.Dispose();
        await host.InvokeAsync(() => navigation.NavigateTo("/agents?tab=overview"));
        Assert.Contains("Outer content", host.Markup);
        Assert.DoesNotContain("Inner content", host.Markup);
        Assert.False(outerResult.IsCompleted);
        await host.InvokeAsync(() => navigation.NavigateTo("/projects"));
        Assert.Null(await outerResult);
        Assert.Empty(host.FindAll(".cda-dialog"));
    }

    [Fact]
    public async Task Already_canceled_owner_never_adds_an_orphan_dialog() {
        using var service = new DialogService(new TestNavigation());
        using var lease = service.PreserveDialogsOnSamePageNavigation();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var result = service.OpenAsync("Canceled", _ => builder => builder.AddContent(0, "Content"), cancellationToken: cancellation.Token);
        Assert.Empty(service.Dialogs);
        Assert.True(result.IsCanceled);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => result);
    }

    [Fact]
    public async Task Explicit_close_after_query_navigation_completes_the_original_result_once() {
        var navigation = new TestNavigation();
        using var service = new DialogService(navigation);
        using var lease = service.PreserveDialogsOnSamePageNavigation();
        using var cancellation = new CancellationTokenSource();
        var result = service.OpenAsync("Owned", _ => builder => builder.AddContent(0, "Content"), cancellationToken: cancellation.Token);
        var reference = Assert.Single(service.Dialogs);
        Assert.Same(result, reference.Result);
        navigation.NavigateTo("/agents?tab=providers#sharing");
        Assert.Same(reference, Assert.Single(service.Dialogs));
        await reference.CloseAsync("accepted");
        cancellation.Cancel();
        await reference.CloseAsync("ignored");
        Assert.Equal("accepted", await result);
        Assert.Empty(service.Dialogs);
    }

    [Fact]
    public async Task Service_disposal_cancels_results_and_detaches_navigation_with_outstanding_leases() {
        var navigation = new TestNavigation();
        var service = new DialogService(navigation);
        var first = service.PreserveDialogsOnSamePageNavigation();
        var second = service.PreserveDialogsOnSamePageNavigation();
        var result = service.OpenAsync("Pending", _ => builder => builder.AddContent(0, "Content"));
        var changes = 0;
        service.Changed += () => changes++;
        service.Dispose();
        service.Dispose();
        first.Dispose();
        first.Dispose();
        second.Dispose();
        navigation.NavigateTo("/projects");
        Assert.Equal(0, changes);
        Assert.Empty(service.Dialogs);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => result);
    }

    [Theory]
    [InlineData("/agents#fragment")]
    [InlineData("/other")]
    public async Task No_owner_navigation_keeps_default_result_semantics(string uri) {
        var navigation = new TestNavigation();
        using var service = new DialogService(navigation);
        var result = service.OpenAsync("Default", _ => builder => builder.AddContent(0, "Content"));
        navigation.NavigateTo(uri);
        Assert.Null(await result);
        Assert.Empty(service.Dialogs);
    }

    [Theory]
    [InlineData("/agents/")]
    [InlineData("/Agents")]
    public async Task Different_canonical_path_is_a_departure(string uri) {
        var navigation = new TestNavigation();
        using var service = new DialogService(navigation);
        using var lease = service.PreserveDialogsOnSamePageNavigation();
        var result = service.OpenAsync("Owned", _ => builder => builder.AddContent(0, "Content"));
        navigation.NavigateTo(uri);
        Assert.Null(await result);
        Assert.Empty(service.Dialogs);
    }

    [Fact]
    public async Task Default_query_navigation_preserves_existing_close_behavior() {
        var navigation = new TestNavigation();
        using var service = new DialogService(navigation);
        var result = service.OpenAsync("Default", _ => builder => builder.AddContent(0, "Content"));
        navigation.NavigateTo("/agents?tab=providers");
        Assert.Empty(service.Dialogs);
        Assert.Null(await result);
    }

    [Theory]
    [InlineData("/agents?tab=providers")]
    [InlineData("/agents#details")]
    public void Opted_in_same_page_navigation_preserves_dialog_identity(string location) {
        var navigation = new TestNavigation();
        using var service = new DialogService(navigation);
        using var owner = service.PreserveDialogsOnSamePageNavigation();
        _ = service.OpenAsync("Independent", _ => builder => builder.AddContent(0, "Content"));
        var reference = Assert.Single(service.Dialogs);
        navigation.NavigateTo(location);
        Assert.Same(reference, Assert.Single(service.Dialogs));
        Assert.False(reference.Result.IsCompleted);
    }

    [Fact]
    public void Path_navigation_still_closes_dialogs_with_an_active_owner() {
        var navigation = new TestNavigation();
        using var service = new DialogService(navigation);
        using var owner = service.PreserveDialogsOnSamePageNavigation();
        _ = service.OpenAsync("Independent", _ => builder => builder.AddContent(0, "Content"));
        navigation.NavigateTo("/projects");
        Assert.Empty(service.Dialogs);
    }

    [Fact]
    public void Last_owner_disposal_restores_default_behavior_and_is_idempotent() {
        var navigation = new TestNavigation();
        using var service = new DialogService(navigation);
        var first = service.PreserveDialogsOnSamePageNavigation();
        var second = service.PreserveDialogsOnSamePageNavigation();
        _ = service.OpenAsync("Independent", _ => builder => builder.AddContent(0, "Content"));
        first.Dispose();
        first.Dispose();
        navigation.NavigateTo("/agents?tab=providers");
        Assert.Single(service.Dialogs);
        second.Dispose();
        navigation.NavigateTo("/agents?tab=overview");
        Assert.Empty(service.Dialogs);
    }

    [Fact]
    public async Task Owner_cancellation_closes_only_its_reference_during_same_page_navigation() {
        var navigation = new TestNavigation();
        using var service = new DialogService(navigation);
        using var owner = service.PreserveDialogsOnSamePageNavigation();
        using var cancellation = new CancellationTokenSource();
        _ = service.OpenAsync("Independent", _ => builder => builder.AddContent(0, "Content"));
        var independent = Assert.Single(service.Dialogs);
        var pending = service.OpenAsync("Owned", _ => builder => builder.AddContent(0, "Content"), cancellationToken: cancellation.Token);
        cancellation.Cancel();
        navigation.NavigateTo("/agents?usageScope=simple-chats");
        Assert.Same(independent, Assert.Single(service.Dialogs));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => pending);
    }

    [Fact]
    public void Disposed_service_rejects_new_navigation_owners() {
        var service = new DialogService(new TestNavigation());
        using var owner = service.PreserveDialogsOnSamePageNavigation();
        service.Dispose();
        Assert.Throws<ObjectDisposedException>(() => service.PreserveDialogsOnSamePageNavigation());
    }

    private sealed class TestNavigation : NavigationManager {
        public TestNavigation() {
            Initialize("http://localhost/", "http://localhost/agents");
        }

        protected override void NavigateToCore(string uri, bool forceLoad) {
            Uri = ToAbsoluteUri(uri).ToString();
            NotifyLocationChanged(false);
        }
    }
}
